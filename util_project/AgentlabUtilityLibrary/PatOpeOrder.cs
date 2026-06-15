using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatOpeOrder
{
	public string Id = "";

	public string Name = "";

	public string Sex = "";

	public string Birth = "";

	public int Date;

	public string Dept = "";

	public string Doctor = "";

	public string Ward = "";

	public string Room = "";

	public Dictionary<string, PatOpeOrderData> DictData = new Dictionary<string, PatOpeOrderData>();

	public string SexName
	{
		get
		{
			string result = "";
			if (Sex.Equals("1"))
			{
				result = "男";
			}
			else if (Sex.Equals("2"))
			{
				result = "女";
			}
			return result;
		}
	}

	public string Age => DateConvert.CalcAge(Birth, DateTime.Now.ToString("yyyyMMdd")).ToString();

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

	public string WardName
	{
		get
		{
			string result = "";
			if (Dict.WardDict.ContainsKey(Ward))
			{
				result = Dict.WardDict[Ward];
			}
			return result;
		}
	}

	public static PatOpeOrder Load(string pt_id, int date)
	{
		PatOpeOrder patOpeOrder = new PatOpeOrder();
		if (pt_id.Length == 0 || date.ToString().Length != 8)
		{
			return patOpeOrder;
		}
		patOpeOrder.Id = pt_id;
		patOpeOrder.Date = date;
		Patient patient = Patient.Load(pt_id);
		patOpeOrder.Name = patient.Name;
		patOpeOrder.Dept = patient.Dept;
		patOpeOrder.Doctor = patient.Doctor;
		patOpeOrder.Ward = patient.Ward;
		patOpeOrder.Room = patient.Room;
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "Select * from PATH手術指示データ" + Env.DB_LINK + " where 患者コード = ? and 日付 = ? order by 指示コード, 連番";
		oleDbCommand.Parameters.Add("患者コード", OleDbType.Numeric).Value = pt_id;
		oleDbCommand.Parameters.Add("日付", OleDbType.Numeric).Value = date;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		string text = "";
		while (oleDbDataReader.Read())
		{
			text = oleDbDataReader["指示コード"].ToString() + "-" + oleDbDataReader["連番"].ToString();
			if (OpeOrderMaster.Dict.ContainsKey(text) && !patOpeOrder.DictData.ContainsKey(text))
			{
				PatOpeOrderData patOpeOrderData = new PatOpeOrderData();
				patOpeOrderData.KeyCode = text;
				patOpeOrderData.Title = OpeOrderMaster.Dict[text].Title;
				if (oleDbDataReader["入力値"].ToString().Trim().Length > 0)
				{
					patOpeOrderData.Text = oleDbDataReader["入力値"].ToString().Trim() + " " + OpeOrderMaster.Dict[text].Text2;
				}
				else
				{
					patOpeOrderData.Text = OpeOrderMaster.Dict[text].Text1;
				}
				patOpeOrder.DictData.Add(text, patOpeOrderData);
			}
		}
		openDBConn.Close();
		return patOpeOrder;
	}

	public static List<PatOpeOrder> Load(int date)
	{
		List<PatOpeOrder> list = new List<PatOpeOrder>();
		if (date.ToString().Length != 8)
		{
			return list;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "Select 患者コード, 指示コード, 連番, 入力値, Trim(IM01RC_F04) as 氏名, Trim(IM01RC_F05) as 性別, Trim(IM01RC_F10) as 生年月日 from PATH手術指示データ" + Env.DB_LINK + " inner join IM01RC" + Env.DB_LINK + " on 患者コード = IM01RC_F01 where 日付 = ? order by 患者コード, 指示コード, 連番";
		oleDbCommand.Parameters.Add("日付", OleDbType.Numeric).Value = date;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		PatOpeOrder patOpeOrder = new PatOpeOrder();
		string text = "";
		List<string> list2 = new List<string>();
		while (oleDbDataReader.Read())
		{
			if (patOpeOrder.Id.Length == 0)
			{
				patOpeOrder.Id = oleDbDataReader["患者コード"].ToString();
				patOpeOrder.Name = oleDbDataReader["氏名"].ToString();
				patOpeOrder.Sex = oleDbDataReader["性別"].ToString();
				patOpeOrder.Birth = oleDbDataReader["生年月日"].ToString();
				patOpeOrder.Date = date;
				list2.Add(patOpeOrder.Id);
			}
			else if (!patOpeOrder.Id.Equals(oleDbDataReader["患者コード"].ToString()))
			{
				list.Add(patOpeOrder);
				patOpeOrder = new PatOpeOrder();
				patOpeOrder.Id = oleDbDataReader["患者コード"].ToString();
				patOpeOrder.Name = oleDbDataReader["氏名"].ToString();
				patOpeOrder.Sex = oleDbDataReader["性別"].ToString();
				patOpeOrder.Birth = oleDbDataReader["生年月日"].ToString();
				patOpeOrder.Date = date;
				list2.Add(patOpeOrder.Id);
			}
			text = oleDbDataReader["指示コード"].ToString() + "-" + oleDbDataReader["連番"].ToString();
			if (OpeOrderMaster.Dict.ContainsKey(text) && !patOpeOrder.DictData.ContainsKey(text))
			{
				PatOpeOrderData patOpeOrderData = new PatOpeOrderData();
				patOpeOrderData.KeyCode = text;
				patOpeOrderData.Title = OpeOrderMaster.Dict[text].Title;
				if (oleDbDataReader["入力値"].ToString().Trim().Length > 0)
				{
					patOpeOrderData.Text = oleDbDataReader["入力値"].ToString().Trim() + " " + OpeOrderMaster.Dict[text].Text2;
				}
				else
				{
					patOpeOrderData.Text = OpeOrderMaster.Dict[text].Text1;
				}
				patOpeOrder.DictData.Add(text, patOpeOrderData);
			}
		}
		if (patOpeOrder.Id.Length > 0)
		{
			list.Add(patOpeOrder);
		}
		openDBConn.Close();
		if (list2.Count > 0)
		{
			Dictionary<string, PatIn> list3 = PatIn.GetList(list2);
			foreach (PatOpeOrder item in list)
			{
				if (list3.ContainsKey(item.Id))
				{
					item.Dept = list3[item.Id].Dept;
					item.Doctor = list3[item.Id].Doctor;
					item.Ward = list3[item.Id].Ward;
					item.Room = list3[item.Id].Room;
				}
			}
		}
		return list;
	}

	public static List<PatOpeOrder> Load(int date, List<string> pt_list)
	{
		List<PatOpeOrder> list = new List<PatOpeOrder>();
		if (date.ToString().Length != 8 || pt_list.Count == 0)
		{
			return list;
		}
		string text = "";
		string text2 = "";
		foreach (string item in pt_list)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += item;
		}
		if (text.Length > 0)
		{
			text2 = " and 患者コード in (" + text + ")";
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "Select 患者コード, 指示コード, 連番, 入力値, Trim(IM01RC_F04) as 氏名, Trim(IM01RC_F05) as 性別, Trim(IM01RC_F10) as 生年月日 from PATH手術指示データ" + Env.DB_LINK + " inner join IM01RC" + Env.DB_LINK + " on 患者コード = IM01RC_F01 where 日付 = ?" + text2 + " order by 患者コード, 指示コード, 連番";
		oleDbCommand.Parameters.Add("日付", OleDbType.Numeric).Value = date;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		PatOpeOrder patOpeOrder = new PatOpeOrder();
		string text3 = "";
		while (oleDbDataReader.Read())
		{
			if (patOpeOrder.Id.Length == 0)
			{
				patOpeOrder.Id = oleDbDataReader["患者コード"].ToString();
				patOpeOrder.Name = oleDbDataReader["氏名"].ToString();
				patOpeOrder.Sex = oleDbDataReader["性別"].ToString();
				patOpeOrder.Birth = oleDbDataReader["生年月日"].ToString();
				patOpeOrder.Date = date;
			}
			else if (!patOpeOrder.Id.Equals(oleDbDataReader["患者コード"].ToString()))
			{
				list.Add(patOpeOrder);
				patOpeOrder = new PatOpeOrder();
				patOpeOrder.Id = oleDbDataReader["患者コード"].ToString();
				patOpeOrder.Name = oleDbDataReader["氏名"].ToString();
				patOpeOrder.Sex = oleDbDataReader["性別"].ToString();
				patOpeOrder.Birth = oleDbDataReader["生年月日"].ToString();
				patOpeOrder.Date = date;
			}
			text3 = oleDbDataReader["指示コード"].ToString() + "-" + oleDbDataReader["連番"].ToString();
			if (OpeOrderMaster.Dict.ContainsKey(text3) && !patOpeOrder.DictData.ContainsKey(text3))
			{
				PatOpeOrderData patOpeOrderData = new PatOpeOrderData();
				patOpeOrderData.KeyCode = text3;
				patOpeOrderData.Title = OpeOrderMaster.Dict[text3].Title;
				if (oleDbDataReader["入力値"].ToString().Trim().Length > 0)
				{
					patOpeOrderData.Text = oleDbDataReader["入力値"].ToString().Trim() + " " + OpeOrderMaster.Dict[text3].Text2;
				}
				else
				{
					patOpeOrderData.Text = OpeOrderMaster.Dict[text3].Text1;
				}
				patOpeOrder.DictData.Add(text3, patOpeOrderData);
			}
		}
		if (patOpeOrder.Id.Length > 0)
		{
			list.Add(patOpeOrder);
		}
		openDBConn.Close();
		if (pt_list.Count > 0)
		{
			Dictionary<string, PatIn> list2 = PatIn.GetList(pt_list);
			foreach (PatOpeOrder item2 in list)
			{
				if (list2.ContainsKey(item2.Id))
				{
					item2.Dept = list2[item2.Id].Dept;
					item2.Doctor = list2[item2.Id].Doctor;
					item2.Ward = list2[item2.Id].Ward;
					item2.Room = list2[item2.Id].Room;
				}
			}
		}
		return list;
	}

	public static List<PatOpeOrder> Load(int start_date, int end_date)
	{
		List<PatOpeOrder> list = new List<PatOpeOrder>();
		if (start_date.ToString().Length != 8 || end_date.ToString().Length != 8)
		{
			return list;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "Select 日付, 患者コード, 指示コード, 連番, 入力値, Trim(IM01RC_F04) as 氏名, Trim(IM01RC_F05) as 性別, Trim(IM01RC_F10) as 生年月日 from PATH手術指示データ" + Env.DB_LINK + " inner join IM01RC" + Env.DB_LINK + " on 患者コード = IM01RC_F01 where 日付 >= " + start_date + " and 日付 <= " + end_date + " order by 日付, 患者コード, 指示コード, 連番";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		PatOpeOrder patOpeOrder = new PatOpeOrder();
		string text = "";
		List<string> list2 = new List<string>();
		while (oleDbDataReader.Read())
		{
			if (patOpeOrder.Id.Length == 0)
			{
				patOpeOrder.Id = oleDbDataReader["患者コード"].ToString();
				patOpeOrder.Name = oleDbDataReader["氏名"].ToString();
				patOpeOrder.Sex = oleDbDataReader["性別"].ToString();
				patOpeOrder.Birth = oleDbDataReader["生年月日"].ToString();
				int.TryParse(oleDbDataReader["日付"].ToString(), out patOpeOrder.Date);
				list2.Add(patOpeOrder.Id);
			}
			else if (!patOpeOrder.Date.ToString().Equals(oleDbDataReader["日付"].ToString()) || !patOpeOrder.Id.Equals(oleDbDataReader["患者コード"].ToString()))
			{
				list.Add(patOpeOrder);
				patOpeOrder = new PatOpeOrder();
				patOpeOrder.Id = oleDbDataReader["患者コード"].ToString();
				patOpeOrder.Name = oleDbDataReader["氏名"].ToString();
				patOpeOrder.Sex = oleDbDataReader["性別"].ToString();
				patOpeOrder.Birth = oleDbDataReader["生年月日"].ToString();
				int.TryParse(oleDbDataReader["日付"].ToString(), out patOpeOrder.Date);
				list2.Add(patOpeOrder.Id);
			}
			text = oleDbDataReader["指示コード"].ToString() + "-" + oleDbDataReader["連番"].ToString();
			if (OpeOrderMaster.Dict.ContainsKey(text) && !patOpeOrder.DictData.ContainsKey(text))
			{
				PatOpeOrderData patOpeOrderData = new PatOpeOrderData();
				patOpeOrderData.KeyCode = text;
				patOpeOrderData.Title = OpeOrderMaster.Dict[text].Title;
				if (oleDbDataReader["入力値"].ToString().Trim().Length > 0)
				{
					patOpeOrderData.Text = oleDbDataReader["入力値"].ToString().Trim() + " " + OpeOrderMaster.Dict[text].Text2;
				}
				else
				{
					patOpeOrderData.Text = OpeOrderMaster.Dict[text].Text1;
				}
				patOpeOrder.DictData.Add(text, patOpeOrderData);
			}
		}
		if (patOpeOrder.Id.Length > 0)
		{
			list.Add(patOpeOrder);
		}
		openDBConn.Close();
		if (list2.Count > 0)
		{
			Dictionary<string, PatIn> list3 = PatIn.GetList(list2);
			foreach (PatOpeOrder item in list)
			{
				if (list3.ContainsKey(item.Id))
				{
					item.Dept = list3[item.Id].Dept;
					item.Doctor = list3[item.Id].Doctor;
					item.Ward = list3[item.Id].Ward;
					item.Room = list3[item.Id].Room;
				}
			}
		}
		return list;
	}
}
