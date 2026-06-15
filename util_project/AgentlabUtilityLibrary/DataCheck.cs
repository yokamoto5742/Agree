using System;

namespace AgentlabUtilityLibrary;

public class DataCheck
{
	public static bool IsValidDate(string checkedDate)
	{
		if (checkedDate.Length != 8)
		{
			return false;
		}
		try
		{
			DateTime.Parse(checkedDate.Insert(4, "/").Insert(7, "/"));
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static bool IsValidId(string checkedId)
	{
		if (checkedId.Length > 9)
		{
			return false;
		}
		try
		{
			int.Parse(checkedId);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}
}
