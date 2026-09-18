using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace Agree;

/// <summary>
/// Pat.csv（電子カルテが書き出すファイル）のうち、本アプリで使う項目だけを保持する。
/// </summary>
internal sealed class PatCsvFields
{
	public int? PtId;       // [2] 患者ID（数字でなければ null）
	public string PtName;   // [3] 氏名
	public string PtKana;   // [5] カナ
	public string PtSex;    // [6] 性別（1=男, 2=女 を「男」「女」に変換済み。それ以外は空）
	public string DrId;     // [9] 入力者ID
	public string DrName;   // [10] 入力者氏名
	public string DeptCode; // [13] 診療科コード
	public string Field14;  // [14] 用途不明（空でなければ診療科を反映する）
	public string Field27;  // [27] 用途不明（"1" のとき医師情報を反映する）
}

/// <summary>
/// 電子カルテとの接点（マスタ M_PATIENT / M_USR / M_DEPT と Pat.csv、そのコード体系）をまとめる。
/// 電子カルテを乗り換える際はこのクラスだけを差し替える想定。同意書側のテーブル（AGREE 等）には触れない。
/// WinForms・COM・外部DLL に依存しないため、テストプロジェクトへソースリンクして結合テストできる。
/// </summary>
internal sealed class Ehr
{
	private readonly OleDbConnection con;

	private readonly string dbLink;

	private readonly string patCsvPath;

	public Ehr(OleDbConnection con, string dbLink, string patCsvPath)
	{
		this.con = con;
		this.dbLink = dbLink;
		this.patCsvPath = patCsvPath;
	}

	/// <summary>診療科の一覧（コード順）。コード → 略称。</summary>
	public List<KeyValuePair<string, string>> LoadDepartments()
	{
		List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
		Db.Read(con, "select CODE, Trim(S_NAME) from M_DEPT" + dbLink + " order by CODE", r =>
		{
			result.Add(new KeyValuePair<string, string>(r[0].ToString(), r[1].ToString()));
		});
		return result;
	}

	/// <summary>患者マスタから1人分を取得する。該当なしは null。性別は 2=女、それ以外=男 に変換する。</summary>
	public (string Name, string Kana, string Sex)? FindPatient(int ptId)
	{
		(string Name, string Kana, string Sex)? result = null;
		Db.Read(con, "select P_NAME, P_KANA, P_SEX from M_PATIENT" + dbLink + " where P_ID = " + ptId, r =>
		{
			result = (r["P_NAME"].ToString(), r["P_KANA"].ToString(), r["P_SEX"].ToString() == "2" ? "女" : "男");
		});
		return result;
	}

	/// <summary>職員マスタから指定コード1人分の氏名だけを取得する。数字でない・該当なしは null。</summary>
	public string StaffName(string code)
	{
		if (!int.TryParse(code.Trim(), out int staffCode))
		{
			return null;
		}
		return Db.Scalar(con, "select Trim(NAME) from M_USR" + dbLink + " where CODE = " + staffCode)?.ToString();
	}

	/// <summary>職員マスタから複数コードの氏名をまとめて取得する。コード → 氏名。数字でないコード・該当なしは含まない。</summary>
	public Dictionary<string, string> StaffNames(IEnumerable<string> codes)
	{
		Dictionary<string, string> result = new Dictionary<string, string>();
		List<long> numbers = codes.Select(c => long.TryParse(c, out long n) ? n : (long?)null).Where(n => n != null).Select(n => n.Value).Distinct().ToList();
		if (numbers.Count == 0)
		{
			return result;
		}
		Db.Read(con, "select CODE, Trim(NAME) from M_USR" + dbLink + " where CODE in (" + string.Join(", ", numbers) + ")", r =>
		{
			result[r[0].ToString()] = r[1].ToString();
		});
		return result;
	}

	/// <summary>
	/// Pat.csv の先頭行から使う項目だけを読み込む。ファイルが無い／空の場合は null を返す。
	/// </summary>
	public PatCsvFields ReadPatCsv()
	{
		if (!File.Exists(patCsvPath))
		{
			return null;
		}
		using (StreamReader reader = new StreamReader(patCsvPath, Encoding.Default))
		{
			string line = reader.ReadLine();
			if (line == null)
			{
				return null;
			}
			string[] fields = line.Split(',');
			string field(int i) => i < fields.Length ? fields[i] : null;
			string sex = field(6);
			return new PatCsvFields
			{
				PtId = int.TryParse(field(2), out int ptId) ? ptId : (int?)null,
				PtName = field(3),
				PtKana = field(5),
				PtSex = sex == "2" ? "女" : sex == "1" ? "男" : "",
				DrId = field(9),
				DrName = field(10),
				DeptCode = field(13),
				Field14 = field(14),
				Field27 = field(27),
			};
		}
	}

	/// <summary>診療科コード（1〜20）として解釈できる場合だけ true を返す。</summary>
	public static bool TryParseDeptCode(string text, out short code)
	{
		return short.TryParse(text, out code) && code >= 1 && code <= 20;
	}

	/// <summary>
	/// 同意書側の表にマスタの名称列を付ける（DB をまたぐ JOIN の代わり）。
	/// 名称列 nameColumn はキー列 keyColumn の直後に挿入する。キーが names に無い行は、
	/// dropUnmatched なら削除（内部結合と同じ）、そうでなければ名称を DBNull にする（外部結合と同じ）。
	/// </summary>
	public static void JoinName(DataTable table, string keyColumn, string nameColumn, IDictionary<string, string> names, bool dropUnmatched)
	{
		DataColumn column = table.Columns.Add(nameColumn, typeof(string));
		column.SetOrdinal(table.Columns[keyColumn].Ordinal + 1);
		foreach (DataRow row in table.Rows.Cast<DataRow>().ToList())
		{
			if (names.TryGetValue(row[keyColumn].ToString(), out string name))
			{
				row[column] = name;
			}
			else if (dropUnmatched)
			{
				table.Rows.Remove(row);
			}
		}
		table.AcceptChanges();
	}
}
