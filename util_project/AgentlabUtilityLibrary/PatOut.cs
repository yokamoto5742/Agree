using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatOut : PatBase
{
	public string ComeDate = "";

	public string Seq1 = "";

	public string Seq2 = "";

	public string Kind = "";

	public string Dept = "";

	public string Doctor = "";

	public string Time1 = "";

	public string Time2 = "";

	public string Time3 = "";

	public string Time4 = "";

	public string Time5 = "";

	public static List<PatOut> GetList(string come_date, string dept, string doctor)
	{
		List<PatOut> list = new List<PatOut>();
		if (come_date.Length != 8)
		{
			return list;
		}
		if (dept.Length == 0)
		{
			return list;
		}
		string text = " UKE_DATE = " + come_date;
		string text2 = " and (CHANGE_DEPT = " + dept + " or (DEPT = " + dept + " and (CHANGE_DEPT = 0 or CHANGE_DEPT is null)))";
		string text3 = "";
		if (doctor.Length > 0)
		{
			text3 = " and (CHANGE_DR = " + doctor + " or (DR = " + doctor + " and (CHANGE_DR = 0 or CHANGE_DR is null)))";
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "Select UKE_NO, DEPT_NO, UKE_TIME, P_ID, P_KANA, P_NAME, P_SEX, P_AGE, Trim(S_NAME) as 種別, DR, CHANGE_DR, SHINSATSU_TIME_S, SHINSATSU_TIME_M, SHINSATSU_TIME_E, BILL_TIME from D_UKETSUKE" + Env.DB_LINK + " left join M_UKENAME" + Env.DB_LINK + " on D_UKETSUKE.UKE_TYPE = M_UKENAME.CODE where " + text + text2 + text3 + " order by UKE_NO";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatOut patOut = new PatOut();
			patOut.Id = oleDbDataReader["P_ID"].ToString();
			patOut.Kana = oleDbDataReader["P_KANA"].ToString();
			patOut.Name = oleDbDataReader["P_NAME"].ToString();
			patOut.Sex = oleDbDataReader["P_SEX"].ToString();
			patOut.Age = oleDbDataReader["P_AGE"].ToString();
			patOut.ComeDate = come_date;
			patOut.Seq1 = oleDbDataReader["UKE_NO"].ToString();
			patOut.Seq2 = oleDbDataReader["DEPT_NO"].ToString();
			patOut.Kind = oleDbDataReader["種別"].ToString();
			patOut.Dept = dept;
			if (oleDbDataReader["CHANGE_DR"].ToString().Length > 0 && !oleDbDataReader["CHANGE_DR"].ToString().Equals("0"))
			{
				patOut.Doctor = oleDbDataReader["CHANGE_DR"].ToString();
			}
			else if (oleDbDataReader["DR"].ToString().Length > 0 && !oleDbDataReader["DR"].ToString().Equals("0"))
			{
				patOut.Doctor = oleDbDataReader["DR"].ToString();
			}
			patOut.Time1 = oleDbDataReader["UKE_TIME"].ToString();
			patOut.Time2 = oleDbDataReader["SHINSATSU_TIME_S"].ToString();
			patOut.Time3 = oleDbDataReader["SHINSATSU_TIME_M"].ToString();
			patOut.Time4 = oleDbDataReader["SHINSATSU_TIME_E"].ToString();
			patOut.Time5 = oleDbDataReader["BILL_TIME"].ToString();
			if (patOut.Time1 != "")
			{
				patOut.Time1 = patOut.Time1.PadLeft(6, '0').Substring(0, 4);
			}
			if (patOut.Time2 != "")
			{
				patOut.Time2 = patOut.Time2.PadLeft(6, '0').Substring(0, 4);
			}
			if (patOut.Time3 != "")
			{
				patOut.Time3 = patOut.Time3.PadLeft(6, '0').Substring(0, 4);
			}
			if (patOut.Time4 != "")
			{
				patOut.Time4 = patOut.Time4.PadLeft(6, '0').Substring(0, 4);
			}
			if (patOut.Time5 != "")
			{
				patOut.Time5 = patOut.Time5.PadLeft(6, '0').Substring(0, 4);
			}
			list.Add(patOut);
		}
		oleDbDataReader.Close();
		return list;
	}

	public static List<PatOut> GetHistory(string pt_id, string start_date)
	{
		List<PatOut> list = new List<PatOut>();
		int result = 0;
		if (pt_id.Length == 0 || !int.TryParse(pt_id, out result))
		{
			return list;
		}
		string text = start_date;
		if (text.Length != 8)
		{
			text = DateTime.Now.AddMonths(-6).ToString("yyyyMMdd");
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select UKE_DATE, CHANGE_DEPT, CHANGE_DR, UKE_TIME, SHINSATSU_TIME_S, SHINSATSU_TIME_M, SHINSATSU_TIME_E, BILL_TIME from D_UKETSUKE" + Env.DB_LINK + " where P_ID = ? and UKE_DATE >= ? order by UKE_DATE desc";
		oleDbCommand.Parameters.Add("P_ID", OleDbType.Numeric).Value = pt_id;
		oleDbCommand.Parameters.Add("UKE_DATE", OleDbType.Numeric).Value = text;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatOut patOut = new PatOut();
			patOut.Id = pt_id;
			patOut.ComeDate = oleDbDataReader["UKE_DATE"].ToString().PadRight(8, '0');
			patOut.Dept = oleDbDataReader["CHANGE_DEPT"].ToString();
			patOut.Doctor = oleDbDataReader["CHANGE_DR"].ToString();
			patOut.Time1 = oleDbDataReader["UKE_TIME"].ToString();
			patOut.Time2 = oleDbDataReader["SHINSATSU_TIME_S"].ToString();
			patOut.Time3 = oleDbDataReader["SHINSATSU_TIME_M"].ToString();
			patOut.Time4 = oleDbDataReader["SHINSATSU_TIME_E"].ToString();
			patOut.Time5 = oleDbDataReader["BILL_TIME"].ToString();
			list.Add(patOut);
		}
		oleDbDataReader.Close();
		oleDbCommand.CommandText = "select UKE_DATE, CHANGE_DEPT, CHANGE_DR, UKE_TIME, SHINSATSU_TIME_S, SHINSATSU_TIME_M, SHINSATSU_TIME_E, BILL_TIME from HIST_D_UKETSUKE where P_ID = ? and UKE_DATE >= ? order by UKE_DATE desc";
		oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatOut patOut2 = new PatOut();
			patOut2.Id = pt_id;
			patOut2.ComeDate = oleDbDataReader["UKE_DATE"].ToString().PadRight(8, '0');
			patOut2.Dept = oleDbDataReader["CHANGE_DEPT"].ToString();
			patOut2.Doctor = oleDbDataReader["CHANGE_DR"].ToString();
			patOut2.Time1 = oleDbDataReader["UKE_TIME"].ToString();
			patOut2.Time2 = oleDbDataReader["SHINSATSU_TIME_S"].ToString();
			patOut2.Time3 = oleDbDataReader["SHINSATSU_TIME_M"].ToString();
			patOut2.Time4 = oleDbDataReader["SHINSATSU_TIME_E"].ToString();
			patOut2.Time5 = oleDbDataReader["BILL_TIME"].ToString();
			list.Add(patOut2);
		}
		oleDbDataReader.Close();
		openDBConn.Close();
		return list;
	}

	public static List<PatOut> GetOneday(string pt_id, string come_date)
	{
		List<PatOut> list = new List<PatOut>();
		int result = 0;
		if (pt_id.Length == 0 || !int.TryParse(pt_id, out result))
		{
			return list;
		}
		string text = come_date;
		if (text.Length != 8)
		{
			text = DateTime.Now.ToString("yyyyMMdd");
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select UKE_NO, P_KANA, UKE_TIME, DEPT, CHANGE_DEPT from D_UKETSUKE" + Env.DB_LINK + " where P_ID = ? and UKE_DATE = ?";
		oleDbCommand.Parameters.Add("P_ID", OleDbType.Numeric).Value = pt_id;
		oleDbCommand.Parameters.Add("UKE_DATE", OleDbType.Numeric).Value = text;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatOut patOut = new PatOut();
			patOut.Id = pt_id;
			patOut.ComeDate = text;
			patOut.Kana = oleDbDataReader["P_KANA"].ToString();
			patOut.Seq1 = oleDbDataReader["UKE_NO"].ToString();
			patOut.Time1 = oleDbDataReader["UKE_TIME"].ToString();
			if (oleDbDataReader["CHANGE_DEPT"].ToString().Length > 0 && !oleDbDataReader["CHANGE_DEPT"].ToString().Equals("0"))
			{
				patOut.Dept = oleDbDataReader["CHANGE_DEPT"].ToString();
			}
			else
			{
				patOut.Dept = oleDbDataReader["DEPT"].ToString();
			}
			list.Add(patOut);
		}
		oleDbDataReader.Close();
		if (list.Count == 0)
		{
			oleDbCommand.CommandText = "select UKE_NO, P_KANA, UKE_TIME, DEPT, CHANGE_DEPT from HIST_D_UKETSUKE where P_ID = ? and UKE_DATE = ?";
			oleDbDataReader = oleDbCommand.ExecuteReader();
			while (oleDbDataReader.Read())
			{
				PatOut patOut2 = new PatOut();
				patOut2.Id = pt_id;
				patOut2.ComeDate = text;
				patOut2.Kana = oleDbDataReader["P_KANA"].ToString();
				patOut2.Seq1 = oleDbDataReader["UKE_NO"].ToString();
				patOut2.Time1 = oleDbDataReader["UKE_TIME"].ToString();
				if (oleDbDataReader["CHANGE_DEPT"].ToString().Length > 0 && !oleDbDataReader["CHANGE_DEPT"].ToString().Equals("0"))
				{
					patOut2.Dept = oleDbDataReader["CHANGE_DEPT"].ToString();
				}
				else
				{
					patOut2.Dept = oleDbDataReader["DEPT"].ToString();
				}
				list.Add(patOut2);
			}
			oleDbDataReader.Close();
		}
		openDBConn.Close();
		return list;
	}

	public static List<PatOut> GetOnedayLast(string pt_id, string come_date)
	{
		List<PatOut> list = new List<PatOut>();
		int result = 0;
		if (pt_id.Length == 0 || !int.TryParse(pt_id, out result))
		{
			return list;
		}
		string text = come_date;
		if (text.Length != 8)
		{
			text = DateTime.Now.ToString("yyyyMMdd");
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select UKE_NO, P_KANA, UKE_TIME, DEPT, CHANGE_DEPT from D_UKETSUKE" + Env.DB_LINK + " where P_ID = ? and UKE_DATE = ? order by UKE_TIME desc";
		oleDbCommand.Parameters.Add("P_ID", OleDbType.Numeric).Value = pt_id;
		oleDbCommand.Parameters.Add("UKE_DATE", OleDbType.Numeric).Value = text;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		string text2 = "";
		while (oleDbDataReader.Read() && (text2.Length <= 0 || oleDbDataReader["UKE_TIME"].ToString().Equals(text2)))
		{
			text2 = oleDbDataReader["UKE_TIME"].ToString();
			PatOut patOut = new PatOut();
			patOut.Id = pt_id;
			patOut.ComeDate = text;
			patOut.Kana = oleDbDataReader["P_KANA"].ToString();
			patOut.Seq1 = oleDbDataReader["UKE_NO"].ToString();
			patOut.Time1 = oleDbDataReader["UKE_TIME"].ToString();
			if (oleDbDataReader["CHANGE_DEPT"].ToString().Length > 0 && !oleDbDataReader["CHANGE_DEPT"].ToString().Equals("0"))
			{
				patOut.Dept = oleDbDataReader["CHANGE_DEPT"].ToString();
			}
			else
			{
				patOut.Dept = oleDbDataReader["DEPT"].ToString();
			}
			list.Add(patOut);
		}
		oleDbDataReader.Close();
		openDBConn.Close();
		return list;
	}

	public static Dictionary<string, string> GetOnedayLastSeq(List<string> pt_id_list, string come_date)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		new List<PatOut>();
		if (pt_id_list.Count == 0)
		{
			return dictionary;
		}
		string text = come_date;
		if (text.Length != 8)
		{
			text = DateTime.Now.ToString("yyyyMMdd");
		}
		string text2 = "";
		foreach (string item in pt_id_list)
		{
			if (text2.Length > 0)
			{
				text2 += ",";
			}
			text2 += item;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select UKE_NO, P_ID from D_UKETSUKE" + Env.DB_LINK + " where P_ID in (" + text2 + ") and UKE_DATE = ? order by P_ID, UKE_TIME desc";
		oleDbCommand.Parameters.Add("UKE_DATE", OleDbType.Numeric).Value = text;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		string text3 = "";
		while (oleDbDataReader.Read())
		{
			if (!oleDbDataReader["P_ID"].ToString().Equals(text3))
			{
				text3 = oleDbDataReader["P_ID"].ToString();
				if (!dictionary.ContainsKey(text3))
				{
					dictionary.Add(text3, oleDbDataReader["UKE_NO"].ToString());
				}
			}
		}
		oleDbDataReader.Close();
		openDBConn.Close();
		return dictionary;
	}
}
