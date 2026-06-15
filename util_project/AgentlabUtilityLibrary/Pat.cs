using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Text;

namespace AgentlabUtilityLibrary;

public class Pat
{
	public class Insurance
	{
		public string Serial;

		public string GroupCode;

		public string GroupName;

		public string InsCode;

		public string ShortKind;

		public string FullKind;

		public string StartDate;

		public string EndDate;

		public Insurance()
		{
			Serial = "0";
			GroupCode = "";
			GroupName = "";
			InsCode = "";
			ShortKind = "";
			FullKind = "";
			StartDate = "";
			EndDate = "";
		}

		public Insurance(string serial, string groupCode, string groupName, string insCode, string shortKind, string fullKind, string startDate, string endDate)
		{
			Serial = serial;
			GroupCode = groupCode;
			GroupName = groupName;
			InsCode = insCode;
			ShortKind = shortKind;
			FullKind = fullKind;
			StartDate = startDate;
			EndDate = endDate;
		}
	}

	public class Contact
	{
		public string Serial = "";

		public string Kana = "";

		public string Name = "";

		public string Birth = "";

		public string Relation = "";

		public string Relation_Comment = "";

		public string Tel1 = "";

		public string Tel1_Type = "";

		public string Tel2 = "";

		public string Tel2_Type = "";

		public string Tel3 = "";

		public string Tel3_Type = "";

		public string Health = "";

		public string Live = "";

		public string Care = "";

		public string Comment = "";
	}

	public class Address
	{
		public string Post = "";

		public string Addr = "";

		public string Tel = "";
	}

	public enum InfoKind
	{
		SHORT_HEIGHT = 1,
		SHORT_WEIGHT
	}

	public class Intro
	{
		public string Kind = "";

		public string IntroDate = "";

		public string Hospital = "";

		public string DeptTo = "";

		public string DoctorTo = "";

		public string DeptFromCode = "";

		public string DeptFromName = "";

		public string DoctorFromCode = "";

		public string DoctorFromName = "";
	}

	private static string id = "";

	private static string name = "";

	private static string kana = "";

	private static string sex = "";

	private static string birth = "";

	private static string age = "";

	private static string in_out = "";

	private static string dept = "";

	private static string doctor = "";

	private static string ins = "";

	private static Dictionary<string, Insurance> insDict;

	private static string[] patCont = new string[50];

	public static string Id => id;

	public static string Name => name;

	public static string Kana => kana;

	public static string Sex => sex;

	public static string Birth => birth;

	public static string Age => age;

	public static string InOut => in_out;

	public static string Dept => dept;

	public static string Doctor => doctor;

	public static string Ins => ins;

	public static Dictionary<string, Insurance> InsDict
	{
		get
		{
			if (insDict == null)
			{
				MakeInsDict();
			}
			return insDict;
		}
	}

	public static string[] PatCont => patCont;

	public static List<Intro> IntroList => GetIntroList(Id);

	public static List<Contact> ContactList
	{
		get
		{
			int result = 0;
			if (id.Length > 0 && int.TryParse(id, out result))
			{
				return GetContactList(id);
			}
			return null;
		}
	}

	public static Address Addr
	{
		get
		{
			int result = 0;
			if (id.Length > 0 && int.TryParse(id, out result))
			{
				return GetAddr(id);
			}
			return null;
		}
	}

	public static List<Intro> GetIntroList(string pt_id)
	{
		List<Intro> list = new List<Intro>();
		int result = 0;
		if (pt_id.Length == 0 || !int.TryParse(pt_id, out result))
		{
			return list;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select INTRO_KIND, INTRO_DATE, HOSPITAL, DEPT_TO, DOCTOR_TO, DEPT_FROM, DOCTOR_FROM from INTRODUCTION where PATIENT_ID = ? and DELETE_FLAG != 1 order by INTRO_DATE desc";
		oleDbCommand.Parameters.Add("PATIENT_ID", OleDbType.Numeric).Value = pt_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			Intro intro = new Intro();
			intro.Kind = oleDbDataReader["INTRO_KIND"].ToString();
			intro.IntroDate = oleDbDataReader["INTRO_DATE"].ToString().Split(' ')[0].Replace("/", "");
			intro.Hospital = oleDbDataReader["HOSPITAL"].ToString().Trim();
			intro.DeptTo = oleDbDataReader["DEPT_TO"].ToString().Trim();
			intro.DoctorTo = oleDbDataReader["DOCTOR_TO"].ToString().Trim();
			intro.DeptFromCode = oleDbDataReader["DEPT_FROM"].ToString().TrimStart('0');
			if (Dict.DeptDict.ContainsKey(intro.DeptFromCode))
			{
				intro.DeptFromName = Dict.DeptDict[intro.DeptFromCode].ShortName;
			}
			intro.DoctorFromCode = oleDbDataReader["DOCTOR_FROM"].ToString();
			if (Dict.DoctorDict.ContainsKey(intro.DoctorFromCode))
			{
				intro.DoctorFromName = Dict.DoctorDict[intro.DoctorFromCode].Name;
			}
			list.Add(intro);
		}
		openDBConn.Close();
		return list;
	}

