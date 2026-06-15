using System.Collections.Generic;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class OpeOrderMaster
{
	public int Code1;

	public int Code2;

	public string Title = "";

	public int Kind = 1;

	public string Text1 = "";

	public string Text2 = "";

	public int Limit;

	private static Dictionary<string, OpeOrderMaster> dict = new Dictionary<string, OpeOrderMaster>();

	public static Dictionary<string, OpeOrderMaster> Dict
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
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "Select * from PATH手術指示マスター" + Env.DB_LINK + " order by 指示コード, 連番";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		string text = "";
		while (oleDbDataReader.Read())
		{
			text = oleDbDataReader["指示コード"].ToString() + "-" + oleDbDataReader["連番"].ToString();
			OpeOrderMaster opeOrderMaster = new OpeOrderMaster();
			int.TryParse(oleDbDataReader["指示コード"].ToString(), out opeOrderMaster.Code1);
			int.TryParse(oleDbDataReader["連番"].ToString(), out opeOrderMaster.Code2);
			opeOrderMaster.Title = oleDbDataReader["タイトル"].ToString().Trim();
			int.TryParse(oleDbDataReader["属性"].ToString(), out opeOrderMaster.Kind);
			opeOrderMaster.Text1 = oleDbDataReader["選択肢名称"].ToString();
			opeOrderMaster.Text2 = oleDbDataReader["選択肢名称２"].ToString();
			int.TryParse(oleDbDataReader["テキスト有無"].ToString(), out opeOrderMaster.Limit);
			dict.Add(text, opeOrderMaster);
		}
		openDBConn.Close();
	}
}
