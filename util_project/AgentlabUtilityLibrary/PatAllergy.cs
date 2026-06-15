using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatAllergy
{
	public string PtId = "";

	public int Kind1;

	public int Kind2;

	public string KindName2 = "";

	public string Comment = "";

	public string Staff = "";

	public int SaveDate;

	public int SaveTime;

	public string KindName1
	{
		get
		{
			string result = "";
			if (Kind1.Equals(1))
			{
				result = "食物";
			}
			else if (Kind1.Equals(2))
			{
				result = "薬剤";
			}
			else if (Kind1.Equals(3))
			{
				result = "その他";
			}
			return result;
		}
	}

	public string StaffName
	{
		get
		{
			string result = "";
			if (Dict.StaffDict.ContainsKey(Staff))
			{
				result = Dict.StaffDict[Staff].Name;
			}
			return result;
		}
	}

	public static List<PatAllergy> GetList(string pt_id)
	{
		List<PatAllergy> list = new List<PatAllergy>();
		if (pt_id.Length == 0)
		{
			return list;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select 禁忌分類コード, 禁忌項目連番, 禁忌対象項目, コメント, 更新日, 更新時間, 更新者 from ADT_アレルギー禁忌情報データ" + Env.DB_LINK + " where 患者コード = " + pt_id + " order by 禁忌項目連番";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			PatAllergy patAllergy = new PatAllergy();
			patAllergy.PtId = pt_id;
			int.TryParse(oleDbDataReader["禁忌分類コード"].ToString(), out patAllergy.Kind1);
			int.TryParse(oleDbDataReader["禁忌項目連番"].ToString(), out patAllergy.Kind2);
			patAllergy.KindName2 = oleDbDataReader["禁忌対象項目"].ToString();
			patAllergy.Comment = oleDbDataReader["コメント"].ToString();
			int.TryParse(oleDbDataReader["更新日"].ToString(), out patAllergy.SaveDate);
			int.TryParse(oleDbDataReader["更新時間"].ToString(), out patAllergy.SaveTime);
			patAllergy.Staff = oleDbDataReader["更新者"].ToString();
			list.Add(patAllergy);
		}
		openDBConn.Close();
		return list;
	}
}
