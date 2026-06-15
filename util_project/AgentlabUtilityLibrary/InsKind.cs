using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class InsKind
{
	public int Code = 1;

	public string Name = "";

	public string Short = "";

	private static Dictionary<string, InsKind> dict = new Dictionary<string, InsKind>();

	public static Dictionary<string, InsKind> Dict
	{
		get
		{
			if (dict.Count == 0)
			{
				DictUpdate();
			}
			return dict;
		}
	}

	public static void DictUpdate()
	{
		dict.Clear();
		bool flag = true;
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		if (openDBConn.State != ConnectionState.Open)
		{
			openDBConn.Open();
			flag = false;
		}
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select CODE, Trim(NAME) as 名称, Trim(S_NAME) as 略称 from M_HOKEN" + Env.DB_LINK;
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			InsKind insKind = new InsKind();
			int.TryParse(oleDbDataReader[0].ToString(), out insKind.Code);
			insKind.Name = oleDbDataReader[1].ToString();
			insKind.Short = oleDbDataReader[2].ToString();
			if (!dict.ContainsKey(insKind.Code.ToString()))
			{
				dict.Add(insKind.Code.ToString(), insKind);
			}
		}
		oleDbDataReader.Close();
		if (flag)
		{
			openDBConn.Close();
		}
	}
}