	public static bool Init()
	{
		for (int i = 0; i < patCont.Length; i++)
		{
			patCont[i] = "";
		}
		ReadPatCSV();
		MakeInsDict();
		if (id.Length > 0)
		{
			return true;
		}
		return false;
	}

	private static void ReadPatCSV()
	{
		string path = Env.LEGACY_HOME + "\\Pat.csv";
		if (!File.Exists(path))
		{
			return;
		}
		id = "";
		name = "";
		kana = "";
		sex = "";
		birth = "";
		age = "";
		in_out = "";
		dept = "";
		doctor = "";
		ins = "";
		StreamReader streamReader = new StreamReader(path, Encoding.Default);
		string text;
		if ((text = streamReader.ReadLine()) != null)
		{
			for (int i = 0; i < 50; i++)
			{
				patCont[i] = text.Split(',')[i];
			}
			id = patCont[2].TrimStart('0');
			name = patCont[3];
			kana = patCont[4];
			sex = patCont[5];
			birth = patCont[6];
			if (birth.Length == 8)
			{
				age = DateConvert.CalcAge(birth, DateTime.Now.ToString("yyyyMMdd")).ToString();
			}
			in_out = patCont[20];
			dept = patCont[13].TrimStart('0');
			doctor = patCont[11].TrimStart('0');
			ins = patCont[31];
		}
		streamReader.Dispose();
	}

