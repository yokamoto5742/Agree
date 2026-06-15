using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public static class SoapDisp
{
	public static string GetText(string orderNo, string ptid, string orderDate)
	{
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select ORDER_COMMENT from D_ORDER_DETAIL" + Env.DB_LINK + " where ORDER_NO = " + orderNo + " order by DETAIL_SEQ";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		string text = "";
		while (oleDbDataReader.Read())
		{
			text = text + oleDbDataReader["ORDER_COMMENT"].ToString() + "\r\n";
		}
		oleDbDataReader.Close();
		openDBConn.Close();
		return text;
	}

	public static string GetText(string orderNo)
	{
		OleDbConnection dBConn = DBConn.GetDBConn();
		dBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = dBConn;
		oleDbCommand.CommandText = "select substr(oh.ORDER_DATE,0,4) || '/' || substr(oh.ORDER_DATE,5,2) || '/' || substr(oh.ORDER_DATE,7,2) as H_DATE,\r\n dp.S_NAME as DP_NAME, dr.NAME as DR_NAME, sd.SOAP_TEXT as order_comment, oh.SEKOU_FLG, oh.BILL_FLG\r\n from D_ORDER_HEADER oh, M_DR dr, M_DEPT dp, D_SOAP_DETAIL sd\r\n where oh.ORDER_NO = sd.ORDER_NO\r\n and sd.ORDER_NO = ? \r\n and sd.SOAP_TYPE = 21\r\n and oh.DR = dr.CODE(+)\r\n and oh.DEPT = dp.CODE(+)\r\n order by oh.ORDER_DATE desc";
		oleDbCommand.Parameters.Add("ORDER_NO", OleDbType.Numeric).Value = orderNo;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		string text = "";
		while (oleDbDataReader.Read())
		{
			text = text + oleDbDataReader["ORDER_COMMENT"].ToString() + "\r\n";
		}
		oleDbDataReader.Close();
		dBConn.Close();
		return text;
	}
}
