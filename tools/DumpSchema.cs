using System;
using System.Data;
using System.Data.OleDb;
using AgentlabUtilityLibrary;

/// <summary>
/// アプリと同じ接続設定（カレントディレクトリの AgentlabUtilityLibrary.ini）で DB へつなぎ、
/// データディクショナリからテーブル／列定義を出力する調査用ツール。SELECT しか実行しない。
///
/// 使い方: DumpSchema.exe [OPEN|EHR] [表名のLIKE または 表名,表名,…] [オーナーのLIKE] [@DBリンク名]
///   OPEN … 同意書側（AGREE 等）。EHR … 電子カルテのマスタ（M_xxx）。
///   オーナー既定は接続ユーザ。DBリンク既定は EHR のとき ini の DB_LINK。
///   第2引数に `%` を含めず表名を直接（カンマ区切りで）書くと、ALL_TAB_COLUMNS が
///   0 件のときだけ `select * from 表 where 1 = 0` の結果セットから列定義を取り直す。
/// 手順は docs/dump_schema_production.md を参照。
/// </summary>
internal static class DumpSchema
{
	private static void Main(string[] args)
	{
		string which = args.Length > 0 ? args[0].ToUpperInvariant() : "OPEN";
		string filter = args.Length > 1 ? args[1].ToUpperInvariant() : "%";
		bool isEhr = which == "EHR";
		string link = args.Length > 3 ? args[3] : (isEhr ? Env.DB_LINK : "");
		// `%` を含まなければ LIKE ではなく表名の直接指定とみなす（ディクショナリが空のときの代替取得用）。
		string[] names = filter.IndexOf('%') < 0 ? filter.Replace("!", "").Split(',') : null;
		using (OleDbConnection con = isEhr ? DBConn.GetEhrDBConn() : DBConn.GetOpenDBConn())
		{
			con.Open();
			string owner = args.Length > 2
				? args[2].ToUpperInvariant()
				: Convert.ToString(Scalar(con, "select USER from DUAL" + link));
			Console.WriteLine("--- {0} : Provider={1} / DataSource={2} / DBリンク={3} / オーナー LIKE {4} / キャラクタセット={5} ---",
				which, isEhr ? Env.EHR_PROVIDER : Env.PROVIDER, con.DataSource,
				link.Length == 0 ? "(なし)" : link, owner, CharacterSet(con, link));

			if (!DumpFromDictionary(con, filter, names, owner, link) && names != null)
			{
				// ディクショナリに行が見えない（ロール経由の権限など）。表が SELECT できるなら
				// 結果セットのスキーマから列定義を取れる。BYTE/CHAR の別と正確な Oracle 型名は出ない。
				Console.WriteLine();
				Console.WriteLine("ALL_TAB_COLUMNS が 0 件のため、select * from 表 where 1 = 0 から取り直します。");
				foreach (string name in names)
				{
					DumpFromSelect(con, name.Trim(), link);
				}
			}
		}
	}

	/// <summary>ALL_TAB_COLUMNS から列定義を出力する。1 行でも出力したら true。</summary>
	private static bool DumpFromDictionary(OleDbConnection con, string filter, string[] names, string owner, string link)
	{
		// ALL_TAB_COLUMNS = 参照権限のある表だけが見える（DBA権限は不要）。
		string cond = names != null
			? "c.TABLE_NAME in ('" + string.Join("', '", Trim(names)) + "')"
			: "c.TABLE_NAME like '" + filter + "' escape '!'";
		string sql =
			"select c.OWNER, c.TABLE_NAME, c.COLUMN_NAME, c.DATA_TYPE, " +
			"       c.CHAR_LENGTH, c.DATA_LENGTH, c.DATA_PRECISION, c.DATA_SCALE, c.NULLABLE " +
			"  from ALL_TAB_COLUMNS" + link + " c " +
			" where " + cond + " and c.OWNER like '" + owner + "' " +
			" order by c.OWNER, c.TABLE_NAME, c.COLUMN_ID";
		using (OleDbCommand cmd = new OleDbCommand(sql, con))
		using (OleDbDataReader r = cmd.ExecuteReader())
		{
			string current = null;
			while (r.Read())
			{
				string table = r.GetString(0) + "." + r.GetString(1);
				if (table != current)
				{
					current = table;
					Console.WriteLine();
					Console.WriteLine("[" + table + "]");
				}
				Console.WriteLine("  {0,-16} {1,-20} {2}", r.GetValue(2), TypeOf(r),
					r.GetString(8) == "N" ? "NOT NULL" : "");
			}
			if (current == null)
			{
				Console.WriteLine();
				Console.WriteLine("該当する表がありません（表名・オーナーの指定、または参照権限を確認してください）。");
			}
			return current != null;
		}
	}

