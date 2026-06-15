using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class Dict
{
	public class Sekou
	{
		public int Code;

		public string FullName;

		public string ShortName;

		public Sekou()
		{
			Code = 0;
			FullName = "";
			ShortName = "";
		}

		public Sekou(int code, string fullName, string shortName)
		{
			Code = code;
			FullName = fullName;
			ShortName = shortName;
		}
	}

	public class Dept
	{
		public int Code;

		public string FullName;

		public string ShortName;

		public Dept()
		{
			Code = 0;
			FullName = "";
			ShortName = "";
		}

		public Dept(int code, string fullName, string shortName)
		{
			Code = code;
			FullName = fullName;
			ShortName = shortName;
		}
	}

	public class Doctor
	{
		public int Code;

		public string Name = "";

		public int Status;

		public int DeptCode;

		public int StaffCode;

		public string StaffName = "";

		public Doctor()
		{
			Code = 0;
			Name = "";
			Status = 0;
			DeptCode = 0;
			StaffCode = 0;
			StaffName = "";
		}

		public Doctor(int code, string name, int status, int deptCode)
		{
			Code = code;
			Name = name;
			Status = status;
			DeptCode = deptCode;
		}

		public Doctor(int code, string name, int status, int deptCode, int staffCode, string staffName)
		{
			Code = code;
			Name = name;
			Status = status;
			DeptCode = deptCode;
			StaffCode = staffCode;
			StaffName = staffName;
		}
	}

	public class Staff
	{
		public int Code;

		public string Name = "";

		public string Kana = "";

		public int QualCode;

		public int Status;

		public int SectionCode;

		public int DeptCode;

		public int DoctorCode;

		public string LastName
		{
			get
			{
				string text = "";
				if (Name.Contains(" "))
				{
					return Name.Split(' ')[0];
				}
				return Name;
			}
		}

		public string FirstName
		{
			get
			{
				string result = "";
				if (Name.Contains(" "))
				{
					result = Name.Split(' ')[1];
				}
				return result;
			}
		}

		public string QualFullName
		{
			get
			{
				string result = "";
				if (QualDict.ContainsKey(QualCode.ToString()))
				{
					result = QualDict[QualCode.ToString()].FullName;
				}
				return result;
			}
		}

		public string QualShortName
		{
			get
			{
				string result = "";
				if (QualDict.ContainsKey(QualCode.ToString()))
				{
					result = QualDict[QualCode.ToString()].ShortName;
				}
				return result;
			}
		}

		public string SectionFullName
		{
			get
			{
				string result = "";
				if (SectionDict.ContainsKey(SectionCode.ToString()))
				{
					result = SectionDict[SectionCode.ToString()].FullName;
				}
				return result;
			}
		}

		public string SectionShortName
		{
			get
			{
				string result = "";
				if (SectionDict.ContainsKey(SectionCode.ToString()))
				{
					result = SectionDict[SectionCode.ToString()].ShortName;
				}
				return result;
			}
		}

		public bool IsDoctor
		{
			get
			{
				bool result = false;
				if (QualCode.Equals(1))
				{
					result = true;
				}
				return result;
			}
		}

		public bool IsNurse
		{
			get
			{
				bool result = false;
				if (QualCode.Equals(16))
				{
					result = true;
				}
				return result;
			}
		}

		public bool IsDrug
		{
			get
			{
				bool result = false;
				if (QualCode.Equals(4))
				{
					result = true;
				}
				return result;
			}
		}

		public Staff()
		{
		}

		public Staff(int code, string name, string kana, int sectionCode, int status, int qualCode, int deptCode, int doctorCode)
		{
			Code = code;
			Name = name;
			Kana = kana;
			SectionCode = sectionCode;
			Status = status;
			QualCode = qualCode;
			DeptCode = deptCode;
			DoctorCode = doctorCode;
		}
	}

	public class Section
	{
		public int Code;

		public string FullName;

		public string ShortName;

		public int Kind1;

		public Section()
		{
			Code = 0;
			FullName = "";
			ShortName = "";
			Kind1 = 0;
		}

		public Section(int code, string fullName, string shortName, int kind1)
		{
			Code = code;
			FullName = fullName;
			ShortName = shortName;
			Kind1 = kind1;
		}
	}

	public class Qual
	{
		public int Code;

		public string FullName;

		public string ShortName;

		public int Kind1;

		public Qual()
		{
			Code = 0;
			FullName = "";
			ShortName = "";
			Kind1 = 0;
		}

		public Qual(int code, string fullName, string shortName, int kind1)
		{
			Code = code;
			FullName = fullName;
			ShortName = shortName;
			Kind1 = kind1;
		}
	}

	private static Dictionary<string, string> inOutDict;

	private static Dictionary<string, string> wardDict;

	private static Dictionary<string, string> kouiDict;

	private static Dictionary<string, Sekou> sekouDict;

	private static Dictionary<string, Dept> deptDict;

	private static Dictionary<string, Doctor> doctorDict;

	private static Dictionary<string, Staff> staffDict;

	private static Dictionary<string, string> holidayDict;

	private static Dictionary<string, Section> sectionDict;

	private static Dictionary<string, Qual> qualDict;

	public static Dictionary<string, string> InOutDict
	{
		get
		{
			if (inOutDict == null || inOutDict.Count == 0)
			{
				InitDict();
			}
			return inOutDict;
		}
	}

	public static Dictionary<string, string> WardDict
	{
		get
		{
			if (wardDict == null || wardDict.Count == 0)
			{
				InitDict();
			}
			return wardDict;
		}
	}

	public static Dictionary<string, string> KouiDict
	{
		get
		{
			if (kouiDict == null || kouiDict.Count == 0)
			{
				InitDict();
			}
			return kouiDict;
		}
	}

	public static Dictionary<string, Sekou> SekouDict
	{
		get
		{
			if (sekouDict == null || sekouDict.Count == 0)
			{
				InitDict();
			}
			return sekouDict;
		}
	}

	public static Dictionary<string, Dept> DeptDict
	{
		get
		{
			if (deptDict == null || deptDict.Count == 0)
			{
				InitDict();
			}
			return deptDict;
		}
	}

	public static Dictionary<string, Doctor> DoctorDict
	{
		get
		{
			if (doctorDict == null || doctorDict.Count == 0)
			{
				InitDict();
			}
			return doctorDict;
		}
	}

	public static Dictionary<string, Staff> StaffDict
	{
		get
		{
			if (staffDict == null || staffDict.Count == 0)
			{
				InitDict();
			}
			return staffDict;
		}
	}

	public static Dictionary<string, string> HolidayDict
	{
		get
		{
			if (holidayDict == null || holidayDict.Count == 0)
			{
				InitDict2();
			}
			return holidayDict;
		}
	}

	public static Dictionary<string, Section> SectionDict
	{
		get
		{
			if (sectionDict == null || sectionDict.Count == 0)
			{
				InitDict();
			}
			return sectionDict;
		}
	}

	public static Dictionary<string, Qual> QualDict
	{
		get
		{
			if (qualDict == null || qualDict.Count == 0)
			{
				InitDict();
			}
			return qualDict;
		}
	}

	private static void InitDict()
	{
		inOutDict = new Dictionary<string, string>();
		inOutDict.Add("0", "");
		inOutDict.Add("1", "外来");
		inOutDict.Add("2", "入院");
		wardDict = new Dictionary<string, string>();
		wardDict.Add("00", "");
		wardDict.Add("03", "わかば");
		wardDict.Add("04", "さくら");
		wardDict.Add("05", "あやめ");
		wardDict.Add("99", "病棟未定");
		kouiDict = new Dictionary<string, string>();
		sekouDict = new Dictionary<string, Sekou>();
		deptDict = new Dictionary<string, Dept>();
		doctorDict = new Dictionary<string, Doctor>();
		staffDict = new Dictionary<string, Staff>();
		sectionDict = new Dictionary<string, Section>();
		qualDict = new Dictionary<string, Qual>();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select CODE, Trim(NAME) from M_SHINKU" + Env.DB_LINK + " order by CODE";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			kouiDict.Add(oleDbDataReader[0].ToString(), oleDbDataReader[1].ToString());
		}
		oleDbDataReader.Close();
		oleDbCommand.CommandText = "select CODE, Trim(NAME), Trim(S_NAME) from M_SEKOU" + Env.DB_LINK + " order by CODE";
		oleDbDataReader = oleDbCommand.ExecuteReader();
		sekouDict.Add("0", new Sekou());
		while (oleDbDataReader.Read())
		{
			sekouDict.Add(oleDbDataReader[0].ToString(), new Sekou(int.Parse(oleDbDataReader[0].ToString()), oleDbDataReader[1].ToString().Trim(), oleDbDataReader[2].ToString().Trim()));
		}
		oleDbDataReader.Close();
		oleDbCommand.CommandText = "select CODE, Trim(NAME), Trim(S_NAME) from M_DEPT" + Env.DB_LINK + " order by CODE";
		oleDbDataReader = oleDbCommand.ExecuteReader();
		deptDict.Add("0", new Dept());
		while (oleDbDataReader.Read())
		{
			deptDict.Add(oleDbDataReader[0].ToString(), new Dept(int.Parse(oleDbDataReader[0].ToString()), oleDbDataReader[1].ToString().Trim(), oleDbDataReader[2].ToString().Trim()));
		}
		oleDbDataReader.Close();
		oleDbCommand.CommandText = "select t1.CODE, Trim(t1.NAME), t1.CATEGORY, t1.VAL_4, t2.CODE, Trim(t2.NAME) from M_DR" + Env.DB_LINK + " t1 left join M_USR" + Env.DB_LINK + " t2 on t1.CODE = t2.DR where SYOZOKU = 1 order by t1.CODE";
		oleDbDataReader = oleDbCommand.ExecuteReader();
		doctorDict.Add("0", new Doctor());
		while (oleDbDataReader.Read())
		{
			Doctor doctor = new Doctor();
			int.TryParse(oleDbDataReader[0].ToString(), out doctor.Code);
			doctor.Name = oleDbDataReader[1].ToString().Trim();
			int.TryParse(oleDbDataReader[2].ToString(), out doctor.Status);
			int.TryParse(oleDbDataReader[3].ToString(), out doctor.DeptCode);
			int.TryParse(oleDbDataReader[4].ToString(), out doctor.StaffCode);
			doctor.StaffName = oleDbDataReader[5].ToString().Trim();
			doctorDict.Add(doctor.Code.ToString(), doctor);
		}
		oleDbDataReader.Close();
		oleDbCommand.CommandText = "select CODE, Trim(NAME), Trim(KANA), SYOZOKU, 0 , SHIKAKU, DEPT, DR from M_USR" + Env.DB_LINK + " order by CODE";
		oleDbDataReader = oleDbCommand.ExecuteReader();
		staffDict.Add("0", new Staff());
		while (oleDbDataReader.Read())
		{
			int sectionCode = 0;
			if (oleDbDataReader[3].ToString().Length > 0)
			{
				sectionCode = int.Parse(oleDbDataReader[3].ToString());
			}
			int status = 0;
			if (oleDbDataReader[4].ToString().Length > 0)
			{
				status = int.Parse(oleDbDataReader[4].ToString());
			}
			int qualCode = 0;
			if (oleDbDataReader[5].ToString().Length > 0)
			{
				qualCode = int.Parse(oleDbDataReader[5].ToString());
			}
			int deptCode = 0;
			if (oleDbDataReader[6].ToString().Length > 0)
			{
				deptCode = int.Parse(oleDbDataReader[6].ToString());
			}
			int doctorCode = 0;
			if (oleDbDataReader[7].ToString().Length > 0)
			{
				doctorCode = int.Parse(oleDbDataReader[7].ToString());
			}
			staffDict.Add(oleDbDataReader[0].ToString(), new Staff(int.Parse(oleDbDataReader[0].ToString()), oleDbDataReader[1].ToString().Trim(), oleDbDataReader[2].ToString().Trim(), sectionCode, status, qualCode, deptCode, doctorCode));
		}
		oleDbDataReader.Close();
		oleDbCommand.CommandText = "select CODE, Trim(NAME), Trim(S_NAME), CATEGORY from M_SYOZOKU" + Env.DB_LINK + " order by CODE";
		oleDbDataReader = oleDbCommand.ExecuteReader();
		sectionDict.Add("0", new Section());
		while (oleDbDataReader.Read())
		{
			sectionDict.Add(oleDbDataReader[0].ToString(), new Section(int.Parse(oleDbDataReader[0].ToString()), oleDbDataReader[1].ToString().Trim(), oleDbDataReader[2].ToString().Trim(), int.Parse(oleDbDataReader[3].ToString().Trim())));
		}
		oleDbDataReader.Close();
		oleDbCommand.CommandText = "select CODE, Trim(NAME), Trim(S_NAME), CATEGORY from M_SHIKAKU" + Env.DB_LINK + " order by CODE";
		oleDbDataReader = oleDbCommand.ExecuteReader();
		qualDict.Add("0", new Qual());
		while (oleDbDataReader.Read())
		{
			qualDict.Add(oleDbDataReader[0].ToString(), new Qual(int.Parse(oleDbDataReader[0].ToString()), oleDbDataReader[1].ToString().Trim(), oleDbDataReader[2].ToString().Trim(), int.Parse(oleDbDataReader[3].ToString().Trim())));
		}
		openDBConn.Close();
	}

	private static void InitDict2()
	{
		holidayDict = new Dictionary<string, string>();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select TM01RC_F02, TM01RC_F03, TM01RC_F04, TM01RC_F06 from TM01RC" + Env.DB_LINK + " where TM01RC_F01 = 99999 and TM01RC_F05 = 1";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		string text = "";
		string text2 = "";
		while (oleDbDataReader.Read())
		{
			text = oleDbDataReader["TM01RC_F02"].ToString() + oleDbDataReader["TM01RC_F03"].ToString().PadLeft(2, '0') + oleDbDataReader["TM01RC_F04"].ToString().PadLeft(2, '0');
			text2 = oleDbDataReader["TM01RC_F06"].ToString();
			holidayDict.Add(text, text2);
		}
		oleDbDataReader.Close();
		openDBConn.Close();
	}
}
