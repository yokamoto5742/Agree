using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatIn : PatBase
{
	public string InDate = "";

	public string OutDate = "";

	public string Dept = "";

	public string Doctor = "";

	public string Ward = "";

	public string Room = "";

	public string Outcome = "";

	public string InDateString => DateTimeAgent.DateFormat(InDate, DateTimeAgent.DateFormatKind.LONG);

	public string InDateStringShort => DateTimeAgent.DateFormat(InDate, DateTimeAgent.DateFormatKind.SHORT);

	public string OutDateString => DateTimeAgent.DateFormat(OutDate, DateTimeAgent.DateFormatKind.LONG);

	public string OutDateStringShort => DateTimeAgent.DateFormat(OutDate, DateTimeAgent.DateFormatKind.SHORT);

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

	public string InfoIn1
	{
		get
		{
			string text = "";
			string text2 = text;
			text = text2 + "ID：" + Id + "\r\n" + base.Info1 + "\r\n";
			text = text + "入院日\u3000" + InDateString + "\r\n";
			string text3 = text;
			text = text3 + "主治医\u3000" + DeptName + "\u3000" + DoctorName + "\r\n";
			return text + WardName + "\u3000" + Room;
		}
	}

	public static PatIn GetPatIn(string pt_id)
	{
		PatIn patIn = new PatIn();
		if (pt_id.Length == 0)
		{
			return patIn;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select IM01RC_F01, Trim(IM01RC_F03), Trim(IM01RC_F04), IM01RC_F05, IM01RC_F10, IM01RC_F08, IM01RC_F14, Trim(IM01RC_F15) || Trim(IM01RC_F16), IM01RC_F31, IM01RC_F24 as 入外, IM20RC_F03 as 診療科, IM20RC_F05 as 主治医, IM20RC_F07 as 入院日, IM20RC_F08 as 退院日, Trim(IM20RC_F12) as 病棟, Trim(IM20RC_F13) as 病室, IM20RC_F18 as 入院予定日, IM20RC_F19 as 退院予定日 from IM01RC" + Env.DB_LINK + " left join IM20RC" + Env.DB_LINK + " on IM20RC_F01 = IM01RC_F01 where IM01RC_F01 = " + pt_id + " and IM20RC_F01 = " + pt_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		if (oleDbDataReader.Read())
		{
			patIn.Id = oleDbDataReader[0].ToString();
			patIn.Kana = oleDbDataReader[1].ToString();
			patIn.Name = oleDbDataReader[2].ToString();
			patIn.Sex = oleDbDataReader[3].ToString();
			patIn.Birth = oleDbDataReader[4].ToString();
			patIn.Tel = oleDbDataReader[5].ToString();
			patIn.Post = oleDbDataReader[6].ToString();
			patIn.Addr = oleDbDataReader[7].ToString();
			patIn.InsDefault = oleDbDataReader[8].ToString();
			if (oleDbDataReader["入外"].ToString().Equals("1"))
			{
				patIn.InOut = "2";
			}
			else
			{
				patIn.InOut = "1";
			}
			patIn.Dept = oleDbDataReader["診療科"].ToString();
			patIn.Doctor = oleDbDataReader["主治医"].ToString();
			if (oleDbDataReader["入院日"].ToString().Length == 8)
			{
				patIn.InDate = oleDbDataReader["入院日"].ToString();
			}
			else if (oleDbDataReader["入院予定日"].ToString().Length == 8)
			{
				patIn.InDate = oleDbDataReader["入院予定日"].ToString();
			}
			if (oleDbDataReader["退院日"].ToString().Length == 8)
			{
				patIn.OutDate = oleDbDataReader["退院日"].ToString();
			}
			else if (oleDbDataReader["退院予定日"].ToString().Length == 8)
			{
				patIn.OutDate = oleDbDataReader["退院予定日"].ToString();
			}
			patIn.Ward = oleDbDataReader["病棟"].ToString();
			patIn.Room = oleDbDataReader["病室"].ToString();
		}
		oleDbDataReader.Close();
		openDBConn.Close();
		return patIn;
	}

	public static List<PatIn> GetList(string adm_date, string ward, string dept)
	{
		List<PatIn> list = new List<PatIn>();
		if (adm_date.Length != 8)
		{
			return list;
		}
		string text = "";
		if (ward.Length > 0)
		{
			text = " and IM24RC_F05 = '" + ward + "'";
		}
		string text2 = "";
		if (dept.Length > 0)
		{
			text2 = " and IM22RC_F05 = '" + dept + "'";
		}
		OleDbConnection dBConn = DBConn.GetDBConn();
		dBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = dBConn;
		oleDbCommand.CommandText = "select IM01RC_F01, IM01RC_F03, IM01RC_F04, IM01RC_F05, IM01RC_F10, IM21RC_F03, IM21RC_F04, IM21RC_F06, IM24RC_F05, IM24RC_F06, IM22RC_F05, IM23RC_F05 from (IM21RC inner join IM01RC on IM21RC_F01 = IM01RC_F01) inner join IM24RC on (IM21RC_F01 = IM24RC_F01) and (IM21RC_F03 = IM24RC_F02) inner join IM22RC on (IM21RC_F01 = IM22RC_F01) and (IM21RC_F03 = IM22RC_F02) inner join IM23RC on (IM21RC_F01 = IM23RC_F01) and (IM21RC_F03 = IM23RC_F02) where (IM24RC_F01, IM24RC_F02, IM24RC_F03) in  (select IM24RC_F01, IM24RC_F02, max(IM24RC_F03) from IM24RC   where (IM24RC_F01, IM24RC_F02) in (select IM21RC_F01, IM21RC_F03 from IM21RC where IM21RC_F03 <= " + adm_date + " and (IM21RC_F04 >= " + adm_date + " or IM21RC_F04 = 0)) and IM24RC_F04 <= " + adm_date + "   group by IM24RC_F01, IM24RC_F02) and (IM22RC_F01, IM22RC_F02, IM22RC_F03) in  (select IM22RC_F01, IM22RC_F02, max(IM22RC_F03) from IM22RC   where (IM22RC_F01, IM22RC_F02) in (select IM21RC_F01, IM21RC_F03 from IM21RC where IM21RC_F03 <= " + adm_date + " and (IM21RC_F04 >= " + adm_date + " or IM21RC_F04 = 0)) and IM22RC_F04 <= " + adm_date + "   group by IM22RC_F01, IM22RC_F02)" + text2 + " and (IM23RC_F01, IM23RC_F02, IM23RC_F03) in  (select IM23RC_F01, IM23RC_F02, max(IM23RC_F03) from IM23RC   where (IM23RC_F01, IM23RC_F02) in (select IM21RC_F01, IM21RC_F03 from IM21RC where IM21RC_F03 <= " + adm_date + " and (IM21RC_F04 >= " + adm_date + " or IM21RC_F04 = 0)) and IM23RC_F04 <= " + adm_date + "   group by IM23RC_F01, IM23RC_F02)" + text + " order by IM24RC_F06, IM21RC_F03 desc, IM21RC_F01";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatIn patIn = new PatIn();
			patIn.Id = oleDbDataReader["IM01RC_F01"].ToString();
			patIn.Kana = oleDbDataReader["IM01RC_F03"].ToString().Trim();
			patIn.Name = oleDbDataReader["IM01RC_F04"].ToString().Trim();
			patIn.Sex = oleDbDataReader["IM01RC_F05"].ToString();
			patIn.Birth = oleDbDataReader["IM01RC_F10"].ToString();
			patIn.InDate = oleDbDataReader["IM21RC_F03"].ToString();
			patIn.OutDate = oleDbDataReader["IM21RC_F04"].ToString();
			patIn.Dept = oleDbDataReader["IM22RC_F05"].ToString();
			patIn.Doctor = oleDbDataReader["IM23RC_F05"].ToString();
			patIn.Ward = oleDbDataReader["IM24RC_F05"].ToString().Trim();
			patIn.Room = oleDbDataReader["IM24RC_F06"].ToString().Trim();
			patIn.Outcome = oleDbDataReader["IM21RC_F06"].ToString();
			list.Add(patIn);
		}
		oleDbDataReader.Close();
		dBConn.Close();
		return list;
	}

	public static Dictionary<string, PatIn> GetList(List<string> pt_list)
	{
		Dictionary<string, PatIn> dictionary = new Dictionary<string, PatIn>();
		if (pt_list.Count == 0)
		{
			return dictionary;
		}
		string text = "";
		foreach (string item in pt_list)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += item;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select IM20RC_F01 as 患者コード, IM20RC_F03 as 診療科, IM20RC_F05 as 主治医, IM20RC_F07 as 入院日, IM20RC_F08 as 退院日, Trim(IM20RC_F12) as 病棟, Trim(IM20RC_F13) as 病室, IM20RC_F18 as 入院予定日, IM20RC_F19 as 退院予定日, Trim(IM01RC_F03) as カナ, Trim(IM01RC_F04) as 氏名, IM01RC_F05 as 性別, IM01RC_F10 as 生年月日, IM01RC_F24 as 入外 from IM20RC" + Env.DB_LINK + " inner join IM01RC" + Env.DB_LINK + " on IM20RC_F01 = IM01RC_F01 where IM20RC_F01 in (" + text + ")";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatIn patIn = new PatIn();
			patIn.Id = oleDbDataReader["患者コード"].ToString();
			patIn.Kana = oleDbDataReader["カナ"].ToString();
			patIn.Name = oleDbDataReader["氏名"].ToString();
			patIn.Sex = oleDbDataReader["性別"].ToString();
			patIn.Birth = oleDbDataReader["生年月日"].ToString();
			if (oleDbDataReader["入外"].ToString().Equals("1"))
			{
				patIn.InOut = "2";
			}
			else
			{
				patIn.InOut = "1";
			}
			patIn.Dept = oleDbDataReader["診療科"].ToString();
			patIn.Doctor = oleDbDataReader["主治医"].ToString();
			if (oleDbDataReader["入院日"].ToString().Length == 8)
			{
				patIn.InDate = oleDbDataReader["入院日"].ToString();
			}
			else if (oleDbDataReader["入院予定日"].ToString().Length == 8)
			{
				patIn.InDate = oleDbDataReader["入院予定日"].ToString();
			}
			if (oleDbDataReader["退院日"].ToString().Length == 8)
			{
				patIn.OutDate = oleDbDataReader["退院日"].ToString();
			}
			else if (oleDbDataReader["退院予定日"].ToString().Length == 8)
			{
				patIn.OutDate = oleDbDataReader["退院予定日"].ToString();
			}
			patIn.Ward = oleDbDataReader["病棟"].ToString();
			patIn.Room = oleDbDataReader["病室"].ToString();
			if (!dictionary.ContainsKey(patIn.Id))
			{
				dictionary.Add(patIn.Id, patIn);
			}
		}
		openDBConn.Close();
		return dictionary;
	}
}
