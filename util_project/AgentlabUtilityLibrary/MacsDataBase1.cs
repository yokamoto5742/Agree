namespace AgentlabUtilityLibrary;

public class MacsDataBase1
{
	public string InOut = "";

	public string Dept = "";

	public string Staff = "";

	public string InOutString
	{
		get
		{
			string result = "";
			if (InOut.Equals("1"))
			{
				result = "外来";
			}
			else if (InOut.Equals("2"))
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
			if (InOut.Equals("1"))
			{
				result = "外";
			}
			else if (InOut.Equals("2"))
			{
				result = "入";
			}
			return result;
		}
	}

	public string DeptName
	{
		get
		{
			string result = "";
			if (Dict.DeptDict.ContainsKey(Dept))
			{
				result = Dict.DeptDict[Dept].FullName;
			}
			return result;
		}
	}

	public string DeptShort
	{
		get
		{
			string result = "";
			if (Dict.DeptDict.ContainsKey(Dept))
			{
				result = Dict.DeptDict[Dept].ShortName;
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
}
