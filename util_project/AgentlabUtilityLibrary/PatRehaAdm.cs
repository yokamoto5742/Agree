using System.Collections.Generic;
using System.Data.Common;

namespace AgentlabUtilityLibrary;

public class PatRehaAdm
{
	public string PtId = "";

	public string PtKana = "";

	public string PtName = "";

	public int PtSex;

	public int PtBirth;

	public string Dept = "";

	public string Doctor = "";

	public int InDate;

	public int OutDate;

	public string Ward = "";

	public string Room = "";

	public string Staff0 = "";

	public string Kind0 = "";

	public int Unit0;

	public string Staff1 = "";

	public string Kind1 = "";

	public int Unit1;

	public string Staff2 = "";

	public string Kind2 = "";

	public int Unit2;

	public string Staff3 = "";

	public string Kind3 = "";

	public int Unit3;

	public string Ins = "";

	public string Comment = "";

	public string Status = "";

	public int SaveDate;

	public string SaveStaff = "";

	public string PtSexString
	{
		get
		{
			string result = "";
			if (PtSex.Equals(1))
			{
				result = "男性";
			}
			else if (PtSex.Equals(2))
			{
				result = "女性";
			}
			return result;
		}
	}

	public string PtSexStringShort
	{
		get
		{
			string result = "";
			if (PtSex.Equals(1))
			{
				result = "男";
			}
			else if (PtSex.Equals(2))
			{
				result = "女";
			}
			return result;
		}
	}

	public string PtBirthString => DateTimeAgent.DateFormat(PtBirth, DateTimeAgent.DateFormatKind.LONG);

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

	public string InDateString => DateTimeAgent.DateFormat(InDate, DateTimeAgent.DateFormatKind.LONG);

	public string InDateStringShort => DateTimeAgent.DateFormat(InDate, DateTimeAgent.DateFormatKind.SHORT);

	public string OutDateString => DateTimeAgent.DateFormat(OutDate, DateTimeAgent.DateFormatKind.LONG);

	public string OutDateStringShort => DateTimeAgent.DateFormat(OutDate, DateTimeAgent.DateFormatKind.SHORT);

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

	public static Dictionary<string, PatRehaAdm> GetAliveDict()
	{
		Dictionary<string, PatRehaAdm> dictionary = new Dictionary<string, PatRehaAdm>();
		DB oPEN = DB.OPEN;
		oPEN.Open();
		oPEN.CommandText = "select REHA_ADM_PT.*, Trim(IM01RC_F03) as カナ, Trim(IM01RC_F04) as 氏名, IM01RC_F05 as 性別, IM01RC_F10 as 生年月日, IM20RC_F03 as 科, IM20RC_F05 as 医師, decode(IM20RC_F07, 0, IM20RC_F18, IM20RC_F07) as 入院, decode(IM20RC_F08, 0, IM20RC_F19, IM20RC_F08) as 退院, Trim(IM20RC_F12) as 病棟, IM20RC_F13 as 病室 from REHA_ADM_PT inner join IM01RC" + Env.DB_LINK + " on PATIENT_ID = IM01RC_F01 left join IM20RC" + Env.DB_LINK + " on PATIENT_ID = IM20RC_F01 where STATUS = 1";
		DbDataReader dbDataReader = oPEN.ExecuteReader();
		while (dbDataReader.Read())
		{
			PatRehaAdm patRehaAdm = new PatRehaAdm();
			patRehaAdm.PtId = dbDataReader["PATIENT_ID"].ToString();
			patRehaAdm.PtKana = dbDataReader["カナ"].ToString();
			patRehaAdm.PtName = dbDataReader["氏名"].ToString();
			int.TryParse(dbDataReader["性別"].ToString(), out patRehaAdm.PtSex);
			int.TryParse(dbDataReader["生年月日"].ToString(), out patRehaAdm.PtBirth);
			int.TryParse(dbDataReader["入院"].ToString(), out patRehaAdm.InDate);
			int.TryParse(dbDataReader["退院"].ToString(), out patRehaAdm.OutDate);
			patRehaAdm.Dept = dbDataReader["科"].ToString();
			patRehaAdm.Doctor = dbDataReader["医師"].ToString();
			patRehaAdm.Ward = dbDataReader["病棟"].ToString();
			patRehaAdm.Room = dbDataReader["病室"].ToString();
			patRehaAdm.Staff0 = dbDataReader["STAFF0"].ToString();
			patRehaAdm.Kind0 = dbDataReader["KIND0"].ToString();
			int.TryParse(dbDataReader["UNIT0"].ToString(), out patRehaAdm.Unit0);
			patRehaAdm.Staff1 = dbDataReader["STAFF1"].ToString();
			patRehaAdm.Kind1 = dbDataReader["KIND1"].ToString();
			int.TryParse(dbDataReader["UNIT1"].ToString(), out patRehaAdm.Unit1);
			patRehaAdm.Staff2 = dbDataReader["STAFF2"].ToString();
			patRehaAdm.Kind2 = dbDataReader["KIND2"].ToString();
			int.TryParse(dbDataReader["UNIT2"].ToString(), out patRehaAdm.Unit2);
			patRehaAdm.Staff3 = dbDataReader["STAFF3"].ToString();
			patRehaAdm.Kind3 = dbDataReader["KIND3"].ToString();
			int.TryParse(dbDataReader["UNIT3"].ToString(), out patRehaAdm.Unit3);
			patRehaAdm.Ins = dbDataReader["INS"].ToString();
			patRehaAdm.Comment = dbDataReader["COM"].ToString();
			patRehaAdm.Status = dbDataReader["STATUS"].ToString();
			int.TryParse(dbDataReader["SAVE_DATE"].ToString(), out patRehaAdm.SaveDate);
			patRehaAdm.SaveStaff = dbDataReader["SAVE_STAFF"].ToString();
			if (!dictionary.ContainsKey(patRehaAdm.PtId))
			{
				dictionary.Add(patRehaAdm.PtId, patRehaAdm);
			}
		}
		oPEN.Close();
		return dictionary;
	}