	public static void WritePatCSV()
	{
		string[] array = new string[50]
		{
			DateTime.Now.ToString("yyyyMMdd"),
			"000",
			id.PadLeft(9, '0'),
			name,
			kana,
			sex,
			birth,
			null,
			null,
			LoginUser.Id.PadLeft(5, '0'),
			LoginUser.Name,
			"00000",
			null,
			"001",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"000000",
			null,
			"0",
			null,
			"0",
			null,
			"0",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
		if (LoginUser.DoctorId.Length > 0 && !LoginUser.DoctorId.Equals("0"))
		{
			array[11] = LoginUser.DoctorId.PadLeft(5, '0');
			array[12] = Dict.DoctorDict[LoginUser.DoctorId].Name;
		}
		if (dept.Length > 0)
		{
			array[13] = dept.PadLeft(3, '0');
			array[14] = Dict.DeptDict[dept].FullName;
		}
		else if (LoginUser.DeptId.Length > 0 && !LoginUser.DeptId.Equals("0"))
		{
			array[13] = LoginUser.DeptId.PadLeft(3, '0');
			array[14] = Dict.DeptDict[LoginUser.DeptId].FullName;
		}
		if (in_out.Equals("2"))
		{
			array[20] = "2";
			if (Dict.DoctorDict.ContainsKey(doctor))
			{
				array[33] = doctor;
				array[34] = Dict.DoctorDict[doctor].Name;
			}
		}
		else
		{
			array[20] = "1";
		}
		array[31] = ins;
		if (LoginUser.Id2.Length > 0 && !LoginUser.Id2.Equals("0"))
		{
			array[32] = LoginUser.Id2;
		}
		string path = Env.LEGACY_HOME + "\\Pat.csv";
		StreamWriter streamWriter = new StreamWriter(new FileStream(path, FileMode.Create), Encoding.Default);
		for (int i = 0; i < array.Length; i++)
		{
			if (i > 0)
			{
				streamWriter.Write(',');
			}
			streamWriter.Write(array[i]);
		}
		streamWriter.Write("\r\n");
		streamWriter.Close();
	}

	public static void SetPat(string _id)
	{
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select Trim(P_KANA), Trim(P_NAME), P_SEX, P_BIRTHDAY_AD, IN_HOSPITAL, HOKEN_NOW from M_PATIENT" + Env.DB_LINK + " where P_ID = " + _id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		if (oleDbDataReader.Read())
		{
			id = _id;
			name = oleDbDataReader[1].ToString();
			kana = oleDbDataReader[0].ToString();
			sex = oleDbDataReader[2].ToString();
			birth = oleDbDataReader[3].ToString();
			if (birth.Length == 8)
			{
				age = DateConvert.CalcAge(birth, DateTime.Now.ToString("yyyyMMdd")).ToString();
			}
			else
			{
				age = "";
			}
			if (oleDbDataReader[4].ToString().Equals("1"))
			{
				in_out = "2";
			}
			else
			{
				in_out = "1";
			}
			dept = "";
			doctor = "";
			ins = oleDbDataReader[5].ToString();
		}
		oleDbDataReader.Close();
		if (!in_out.Equals("2"))
		{
			oleDbCommand.CommandText = "select DEPT, DR, P_HOKEN from D_UKETSUKE" + Env.DB_LINK + " where UKE_DATE = " + DateTime.Now.ToString("yyyyMMdd") + " and P_ID = " + _id;
			oleDbDataReader = oleDbCommand.ExecuteReader();
			if (oleDbDataReader.Read())
			{
				dept = oleDbDataReader[0].ToString();
				doctor = oleDbDataReader[1].ToString();
				ins = oleDbDataReader[2].ToString();
			}
			oleDbDataReader.Close();
		}
		openDBConn.Close();
		MakeInsDict();
	}

	private static void MakeInsDict()
	{
		if (id.Length <= 0)
		{
			return;
		}
		if (insDict != null)
		{
			insDict.Clear();
		}
		else
		{
			insDict = new Dictionary<string, Insurance>();
		}
		insDict.Add("0", new Insurance());
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select P_HOKEN, MAIN_NO, MAIN_KIGOU, MAIN_BANGOU, Trim(S_NAME), Trim(NAME), MAIN_DATE_S, MAIN_DATE_E from M_PATIENT_HOKEN" + Env.DB_LINK + " left join M_HOKEN" + Env.DB_LINK + " on HOKEN_TYPE = CODE where P_ID = " + id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			Insurance insurance = new Insurance();
			insurance.Serial = oleDbDataReader[0].ToString();
			insurance.GroupCode = oleDbDataReader[1].ToString();
			insurance.GroupName = oleDbDataReader[2].ToString();
			insurance.InsCode = oleDbDataReader[3].ToString();
			insurance.ShortKind = oleDbDataReader[4].ToString();
			insurance.FullKind = oleDbDataReader[5].ToString();
			if (oleDbDataReader[6].ToString().Length > 0)
			{
				insurance.StartDate = oleDbDataReader[6].ToString();
			}
			if (oleDbDataReader[7].ToString().Length > 0)
			{
				insurance.EndDate = oleDbDataReader[7].ToString();
			}
			insDict.Add(oleDbDataReader[0].ToString(), insurance);
		}
		oleDbDataReader.Close();
		openDBConn.Close();
	}

	public static string GetAge(string crit_day)
	{
		if (birth.Length == 8)
		{
			if (crit_day.Length == 8)
			{
				return DateConvert.CalcAge(birth, crit_day).ToString();
			}
			return DateConvert.CalcAge(birth, DateTime.Now.ToString("yyyyMMdd")).ToString();
		}
		return "";
	}

	public static List<PatOut> GetOutHistory(string start_date)
	{
		int result = 0;
		if (Id.Length == 0 || !int.TryParse(Id, out result))
		{
			return new List<PatOut>();
		}
		return PatOut.GetHistory(Id, start_date);
	}

	public static List<PatOut> GetOutHistory(string pt_id, string start_date)
	{
		int result = 0;
		if (pt_id.Length == 0 || !int.TryParse(pt_id, out result))
		{
			return new List<PatOut>();
		}
		return PatOut.GetHistory(pt_id, start_date);
	}

	public static string GetInfection()
	{
		int result = 0;
		if (Id.Length == 0 || !int.TryParse(Id, out result))
		{
			return "";
		}
		return GetInfection(Id);
	}

