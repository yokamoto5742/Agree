using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatIns
{
	public string PtId = "";

	public int SEQ = 1;

	public string InsCode = "";

	public string InsString1 = "";

	public string InsString2 = "";

	public int StartDate;

	public int EndDate;

	public int Kind;

	public bool IsValidNow => IsValid(int.Parse(DateTime.Now.ToString("yyyyMMdd")));

	public string KindString
	{
		get
		{
			string result = "";
			if (InsKind.Dict.ContainsKey(Kind.ToString()))
			{
				result = InsKind.Dict[Kind.ToString()].Name;
			}
			return result;
		}
	}

	public string KindStringShort
	{
		get
		{
			string result = "";
			if (InsKind.Dict.ContainsKey(Kind.ToString()))
			{
				result = InsKind.Dict[Kind.ToString()].Short;
			}
			return result;
		}
	}

	public bool IsValid(int crit_date)
	{
		bool result = false;
		if (StartDate <= crit_date && (EndDate == 0 || EndDate >= crit_date))
		{
			result = true;
		}
		return result;
	}

	public static PatIns GetPatIns(string pt_id)
	{
		PatIns patIns = new PatIns();
		if (pt_id.Length == 0)
		{
			return patIns;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select P_ID, P_HOKEN, MAIN_NO, MAIN_KIGOU, MAIN_BANGOU, MAIN_DATE_S, MAIN_DATE_E, HOKEN_TYPE from M_PATIENT_HOKEN" + Env.DB_LINK + " where P_ID = " + pt_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		if (oleDbDataReader.Read())
		{
			patIns.PtId = oleDbDataReader[0].ToString();
			int.TryParse(oleDbDataReader[1].ToString(), out patIns.SEQ);
			patIns.InsCode = oleDbDataReader[2].ToString();
			patIns.InsString1 = oleDbDataReader[3].ToString();
			patIns.InsString2 = oleDbDataReader[4].ToString();
			int.TryParse(oleDbDataReader[5].ToString(), out patIns.StartDate);
			int.TryParse(oleDbDataReader[6].ToString(), out patIns.EndDate);
			int.TryParse(oleDbDataReader[7].ToString(), out patIns.Kind);
		}
		oleDbDataReader.Close();
		openDBConn.Close();
		return patIns;
	}

	public static Dictionary<string, PatIns> GetDict(string pt_ids)
	{
		Dictionary<string, PatIns> dictionary = new Dictionary<string, PatIns>();
		if (pt_ids.Length == 0)
		{
			return dictionary;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select P_ID, P_HOKEN, MAIN_NO, MAIN_KIGOU, MAIN_BANGOU, MAIN_DATE_S, MAIN_DATE_E, HOKEN_TYPE from M_PATIENT_HOKEN" + Env.DB_LINK + " where P_ID in (" + pt_ids + ")";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatIns patIns = new PatIns();
			patIns.PtId = oleDbDataReader[0].ToString();
			int.TryParse(oleDbDataReader[1].ToString(), out patIns.SEQ);
			patIns.InsCode = oleDbDataReader[2].ToString();
			patIns.InsString1 = oleDbDataReader[3].ToString();
			patIns.InsString2 = oleDbDataReader[4].ToString();
			int.TryParse(oleDbDataReader[5].ToString(), out patIns.StartDate);
			int.TryParse(oleDbDataReader[6].ToString(), out patIns.EndDate);
			int.TryParse(oleDbDataReader[7].ToString(), out patIns.Kind);
			if (!dictionary.ContainsKey(patIns.SEQ.ToString()))
			{
				dictionary.Add(patIns.SEQ.ToString(), patIns);
			}
		}
		oleDbDataReader.Close();
		openDBConn.Close();
		return dictionary;
	}

	public static Dictionary<string, PatIns> GetDict(List<string> pt_id_list)
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