	public static Dictionary<string, PatRehaAdm> GetDict(string pat_list_str)
	{
		Dictionary<string, PatRehaAdm> dictionary = new Dictionary<string, PatRehaAdm>();
		if (pat_list_str.Length == 0)
		{
			return dictionary;
		}
		DB oPEN = DB.OPEN;
		oPEN.Open();
		oPEN.CommandText = "select * from REHA_ADM_PT where PATIENT_ID in (" + pat_list_str + ")";
		DbDataReader dbDataReader = oPEN.ExecuteReader();
		while (dbDataReader.Read())
		{
			PatRehaAdm patRehaAdm = new PatRehaAdm();
			patRehaAdm.PtId = dbDataReader["PATIENT_ID"].ToString();
			patRehaAdm.Staff0 = dbDataReader["STAFF0"].ToString();
			patRehaAdm.Kind0 = dbDataReader["KIND0"].ToString();
			int.TryParse(dbDataReader["UNIT0"].ToString(), out patRehaAdm.Unit0);
			patRehaAdm.Staff1 = dbDataReader["STAFF1"].ToString();
			patRehaAdm.Kind1 = dbDataReader["KIND1"].ToString();
			int.TryParse(dbDataReader["UNIT1"].ToString(), out patRehaAdm.Unit1);
			patRehaAdm.Staff2 = dbDataReader["STAFF2"].ToString();
			patRehaAdm.Kind2 = dbDataReader["KIND2"].ToString();
			int.TryParse(dbDataReader["UNIT2"].ToString(), out patRehaAdm.Unit2);
			patRehaAdm.Staff3 = dbDataReader["STAFF3"].ToString();
			patRehaAdm.Kind3 = dbDataReader["KIND3"].ToString();
			int.TryParse(dbDataReader["UNIT3"].ToString(), out patRehaAdm.Unit3);
			patRehaAdm.Ins = dbDataReader["INS"].ToString();
			patRehaAdm.Comment = dbDataReader["COM"].ToString();
			patRehaAdm.Status = dbDataReader["STATUS"].ToString();
			int.TryParse(dbDataReader["SAVE_DATE"].ToString(), out patRehaAdm.SaveDate);
			patRehaAdm.SaveStaff = dbDataReader["SAVE_STAFF"].ToString();
			if (!dictionary.ContainsKey(patRehaAdm.PtId))
			{
				dictionary.Add(patRehaAdm.PtId, patRehaAdm);
			}
		}
		oPEN.Close();
		return dictionary;
	}

	public static Dictionary<string, PatRehaAdm> GetDict(List<string> pat_list)
	{
		string text = "";
		foreach (string item in pat_list)
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
