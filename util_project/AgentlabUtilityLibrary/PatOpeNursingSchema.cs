using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatOpeNursingSchema
{
	public int OpeNursingId;

	public int Id;

	public int Bg = -1;

	public string Item = "";

	public static Dictionary<string, PatOpeNursingSchema> GetDict(int ope_nursing_id)
	{
		Dictionary<string, PatOpeNursingSchema> dictionary = new Dictionary<string, PatOpeNursingSchema>();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select * from OPE_NURSING_SCHEMA where OPE_NURSING_ID = " + ope_nursing_id + " order by SCHEMA_ID";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatOpeNursingSchema patOpeNursingSchema = new PatOpeNursingSchema();
			int.TryParse(oleDbDataReader["OPE_NURSING_ID"].ToString(), out patOpeNursingSchema.OpeNursingId);
			int.TryParse(oleDbDataReader["SCHEMA_ID"].ToString(), out patOpeNursingSchema.Id);
			int.TryParse(oleDbDataReader["SCHEMA_BG"].ToString(), out patOpeNursingSchema.Bg);
			patOpeNursingSchema.Item = oleDbDataReader["SCHEMA_ITEM"].ToString();
			if (!dictionary.ContainsKey(patOpeNursingSchema.Id.ToString()))
			{
				dictionary.Add(patOpeNursingSchema.Id.ToString(), patOpeNursingSchema);
			}
		}
		openDBConn.Close();
		return dictionary;
	}
}
