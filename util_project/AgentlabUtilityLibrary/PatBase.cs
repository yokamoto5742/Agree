using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatBase
{
	public string Id = "";

	public string Name = "";

	public string Kana = "";

	public string Birth = "";

	private string age = "";

	public string Sex = "";

	public string Tel = "";

	public string Post = "";

	public string Addr = "";

	public string InOut = "";

	private Dictionary<string, PatIns> insDict = new Dictionary<string, PatIns>();

	public string InsDefault = "";

	public string BirthString => DateTimeAgent.DateFormat(Birth, DateTimeAgent.DateFormatKind.LONG);

	public string BirthStringJ => DateTimeAgent.DateFormat(Birth, DateTimeAgent.DateFormatKind.J1);

	public string Age
	{
		get
		{
			string result = "";
			if (age.Length > 0)
			{
				result = age;
			}
			else if (Birth.Length == 8)
			{
				result = DateConvert.CalcAge(Birth, DateTime.Now.ToString("yyyyMMdd")).ToString();
			}
			return result;
		}
		set
		{
			age = value;
		}
	}

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

	public string SexNameEng
	{
		get
		{
			string result = "";
			if (Sex.Equals("1"))
			{
				result = "M";
			}
			else if (Sex.Equals("2"))
			{
				result = "F";
			}
			return result;
		}
	}

	public string Info1 => Name + "（" + Kana + "）様\u3000" + SexName + "\u3000" + BirthStringJ + "生\u3000" + Age + "歳";

	public Dictionary<string, PatIns> InsDict
	{
		get
		{
			if (insDict.Count == 0)
			{
				insDict = PatIns.GetDict(Id);
			}
			return insDict;
		}
	}

	public string InsDefaultKindStringShort
	{
		get
		{
			string result = "";
			if (insDict.ContainsKey(InsDefault))
			{
				result = insDict[InsDefault].KindStringShort;
			}
			return result;
		}
	}

	public string AgeCalc(string crit_date)
	{
		string result = "";
		if (Birth.Length == 8 && crit_date.Length == 8)
		{
			result = DateConvert.CalcAge(Birth, crit_date).ToString();
		}
		return result;
	}

	public string GetInfo1(string crit_date)
	{
		return Name + "（" + Kana + "）様\u3000" + SexName + "\u3000" + BirthStringJ + "生\u3000" + AgeCalc(crit_date) + "歳";
	}

	public static PatBase GetPatBase(string pt_id)
	{
		PatBase patBase = new PatBase();
		if (pt_id.Length == 0)
		{
			return patBase;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select P_ID, Trim(P_KANA), Trim(P_NAME), P_SEX, P_BIRTHDAY_AD, TEL, POST, Trim(ADDR_1) || Trim(ADDR_2), HOKEN_NOW from M_PATIENT" + Env.DB_LINK + " where P_ID = " + pt_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		if (oleDbDataReader.Read())
		{
			patBase.Id = oleDbDataReader[0].ToString();
			patBase.Kana = oleDbDataReader[1].ToString();
			patBase.Name = oleDbDataReader[2].ToString();
			patBase.Sex = oleDbDataReader[3].ToString();
			patBase.Birth = oleDbDataReader[4].ToString();
			patBase.Tel = oleDbDataReader[5].ToString();
			patBase.Post = oleDbDataReader[6].ToString();
			patBase.Addr = oleDbDataReader[7].ToString();
			patBase.InsDefault = oleDbDataReader[8].ToString();
		}
		oleDbDataReader.Close();
		openDBConn.Close();
		return patBase;
	}

	public static Dictionary<string, PatBase> GetDict(string pt_ids)
	{
		Dictionary<string, PatBase> dictionary = new Dictionary<string, PatBase>();
		if (pt_ids.Length == 0)
		{
			return dictionary;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select P_ID, Trim(P_KANA), Trim(P_NAME), P_SEX, P_BIRTHDAY_AD, TEL, POST, Trim(ADDR_1) || Trim(ADDR_2), HOKEN_NOW from M_PATIENT" + Env.DB_LINK + " where P_ID in (" + pt_ids + ")";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatBase patBase = new PatBase();
			patBase.Id = oleDbDataReader[0].ToString();
			patBase.Kana = oleDbDataReader[1].ToString();
			patBase.Name = oleDbDataReader[2].ToString();
			patBase.Sex = oleDbDataReader[3].ToString();
			patBase.Birth = oleDbDataReader[4].ToString();
			patBase.Tel = oleDbDataReader[5].ToString();
			patBase.Post = oleDbDataReader[6].ToString();
			patBase.Addr = oleDbDataReader[7].ToString();
			patBase.InsDefault = oleDbDataReader[8].ToString();
			if (!dictionary.ContainsKey(patBase.Id))
			{
				dictionary.Add(patBase.Id, patBase);
			}
		}
		oleDbDataReader.Close();
		openDBConn.Close();
		return dictionary;
	}

	public static Dictionary<string, PatBase> GetDict(List<string> pt_id_list)
	{
		string text = "";
		foreach (string item in pt_id_list)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += item;
		}
		return GetDict(text);
	}
}