	/// <summary>SELECT の結果セットのスキーマから列定義を出力する（ディクショナリが見えないとき用）。</summary>
	private static void DumpFromSelect(OleDbConnection con, string table, string link)
	{
		Console.WriteLine();
		Console.WriteLine("[" + table + link + "] ※結果セットから取得。BYTE/CHAR の別と桁数無指定の NUMBER は分からない");
		try
		{
			using (OleDbCommand cmd = new OleDbCommand("select * from " + table + link + " where 1 = 0", con))
			using (OleDbDataReader r = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
			using (DataTable t = r.GetSchemaTable())
			{
				foreach (DataRow row in t.Rows)
				{
					Console.WriteLine("  {0,-16} {1,-20} {2}", row["ColumnName"], SchemaTypeOf(row),
						Convert.ToBoolean(row["AllowDBNull"]) ? "" : "NOT NULL");
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("  取得できません: " + ex.Message.Trim());
		}
	}

	/// <summary>VARCHAR2(100 CHAR) / NUMBER(8) のように桁数付きで型名を組み立てる。</summary>
	private static string TypeOf(OleDbDataReader r)
	{
		string type = r.GetString(3);
		if (!r.IsDBNull(6))
		{
			return type + "(" + r.GetValue(6) + (r.IsDBNull(7) ? "" : "," + r.GetValue(7)) + ")";
		}
		if (type.StartsWith("VARCHAR") || type.StartsWith("CHAR") || type.StartsWith("NVARCHAR"))
		{
			return type + "(" + r.GetValue(4) + " CHAR / " + r.GetValue(5) + " BYTE)";
		}
		return type;
	}

	/// <summary>GetSchemaTable の1行から OleDbType(桁数) 形式の型名を組み立てる。</summary>
	private static string SchemaTypeOf(DataRow row)
	{
		string type = Enum.GetName(typeof(OleDbType), Convert.ToInt32(row["ProviderType"])) ?? row["ProviderType"].ToString();
		object precision = row["NumericPrecision"];
		if (precision != DBNull.Value && Convert.ToInt32(precision) > 0 && Convert.ToInt32(precision) < 255)
		{
			object scale = row["NumericScale"];
			return type + "(" + Convert.ToInt32(precision) + "," + (scale == DBNull.Value ? 0 : Convert.ToInt32(scale)) + ")";
		}
		return type + "(" + row["ColumnSize"] + ")";
	}

	/// <summary>DBキャラクタセット。VARCHAR2 の桁数(バイト)が全角何文字分かの判断に使う。</summary>
	private static string CharacterSet(OleDbConnection con, string link)
	{
		try
		{
			return Convert.ToString(Scalar(con,
				"select VALUE from NLS_DATABASE_PARAMETERS" + link + " where PARAMETER = 'NLS_CHARACTERSET'"));
		}
		catch (Exception ex)
		{
			return "(取得できません: " + ex.Message.Trim() + ")";
		}
	}

	private static string[] Trim(string[] names)
	{
		string[] result = new string[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			result[i] = names[i].Trim();
		}
		return result;
	}

	private static object Scalar(OleDbConnection con, string sql)
	{
		using (OleDbCommand cmd = new OleDbCommand(sql, con))
		{
			return cmd.ExecuteScalar();
		}
	}
}
