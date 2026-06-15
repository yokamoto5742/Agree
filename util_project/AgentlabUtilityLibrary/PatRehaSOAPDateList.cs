using System.Collections.Generic;

namespace AgentlabUtilityLibrary;

public class PatRehaSOAPDateList
{
	public string PtId = "";

	public int RehaDate;

	public List<PatRehaSOAP> List = new List<PatRehaSOAP>();

	public string RehaDateString => DateTimeAgent.DateFormat(RehaDate, DateTimeAgent.DateFormatKind.LONG);

	public string RehaDateStringShort => DateTimeAgent.DateFormat(RehaDate, DateTimeAgent.DateFormatKind.SHORT);

	public void Add(PatRehaSOAP soap)
	{
		List.Add(soap);
	}
}
