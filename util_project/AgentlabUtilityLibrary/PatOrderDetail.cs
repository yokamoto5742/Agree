using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatOrderDetail
{
	public string OrderId = "";

	public string DetailId = "";

	public string PtId = "";

	public string SekouDate = "";

	public string Code = "";

	public string Name = "";

	public string Amount = "";

	public string Count = "";

	public static List<PatOrderDetail> Load(string order_id)
	{
		List<PatOrderDetail> list = new List<PatOrderDetail>();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select * from D_ORDER_DETAIL" + Env.DB_LINK + " where ORDER_NO = ?";
		oleDbCommand.Parameters.Add("ORDER_NO", OleDbType.Numeric).Value = order_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatOrderDetail patOrderDetail = new PatOrderDetail();
			patOrderDetail.OrderId = order_id;
			patOrderDetail.DetailId = oleDbDataReader["明細UKE_INDEX"].ToString();
			patOrderDetail.PtId = oleDbDataReader["P_ID"].ToString();
			patOrderDetail.SekouDate = oleDbDataReader["ORDER_DATE"].ToString();
			patOrderDetail.Code = oleDbDataReader["ORDER_CODE"].ToString();
			patOrderDetail.Name = oleDbDataReader["ORDER_COMMENT"].ToString();
			patOrderDetail.Amount = oleDbDataReader["QTY"].ToString();
			patOrderDetail.Count = oleDbDataReader["TIMES"].ToString();
			list.Add(patOrderDetail);
		}
		openDBConn.Close();
		return list;
	}

	public static List<PatOrderDetail> Load(List<string> order_id_list)
	{
		List<PatOrderDetail> list = new List<PatOrderDetail>();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		string text = "";
		foreach (string item in order_id_list)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += item;
		}
		if (text.Length == 0)
		{
			return list;
		}
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select * from D_ORDER_DETAIL" + Env.DB_LINK + " where ORDER_NO in (" + text + ")";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatOrderDetail patOrderDetail = new PatOrderDetail();
			patOrderDetail.OrderId = oleDbDataReader["ORDER_NO"].ToString();
			patOrderDetail.DetailId = oleDbDataReader["DETAIL_SEQ"].ToString();
			patOrderDetail.PtId = oleDbDataReader["P_ID"].ToString();
			patOrderDetail.SekouDate = oleDbDataReader["ORDER_DATE"].ToString();
			patOrderDetail.Code = oleDbDataReader["ORDER_CODE"].ToString();
			patOrderDetail.Name = oleDbDataReader["ORDER_COMMENT"].ToString();
			patOrderDetail.Amount = oleDbDataReader["QTY"].ToString();
			patOrderDetail.Count = oleDbDataReader["TIMES"].ToString();
			list.Add(patOrderDetail);
		}
		openDBConn.Close();
		return list;
	}

	public static List<PatOrderDetail> FindByMasterCode(List<string> master_code_list, string start_date, string end_date)
	{
		List<PatOrderDetail> list = new List<PatOrderDetail>();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		string text = "";
		foreach (string item in master_code_list)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text = text + "'" + item + "'";
		}
		if (text.Length == 0)
		{
			return list;
		}
		string text2 = "";
		text2 = ((start_date.Length != 8) ? (DateTime.Now.ToString("yyyyMM") + "00") : start_date);
		string text3 = "";
		text3 = ((end_date.Length != 8) ? (DateTime.Now.ToString("yyyyMM") + "99") : end_date);
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select * from D_ORDER_DETAIL" + Env.DB_LINK + " where ORDER_DATE >= " + text2 + " and ORDER_DATE <= " + text3 + " and ORDER_CODE in (" + text + ")";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatOrderDetail patOrderDetail = new PatOrderDetail();
			patOrderDetail.OrderId = oleDbDataReader["ORDER_NO"].ToString();
			patOrderDetail.DetailId = oleDbDataReader["明細UKE_INDEX"].ToString();
			patOrderDetail.PtId = oleDbDataReader["P_ID"].ToString();
			patOrderDetail.SekouDate = oleDbDataReader["ORDER_DATE"].ToString();
			patOrderDetail.Code = oleDbDataReader["ORDER_CODE"].ToString();
			patOrderDetail.Name = oleDbDataReader["ORDER_COMMENT"].ToString();
			patOrderDetail.Amount = oleDbDataReader["QTY"].ToString();
			patOrderDetail.Count = oleDbDataReader["TIMES"].ToString();
			list.Add(patOrderDetail);
		}
		openDBConn.Close();
		return list;
	}
}
