using System.Collections.Generic;
using AgentlabUtilityLibrary;
using Oracle.ManagedDataAccess.Client;

namespace Agree;

/// <summary>
/// 本アプリが使うマスタ（診療科・職員）のコード→名称辞書。
/// 旧 AgentlabUtilityLibrary.Dict.DeptDict / Dict.StaffDict の置き換え。
///
/// 外部DLLの Dict は内部で OLE DB 接続を張るため Oracle クライアントを必要としていた。
/// アプリが実際に使うのは「診療科の略称」と「職員の氏名」の2つだけなので、
/// ODP.NET マネージドで同じ2表を引き直す。参照する表は Form1.Plan / TmpStaff が
/// 直接 join しているものと同一。
///
/// 値は初回アクセス時に一度だけ読み込んでキャッシュする（旧 Dict と同じ遅延ロード）。
/// 名称は CHAR 列の空白埋めを落とすため Trim する
/// （一覧の Trim(S_NAME) / Trim(NAME) と突き合わせるため、揃っていないと選択が一致しない）。
/// </summary>
internal static class MasterDict
{
	private static Dictionary<string, string> deptDict;

	private static Dictionary<string, string> staffDict;

	/// <summary>診療科コード → 略称（M_DEPT.S_NAME）。</summary>
	public static Dictionary<string, string> Dept =>
		deptDict ?? (deptDict = Load("Select CODE, S_NAME from M_DEPT" + Env.DB_LINK + " order by CODE"));

	/// <summary>職員コード → 氏名（M_USR.NAME）。</summary>
	public static Dictionary<string, string> Staff =>
		staffDict ?? (staffDict = Load("Select CODE, NAME from M_USR" + Env.DB_LINK + " order by CODE"));

	/// <summary>
	/// コード・名称の2列を読んで辞書化する。
	/// DBに接続できない場合は例外をそのまま呼び出し元へ投げる
	/// （Form1 のコンストラクタがこれを捕捉してオフラインモードへ落とすため、ここで握り潰さないこと）。
	/// </summary>
	private static Dictionary<string, string> Load(string sql)
	{
		Dictionary<string, string> dict = new Dictionary<string, string>();
		using (OracleConnection conn = OracleDb.CreateOpenConnection())
		{
			conn.Open();
			using (OracleCommand cmd = new OracleCommand(sql, conn))
			using (OracleDataReader reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					dict[reader[0].ToString().Trim()] = reader[1].ToString().Trim();
				}
			}
		}
		return dict;
	}
}
