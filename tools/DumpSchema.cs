using System;
using System.Data.OleDb;
using AgentlabUtilityLibrary;

/// <summary>
/// アプリと同じ接続設定（カレントディレクトリの AgentlabUtilityLibrary.ini）で DB へつなぎ、
/// データディクショナリからテーブル／列定義を出力する調査用ツール。SELECT しか実行しない。
///
/// 使い方: DumpSchema.exe [OPEN|EHR] [表名のLIKE] [オーナーのLIKE] [@DBリンク名]
///   OPEN … 同意書側（AGREE 等）。EHR … 電子カルテのマスタ（M_xxx）。
///   オーナー既定は接続ユーザ。DBリンク既定は EHR のとき ini の DB_LINK。
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
		using (OleDbConnection con = isEhr ? DBConn.GetEhrDBConn() : DBConn.GetOpenDBConn())
		{
			con.Open();
			string owner = args.Length > 2
				? args[2].ToUpperInvariant()
				: Convert.ToString(Scalar(con, "select USER from DUAL" + link));
			Console.WriteLine("--- {0} : Provider={1} / DataSource={2} / DBリンク={3} / オーナー LIKE {4} ---",
				which, isEhr ? Env.EHR_PROVIDER : Env.PROVIDER, con.DataSource,
				link.Length == 0 ? "(なし)" : link, owner);

			// ALL_TAB_COLUMNS = 参照権限のある表だけが見える（DBA権限は不要）。
			string sql =
				"select c.OWNER, c.TABLE_NAME, c.COLUMN_NAME, c.DATA_TYPE, " +
				"       c.CHAR_LENGTH, c.DATA_LENGTH, c.DATA_PRECISION, c.DATA_SCALE, c.NULLABLE " +
				"  from ALL_TAB_COLUMNS" + link + " c " +
				" where c.TABLE_NAME like '" + filter + "' escape '!' and c.OWNER like '" + owner + "' " +
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
			}
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

	private static object Scalar(OleDbConnection con, string sql)
	{
		using (OleDbCommand cmd = new OleDbCommand(sql, con))
		{
			return cmd.ExecuteScalar();
		}
	}
}
