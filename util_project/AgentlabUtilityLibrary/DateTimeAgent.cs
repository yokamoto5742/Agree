using System;
using System.Globalization;

namespace AgentlabUtilityLibrary;

public class DateTimeAgent
{
	public enum DateFormatKind
	{
		LONG = 1,
		SHORT,
		WLONG,
		WSHORT,
		J1,
		J2,
		JW1,
		JW2
	}

	public static CultureInfo DefaultCulture
	{
		get
		{
			CultureInfo cultureInfo = new CultureInfo("ja-JP", useUserOverride: true);
			cultureInfo.DateTimeFormat.Calendar = new JapaneseCalendar();
			return cultureInfo;
		}
	}

	public static string DateFormat(string org_date, DateFormatKind kind)
	{
		string result = "";
		if (org_date.Length == 8)
		{
			switch (kind)
			{
			case DateFormatKind.LONG:
				result = org_date.Insert(4, "/").Insert(7, "/");
				break;
			case DateFormatKind.SHORT:
				result = org_date.Substring(2).Insert(2, "/").Insert(5, "/");
				break;
			case DateFormatKind.WLONG:
				result = DateTime.Parse(org_date.Insert(4, "/").Insert(7, "/")).ToString("yyyy/MM/dd(ddd)");
				break;
			case DateFormatKind.WSHORT:
				result = DateTime.Parse(org_date.Insert(4, "/").Insert(7, "/")).ToString("yy/MM/dd(ddd)");
				break;
			case DateFormatKind.J1:
				result = DateTime.Parse(org_date.Insert(4, "/").Insert(7, "/")).ToString("ggyy年M月d日", DefaultCulture);
				break;
			case DateFormatKind.J2:
				result = DateTime.Parse(org_date.Insert(4, "/").Insert(7, "/")).ToString("gyy/M/d", DefaultCulture);
				break;
			case DateFormatKind.JW1:
				result = DateTime.Parse(org_date.Insert(4, "/").Insert(7, "/")).ToString("ggyy年M月d日(ddd)", DefaultCulture);
				break;
			case DateFormatKind.JW2:
				result = DateTime.Parse(org_date.Insert(4, "/").Insert(7, "/")).ToString("gyy/M/d(ddd)", DefaultCulture);
				break;
			}
		}
		return result;
	}

	public static string DateFormat(int org_date, DateFormatKind kind)
	{
		string result = "";
		if (org_date.ToString().Length == 8)
		{
			result = DateFormat(org_date.ToString(), kind);
		}
		return result;
	}

	public static int DateToInt(string org_date)
	{
		int result = 0;
		DateTime result2 = default(DateTime);
		if (DateTime.TryParse(org_date, out result2))
		{
			result = result2.Year * 10000 + result2.Month * 100 + result2.Day;
		}
		return result;
	}

	public static DateTime DateTimeFromInt(int org_date)
	{
		DateTime result = DateTime.Now;
		if (org_date.ToString().Length == 8)
		{
			DateTime.TryParse(org_date.ToString().Insert(4, "/").Insert(7, "/"), out result);
		}
		return result;
	}

	public static int AddDays(int crit_date, int days)
	{
		return int.Parse(DateTime.Parse(crit_date.ToString().Insert(4, "/").Insert(7, "/")).AddDays(days).ToString("yyyyMMdd"));
	}

	public static int AddMonths(int crit_date, int months)
	{
		return int.Parse(DateTime.Parse(crit_date.ToString().Insert(4, "/").Insert(7, "/")).AddMonths(months).ToString("yyyyMMdd"));
	}

	public static int Days(DateTime crit_date)
	{
		return int.Parse(DateTime.Parse(crit_date.AddMonths(1).ToString("yyyy/MM/01")).AddDays(-1.0).ToString("dd")
			.TrimStart('0'));
	}

	public static DateTime LastDateOfMonth(DateTime crit_date)
	{
		return DateTime.Parse(crit_date.ToString("yyyy/MM/01")).AddMonths(1).AddDays(-1.0);
	}

	public static int LastDateOfMonth(int crit_date)
	{
		int result = 0;
		DateTime result2 = DateTime.Now;
		if (crit_date.ToString().Length == 8 && DateTime.TryParse(crit_date.ToString().Insert(4, "/").Insert(7, "/"), out result2))
		{
			result = int.Parse(DateTime.Parse(result2.ToString("yyyy/MM/01")).AddMonths(1).AddDays(-1.0)
				.ToString("yyyyMMdd"));
		}
		return result;
	}

	public static DateTime LastDateOfPrevMonth(DateTime crit_date)
	{
		return DateTime.Parse(crit_date.ToString("yyyy/MM/01")).AddDays(-1.0);
	}

	public static int LastDateOfPrevMonth(int crit_date)
	{
		int result = 0;
		DateTime result2 = DateTime.Now;
		if (crit_date.ToString().Length == 8 && DateTime.TryParse(crit_date.ToString().Insert(4, "/").Insert(7, "/"), out result2))
		{
			result = int.Parse(DateTime.Parse(result2.ToString("yyyy/MM/01")).AddDays(-1.0).ToString("yyyyMMdd"));
		}
		return result;
	}

	public static int IntervalDays(int date1, int date2)
	{
		DateTime dateTime = DateTime.Parse(date1.ToString().Insert(4, "/").Insert(7, "/"));
		DateTime value = DateTime.Parse(date2.ToString().Insert(4, "/").Insert(7, "/"));
		return dateTime.Subtract(value).Days;
	}

	public static string TimeFormat(string org_time)
	{
		string result = "";
		if (org_time.Length <= 4)
		{
			result = org_time.PadLeft(4, '0').Insert(2, ":");
		}
		return result;
	}

	public static string TimeFormat(int org_time)
	{
		string result = "";
		DateTime result2 = default(DateTime);
		if (org_time > 0 && org_time.ToString().Length <= 4 && DateTime.TryParse(org_time.ToString().PadLeft(4, '0').Insert(2, ":"), out result2))
		{
			result = result2.ToString("HH:mm");
		}
		return result;
	}

	public static int AddTime(int crit_time, int minutes)
	{
		int num = crit_time / 100 + minutes / 60;
		int num2 = crit_time % 100 + minutes % 60;
		if (num2 >= 60)
		{
			num++;
			num2 -= 60;
		}
		return num * 100 + num2;
	}

	public static int IntervalMinutes(int start_time, int end_time)
	{
		int num = end_time / 100 - start_time / 100;
		int num2 = end_time % 100 - start_time % 100;
		if (num2 < 0)
		{
			num--;
			num2 += 60;
		}
		return num * 60 + num2;
	}

	public static int TimeToInt(string org_time)
	{
		int result = 0;
		DateTime result2 = default(DateTime);
		if (DateTime.TryParse(org_time, out result2))
		{
			result = result2.Hour * 100 + result2.Minute;
		}
		return result;
	}

	public static int AgeCalc(int birth, int today)
	{
		int result = -1;
		if (birth.ToString().Length == 8 && today.ToString().Length == 8)
		{
			int num = birth / 10000;
			int num2 = birth % 10000 / 100;
			int num3 = birth % 100;
			int num4 = today / 10000;
			int num5 = today % 10000 / 100;
			int num6 = today % 100;
			result = ((num2 >= num5 && (num2 != num5 || num3 > num6)) ? (num4 - num - 1) : (num4 - num));
		}
		return result;
	}

	public static int AgeCalc(string birth, string today)
	{
		int result = 0;
		int result2 = 0;
		int.TryParse(birth, out result);
		int.TryParse(today, out result2);
		return AgeCalc(result, result2);
	}
}
