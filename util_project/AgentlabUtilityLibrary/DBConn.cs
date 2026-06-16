using System.Data.OleDb;
using System.IO;
using System.Text;

namespace AgentlabUtilityLibrary;

public class DBConn
{
	private static string dbName = Env.MAIN_DB;

	private static string dbUser = Env.MAIN_USER;

	private static string dbPwd = Env.MAIN_PWD;

	private static string openDbName = Env.OPEN_DB;

	private static string openDbUser = Env.OPEN_USER;

	private static string openDbPwd = Env.OPEN_PWD;

	public static void Init()
	{
		string path = "C:\\windows\\macs.ini";
		StreamReader streamReader = new StreamReader(path, Encoding.Default);
		bool flag = true;
		bool flag2 = true;
		string text;
		while ((text = streamReader.ReadLine()) != null && (flag || flag2))
		{
			if (text.IndexOf('=') >= 0)
			{
				string text2 = text.Substring(0, text.IndexOf('=')).Trim();
				string text3 = text.Substring(text.IndexOf('=') + 1).Trim();
				if (text2 == "DBNAME")
				{
					dbName = text3;
					flag = false;
				}
				else if (text2 == "CONNECT")
				{
					dbUser = text3.Substring(0, text3.IndexOf('/')).Trim();
					dbPwd = text3.Substring(text3.IndexOf('/') + 1).Trim();
					flag2 = false;
				}
			}
		}
		streamReader.Close();
	}

	public static OleDbConnection GetDBConn()
	{
		if (dbName.Length == 0 || dbUser.Length == 0 || dbPwd.Length == 0)
		{
			Init();
		}
		return new OleDbConnection("Provider=" + Env.PROVIDER + ";Data Source=" + dbName + ";User ID=" + dbUser + ";Password=" + dbPwd);
	}

	public static OleDbConnection GetOpenDBConn()
	{
		return new OleDbConnection("Provider=" + Env.PROVIDER + ";Data Source=" + openDbName + ";User ID=" + openDbUser + ";Password=" + openDbPwd);
	}
}
