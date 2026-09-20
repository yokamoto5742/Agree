using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AgentlabUtilityLibrary;

/// <summary>
/// AgentlabUtilityLibrary.ini(カレントディレクトリ → c:\macs\utility の順に探す)から読み込む実行環境の設定。
/// 初回アクセス時に1回だけ読み込む。
/// INIファイルが無い場合、LEGACY_HOME / AGENT_HOME / PROVIDER は既定値になり、DB接続情報は空になる。
/// 電子カルテ側の接続情報 EHR_* は、キーが無ければ同意書側の OPEN_* / PROVIDER と同じ値になる。
/// </summary>
public static class Env
{
	private const string DEFAULT_PROVIDER = "MSDAORA.1";

	private static bool loaded;

	private static string legacy_home = "";

	private static string agent_home = "";

	private static string db_link = "";

	private static string open_db = "";

	private static string open_user = "";

	private static string open_pwd = "";

	private static string provider = "";

	private static string ehr_db = "";

	private static string ehr_user = "";

	private static string ehr_pwd = "";

	private static string ehr_provider = "";

	public static string LEGACY_HOME
	{
		get
		{
			EnsureLoaded();
			return legacy_home;
		}
	}

	public static string AGENT_HOME
	{
		get
		{
			EnsureLoaded();
			return agent_home;
		}
	}


	public static string DB_LINK
	{
		get
		{
			EnsureLoaded();
			return db_link;
		}
	}

	public static string OPEN_DB
	{
		get
		{
			EnsureLoaded();
			return open_db;
		}
	}

	public static string OPEN_USER
	{
		get
		{
			EnsureLoaded();
			return open_user;
		}
	}

	public static string OPEN_PWD
	{
		get
		{
			EnsureLoaded();
			return open_pwd;
		}
	}

	public static string PROVIDER
	{
		get
		{
			EnsureLoaded();
			return provider;
		}
	}

	public static string EHR_DB
	{
		get
		{
			EnsureLoaded();
			return ehr_db;
		}
	}

	public static string EHR_USER
	{
		get
		{
			EnsureLoaded();
			return ehr_user;
		}
	}

	public static string EHR_PWD
	{
		get
		{
			EnsureLoaded();
			return ehr_pwd;
		}
	}

	public static string EHR_PROVIDER
	{
		get
		{
			EnsureLoaded();
			return ehr_provider;
		}
	}

	private static void EnsureLoaded()
	{
		if (loaded)
		{
			return;
		}
		Load();
		loaded = true;
	}

	private static void Load()
	{
		string path = Directory.GetCurrentDirectory() + "\\AgentlabUtilityLibrary.ini";
		if (!File.Exists(path))
		{
			path = "c:\\macs\\utility\\AgentlabUtilityLibrary.ini";
		}
		if (!File.Exists(path))
		{
			legacy_home = "C:\\macs";
			agent_home = "C:\\macs\\utility";
			provider = DEFAULT_PROVIDER;
			ehr_provider = DEFAULT_PROVIDER;
			return;
		}
		Dictionary<string, string> homeSection = new Dictionary<string, string>();
		Dictionary<string, string> dbSection = new Dictionary<string, string>();
		Dictionary<string, string> currentSection = null;
		using (StreamReader reader = new StreamReader(path, Encoding.Default))
		{
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				switch (line)
				{
				case "[HOME Config Start]":
					currentSection = homeSection;
					continue;
				case "[DB Config Start]":
					currentSection = dbSection;
					continue;
				case "[HOME Config End]":
				case "[DB Config End]":
					currentSection = null;
					continue;
				}
				int separator = line.IndexOf('=');
				if (currentSection != null && separator >= 0 && separator < line.Length - 1)
				{
					currentSection[line.Substring(0, separator)] = line.Substring(separator + 1);
				}
			}
		}
		legacy_home = GetValue(homeSection, "LEGACY_HOME");
		agent_home = GetValue(homeSection, "AGENT_HOME");
		db_link = Enc.Decrypt(GetValue(dbSection, "DB_LINK"));
		open_db = Enc.Decrypt(GetValue(dbSection, "OPEN_DB"));
		open_user = Enc.Decrypt(GetValue(dbSection, "OPEN_USER"));
		open_pwd = Enc.Decrypt(GetValue(dbSection, "OPEN_PWD"));
		provider = GetValue(dbSection, "PROVIDER");
		if (provider.Length == 0)
		{
			provider = DEFAULT_PROVIDER;
		}
		ehr_db = OrDefault(Enc.Decrypt(GetValue(dbSection, "EHR_DB")), open_db);
		ehr_user = OrDefault(Enc.Decrypt(GetValue(dbSection, "EHR_USER")), open_user);
		ehr_pwd = OrDefault(Enc.Decrypt(GetValue(dbSection, "EHR_PWD")), open_pwd);
		ehr_provider = OrDefault(GetValue(dbSection, "EHR_PROVIDER"), provider);
	}

	private static string GetValue(Dictionary<string, string> section, string key)
	{
		return section.TryGetValue(key, out string value) ? value : "";
	}

	private static string OrDefault(string value, string defaultValue)
	{
		return value.Length > 0 ? value : defaultValue;
	}
}
