using System;
using System.Data.OleDb;
using System.IO;
using System.Text;

namespace AgentlabUtilityLibrary;

public class Patient : PatBase
{
	public string Ward = "";

	public string Room = "";

	public string InDate = "";

	public string OutDate = "";

	public string Dept = "";

	public string Doctor = "";

	public string Ins = "";

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

	public static Patient Load(string pt_id)
	{
		Patient patient = new Patient();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select Trim(P_KANA) as カナ, Trim(P_NAME) as 氏名, P_SEX as P_SEX, P_BIRTHDAY_AD as 生年月日, IN_HOSPITAL as 入外, HOKEN_NOW as 保険 from M_PATIENT" + Env.DB_LINK + " where P_ID = " + pt_id;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		if (oleDbDataReader.Read())
		{
			patient.Id = pt_id;
			patient.Name = oleDbDataReader["氏名"].ToString();
			patient.Kana = oleDbDataReader["カナ"].ToString();
			patient.Sex = oleDbDataReader["P_SEX"].ToString();
			patient.Birth = oleDbDataReader["生年月日"].ToString();
			if (oleDbDataReader["入外"].ToString().Equals("1"))
			{
				patient.InOut = "2";
			}
			else
			{
				patient.InOut = "1";
			}
			patient.Dept = "";
			patient.Doctor = "";
			patient.Ins = oleDbDataReader["保険"].ToString();
		}
		oleDbDataReader.Close();
		if (!patient.InOut.Equals("2"))
		{
			oleDbCommand.CommandText = "select DEPT as 科, DR as 医師, P_HOKEN as 保険 from D_UKETSUKE" + Env.DB_LINK + " where UKE_DATE = " + DateTime.Now.ToString("yyyyMMdd") + " and P_ID = " + pt_id;
			oleDbDataReader = oleDbCommand.ExecuteReader();
			if (oleDbDataReader.Read())
			{
				patient.Dept = oleDbDataReader["科"].ToString();
				patient.Doctor = oleDbDataReader["医師"].ToString();
				patient.Ins = oleDbDataReader["保険"].ToString();
			}
			oleDbDataReader.Close();
		}
		openDBConn.Close();
		return patient;
	}

	public static Patient ReadPatCSV()
	{
		Patient patient = new Patient();
		string path = Env.LEGACY_HOME + "\\Pat.csv";
		if (File.Exists(path))
		{
			StreamReader streamReader = new StreamReader(path, Encoding.Default);
			string[] array = new string[50];
			string text;
			if ((text = streamReader.ReadLine()) != null)
			{
				for (int i = 0; i < 50; i++)
				{
					array[i] = text.Split(',')[i];
				}
				patient.Id = array[2].TrimStart('0');
				patient.Name = array[3];
				patient.Kana = array[4];
				patient.Sex = array[5];
				patient.Birth = array[6];
				patient.InOut = array[20];
				patient.Dept = array[13].TrimStart('0');
				patient.Doctor = array[11].TrimStart('0');
				patient.Ins = array[31];
			}
			streamReader.Dispose();
		}
		return patient;
	}

	public void WritePatCSV()
	{
		string[] array = new string[50]
		{
			DateTime.Now.ToString("yyyyMMdd"),
			"000",
			Id.PadLeft(9, '0'),
			Name,
			Kana,
			Sex,
			Birth,
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
			null,
			null
		};
		if (LoginUser.DoctorId.Length > 0 && !LoginUser.DoctorId.Equals("0"))
		{
			array[11] = LoginUser.DoctorId.PadLeft(5, '0');
		}
		if (Dept.Length > 0)
		{
			array[12] = Dept;
			array[13] = Dept.PadLeft(3, '0');
		}
		else if (LoginUser.DeptId.Length > 0 && !LoginUser.DeptId.Equals("0"))
		{
			array[12] = LoginUser.DeptId;
			array[13] = LoginUser.DeptId.PadLeft(3, '0');
		}
		if (Dict.DeptDict.ContainsKey(array[12]))
		{
			array[14] = Dict.DeptDict[array[12]].FullName;
		}
		if (InOut.Equals("2"))
		{
			array[20] = "2";
			if (Dict.DoctorDict.ContainsKey(Doctor))
			{
				array[33] = Doctor;
				array[34] = Dict.DoctorDict[Doctor].Name;
			}
		}
		else
		{
			array[20] = "1";
		}
		array[31] = Ins;
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
}
