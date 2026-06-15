using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatInfection
{
	public string Id = "";

	public string Date = "";

	public Dictionary<string, PatInfectionData> DictData = new Dictionary<string, PatInfectionData>();

	public string ResultString
	{
		get
		{
			string text = DateTimeAgent.DateFormat(Date, DateTimeAgent.DateFormatKind.LONG);
			foreach (PatInfectionData value in DictData.Values)
			{
				string text2 = text;
				text = text2 + ", " + value.Name + " " + value.Data;
			}
			return text;
		}
	}

	public static PatInfection GetPatInfection(string pt_id)
	{
		PatInfection patInfection = new PatInfection();
		if (pt_id.Length == 0)
		{
			return patInfection;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select D.患者コード as 患者コード, D.検査日 as 検査日, D.感染症コード as 感染症コード, M.項目名 as 項目名, D.検査結果 as 検査結果 from AMB_感染症項目マスター" + Env.DB_LINK + " M, ADT_感染症情報データ" + Env.DB_LINK + " D where D.患者コード = " + pt_id + " and D.感染症コード = M.感染症コード order by D.検査日 desc";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			if (patInfection.Id.Length == 0)
			{
				patInfection.Id = oleDbDataReader["患者コード"].ToString();
				patInfection.Date = oleDbDataReader["検査日"].ToString();
				patInfection.DictData.Add(oleDbDataReader["感染症コード"].ToString(), new PatInfectionData(oleDbDataReader["感染症コード"].ToString(), oleDbDataReader["項目名"].ToString(), oleDbDataReader["検査結果"].ToString()));
				continue;
			}
			if (!patInfection.Date.Equals(oleDbDataReader["検査日"].ToString()))
			{
				break;
			}
			if (!patInfection.DictData.ContainsKey(oleDbDataReader["感染症コード"].ToString()))
			{
				patInfection.DictData.Add(oleDbDataReader["感染症コード"].ToString(), new PatInfectionData(oleDbDataReader["感染症コード"].ToString(), oleDbDataReader["項目名"].ToString(), oleDbDataReader["検査結果"].ToString()));
			}
		}
		openDBConn.Close();
		return patInfection;
	}

	public static Dictionary<string, PatInfection> GetDict(string pt_ids)
	{
		Dictionary<string, PatInfection> dictionary = new Dictionary<string, PatInfection>();
		if (pt_ids.Length == 0)
		{
			return dictionary;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select D.患者コード as 患者コード, D.検査日 as 検査日, D.感染症コード as 感染症コード, M.項目名 as 項目名, D.検査結果 as 検査結果 from AMB_感染症項目マスター" + Env.DB_LINK + " M, ADT_感染症情報データ" + Env.DB_LINK + " D where D.患者コード in (" + pt_ids + ") and D.感染症コード = M.感染症コード order by D.患者コード, D.検査日 desc";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		PatInfection patInfection = new PatInfection();
		while (oleDbDataReader.Read())
		{
			if (patInfection.Id.Length == 0)
			{
				patInfection.Id = oleDbDataReader["患者コード"].ToString();
				patInfection.Date = oleDbDataReader["検査日"].ToString();
				patInfection.DictData.Add(oleDbDataReader["感染症コード"].ToString(), new PatInfectionData(oleDbDataReader["感染症コード"].ToString(), oleDbDataReader["項目名"].ToString(), oleDbDataReader["検査結果"].ToString()));
			}
			else if (!patInfection.Id.Equals(oleDbDataReader["患者コード"].ToString()))
			{
				if (!dictionary.ContainsKey(patInfection.Id))
				{
					dictionary.Add(patInfection.Id, patInfection);
				}
				patInfection = new PatInfection();
				patInfection.Id = oleDbDataReader["患者コード"].ToString();
				patInfection.Date = oleDbDataReader["検査日"].ToString();
				patInfection.DictData.Add(oleDbDataReader["感染症コード"].ToString(), new PatInfectionData(oleDbDataReader["感染症コード"].ToString(), oleDbDataReader["項目名"].ToString(), oleDbDataReader["検査結果"].ToString()));
			}
			else if (patInfection.Date.Equals(oleDbDataReader["検査日"].ToString()) && !patInfection.DictData.ContainsKey(oleDbDataReader["感染症コード"].ToString()))
			{
				patInfection.DictData.Add(oleDbDataReader["感染症コード"].ToString(), new PatInfectionData(oleDbDataReader["感染症コード"].ToString(), oleDbDataReader["項目名"].ToString(), oleDbDataReader["検査結果"].ToString()));
			}
		}
		if (patInfection.Id.Length > 0 && !dictionary.ContainsKey(patInfection.Id))
		{
			dictionary.Add(patInfection.Id, patInfection);
		}
		openDBConn.Close();
		return dictionary;
	}

	public static Dictionary<string, PatInfection> GetDict(List<string> pt_list)
	{
		string text = "";
		foreach (string item in pt_list)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += item;
		}
		return GetDict(text);
	}
}
