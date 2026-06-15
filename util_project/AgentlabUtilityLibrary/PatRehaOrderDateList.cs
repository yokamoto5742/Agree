using System.Collections.Generic;

namespace AgentlabUtilityLibrary;

public class PatRehaOrderDateList
{
	public string PtId = "";

	public int RehaDate;

	public List<PatRehaOrder> List = new List<PatRehaOrder>();

	public string RehaDateString => DateTimeAgent.DateFormat(RehaDate, DateTimeAgent.DateFormatKind.LONG);

	public string RehaDateStringShort => DateTimeAgent.DateFormat(RehaDate, DateTimeAgent.DateFormatKind.SHORT);

	public void Add(PatRehaOrder order)
	{
		List.Add(order);
	}
}
