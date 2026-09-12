using AgentlabUtilityLibrary;
using Oracle.ManagedDataAccess.Client;

namespace Agree;

/// <summary>
/// OPEN_DB への接続を生成する。
/// 旧 AgentlabUtilityLibrary.DBConn.GetOpenDBConn() の置き換え。
///
/// 外部DLLの DBConn は OLE DB（MSDAORA.1 / OraOLEDB.Oracle）で接続を作るが、
/// どちらもネイティブCOMのため Oracle クライアントのインストールが必須だった。
/// ここでは ODP.NET マネージド・ドライバで接続を作り、クライアント非依存にする。
/// 接続情報（復号済み）は従来どおり Env（ini）から取得する。
/// </summary>
internal static class OracleDb
{
	/// <summary>
	/// 未接続の OracleConnection を返す。呼び出し側で Open() すること
	/// （DBConn.GetOpenDBConn() と同じ扱い。"Open" は OPEN_DB を指し、接続状態のことではない）。
	/// </summary>
	public static OracleConnection CreateOpenConnection()
	{
		OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder
		{
			UserID = Env.OPEN_USER,
			Password = Env.OPEN_PWD,
			DataSource = Env.OPEN_DB
		};
		return new OracleConnection(builder.ConnectionString);
	}
}