	public static string GetInfection(string pt_id)
	{
		int result = 0;
		if (pt_id.Length == 0 || !int.TryParse(pt_id, out result))
		{
			return "";
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select D.検査日, M.項目名, D.検査結果 from AMB_感染症項目マスター" + Env.DB_LINK + " M, ADT_感染症情報データ" + Env.DB_LINK + " D where D.P_ID = ? and D.感染症コード = M.感染症コード order by D.検査日 desc";
		oleDbCommand.Parameters.Clear();
		oleDbCommand.Parameters.Add("P_ID", OleDbType.Numeric).Value = pt_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		string text = "";
		string text2 = "";
		while (oleDbDataReader.Read())
		{
			if (text2.Length == 0)
			{
				text2 = oleDbDataReader[0].ToString();
				if (text2.Length == 8)
				{
					text += text2.Insert(4, "/").Insert(7, "/");
				}
			}
			else if (text2 != oleDbDataReader[0].ToString())
			{
				break;
			}
			if (text.Length > 0)
			{
				text += ", ";
			}
			text = text + oleDbDataReader[1].ToString() + " " + oleDbDataReader[2].ToString();
		}
		openDBConn.Close();
		return text;
	}

	public static string GetPatInfo(string pt_id)
	{
		string text = "";
		int result = 0;
		if (pt_id.Length == 0 || !int.TryParse(pt_id, out result))
		{
			return text;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select Trim(P_KANA) as カナ, Trim(P_NAME) as 氏名, P_SEX as P_SEX, P_BIRTHDAY_AD as 生年月日 from M_PATIENT" + Env.DB_LINK + " where P_ID = ?";
		oleDbCommand.Parameters.Add("P_ID", OleDbType.Numeric).Value = pt_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		if (oleDbDataReader.Read())
		{
			text = oleDbDataReader["氏名"].ToString() + "（" + oleDbDataReader["カナ"].ToString() + "）様";
			string text2 = "";
			if (oleDbDataReader["P_SEX"].ToString().Equals("1"))
			{
				text2 = "男";
			}
			else if (oleDbDataReader["P_SEX"].ToString().Equals("2"))
			{
				text2 = "女";
			}
			text = text + "\u3000" + text2;
			string text3 = oleDbDataReader["生年月日"].ToString();
			if (text3.Length == 8)
			{
				string text4 = text;
				text = text4 + "\u3000" + text3.Insert(4, "/").Insert(7, "/") + "生\u3000" + DateConvert.CalcAge(text3, DateTime.Now.ToString("yyyyMMdd")) + "歳";
			}
		}
		openDBConn.Close();
		return text;
	}

	public static string GetPatInfo(string pt_id, InfoKind info_kind)
	{
		string info_kind2 = "";
		switch (info_kind)
		{
		case InfoKind.SHORT_HEIGHT:
			info_kind2 = "150-6-12";
			break;
		case InfoKind.SHORT_WEIGHT:
			info_kind2 = "150-6-13";
			break;
		}
		return GetPatInfo(pt_id, info_kind2);
	}

	public static string GetPatInfo(string pt_id, string info_kind)
	{
		string result = "";
		if (pt_id.Length == 0 || info_kind.Length == 0)
		{
			return result;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select データ値 from ADT_患者基本情報データ" + Env.DB_LINK + " where P_ID = ? and 項目ユニークキー = ? order by UKE_INDEX desc";
		oleDbCommand.Parameters.Clear();
		oleDbCommand.Parameters.Add("P_ID", OleDbType.Numeric).Value = pt_id;
		oleDbCommand.Parameters.Add("項目ユニークキー", OleDbType.VarWChar).Value = info_kind;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		if (oleDbDataReader.Read())
		{
			result = oleDbDataReader["データ値"].ToString();
		}
		openDBConn.Close();
		return result;
	}

	public static List<Contact> GetContactList(string pt_id)
	{
		List<Contact> list = new List<Contact>();
		int result = 0;
		if (pt_id.Length == 0 || !int.TryParse(pt_id, out result))
		{
			return list;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("0", "");
		dictionary.Add("1", "本人");
		dictionary.Add("2", "夫");
		dictionary.Add("3", "妻");
		dictionary.Add("4", "息子");
		dictionary.Add("5", "娘");
		dictionary.Add("6", "父");
		dictionary.Add("7", "母");
		dictionary.Add("8", "兄");
		dictionary.Add("9", "姉");
		dictionary.Add("10", "弟");
		dictionary.Add("11", "妹");
		dictionary.Add("12", "叔父・伯父");
		dictionary.Add("13", "叔母・伯母");
		dictionary.Add("14", "甥");
		dictionary.Add("15", "姪");
		dictionary.Add("16", "祖父");
		dictionary.Add("17", "祖母");
		dictionary.Add("18", "親族");
		dictionary.Add("19", "同居人");
		dictionary.Add("20", "知人・友人");
		dictionary.Add("21", "その他");
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		dictionary2.Add("0", "");
		dictionary2.Add("1", "自宅");
		dictionary2.Add("2", "勤務先");
		dictionary2.Add("3", "携帯");
		dictionary2.Add("4", "その他");
		Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
		dictionary3.Add("0", "");
		dictionary3.Add("1", "同居");
		dictionary3.Add("2", "別居");
		dictionary3.Add("3", "その他");
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select * from ADT_患者家族連絡先データ" + Env.DB_LINK + " where P_ID = ? order by 連絡優先順位";
		oleDbCommand.Parameters.Clear();
		oleDbCommand.Parameters.Add("P_ID", OleDbType.Numeric).Value = pt_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			Contact contact = new Contact();
			contact.Serial = oleDbDataReader["連絡優先順位"].ToString();
			contact.Kana = oleDbDataReader["P_KANA"].ToString();
			contact.Name = oleDbDataReader["P_NAME"].ToString();
			if (oleDbDataReader["生年月日"].ToString().Length == 8)
			{
				contact.Birth = oleDbDataReader["生年月日"].ToString().Insert(4, "/").Insert(7, "/");
			}
			if (dictionary.ContainsKey(oleDbDataReader["続柄区分"].ToString()))
			{
				contact.Relation = dictionary[oleDbDataReader["続柄区分"].ToString()];
			}
			contact.Relation_Comment = oleDbDataReader["続柄ORDER_COMMENT"].ToString();
			contact.Tel1 = oleDbDataReader["電話番号１"].ToString();
			if (dictionary2.ContainsKey(oleDbDataReader["連絡先区DEVIDE01"].ToString()))
			{
				contact.Tel1_Type = dictionary2[oleDbDataReader["連絡先区DEVIDE01"].ToString()];
			}
			contact.Tel2 = oleDbDataReader["電話番号２"].ToString();
			if (dictionary2.ContainsKey(oleDbDataReader["連絡先区DEVIDE02"].ToString()))
			{
				contact.Tel2_Type = dictionary2[oleDbDataReader["連絡先区DEVIDE02"].ToString()];
			}
			contact.Tel3 = oleDbDataReader["電話番号３"].ToString();
			if (dictionary2.ContainsKey(oleDbDataReader["連絡先区DEVIDE03"].ToString()))
			{
				contact.Tel3_Type = dictionary2[oleDbDataReader["連絡先区DEVIDE03"].ToString()];
			}
			contact.Health = oleDbDataReader["健康状態"].ToString();
			if (dictionary3.ContainsKey(oleDbDataReader["同別居区分"].ToString()))
			{
				contact.Live = dictionary3[oleDbDataReader["同別居区分"].ToString()];
			}
			contact.Care = oleDbDataReader["介護役割"].ToString();
			contact.Comment = oleDbDataReader["備考"].ToString();
			list.Add(contact);
		}
		openDBConn.Close();
		return list;
	}

	public static Address GetAddr(string pt_id)
	{
		Address address = new Address();
		int result = 0;
		if (pt_id.Length == 0 || !int.TryParse(pt_id, out result))
		{
			return address;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select TEL, POST, ADDR_1, ADDR_2 from M_PATIENT" + Env.DB_LINK + " where P_ID = ?";
		oleDbCommand.Parameters.Clear();
		oleDbCommand.Parameters.Add("P_ID", OleDbType.Numeric).Value = pt_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		if (oleDbDataReader.Read())
		{
			address.Post = oleDbDataReader["POST"].ToString().Trim();
			address.Addr = oleDbDataReader["ADDR_1"].ToString().Trim() + oleDbDataReader["ADDR_2"].ToString().Trim();
			address.Tel = oleDbDataReader["TEL"].ToString().Trim();
		}
		openDBConn.Close();
		return address;
	}
}
