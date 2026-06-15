using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatBringDrug
{
	public string PtId = "";

	public int SEQ;

	public int StartDate;

	public int EndDate;

	public string Drug = "";

	public string Staff = "";

	public int RegDate;

	public int DelFlg;

	public int ShohoFlg;

	public int KanrenyakuFlg;

	public string StartDateString => DateTimeAgent.DateFormat(StartDate, DateTimeAgent.DateFormatKind.LONG);

	public string StartDateStringShort => DateTimeAgent.DateFormat(StartDate, DateTimeAgent.DateFormatKind.SHORT);

	public string EndDateString => DateTimeAgent.DateFormat(EndDate, DateTimeAgent.DateFormatKind.LONG);

	public string EndDateStringShort => DateTimeAgent.DateFormat(EndDate, DateTimeAgent.DateFormatKind.SHORT);

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

	public string StaffLastName
	{
		get
		{
			string result = "";
			if (Dict.StaffDict.ContainsKey(Staff))
			{
				result = Dict.StaffDict[Staff].LastName;
			}
			return result;
		}
	}

	public string RegDateString => DateTimeAgent.DateFormat(RegDate, DateTimeAgent.DateFormatKind.LONG);

	public string RegDateStringShort => DateTimeAgent.DateFormat(RegDate, DateTimeAgent.DateFormatKind.SHORT);

	public static List<PatBringDrug> Find(string pt_id)
	{
		List<PatBringDrug> list = new List<PatBringDrug>();
		if (pt_id.Length == 0)
		{
			return list;
		}
		bool flag = false;
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		if (openDBConn.State != ConnectionState.Open)
		{
			openDBConn.Open();
			flag = true;
		}
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select * from ADT_持参薬登録データ" + Env.DB_LINK + " where 患者コード = " + pt_id + " order by 開始日 desc, 連番 desc";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatBringDrug patBringDrug = new PatBringDrug();
			patBringDrug.PtId = pt_id;
			int.TryParse(oleDbDataReader["連番"].ToString(), out patBringDrug.SEQ);
			int.TryParse(oleDbDataReader["開始日"].ToString(), out patBringDrug.StartDate);
			int.TryParse(oleDbDataReader["終了日"].ToString(), out patBringDrug.EndDate);
			patBringDrug.Drug = oleDbDataReader["名称"].ToString();
			patBringDrug.Staff = oleDbDataReader["入力者コード"].ToString();
			int.TryParse(oleDbDataReader["入力日"].ToString(), out patBringDrug.RegDate);
			int.TryParse(oleDbDataReader["削除フラグ"].ToString(), out patBringDrug.DelFlg);
			int.TryParse(oleDbDataReader["BIKOU1"].ToString(), out patBringDrug.ShohoFlg);
			int.TryParse(oleDbDataReader["BIKOU2"].ToString(), out patBringDrug.KanrenyakuFlg);
			list.Add(patBringDrug);
		}
		if (flag)
		{
			openDBConn.Close();
		}
		return list;
	}

	public static List<PatBringDrug> Find(string pt_id, int start_date, int end_date)
	{
		List<PatBringDrug> list = new List<PatBringDrug>();
		if (pt_id.Length == 0)
		{
			return list;
		}
		bool flag = false;
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		if (openDBConn.State != ConnectionState.Open)
		{
			openDBConn.Open();
			flag = true;
		}
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select * from ADT_持参薬登録データ" + Env.DB_LINK + " where 患者コード = " + pt_id + " and 開始日 <= " + start_date + " and (終了日 >= " + end_date + " or 終了日 = 0) order by 開始日 desc, 連番 desc";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatBringDrug patBringDrug = new PatBringDrug();
			patBringDrug.PtId = pt_id;
			int.TryParse(oleDbDataReader["連番"].ToString(), out patBringDrug.SEQ);
			int.TryParse(oleDbDataReader["開始日"].ToString(), out patBringDrug.StartDate);
			int.TryParse(oleDbDataReader["終了日"].ToString(), out patBringDrug.EndDate);
			patBringDrug.Drug = oleDbDataReader["名称"].ToString();
			patBringDrug.Staff = oleDbDataReader["入力者コード"].ToString();
			int.TryParse(oleDbDataReader["入力日"].ToString(), out patBringDrug.RegDate);
			int.TryParse(oleDbDataReader["削除フラグ"].ToString(), out patBringDrug.DelFlg);
			int.TryParse(oleDbDataReader["BIKOU1"].ToString(), out patBringDrug.ShohoFlg);
			int.TryParse(oleDbDataReader["BIKOU2"].ToString(), out patBringDrug.KanrenyakuFlg);
			list.Add(patBringDrug);
		}
		if (flag)
		{
			openDBConn.Close();
		}
		return list;
	}

	private bool StrToChk(string s)
	{
		if (s == "1")
		{
			return true;
		}
		return false;
	}

	public bool Save()
	{
		if (StartDate.ToString().Length != 8 || EndDate.ToString().Length > 8 || RegDate.ToString().Length != 8 || Drug.Length == 0)
		{
			return false;
		}
		bool flag = false;
		OleDbConnection dBConn = DBConn.GetDBConn();
		if (dBConn.State != ConnectionState.Open)
		{
			dBConn.Open();
			flag = true;
		}
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = dBConn;
		oleDbCommand.CommandText = "update ADT_持参薬登録データ set 開始日 = " + StartDate + ", 終了日 = " + EndDate + ", 名称 = '" + Drug + "', 入力者コード = " + LoginUser.Id + ", 入力日 = " + RegDate + ", 削除フラグ = " + DelFlg + ", BIKOU1 = " + ShohoFlg + ", BIKOU2 = " + KanrenyakuFlg + " where 患者コード = " + PtId + " and 連番 = " + SEQ;
		if (oleDbCommand.ExecuteNonQuery() == 0)
		{
			oleDbCommand.CommandText = "select max(連番) from ADT_持参薬登録データ where 患者コード = " + PtId;
			OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
			if (oleDbDataReader.Read())
			{
				int.TryParse(oleDbDataReader[0].ToString(), out SEQ);
				SEQ++;
			}
			oleDbDataReader.Close();
			oleDbCommand.CommandText = "insert into ADT_持参薬登録データ (患者コード, 連番, 開始日, 終了日, 名称, 入力者コード, 入力日, 削除フラグ, BIKOU1, BIKOU2) values (" + PtId + ", " + SEQ + ", " + StartDate + ", " + EndDate + ", '" + Drug + "', " + Staff + ", " + RegDate + ", " + DelFlg + ", " + ShohoFlg + ", " + KanrenyakuFlg + ")";
			oleDbCommand.ExecuteNonQuery();
		}
		if (flag)
		{
			dBConn.Close();
		}
		return true;
	}

	public static bool Delete(string pt_id, int seq)
	{
		bool result = false;
		if (pt_id.Length == 0)
		{
			return result;
		}
		bool flag = false;
		OleDbConnection dBConn = DBConn.GetDBConn();
		if (dBConn.State != ConnectionState.Open)
		{
			dBConn.Open();
			flag = true;
		}
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = dBConn;
		oleDbCommand.CommandText = "update ADT_持参薬登録データ set 削除フラグ = 1 where 患者コード = " + pt_id + " and 連番 = " + seq;
		if (oleDbCommand.ExecuteNonQuery() > 0)
		{
			result = true;
		}
		if (flag)
		{
			dBConn.Close();
		}
		return result;
	}
}
