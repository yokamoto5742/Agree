using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

/// <summary>
/// OleDb 接続の生成。接続情報の設定元は Env(AgentlabUtilityLibrary.ini)。
/// GetOpenDBConn は同意書側のテーブル用、GetEhrDBConn は電子カルテのマスタ用。
/// </summary>
public class DBConn
{
	private static string openDbName = Env.OPEN_DB;

	private static string openDbUser = Env.OPEN_USER;

	private static string openDbPwd = Env.OPEN_PWD;

	internal static string OpenConnectionString => ConnectionString(Env.PROVIDER, openDbName, openDbUser, openDbPwd);

	internal static string EhrConnectionString => ConnectionString(Env.EHR_PROVIDER, Env.EHR_DB, Env.EHR_USER, Env.EHR_PWD);

	public static OleDbConnection GetOpenDBConn()
	{
		return new OleDbConnection(OpenConnectionString);
	}

	/// <summary>電子カルテ用の接続。INI に EHR_* が無ければ GetOpenDBConn と同じ接続先になる。</summary>
	public static OleDbConnection GetEhrDBConn()
	{
		return new OleDbConnection(EhrConnectionString);
	}

	private static string ConnectionString(string provider, string dataSource, string user, string password)
	{
		return "Provider=" + provider + ";Data Source=" + dataSource + ";User ID=" + user + ";Password=" + password;
	}
}
