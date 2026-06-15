using System;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class InfoShare
{
	public string PtId = "";

	public string InOut = "0";

	public string Cont = "";

	public string SaveDate = "";

	public string SaveTime = "";

	public string Staff = "";

	public void Save()
	{
		if (PtId.Length != 0 && InOut.Length != 0 && Staff.Length != 0)
		{
			OleDbConnection dBConn = DBConn.GetDBConn();
			dBConn.Open();
			OleDbCommand oleDbCommand = new OleDbCommand();
			oleDbCommand.Connection = dBConn;
			oleDbCommand.CommandText = "select 患者コード from ADT_患者サマリデータ where 患者コード = ? and 入外区分 = ?";
			oleDbCommand.Parameters.Add("患者コード", OleDbType.Numeric).Value = PtId;
			oleDbCommand.Parameters.Add("入外区分", OleDbType.Numeric).Value = InOut;
			OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
			bool flag = false;
			if (oleDbDataReader.Read())
			{
				flag = true;
			}
			oleDbDataReader.Close();
			if (flag)
			{
				oleDbCommand.CommandText = "update ADT_患者サマリデータ set サマリ内容 = ?, 更新日 = " + DateTime.Now.ToString("yyyyMMdd") + ", 更新時間 = " + DateTime.Now.ToString("HHmmss") + ", 更新者 = ?, 代行更新者 = 0 where 患者コード = ? and 入外区分 = ?";
				oleDbCommand.Parameters.Clear();
				oleDbCommand.Parameters.Add("サマリ内容", OleDbType.VarWChar).Value = Cont;
				oleDbCommand.Parameters.Add("更新者", OleDbType.Numeric).Value = Staff;
				oleDbCommand.Parameters.Add("患者コード", OleDbType.Numeric).Value = PtId;
				oleDbCommand.Parameters.Add("入外区分", OleDbType.Numeric).Value = InOut;
				oleDbCommand.ExecuteNonQuery();
			}
			else
			{
				oleDbCommand.CommandText = "insert into ADT_患者サマリデータ (患者コード, 入外区分, サマリ内容, 登録日, 登録時間, 登録者, 更新日, 更新時間, 更新者) values (?, ?, ?, " + DateTime.Now.ToString("yyyyMMdd") + ", " + DateTime.Now.ToString("HHmmss") + ", ?, " + DateTime.Now.ToString("yyyyMMdd") + ", " + DateTime.Now.ToString("HHmmss") + ", ?)";
				oleDbCommand.Parameters.Clear();
				oleDbCommand.Parameters.Add("患者コード", OleDbType.Numeric).Value = PtId;
				oleDbCommand.Parameters.Add("入外区分", OleDbType.Numeric).Value = InOut;
				oleDbCommand.Parameters.Add("サマリ内容", OleDbType.VarWChar).Value = Cont;
				oleDbCommand.Parameters.Add("登録者", OleDbType.Numeric).Value = Staff;
				oleDbCommand.Parameters.Add("更新者", OleDbType.Numeric).Value = Staff;
				oleDbCommand.ExecuteNonQuery();
			}
			dBConn.Close();
		}
	}

	public static InfoShare Load(string pt_id, string in_out)
	{
		InfoShare infoShare = new InfoShare();
		int result = 0;
		if (pt_id.Length == 0 || !int.TryParse(pt_id, out result))
		{
			return infoShare;
		}
		if (in_out.Length == 0)
		{
			return infoShare;
		}
		OleDbConnection dBConn = DBConn.GetDBConn();
		dBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = dBConn;
		oleDbCommand.CommandText = "select * from ADT_患者サマリデータ where 患者コード = ? and 入外区分 = ?";
		oleDbCommand.Parameters.Add("患者コード", OleDbType.Numeric).Value = pt_id;
		oleDbCommand.Parameters.Add("入外区分", OleDbType.Numeric).Value = in_out;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		if (oleDbDataReader.Read())
		{
			infoShare.PtId = pt_id;
			infoShare.InOut = in_out;
			infoShare.Cont = oleDbDataReader["サマリ内容"].ToString();
			infoShare.SaveDate = oleDbDataReader["更新日"].ToString();
			infoShare.SaveTime = oleDbDataReader["更新時間"].ToString();
			infoShare.Staff = oleDbDataReader["更新者"].ToString();
		}
		dBConn.Close();
		return infoShare;
	}

	public static InfoShare Load(string pt_id)
	{
		return Load(pt_id, "0");
	}
}
