using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatOpeNursingAs
{
	public int OpeNursingId;

	public int Id;

	public string Title = "";

	public string Text = "";

	public static Dictionary<string, PatOpeNursingAs> GetDict(int ope_nursing_id)
	{
		Dictionary<string, PatOpeNursingAs> dictionary = new Dictionary<string, PatOpeNursingAs>();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select * from OPE_NURSING_AS where OPE_NURSING_ID = " + ope_nursing_id + " order by AS_ID";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatOpeNursingAs patOpeNursingAs = new PatOpeNursingAs();
			int.TryParse(oleDbDataReader["OPE_NURSING_ID"].ToString(), out patOpeNursingAs.OpeNursingId);
			int.TryParse(oleDbDataReader["AS_ID"].ToString(), out patOpeNursingAs.Id);
			patOpeNursingAs.Title = oleDbDataReader["AS_Title"].ToString();
			patOpeNursingAs.Text = oleDbDataReader["AS_TEXT"].ToString();
			if (!dictionary.ContainsKey(patOpeNursingAs.Id.ToString()))
			{
				dictionary.Add(patOpeNursingAs.Id.ToString(), patOpeNursingAs);
			}
		}
		openDBConn.Close();
		return dictionary;
	}
}
