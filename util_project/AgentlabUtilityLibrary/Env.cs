using System.IO;
using System.Text;

namespace AgentlabUtilityLibrary;

public static class Env
{
	private static string legacy_home = "";

	private static string agent_home = "";

	private static string main_db = "";

	private static string main_user = "";

	private static string main_pwd = "";

	private static string db_link = "";

	private static string open_db = "";

	private static string open_user = "";

	private static string open_pwd = "";

	public static string LEGACY_HOME
	{
		get
		{
			if (legacy_home == null || legacy_home.Length == 0)
			{
				init();
			}
			return legacy_home;
		}
	}

	public static string AGENT_HOME
	{
		get
		{
			if (agent_home == null || agent_home.Length == 0)
			{
				init();
			}
			return agent_home;
		}
	}

	public static string MAIN_DB
	{
		get
		{
			if (main_db == null || main_db.Length == 0)
			{
				init();
			}
			return main_db;
		}
	}

	public static string MAIN_USER
	{
		get
		{
			if (main_user == null || main_user.Length == 0)
			{
				init();
			}
			return main_user;
		}
	}

	public static string MAIN_PWD
	{
		get
		{
			if (main_pwd == null || main_pwd.Length == 0)
			{
				init();
			}
			return main_pwd;
		}
	}

	public static string DB_LINK
	{
		get
		{
			if (db_link == null || db_link.Length == 0)
			{
				init();
			}
			return db_link;
		}
	}

	public static string OPEN_DB
	{
		get
		{
			if (open_db == null || open_db.Length == 0)
			{
				init();
			}
			return open_db;
		}
	}

	public static string OPEN_USER
	{
		get
		{
			if (open_user == null || open_user.Length == 0)
			{
				init();
			}
			return open_user;
		}
	}

	public static string OPEN_PWD
	{
		get
		{
			if (open_pwd == null || open_pwd.Length == 0)
			{
				init();
			}
			return open_pwd;
		}
	}

	private static void init()
	{
		string path = Directory.GetCurrentDirectory() + "\\AgentlabUtilityLibrary.ini";
		if (!File.Exists(path))
		{
			path = "c:\\macs\\utility\\AgentlabUtilityLibrary.ini";
		}
		if (File.Exists(path))
		{
			StreamReader streamReader = new StreamReader(path, Encoding.Default);
			string[] array = new string[2];
			bool flag = false;
			bool flag2 = false;
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				array[0] = "";
				array[1] = "";
				switch (text)
				{
				case "[HOME Config Start]":
					flag = true;
					continue;
				case "[HOME Config End]":
					flag = false;
					continue;
				case "[DB Config Start]":
					flag2 = true;
					continue;
				case "[DB Config End]":
					flag2 = false;
					continue;
				}
				if (flag)
				{
					if (text.Contains("="))
					{
						array[0] = text.Split('=')[0];
						if (text.Length > array[0].Length + 1)
						{
							array[1] = text.Substring(array[0].Length + 1);
						}
					}
					if (array[0].Equals("LEGACY_HOME"))
					{
						legacy_home = array[1];
					}
					else if (array[0].Equals("AGENT_HOME"))
					{
						agent_home = array[1];
					}
				}
				if (!flag2 || !text.Contains("="))
				{
					continue;
				}
				array[0] = text.Split('=')[0];
				if (text.Length > array[0].Length + 1)
				{
					array[1] = text.Substring(array[0].Length + 1);
					if (array[0].Equals("MAIN_DB"))
					{
						main_db = Enc.Decrypt(array[1]);
					}
					else if (array[0].Equals("MAIN_USER"))
					{
						main_user = Enc.Decrypt(array[1]);
					}
					else if (array[0].Equals("MAIN_PWD"))
					{
						main_pwd = Enc.Decrypt(array[1]);
					}
					else if (array[0].Equals("DB_LINK"))
					{
						db_link = Enc.Decrypt(array[1]);
					}
					else if (array[0].Equals("OPEN_DB"))
					{
						open_db = Enc.Decrypt(array[1]);
					}
					else if (array[0].Equals("OPEN_USER"))
					{
						open_user = Enc.Decrypt(array[1]);
					}
					else if (array[0].Equals("OPEN_PWD"))
					{
						open_pwd = Enc.Decrypt(array[1]);
					}
				}
			}
			streamReader.Close();
		}
		else
		{
			legacy_home = "C:\\macs";
			agent_home = "C:\\macs\\utility";
			main_db = "WGS_ODBC_ORCL";
			main_user = "macs";
			main_pwd = "system";
			db_link = "";
			open_db = "WGS_ODBC_ORCL";
			open_user = "macs";
			open_pwd = "system";
		}
	}
}
