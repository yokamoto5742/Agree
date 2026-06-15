using System.Collections.Generic;
using System.Data.Common;

namespace AgentlabUtilityLibrary;

public class PatRehaOrder
{
	public string PtId = "";

	public string OrderCode = "";

	public int InOut;

	public string Staff = "";

	public int RehaKind;

	public int RehaStart = -1;

	public int RehaEnd = -1;

	public string InOutString
	{
		get
		{
			string result = "";
			if (InOut.Equals(1))
			{
				result = "外来";
			}
			else if (InOut.Equals(2))
			{
				result = "入院";
			}
			return result;
		}
	}

	public string InOutStringShort
	{
		get
		{
			string result = "";
			if (InOut.Equals(1))
			{
				result = "外";
			}
			else if (InOut.Equals(2))
			{
				result = "入";
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

	public string RehaKindString
	{
		get
		{
			string result = "";
			if (RehaKind.Equals(0))
			{
				result = "消鎮";
			}
			else if (RehaKind.Equals(4))
			{
				result = "言語";
			}
			else if (RehaKind.Equals(5))
			{
				result = "作業";
			}
			else if (RehaKind.Equals(6))
			{
				result = "理学";
			}
			return result;
		}
	}

	public string RehaKindStringShort
	{
		get
		{
			string result = "";
			if (RehaKind.Equals(0))
			{
				result = "消";
			}
			else if (RehaKind.Equals(4))
			{
				result = "言";
			}
			else if (RehaKind.Equals(5))
			{
				result = "作";
			}
			else if (RehaKind.Equals(6))
			{
				result = "理";
			}
			return result;
		}
	}

	public string RehaStartTime => DateTimeAgent.TimeFormat(RehaStart);

	public string RehaEndTime => DateTimeAgent.TimeFormat(RehaEnd);

	public static Dictionary<string, PatRehaOrderDateList> GetDictByDateAndStaff(string date, string staff_list_str)
	{
		Dictionary<string, PatRehaOrderDateList> dictionary = new Dictionary<string, PatRehaOrderDateList>();
		DB oPEN = DB.OPEN;
		oPEN.Open();
		oPEN.CommandText = "select ＮＴオーダーヘッダー.オーダー番号, ＮＴオーダーヘッダー.患者コード, ＮＴオーダーヘッダー.入外区分, ＮＴオーダーヘッダー.施行者コード, 実施行為データ.予備区分１,  max(decode(実施行為データ.行為状態区分, 1, to_char(実施行為データ.日時, 'HH24:MI'), '0')) as 開始,  max(decode(実施行為データ.行為状態区分, 2, to_char(実施行為データ.日時, 'HH24:MI'), '0')) as 終了  from ＮＴオーダーヘッダー" + Env.DB_LINK + " inner join 実施行為データ" + Env.DB_LINK + " on ＮＴオーダーヘッダー.施行予定日 = " + date + " and ＮＴオーダーヘッダー.診療区分 in (40, 80) and ＮＴオーダーヘッダー.施行者コード in (" + staff_list_str + ") and 実施行為データ.施行予定日 = " + date + " and 実施行為データ.診療区分 in (40, 80) and ＮＴオーダーヘッダー.オーダー番号 = 実施行為データ.オーダー番号  group by ＮＴオーダーヘッダー.オーダー番号, ＮＴオーダーヘッダー.患者コード, ＮＴオーダーヘッダー.入外区分, ＮＴオーダーヘッダー.施行者コード, 実施行為データ.予備区分１ order by 患者コード, 開始";
		DbDataReader dbDataReader = oPEN.ExecuteReader();
		string text = "";
		PatRehaOrderDateList patRehaOrderDateList = new PatRehaOrderDateList();
		while (dbDataReader.Read())
		{
			if (text.Length == 0 || !text.Equals(dbDataReader["患者コード"].ToString()))
			{
				if (text.Length > 0 && !dictionary.ContainsKey(text))
				{
					dictionary.Add(text, patRehaOrderDateList);
				}
				text = dbDataReader["患者コード"].ToString();
				patRehaOrderDateList = new PatRehaOrderDateList();
				patRehaOrderDateList.PtId = text;
				int.TryParse(date, out patRehaOrderDateList.RehaDate);
			}
			PatRehaOrder patRehaOrder = new PatRehaOrder();
			patRehaOrder.PtId = dbDataReader["患者コード"].ToString();
			patRehaOrder.OrderCode = dbDataReader["オーダー番号"].ToString();
			int.TryParse(dbDataReader["入外区分"].ToString(), out patRehaOrder.InOut);
			patRehaOrder.Staff = dbDataReader["施行者コード"].ToString();
			int.TryParse(dbDataReader["予備区分１"].ToString(), out patRehaOrder.RehaKind);
			int.TryParse(dbDataReader["開始"].ToString(), out patRehaOrder.RehaStart);
			int.TryParse(dbDataReader["終了"].ToString(), out patRehaOrder.RehaEnd);
			patRehaOrderDateList.Add(patRehaOrder);
		}
		if (text.Length > 0 && !dictionary.ContainsKey(text))
		{
			dictionary.Add(text, patRehaOrderDateList);
		}
		dbDataReader.Close();
		oPEN.Close();
		return dictionary;
	}

	public static Dictionary<string, PatRehaOrderDateList> GetDictByDateAndStaff(string date, List<string> staff_list)
	{
		string text = "";
		foreach (string item in staff_list)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += item;
		}
		return GetDictByDateAndStaff(date, text);
	}
}
