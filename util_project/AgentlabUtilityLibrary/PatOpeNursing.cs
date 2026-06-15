using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatOpeNursing : MacsDataBase1
{
	public int Id;

	public PatBase PatBase1 = new PatBase();

	public string Infection = "";

	public string Ins = "";

	public string OpeDate = "";

	public string RecKind = "";

	public string TimeOut = "";

	public string Ope = "";

	public string Part = "";

	public string Anes = "";

	public string Doctor1 = "";

	public string Doctor2 = "";

	public string Doctor3 = "";

	public string Ns1 = "";

	public string Ns2 = "";

	public string Ns3 = "";

	public string Room = "";

	public string NsRec = "";

	public int AnesTime1 = -1;

	public int AnesTime2 = -1;

	public int OpeTime1 = -1;

	public int OpeTime2 = -1;

	public int RoomTime1 = -1;

	public int RoomTime2 = -1;

	public string RecHist = "";

	public string Status = "";

	public string PDFSave = "";

	public Dictionary<string, PatOpeNursingAs> AsDict = new Dictionary<string, PatOpeNursingAs>();

	public Dictionary<string, PatOpeNursingSchema> SchemaDict = new Dictionary<string, PatOpeNursingSchema>();

	public string InfectionFlg
	{
		get
		{
			string text = "";
			if (Infection.Contains("+"))
			{
				return "+";
			}
			return "-";
		}
	}

	public string OpeDateString => DateTimeAgent.DateFormat(OpeDate, DateTimeAgent.DateFormatKind.LONG);

	public string OpeDateStringShort => DateTimeAgent.DateFormat(OpeDate, DateTimeAgent.DateFormatKind.SHORT);

	public string RecKindString
	{
		get
		{
			string result = "";
			if (RecKind.Equals("1"))
			{
				result = "術前";
			}
			else if (RecKind.Equals("2"))
			{
				result = "術中";
			}
			else if (RecKind.Equals("3"))
			{
				result = "術後";
			}
			return result;
		}
	}

	public string RecKindStringShort
	{
		get
		{
			string result = "";
			if (RecKind.Equals("1"))
			{
				result = "前";
			}
			else if (RecKind.Equals("2"))
			{
				result = "中";
			}
			else if (RecKind.Equals("3"))
			{
				result = "後";
			}
			return result;
		}
	}

	public string TimeOutString
	{
		get
		{
			string result = "";
			if (TimeOut.Equals("1"))
			{
				result = "緊内";
			}
			else if (TimeOut.Equals("2"))
			{
				result = "緊外";
			}
			return result;
		}
	}

	public string AnesTimeString1
	{
		get
		{
			string result = "";
			if (AnesTime1 >= 0)
			{
				result = AnesTime1.ToString().PadLeft(4, '0').Insert(2, ":");
			}
			return result;
		}
	}

	public string AnesTimeString2
	{
		get
		{
			string result = "";
			if (AnesTime2 >= 0)
			{
				result = AnesTime2.ToString().PadLeft(4, '0').Insert(2, ":");
			}
			return result;
		}
	}

	public string OpeTimeString1
	{
		get
		{
			string result = "";
			if (OpeTime1 >= 0)
			{
				result = OpeTime1.ToString().PadLeft(4, '0').Insert(2, ":");
			}
			return result;
		}
	}

	public string OpeTimeString2
	{
		get
		{
			string result = "";
			if (OpeTime2 >= 0)
			{
				result = OpeTime2.ToString().PadLeft(4, '0').Insert(2, ":");
			}
			return result;
		}
	}

	public string RoomTimeString1
	{
		get
		{
			string result = "";
			if (RoomTime1 >= 0)
			{
				result = RoomTime1.ToString().PadLeft(4, '0').Insert(2, ":");
			}
			return result;
		}
	}

	public string RoomTimeString2
	{
		get
		{
			string result = "";
			if (RoomTime2 >= 0)
			{
				result = RoomTime2.ToString().PadLeft(4, '0').Insert(2, ":");
			}
			return result;
		}
	}

	public string StatusFlg
	{
		get
		{
			string result = "";
			if (Status.Equals("1"))
			{
				result = "●";
			}
			return result;
		}
	}

	public void GetAsDict()
	{
		AsDict = PatOpeNursingAs.GetDict(Id);
	}

	public void GetSchemaDict()
	{
		SchemaDict = PatOpeNursingSchema.GetDict(Id);
	}

	public static PatOpeNursing Load(string id)
	{
		PatOpeNursing patOpeNursing = new PatOpeNursing();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select OPE_NURSING.*, Trim(IM01RC_F03) as カナ, Trim(IM01RC_F04) as 氏名, IM01RC_F05 as 性別, IM01RC_F10 as 生年月日 from OPE_NURSING inner join IM01RC" + Env.DB_LINK + " on PATIENT_ID = IM01RC_F01 where ID = ?";
		oleDbCommand.Parameters.Add("ID", OleDbType.Numeric).Value = id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		if (oleDbDataReader.Read())
		{
			int.TryParse(oleDbDataReader["ID"].ToString(), out patOpeNursing.Id);
			patOpeNursing.PatBase1.Id = oleDbDataReader["PATIENT_ID"].ToString();
			patOpeNursing.PatBase1.Name = oleDbDataReader["氏名"].ToString();
			patOpeNursing.PatBase1.Kana = oleDbDataReader["カナ"].ToString();
			patOpeNursing.PatBase1.Sex = oleDbDataReader["性別"].ToString();
			patOpeNursing.PatBase1.Birth = oleDbDataReader["生年月日"].ToString();
			patOpeNursing.Infection = oleDbDataReader["INFECTION"].ToString();
			patOpeNursing.Ins = oleDbDataReader["INS"].ToString();
			patOpeNursing.OpeDate = oleDbDataReader["OPE_DATE"].ToString();
			patOpeNursing.InOut = oleDbDataReader["IN_OUT"].ToString();
			patOpeNursing.TimeOut = oleDbDataReader["TIME_OUT"].ToString();
			patOpeNursing.Dept = oleDbDataReader["DEPT"].ToString();
			patOpeNursing.Ope = oleDbDataReader["OPE"].ToString();
			patOpeNursing.Part = oleDbDataReader["PART"].ToString();
			patOpeNursing.Anes = oleDbDataReader["ANES"].ToString();
			patOpeNursing.Doctor1 = oleDbDataReader["DOCTOR1"].ToString();
			patOpeNursing.Doctor2 = oleDbDataReader["DOCTOR2"].ToString();
			patOpeNursing.Doctor3 = oleDbDataReader["DOCTOR3"].ToString();
			patOpeNursing.Ns1 = oleDbDataReader["NS1"].ToString();
			patOpeNursing.Ns2 = oleDbDataReader["NS2"].ToString();
			patOpeNursing.Ns3 = oleDbDataReader["NS3"].ToString();
			patOpeNursing.Room = oleDbDataReader["ROOM"].ToString();
			patOpeNursing.NsRec = oleDbDataReader["NS_REC"].ToString();
			patOpeNursing.Staff = oleDbDataReader["STAFF"].ToString();
			patOpeNursing.Status = oleDbDataReader["STATUS"].ToString();
			patOpeNursing.PDFSave = oleDbDataReader["PDF_SAVE"].ToString();
			int.TryParse(oleDbDataReader["ANES_TIME1"].ToString(), out patOpeNursing.AnesTime1);
			int.TryParse(oleDbDataReader["ANES_TIME2"].ToString(), out patOpeNursing.AnesTime2);
			int.TryParse(oleDbDataReader["OPE_TIME1"].ToString(), out patOpeNursing.OpeTime1);
			int.TryParse(oleDbDataReader["OPE_TIME2"].ToString(), out patOpeNursing.OpeTime2);
			int.TryParse(oleDbDataReader["ROOM_TIME1"].ToString(), out patOpeNursing.RoomTime1);
			int.TryParse(oleDbDataReader["ROOM_TIME2"].ToString(), out patOpeNursing.RoomTime2);
			patOpeNursing.RecHist = oleDbDataReader["REC_HIST"].ToString();
			patOpeNursing.RecKind = oleDbDataReader["REC_KIND"].ToString();
		}
		openDBConn.Close();
		return patOpeNursing;
	}

	public static List<PatOpeNursing> Find(string pt_id)
	{
		List<PatOpeNursing> list = new List<PatOpeNursing>();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select OPE_NURSING.*, Trim(IM01RC_F03) as カナ, Trim(IM01RC_F04) as 氏名, IM01RC_F05 as 性別, IM01RC_F10 as 生年月日 from OPE_NURSING inner join IM01RC" + Env.DB_LINK + " on PATIENT_ID = IM01RC_F01 where PATIENT_ID = " + pt_id + " order by OPE_DATE desc";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatOpeNursing patOpeNursing = new PatOpeNursing();
			int.TryParse(oleDbDataReader["ID"].ToString(), out patOpeNursing.Id);
			patOpeNursing.PatBase1.Id = oleDbDataReader["PATIENT_ID"].ToString();
			patOpeNursing.PatBase1.Name = oleDbDataReader["氏名"].ToString();
			patOpeNursing.PatBase1.Kana = oleDbDataReader["カナ"].ToString();
			patOpeNursing.PatBase1.Sex = oleDbDataReader["性別"].ToString();
			patOpeNursing.PatBase1.Birth = oleDbDataReader["生年月日"].ToString();
			patOpeNursing.Infection = oleDbDataReader["INFECTION"].ToString();
			patOpeNursing.Ins = oleDbDataReader["INS"].ToString();
			patOpeNursing.OpeDate = oleDbDataReader["OPE_DATE"].ToString();
			patOpeNursing.InOut = oleDbDataReader["IN_OUT"].ToString();
			patOpeNursing.TimeOut = oleDbDataReader["TIME_OUT"].ToString();
			patOpeNursing.Dept = oleDbDataReader["DEPT"].ToString();
			patOpeNursing.Ope = oleDbDataReader["OPE"].ToString();
			patOpeNursing.Part = oleDbDataReader["PART"].ToString();
			patOpeNursing.Anes = oleDbDataReader["ANES"].ToString();
			patOpeNursing.Doctor1 = oleDbDataReader["DOCTOR1"].ToString();
			patOpeNursing.Doctor2 = oleDbDataReader["DOCTOR2"].ToString();
			patOpeNursing.Doctor3 = oleDbDataReader["DOCTOR3"].ToString();
			patOpeNursing.Ns1 = oleDbDataReader["NS1"].ToString();
			patOpeNursing.Ns2 = oleDbDataReader["NS2"].ToString();
			patOpeNursing.Ns3 = oleDbDataReader["NS3"].ToString();
			patOpeNursing.Room = oleDbDataReader["ROOM"].ToString();
			patOpeNursing.NsRec = oleDbDataReader["NS_REC"].ToString();
			patOpeNursing.Staff = oleDbDataReader["STAFF"].ToString();
			patOpeNursing.Status = oleDbDataReader["STATUS"].ToString();
			patOpeNursing.PDFSave = oleDbDataReader["PDF_SAVE"].ToString();
			int.TryParse(oleDbDataReader["ANES_TIME1"].ToString(), out patOpeNursing.AnesTime1);
			int.TryParse(oleDbDataReader["ANES_TIME2"].ToString(), out patOpeNursing.AnesTime2);
			int.TryParse(oleDbDataReader["OPE_TIME1"].ToString(), out patOpeNursing.OpeTime1);
			int.TryParse(oleDbDataReader["OPE_TIME2"].ToString(), out patOpeNursing.OpeTime2);
			int.TryParse(oleDbDataReader["ROOM_TIME1"].ToString(), out patOpeNursing.RoomTime1);
			int.TryParse(oleDbDataReader["ROOM_TIME2"].ToString(), out patOpeNursing.RoomTime2);
			patOpeNursing.RecHist = oleDbDataReader["REC_HIST"].ToString();
			patOpeNursing.RecKind = oleDbDataReader["REC_KIND"].ToString();
			list.Add(patOpeNursing);
		}
		openDBConn.Close();
		return list;
	}

	public static List<PatOpeNursing> Find(string start_date, string end_date)
	{
		List<PatOpeNursing> list = new List<PatOpeNursing>();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select OPE_NURSING.*, Trim(IM01RC_F03) as カナ, Trim(IM01RC_F04) as 氏名, IM01RC_F05 as 性別, IM01RC_F10 as 生年月日 from OPE_NURSING inner join IM01RC" + Env.DB_LINK + " on PATIENT_ID = IM01RC_F01 where OPE_NURSING.OPE_DATE >= " + start_date + " and OPE_NURSING.OPE_DATE <= " + end_date + " and STATUS != 0 order by OPE_DATE desc";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatOpeNursing patOpeNursing = new PatOpeNursing();
			int.TryParse(oleDbDataReader["ID"].ToString(), out patOpeNursing.Id);
			patOpeNursing.PatBase1.Id = oleDbDataReader["PATIENT_ID"].ToString();
			patOpeNursing.PatBase1.Name = oleDbDataReader["氏名"].ToString();
			patOpeNursing.PatBase1.Kana = oleDbDataReader["カナ"].ToString();
			patOpeNursing.PatBase1.Sex = oleDbDataReader["性別"].ToString();
			patOpeNursing.PatBase1.Birth = oleDbDataReader["生年月日"].ToString();
			patOpeNursing.Infection = oleDbDataReader["INFECTION"].ToString();
			patOpeNursing.Ins = oleDbDataReader["INS"].ToString();
			patOpeNursing.OpeDate = oleDbDataReader["OPE_DATE"].ToString();
			patOpeNursing.InOut = oleDbDataReader["IN_OUT"].ToString();
			patOpeNursing.TimeOut = oleDbDataReader["TIME_OUT"].ToString();
			patOpeNursing.Dept = oleDbDataReader["DEPT"].ToString();
			patOpeNursing.Ope = oleDbDataReader["OPE"].ToString();
			patOpeNursing.Part = oleDbDataReader["PART"].ToString();
			patOpeNursing.Anes = oleDbDataReader["ANES"].ToString();
			patOpeNursing.Doctor1 = oleDbDataReader["DOCTOR1"].ToString();
			patOpeNursing.Doctor2 = oleDbDataReader["DOCTOR2"].ToString();
			patOpeNursing.Doctor3 = oleDbDataReader["DOCTOR3"].ToString();
			patOpeNursing.Ns1 = oleDbDataReader["NS1"].ToString();
			patOpeNursing.Ns2 = oleDbDataReader["NS2"].ToString();
			patOpeNursing.Ns3 = oleDbDataReader["NS3"].ToString();
			patOpeNursing.Room = oleDbDataReader["ROOM"].ToString();
			patOpeNursing.NsRec = oleDbDataReader["NS_REC"].ToString();
			patOpeNursing.Staff = oleDbDataReader["STAFF"].ToString();
			patOpeNursing.Status = oleDbDataReader["STATUS"].ToString();
			patOpeNursing.PDFSave = oleDbDataReader["PDF_SAVE"].ToString();
			int.TryParse(oleDbDataReader["ANES_TIME1"].ToString(), out patOpeNursing.AnesTime1);
			int.TryParse(oleDbDataReader["ANES_TIME2"].ToString(), out patOpeNursing.AnesTime2);
			int.TryParse(oleDbDataReader["OPE_TIME1"].ToString(), out patOpeNursing.OpeTime1);
			int.TryParse(oleDbDataReader["OPE_TIME2"].ToString(), out patOpeNursing.OpeTime2);
			int.TryParse(oleDbDataReader["ROOM_TIME1"].ToString(), out patOpeNursing.RoomTime1);
			int.TryParse(oleDbDataReader["ROOM_TIME2"].ToString(), out patOpeNursing.RoomTime2);
			patOpeNursing.RecHist = oleDbDataReader["REC_HIST"].ToString();
			patOpeNursing.RecKind = oleDbDataReader["REC_KIND"].ToString();
			list.Add(patOpeNursing);
		}
		openDBConn.Close();
		return list;
	}

	public bool Save()
	{
		bool flag = false;
		int result = Id;
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "update OPE_NURSING set PATIENT_ID = ?, INFECTION = ?, INS = ?, OPE_DATE = ?, IN_OUT = ?, TIME_OUT = ?, DEPT = ?, OPE = ?, PART = ?, ANES = ?, DOCTOR1 = ?, DOCTOR2 = ?, DOCTOR3 = ?, NS1 = ?, NS2 = ?, NS3 = ?, ROOM = ?, NS_REC = ?, ANES_TIME1 = ?, ANES_TIME2 = ?, OPE_TIME1 = ?, OPE_TIME2 = ?, ROOM_TIME1 = ?, ROOM_TIME2 = ?, REC_HIST = ?, REC_KIND = ?, STAFF = ?, SAVE_DATE = ?, SAVE_TIME = ?, STATUS = ? where ID = ?";
		oleDbCommand.Parameters.Add("PATIENT_ID", OleDbType.Numeric).Value = PatBase1.Id;
		oleDbCommand.Parameters.Add("INFECTION", OleDbType.VarWChar).Value = Infection;
		oleDbCommand.Parameters.Add("INS", OleDbType.Numeric).Value = Ins;
		oleDbCommand.Parameters.Add("OPE_DATE", OleDbType.Numeric).Value = OpeDate;
		oleDbCommand.Parameters.Add("IN_OUT", OleDbType.Numeric).Value = InOut;
		oleDbCommand.Parameters.Add("TIME_OUT", OleDbType.Numeric).Value = TimeOut;
		oleDbCommand.Parameters.Add("DEPT", OleDbType.Numeric).Value = Dept;
		oleDbCommand.Parameters.Add("OPE", OleDbType.VarWChar).Value = Ope;
		oleDbCommand.Parameters.Add("PART", OleDbType.VarWChar).Value = Part;
		oleDbCommand.Parameters.Add("ANES", OleDbType.VarWChar).Value = Anes;
		oleDbCommand.Parameters.Add("DOCTOR1", OleDbType.VarWChar).Value = Doctor1;
		oleDbCommand.Parameters.Add("DOCTOR2", OleDbType.VarWChar).Value = Doctor2;
		oleDbCommand.Parameters.Add("DOCTOR3", OleDbType.VarWChar).Value = Doctor3;
		oleDbCommand.Parameters.Add("NS1", OleDbType.VarWChar).Value = Ns1;
		oleDbCommand.Parameters.Add("NS2", OleDbType.VarWChar).Value = Ns2;
		oleDbCommand.Parameters.Add("NS3", OleDbType.VarWChar).Value = Ns3;
		oleDbCommand.Parameters.Add("ROOM", OleDbType.VarWChar).Value = Room;
		oleDbCommand.Parameters.Add("NS_REC", OleDbType.VarWChar).Value = NsRec;
		oleDbCommand.Parameters.Add("ANES_TIME1", OleDbType.Numeric).Value = AnesTime1;
		oleDbCommand.Parameters.Add("ANES_TIME2", OleDbType.Numeric).Value = AnesTime2;
		oleDbCommand.Parameters.Add("OPE_TIME1", OleDbType.Numeric).Value = OpeTime1;
		oleDbCommand.Parameters.Add("OPE_TIME2", OleDbType.Numeric).Value = OpeTime2;
		oleDbCommand.Parameters.Add("ROOM_TIME1", OleDbType.Numeric).Value = RoomTime1;
		oleDbCommand.Parameters.Add("ROOM_TIME2", OleDbType.Numeric).Value = RoomTime2;
		oleDbCommand.Parameters.Add("REC_HIST", OleDbType.VarWChar).Value = RecHist;
		oleDbCommand.Parameters.Add("REC_KIND", OleDbType.Numeric).Value = RecKind;
		oleDbCommand.Parameters.Add("STAFF", OleDbType.Numeric).Value = LoginUser.Id;
		oleDbCommand.Parameters.Add("SAVE_DATE", OleDbType.Numeric).Value = DateTime.Now.ToString("yyyyMMdd");
		oleDbCommand.Parameters.Add("SAVE_TIME", OleDbType.Numeric).Value = DateTime.Now.ToString("HHmmss");
		oleDbCommand.Parameters.Add("STATUS", OleDbType.Numeric).Value = Status;
		oleDbCommand.Parameters.Add("ID", OleDbType.Numeric).Value = Id;
		if (oleDbCommand.ExecuteNonQuery() == 0)
		{
			oleDbCommand.CommandText = "select OPE_NURSING_SEQ.nextval from DUAL";
			OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
			if (oleDbDataReader.Read())
			{
				int.TryParse(oleDbDataReader[0].ToString(), out result);
			}
			oleDbDataReader.Close();
			oleDbCommand.CommandText = "insert into OPE_NURSING (ID, PATIENT_ID, INFECTION, INS, OPE_DATE, IN_OUT, TIME_OUT, DEPT, OPE, PART, ANES, DOCTOR1, DOCTOR2, DOCTOR3, NS1, NS2, NS3, ROOM, NS_REC, ANES_TIME1, ANES_TIME2, OPE_TIME1, OPE_TIME2, ROOM_TIME1, ROOM_TIME2, REC_HIST, REC_KIND, STAFF, SAVE_DATE, SAVE_TIME, STATUS) values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
			oleDbCommand.Parameters["ID"].Value = result;
			oleDbCommand.ExecuteNonQuery();
		}
		int num = 1;
		oleDbCommand.CommandText = "delete from OPE_NURSING_AS where OPE_NURSING_ID = ?";
		oleDbCommand.Parameters.Clear();
		oleDbCommand.Parameters.Add("OPE_NURSING_ID", OleDbType.Numeric).Value = result;
		oleDbCommand.ExecuteNonQuery();
		foreach (PatOpeNursingAs value in AsDict.Values)
		{
			oleDbCommand.CommandText = "insert into OPE_NURSING_AS (OPE_NURSING_ID, AS_ID, AS_TITLE, AS_TEXT) values (?, ?, ?, ?)";
			oleDbCommand.Parameters.Clear();
			oleDbCommand.Parameters.Add("OPE_NURSING_ID", OleDbType.Numeric).Value = result;
			oleDbCommand.Parameters.Add("AS_ID", OleDbType.Numeric).Value = num;
			oleDbCommand.Parameters.Add("AS_TITLE", OleDbType.VarWChar).Value = value.Title;
			oleDbCommand.Parameters.Add("AS_TEXT", OleDbType.VarWChar).Value = value.Text;
			oleDbCommand.ExecuteNonQuery();
			num++;
		}
		num = 1;
		oleDbCommand.CommandText = "delete from OPE_NURSING_SCHEMA where OPE_NURSING_ID = ?";
		oleDbCommand.Parameters.Clear();
		oleDbCommand.Parameters.Add("OPE_NURSING_ID", OleDbType.Numeric).Value = result;
		oleDbCommand.ExecuteNonQuery();
		foreach (PatOpeNursingSchema value2 in SchemaDict.Values)
		{
			oleDbCommand.CommandText = "insert into OPE_NURSING_SCHEMA (OPE_NURSING_ID, SCHEMA_ID, SCHEMA_BG, SCHEMA_ITEM) values (?, ?, ?, ?)";
			oleDbCommand.Parameters.Clear();
			oleDbCommand.Parameters.Add("OPE_NURSING_ID", OleDbType.Numeric).Value = result;
			oleDbCommand.Parameters.Add("SCHEMA_ID", OleDbType.Numeric).Value = num;
			oleDbCommand.Parameters.Add("SCHEMA_BG", OleDbType.Numeric).Value = value2.Bg;
			oleDbCommand.Parameters.Add("SCHEMA_ITEM", OleDbType.VarWChar).Value = value2.Item;
			oleDbCommand.ExecuteNonQuery();
			num++;
		}
		openDBConn.Close();
		return true;
	}
}
