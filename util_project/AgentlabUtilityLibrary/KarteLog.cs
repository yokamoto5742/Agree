using System;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class KarteLog
{
	public static void Log(DateTime log_time, string staff_id, string pt_id, string log_cont)
	{
		try
		{
			OleDbConnection openDBConn = DBConn.GetOpenDBConn();
			openDBConn.Open();
			OleDbCommand oleDbCommand = new OleDbCommand();
			oleDbCommand.Connection = openDBConn;
			oleDbCommand.CommandText = "insert into KARTE_LOG (KARTE_LOG_ID, LOG_TIME, STAFF, PATIENT_ID, LOG_CONT) values (KARTE_LOG_SEQ.nextval, to_date('" + log_time.ToString("yyyy/MM/dd HH:mm:ss") + "', 'YYYY/MM/DD HH24:MI:SS'), " + staff_id + ", ?, ?)";
			OleDbParameterCollection parameters = oleDbCommand.Parameters;
			parameters.Add("PATIENT_ID", OleDbType.Numeric).Value = pt_id;
			parameters.Add("LOG_CONT", OleDbType.VarWChar).Value = log_cont;
			oleDbCommand.ExecuteNonQuery();
			openDBConn.Close();
		}
		catch (Exception)
		{
		}
	}

	public static void Log(DateTime log_time, string staff_id, string log_cont)
	{
		try
		{
			OleDbConnection openDBConn = DBConn.GetOpenDBConn();
			openDBConn.Open();
			OleDbCommand oleDbCommand = new OleDbCommand();
			oleDbCommand.Connection = openDBConn;
			oleDbCommand.CommandText = "insert into KARTE_LOG (KARTE_LOG_ID, LOG_TIME, STAFF, LOG_CONT) values (KARTE_LOG_SEQ.nextval, to_date('" + log_time.ToString("yyyy/MM/dd HH:mm:ss") + "', 'YYYY/MM/DD HH24:MI:SS'), " + staff_id + ", ?)";
			OleDbParameterCollection parameters = oleDbCommand.Parameters;
			parameters.Add("LOG_CONT", OleDbType.VarWChar).Value = log_cont;
			oleDbCommand.ExecuteNonQuery();
			openDBConn.Close();
		}
		catch (Exception)
		{
		}
	}
}
