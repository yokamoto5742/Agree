using System;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class KarteMessage
{
	public string To = "";

	public string From = "";

	public string Title = "";

	public string Msg = "";

	public string PtId = "";

	public string Priority = "";

	private static void Init()
	{
	}

	public bool Send()
	{
		if (To.Length == 0 || From.Length == 0 || Msg.Length == 0)
		{
			return false;
		}
		OleDbConnection dBConn = DBConn.GetDBConn();
		dBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = dBConn;
		oleDbCommand.CommandText = "insert into ADT_メールデータ (受信者コード, 送信者コード, 送信日, 送信時間, 件名, 本文, 患者コード, 重要度, 開封フラグ, 開封日, 開封時間, 受信者削除フラグ, 送信者削除フラグ) values (?, ?, " + DateTime.Now.ToString("yyyyMMdd") + ", " + DateTime.Now.ToString("HHmmss") + ", ?, ?, ?, ?, 0, 0, 0, 0, 0)";
		oleDbCommand.Parameters.Add("受信者コード", OleDbType.Numeric).Value = To;
		oleDbCommand.Parameters.Add("送信者コード", OleDbType.Numeric).Value = From;
		oleDbCommand.Parameters.Add("件名", OleDbType.VarWChar).Value = Title;
		oleDbCommand.Parameters.Add("本文", OleDbType.VarWChar).Value = Msg;
		if (PtId.Length > 0)
		{
			oleDbCommand.Parameters.Add("患者コード", OleDbType.Numeric).Value = PtId;
		}
		else
		{
			oleDbCommand.Parameters.Add("患者コード", OleDbType.Numeric).Value = 0;
		}
		if (Priority.Length > 0)
		{
			oleDbCommand.Parameters.Add("重要度", OleDbType.Numeric).Value = Priority;
		}
		else
		{
			oleDbCommand.Parameters.Add("重要度", OleDbType.Numeric).Value = 0;
		}
		oleDbCommand.ExecuteNonQuery();
		dBConn.Close();
		return true;
	}

	public static void Send(string sendTo, string sendFrom, string title, string msg, string ptId, string priority)
	{
		if (sendTo.Length != 0 && sendFrom.Length != 0 && msg.Length != 0)
		{
			KarteMessage karteMessage = new KarteMessage();
			karteMessage.To = sendTo;
			karteMessage.From = sendFrom;
			karteMessage.Title = title;
			karteMessage.Msg = msg;
			karteMessage.PtId = ptId;
			karteMessage.Priority = priority;
			karteMessage.Send();
		}
	}
}
