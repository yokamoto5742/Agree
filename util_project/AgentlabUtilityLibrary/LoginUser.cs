using System.Data.OleDb;
using System.IO;
using System.Text;

namespace AgentlabUtilityLibrary;

public class LoginUser
{
	private static string id = "";

	private static string name = "";

	private static string id2 = "";

	private static string name2 = "";

	private static string section_id = "";

	private static string section_name = "";

	private static string qual_id = "";

	private static string qual_name = "";

	private static string dept_id = "";

	private static string doctor_id = "";

	private static string[] patCont = new string[50];

	public static string Id => id;

	public static string Name => name;

	public static string Id2
	{
		get
		{
			return id2;
		}
		set
		{
			id2 = value;
		}
	}

	public static string Name2 => name2;

	public static string SectionId => section_id;

	public static string SectionName => section_name;

	public static string QualId => qual_id;

	public static string QualName => qual_name;

	public static string DeptId => dept_id;

	public static string DeptName
	{
		get
		{
			string result = "";
			if (dept_id.Length > 0 && Dict.DeptDict.ContainsKey(dept_id))
			{
				result = Dict.DeptDict[dept_id].ShortName;
			}
			return result;
		}
	}

	public static string DoctorId => doctor_id;

	public static string DoctorName
	{
		get
		{
			string result = "";
			if (doctor_id.Length > 0 && Dict.DoctorDict.ContainsKey(doctor_id))
			{
				result = Dict.DoctorDict[doctor_id].Name;
			}
			return result;
		}
	}

	public static bool IsDoctor
	{
		get
		{
			bool result = false;
			if (qual_id.Equals("1"))
			{
				result = true;
			}
			return result;
		}
	}

	public static bool IsNurse
	{
		get
		{
			bool result = false;
			if (qual_id.Equals("11"))
			{
				result = true;
			}
			return result;
		}
	}

	public static bool IsDrug
	{
		get
		{
			bool result = false;
			if (qual_id.Equals("21"))
			{
				result = true;
			}
			return result;
		}
	}

	public static bool IsReha
	{
		get
		{
			bool result = false;
			if (qual_id.Equals("25"))
			{
				result = true;
			}
			return result;
		}
	}

	public static Dict.Staff LoginStaff
	{
		get
		{
			Dict.Staff staff = new Dict.Staff();
			int.TryParse(id, out staff.Code);
			staff.Name = name;
			int.TryParse(dept_id, out staff.DeptCode);
			int.TryParse(doctor_id, out staff.DoctorCode);
			return staff;
		}
	}

	public static bool Init()
	{
		readPatCSV();
		if (id.Length == 0)
		{
			showLoginPrompt();
		}
		if (id.Length > 0)
		{
			return true;
		}
		return false;
	}

	private static void showLoginPrompt()
	{
		LoginPrompt loginPrompt = new LoginPrompt();
		loginPrompt.ShowDialog();
	}

	private static void readPatCSV()
	{
		string path = Env.LEGACY_HOME + "\\Pat.csv";
		if (!File.Exists(path))
		{
			return;
		}
		id = "";
		name = "";
		id2 = "";
		name2 = "";
		section_id = "";
		section_name = "";
		qual_id = "";
		qual_name = "";
		dept_id = "";
		doctor_id = "";
		StreamReader streamReader = new StreamReader(path, Encoding.Default);
		string text;
		if ((text = streamReader.ReadLine()) != null)
		{
			for (int i = 0; i < 50; i++)
			{
				patCont[i] = text.Split(',')[i];
			}
			id = patCont[9].TrimStart('0');
			name = patCont[10];
			if (!patCont[32].Equals("0"))
			{
				id2 = patCont[32].TrimStart('0');
			}
			qual_id = patCont[27];
			if (!patCont[13].Equals("000"))
			{
				dept_id = patCont[13].TrimStart('0');
			}
			if (!patCont[11].Equals("00000"))
			{
				doctor_id = patCont[11].TrimStart('0');
			}
		}
		streamReader.Dispose();
	}

	public static void SetUser(string _id)
	{
		int result = 0;
		if (_id.Length == 0 || !int.TryParse(_id, out result))
		{
			return;
		}
		id = _id;
		OleDbConnection dBConn = DBConn.GetDBConn();
		dBConn.Open();
		string cmdText = "select CODE, Trim(NAME), SYOZOKU, DEPT, DR from M_USR where CODE = " + id;
		OleDbCommand oleDbCommand = new OleDbCommand(cmdText, dBConn);
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		if (oleDbDataReader.Read())
		{
			id = oleDbDataReader[0].ToString();
			name = oleDbDataReader[1].ToString();
			qual_id = oleDbDataReader[2].ToString();
			if (oleDbDataReader[3].ToString().Length > 0 && oleDbDataReader[3].ToString() != "0")
			{
				dept_id = oleDbDataReader[3].ToString();
			}
			if (oleDbDataReader[4].ToString().Length > 0 && oleDbDataReader[4].ToString() != "0")
			{
				doctor_id = oleDbDataReader[4].ToString();
			}
		}
		dBConn.Close();
	}
}
