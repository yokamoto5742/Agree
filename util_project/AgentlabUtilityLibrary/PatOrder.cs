using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatOrder
{
	public enum MITEI
	{
		NO,
		YES
	}

	public enum DETAIL
	{
		NO,
		YES
	}

	public string OrderId = "";

	public string SekouDate = "";

	public string PtId = "";

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

	public List<PatOrderDetail> DetailList = new List<PatOrderDetail>();

	public string SekouDateString => DateTimeAgent.DateFormat(SekouDate, DateTimeAgent.DateFormatKind.LONG);

	public string SekouDateStringShort => DateTimeAgent.DateFormat(SekouDate, DateTimeAgent.DateFormatKind.SHORT);

	public string ShinkuString
	{
		get
		{
			string result = "";
			if (Dict.KouiDict.ContainsKey(Shinku))
			{
				result = Dict.KouiDict[Shinku];
			}
			return result;
		}
	}

	public string InOutString
	{
		get
		{
			string result = "";
			if (InOut.Equals("1"))
			{
				result = "外来";
			}
			else if (InOut.Equals("2"))
			{
				result = "入院";
			}
			return result;
		}
	}

	public string InOutStringShort
	{
		get
		{
			string result = "";
			if (InOut.Equals("1"))
			{
				result = "外";
			}
			else if (InOut.Equals("2"))
			{
				result = "入";
			}
			return result;
		}
	}

	public string DeptName
	{
		get
		{
			string result = "";
			if (Dict.DeptDict.ContainsKey(Dept))
			{
				result = Dict.DeptDict[Dept].ShortName;
			}
			return result;
		}
	}

	public string DoctorName
	{
		get
		{
			string result = "";
			if (Dict.DoctorDict.ContainsKey(Doctor))
			{
				result = Dict.DoctorDict[Doctor].Name;
			}
			return result;
		}
	}

	public string SekouString1
	{
		get
		{
			string result = "";
			if (Dict.SekouDict.ContainsKey(Sekou1))
			{
				result = Dict.SekouDict[Sekou1].ShortName;
			}
			return result;
		}
	}

	public string SekouString2
	{
		get
		{
			string result = "";
			if (Dict.SekouDict.ContainsKey(Sekou2))
			{
				result = Dict.SekouDict[Sekou2].ShortName;
			}
			return result;
		}
	}

	public string OrderDateString => DateTimeAgent.DateFormat(OrderDate, DateTimeAgent.DateFormatKind.LONG);

	public string OrderDateStringShort => DateTimeAgent.DateFormat(OrderDate, DateTimeAgent.DateFormatKind.SHORT);

	public string StartDateString => DateTimeAgent.DateFormat(StartDate, DateTimeAgent.DateFormatKind.LONG);

	public string StartDateStringShort => DateTimeAgent.DateFormat(StartDate, DateTimeAgent.DateFormatKind.SHORT);

	public string EndDateString => DateTimeAgent.DateFormat(EndDate, DateTimeAgent.DateFormatKind.LONG);

	public string EndDateStringShort => DateTimeAgent.DateFormat(EndDate, DateTimeAgent.DateFormatKind.SHORT);

	public static List<PatOrder> Load(string pt_id, string sekou_date, MITEI mitei, string in_out, List<string> shinku_list, List<string> dept_list, List<string> sekou1_list, DETAIL detail)
	{
		List<PatOrder> list = new List<PatOrder>();
		if (pt_id.Length == 0 || sekou_date.Length != 8)
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
		oleDbCommand.CommandText = "select * from D_ORDER_HEADER" + Env.DB_LINK + " where P_ID = ? and ORDER_DATE in (" + text + ")" + text2 + text3 + text4 + text5;
		oleDbCommand.Parameters.Add("P_ID", OleDbType.Numeric).Value = pt_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		List<string> list2 = new List<string>();
		while (oleDbDataReader.Read())
		{
			PatOrder patOrder = new PatOrder();
			patOrder.OrderId = oleDbDataReader["ORDER_NO"].ToString();
			patOrder.SekouDate = oleDbDataReader["ORDER_DATE"].ToString();
			patOrder.PtId = oleDbDataReader["P_ID"].ToString();
			patOrder.Ins = oleDbDataReader["P_HOKEN"].ToString();
			patOrder.Shinku = oleDbDataReader["SHINKU"].ToString();
			patOrder.InOut = oleDbDataReader["INOUT"].ToString();
			patOrder.Dept = oleDbDataReader["DEPT"].ToString();
			patOrder.Doctor = oleDbDataReader["DR"].ToString();
			patOrder.Sekou1 = oleDbDataReader["SEKOU_CODE"].ToString();
			patOrder.Sekou2 = oleDbDataReader["施行部署２"].ToString();
			patOrder.OrderDate = oleDbDataReader["DIRECTION_DATE"].ToString();
			patOrder.SekouFlg = oleDbDataReader["SEKOU_FLG"].ToString();
			patOrder.StartDate = oleDbDataReader["DATE_S"].ToString();
			patOrder.EndDate = oleDbDataReader["DATE_E"].ToString();
			patOrder.SOAP = oleDbDataReader["ＳＯＡＰ表示名称"].ToString();
			list2.Add(patOrder.OrderId);
			list.Add(patOrder);
		}
		openDBConn.Close();
		if (detail == DETAIL.YES)
		{
			List<PatOrderDetail> list3 = PatOrderDetail.Load(list2);
			foreach (PatOrderDetail item4 in list3)
			{
				foreach (PatOrder item5 in list)
				{
					if (item5.OrderId.Equals(item4.OrderId))
					{
						item5.DetailList.Add(item4);
						break;
					}
				}
			}
		}
		return list;
	}

	public static List<PatOrder> FindByStartEndDate(string pt_id, string start_date, string end_date, string in_out, List<string> shinku_list, List<string> dept_list, DETAIL detail)
	{
		List<PatOrder> list = new List<PatOrder>();
		if (pt_id.Length == 0)
		{
			return list;
		}
		string text = "";
		text = ((start_date.Length > 0) ? ((end_date.Length <= 0) ? (" and DATE_S <= " + DateTime.Today.ToString("yyyyMMdd") + " and DATE_E >= " + start_date) : (" and DATE_S <= " + end_date + " and DATE_E >= " + start_date)) : ((end_date.Length <= 0) ? (" and DATE_S <= " + DateTime.Today.ToString("yyyyMMdd") + " and DATE_E >= " + DateTime.Today.ToString("yyyyMMdd")) : (" and DATE_S <= " + end_date + " and DATE_E >= " + DateTime.Today.ToString("yyyyMMdd"))));
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
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select * from D_ORDER_HEADER" + Env.DB_LINK + " where P_ID = ?" + text + text2 + text3 + text4;
		oleDbCommand.Parameters.Add("P_ID", OleDbType.Numeric).Value = pt_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		List<string> list2 = new List<string>();
		while (oleDbDataReader.Read())
		{
			PatOrder patOrder = new PatOrder();
			patOrder.OrderId = oleDbDataReader["ORDER_NO"].ToString();
			patOrder.SekouDate = oleDbDataReader["ORDER_DATE"].ToString();
			patOrder.PtId = oleDbDataReader["P_ID"].ToString();
			patOrder.Ins = oleDbDataReader["P_HOKEN"].ToString();
			patOrder.Shinku = oleDbDataReader["SHINKU"].ToString();
			patOrder.InOut = oleDbDataReader["INOUT"].ToString();
			patOrder.Dept = oleDbDataReader["DEPT"].ToString();
			patOrder.Doctor = oleDbDataReader["DR"].ToString();
			patOrder.Sekou1 = oleDbDataReader["SEKOU_CODE"].ToString();
			patOrder.OrderDate = oleDbDataReader["DIRECTION_DATE"].ToString();
			patOrder.SekouFlg = oleDbDataReader["SEKOU_FLG"].ToString();
			patOrder.StartDate = oleDbDataReader["DATE_S"].ToString();
			patOrder.EndDate = oleDbDataReader["DATE_E"].ToString();
			list2.Add(patOrder.OrderId);
			list.Add(patOrder);
		}
		openDBConn.Close();
		if (detail == DETAIL.YES)
		{
			List<PatOrderDetail> list3 = PatOrderDetail.Load(list2);
			foreach (PatOrderDetail item3 in list3)
			{
				foreach (PatOrder item4 in list)
				{
					if (item4.OrderId.Equals(item3.OrderId))
					{
						item4.DetailList.Add(item3);
						break;
					}
				}
			}
		}
		return list;
	}
}
