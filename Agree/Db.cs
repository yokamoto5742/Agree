using System;
using System.Data.OleDb;
using AgentlabUtilityLibrary;

namespace Agree;

/// <summary>
/// OleDb 接続の Open → 実行 → Close をまとめたヘルパー。
/// 例外時も必ず Close し、接続が開いたまま残って以後の Open がすべて失敗する状態を防ぐ。
/// （OleDbConnection.Close は閉じた接続に呼んでも何もしない。）
/// </summary>
internal static class Db
{
	public static int Execute(OleDbConnection con, string sql)
	{
		con.Open();
		try
		{
			using (OleDbCommand cmd = new OleDbCommand(sql, con))
			{
				return cmd.ExecuteNonQuery();
			}
		}
		finally
		{
			con.Close();
		}
	}

	/// <summary>先頭行の先頭列を返す。行が無い場合は null。</summary>
	public static object Scalar(OleDbConnection con, string sql)
	{
		con.Open();
		try
		{
			using (OleDbCommand cmd = new OleDbCommand(sql, con))
			{
				return cmd.ExecuteScalar();
			}
		}
		finally
		{
			con.Close();
		}
	}

	/// <summary>職員マスタ(M_USR)から指定コード1人分の氏名だけを取得する。数字でない・該当なしは null。</summary>
	public static string StaffName(OleDbConnection con, string code)
	{
		if (!int.TryParse(code.Trim(), out int staffCode))
		{
			return null;
		}
		return Scalar(con, "select Trim(NAME) from M_USR" + Env.DB_LINK + " where CODE = " + staffCode)?.ToString();
	}

	public static void Read(OleDbConnection con, string sql, Action<OleDbDataReader> onRow)
	{
		con.Open();
		try
		{
			using (OleDbCommand cmd = new OleDbCommand(sql, con))
			using (OleDbDataReader reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					onRow(reader);
				}
			}
		}
		finally
		{
			con.Close();
		}
	}
}
