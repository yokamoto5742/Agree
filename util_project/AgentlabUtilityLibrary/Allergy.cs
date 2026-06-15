using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class Allergy
{
	public string GroupName = "";

	public int SEQ;

	public string Name = "";

	public string Cont = "";

	public string SaveDate = "";

	public string SaveStaff = "";

	public static List<Allergy> GetList(string pt_id)
	{
		List<Allergy> result = new List<Allergy>();
		if (pt_id.Length == 0)
		{
			return result;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select t1.* , t2.NAME from (select * from D_ALLERGY where INPUT_TYPE = 0 ) t1 left outer join (select * from M_ALLERGY_CLASS where INPUT_TYPE = 0 ) t2 on t1.CLASS_NO = t2.CLASS_NO  where t1.P_ID = " + pt_id + " order by t1.CLASS_NO, t1.ITEM_NO";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			Allergy allergy = new Allergy();
			allergy.GroupName = oleDbDataReader["NAME"].ToString();
			allergy.Name = oleDbDataReader["ITEM_NAME"].ToString();
			allergy.Cont = oleDbDataReader["ITEM_CONT"].ToString();
			allergy.SaveDate = oleDbDataReader["UP_DATE"].ToString();
			allergy.SaveStaff = oleDbDataReader["UP_USR"].ToString();
		}
		oleDbDataReader.Close();
		openDBConn.Close();
		return result;
	}
}
