using System.Collections.Generic;
using System.Data.Common;

namespace AgentlabUtilityLibrary;

public class PatRehaSOAP
{
	public int SEQ;

	public string PtId = "";

	public int RehaDate;

	public int InOut;

	public int RehaKind;

	public int Ins;

	public int RehaStart = -1;

	public int RehaEnd = -1;

	public string SO = "";

	public string AP = "";

	public string Staff = "";

	public int SaveDate;

	public int SaveTime;

	public string Status = "";

	public string RehaDateString => DateTimeAgent.DateFormat(RehaDate, DateTimeAgent.DateFormatKind.LONG);

	public string RehaDateStringShort => DateTimeAgent.DateFormat(RehaDate, DateTimeAgent.DateFormatKind.SHORT);

	public string InOutString
	{
		get
		{
			string result = "";
			if (InOut.Equals(1))
			{
				result = "外来";
			}
			else if (InOut.Equals(2))
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
			if (InOut.Equals(1))
			{
				result = "外";
			}
			else if (InOut.Equals(2))
			{
				result = "入";
			}
			return result;
		}
	}

	public string RehaKindString
	{
		get
		{
			string result = "";
			if (RehaKind.Equals(0))
			{
				result = "消鎮";
			}
			else if (RehaKind.Equals(4))
			{
				result = "言語";
			}
			else if (RehaKind.Equals(5))
			{
				result = "作業";
			}
			else if (RehaKind.Equals(6))
			{
				result = "理学";
			}
			return result;
		}
	}

	public string RehaKindStringShort
	{
		get
		{
			string result = "";
			if (RehaKind.Equals(0))
			{
				result = "消";
			}
			else if (RehaKind.Equals(4))
			{
				result = "言";
			}
			else if (RehaKind.Equals(5))
			{
				result = "作";
			}
			else if (RehaKind.Equals(6))
			{
				result = "理";
			}
			return result;
		}
	}

	public string RehaStartTime => DateTimeAgent.TimeFormat(RehaStart);

	public string RehaEndTime => DateTimeAgent.TimeFormat(RehaEnd);

	public string StaffName
	{
		get
		{
			string result = "";
			if (Dict.StaffDict.ContainsKey(Staff))
			{
				result = Dict.StaffDict[Staff].Name;
			}
			return result;
		}
	}

	public static Dictionary<string, PatRehaSOAPDateList> GetDictByDate(string date)
	{
		Dictionary<string, PatRehaSOAPDateList> dictionary = new Dictionary<string, PatRehaSOAPDateList>();
		if (date.Length == 0)
		{
			return dictionary;
		}
		DB oPEN = DB.OPEN;
		oPEN.Open();
		oPEN.CommandText = "select * from REHA_SOAP where REHA_DATE = " + date + " order by PATIENT_ID";
		DbDataReader dbDataReader = oPEN.ExecuteReader();
		string text = "";
		PatRehaSOAPDateList patRehaSOAPDateList = new PatRehaSOAPDateList();
		while (dbDataReader.Read())
		{
			if (text.Length == 0 || !text.Equals(dbDataReader["PATIENT_ID"].ToString()))
			{
				if (text.Length > 0 && !dictionary.ContainsKey(text))
				{
					dictionary.Add(text, patRehaSOAPDateList);
				}
				text = dbDataReader["PATIENT_ID"].ToString();
				patRehaSOAPDateList = new PatRehaSOAPDateList();
				patRehaSOAPDateList.PtId = text;
				int.TryParse(date, out patRehaSOAPDateList.RehaDate);
			}
			PatRehaSOAP patRehaSOAP = new PatRehaSOAP();
			int.TryParse(dbDataReader["REHA_SOAP_ID"].ToString(), out patRehaSOAP.SEQ);
			patRehaSOAP.PtId = dbDataReader["PATIENT_ID"].ToString();
			int.TryParse(dbDataReader["REHA_DATE"].ToString(), out patRehaSOAP.RehaDate);
			int.TryParse(dbDataReader["IN_OUT"].ToString(), out patRehaSOAP.InOut);
			int.TryParse(dbDataReader["REHA_KIND"].ToString(), out patRehaSOAP.RehaKind);
			int.TryParse(dbDataReader["INS"].ToString(), out patRehaSOAP.Ins);
			int.TryParse(dbDataReader["REHA_START"].ToString(), out patRehaSOAP.RehaStart);
			int.TryParse(dbDataReader["REHA_END"].ToString(), out patRehaSOAP.RehaEnd);
			patRehaSOAP.SO = dbDataReader["SO"].ToString();
			patRehaSOAP.AP = dbDataReader["AP"].ToString();
			patRehaSOAP.Staff = dbDataReader["STAFF"].ToString();
			int.TryParse(dbDataReader["SAVE_DATE"].ToString(), out patRehaSOAP.SaveDate);
			int.TryParse(dbDataReader["SAVE_TIME"].ToString(), out patRehaSOAP.SaveTime);
			patRehaSOAP.Status = dbDataReader["STATUS"].ToString();
			patRehaSOAPDateList.Add(patRehaSOAP);
		}
		if (text.Length > 0 && !dictionary.ContainsKey(text))
		{
			dictionary.Add(text, patRehaSOAPDateList);
		}
		dbDataReader.Close();
		oPEN.Close();
		return dictionary;
	}
}
