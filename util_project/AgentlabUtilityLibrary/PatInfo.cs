using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class PatInfo
{
	public string Id = "";

	public Dictionary<string, PatInfoData> DictData = new Dictionary<string, PatInfoData>();

	public static Dictionary<string, PatInfo> GetDict(List<string> pt_list)
	{
		Dictionary<string, PatInfo> dictionary = new Dictionary<string, PatInfo>();
		if (pt_list.Count == 0)
		{
			return dictionary;
		}
		string text = "";
		foreach (string item in pt_list)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += item;
		}
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select 患者コード, 項目ユニークキー, 連番, 項目名タイトル１, 項目名タイトル２, データ値 from ADT_患者基本情報データ" + Env.DB_LINK + " where 患者コード in (" + text + ") order by 患者コード, 項目ユニークキー, 連番 desc";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		PatInfo patInfo = new PatInfo();
		while (oleDbDataReader.Read())
		{
			if (patInfo.Id.Length == 0)
			{
				patInfo.Id = oleDbDataReader["患者コード"].ToString();
				patInfo.DictData.Add(oleDbDataReader["項目ユニークキー"].ToString(), new PatInfoData(oleDbDataReader["項目ユニークキー"].ToString(), oleDbDataReader["項目名タイトル１"].ToString(), oleDbDataReader["項目名タイトル２"].ToString(), oleDbDataReader["データ値"].ToString()));
			}
			else if (!patInfo.Id.Equals(oleDbDataReader["患者コード"].ToString()))
			{
				if (!dictionary.ContainsKey(patInfo.Id))
				{
					dictionary.Add(patInfo.Id, patInfo);
				}
				patInfo = new PatInfo();
				patInfo.Id = oleDbDataReader["患者コード"].ToString();
				patInfo.DictData.Add(oleDbDataReader["項目ユニークキー"].ToString(), new PatInfoData(oleDbDataReader["項目ユニークキー"].ToString(), oleDbDataReader["項目名タイトル１"].ToString(), oleDbDataReader["項目名タイトル２"].ToString(), oleDbDataReader["データ値"].ToString()));
			}
			else if (!patInfo.DictData.ContainsKey(oleDbDataReader["項目ユニークキー"].ToString()))
			{
				patInfo.DictData.Add(oleDbDataReader["項目ユニークキー"].ToString(), new PatInfoData(oleDbDataReader["項目ユニークキー"].ToString(), oleDbDataReader["項目名タイトル１"].ToString(), oleDbDataReader["項目名タイトル２"].ToString(), oleDbDataReader["データ値"].ToString()));
			}
		}
		if (patInfo.Id.Length > 0 && !dictionary.ContainsKey(patInfo.Id))
		{
			dictionary.Add(patInfo.Id, patInfo);
		}
		openDBConn.Close();
		return dictionary;
	}
}
