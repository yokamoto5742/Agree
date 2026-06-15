using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatSekou
{
	public enum MITEI
	{
		NO,
		YES
	}

	public string OrderId = "";

	public string SekouDate = "";

	public string Seq1 = "";

	public string PtId = "";

	public string PtKana = "";

	public string PtName = "";

	public string Ins = "";

	public string Shinku = "";

	public string InOut = "";

	public string Dept = "";

	public string Doctor = "";

	public string Sekou1 = "";

	public string Sekou2 = "";

	public string OrderDate = "";

	public string SekouFlg = "";

	public string StartDate = "";

	public string EndDate = "";

	public string SOAP = "";

	public static List<PatSekou> Load(string sekou_date, MITEI mitei, string in_out, List<string> shinku_list, List<string> dept_list, List<string> sekou1_list)
	{
		List<PatSekou> list = new List<PatSekou>();
		if (sekou_date.Length != 8)
		{
			return list;
		}
		string text = sekou_date;
		if (mitei == MITEI.YES)
		{
			text += ",99999999";
		}
		string text2 = "";
		if (in_out.Equals("1"))
		{
			text2 = " and INOUT = 1";
		}
		else if (in_out.Equals("2"))
		{
			text2 = " and INOUT = 2";
		}
		string text3 = "";
		foreach (string item in shinku_list)
		{
			if (text3.Length > 0)
			{
				text3 += ",";
			}
			text3 += item;
		}
		if (text3.Length > 0)
		{
			text3 = " and SHINKU in (" + text3 + ")";
		}
		string text4 = "";
		foreach (string item2 in dept_list)
		{
			if (text4.Length > 0)
			{
				text4 += ",";
			}
			text4 += item2;
		}
		if (text4.Length > 0)
		{
			text4 = " and DEPT in (" + text4 + ")";
		}
		string text5 = "";
		foreach (string item3 in sekou1_list)
		{
			if (text5.Length > 0)
			{
				text5 += ",";
			}
			text5 += item3;
		}
		if (text5.Length > 0)
		{
			text5 = " and SEKOU_CODE in (" + text5 + ")";
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select ORDER_NO, ORDER_DATE, P_ID, Trim(P_KANA) as P_KANA, Trim(P_NAME) as 氏名, P_HOKEN, SHINKU, INOUT, DEPT, DR, SEKOU_CODE, 施行部署２, DIRECTION_DATE, SEKOU_FLG, DATE_S, DATE_E, ＳＯＡＰ表示名称 from D_ORDER_HEADER" + Env.DB_LINK + " inner join M_PATIENT" + Env.DB_LINK + " on P_ID = P_ID where ORDER_DATE in (" + text + ")" + text2 + text3 + text4 + text5;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		List<string> list2 = new List<string>();
		while (oleDbDataReader.Read())
		{
			PatSekou patSekou = new PatSekou();
			patSekou.OrderId = oleDbDataReader["ORDER_NO"].ToString();
			patSekou.SekouDate = oleDbDataReader["ORDER_DATE"].ToString();
			patSekou.PtId = oleDbDataReader["P_ID"].ToString();
			patSekou.PtKana = oleDbDataReader["P_KANA"].ToString();
			patSekou.PtName = oleDbDataReader["氏名"].ToString();
			patSekou.Ins = oleDbDataReader["P_HOKEN"].ToString();
			patSekou.Shinku = oleDbDataReader["SHINKU"].ToString();
			patSekou.InOut = oleDbDataReader["INOUT"].ToString();
			patSekou.Dept = oleDbDataReader["DEPT"].ToString();
			patSekou.Doctor = oleDbDataReader["DR"].ToString();
			patSekou.Sekou1 = oleDbDataReader["SEKOU_CODE"].ToString();
			patSekou.Sekou2 = oleDbDataReader["施行部署２"].ToString();
			patSekou.OrderDate = oleDbDataReader["DIRECTION_DATE"].ToString();
			patSekou.SekouFlg = oleDbDataReader["SEKOU_FLG"].ToString();
			patSekou.StartDate = oleDbDataReader["DATE_S"].ToString();
			patSekou.EndDate = oleDbDataReader["DATE_E"].ToString();
			patSekou.SOAP = oleDbDataReader["ＳＯＡＰ表示名称"].ToString();
			if (!list2.Contains(patSekou.PtId))
			{
				list2.Add(patSekou.PtId);
			}
			list.Add(patSekou);
		}
		openDBConn.Close();
		Dictionary<string, string> onedayLastSeq = PatOut.GetOnedayLastSeq(list2, sekou_date);
		foreach (PatSekou item4 in list)
		{
			if (onedayLastSeq.ContainsKey(item4.PtId))
			{
				item4.Seq1 = onedayLastSeq[item4.PtId];
			}
		}
		return list;
	}
}
