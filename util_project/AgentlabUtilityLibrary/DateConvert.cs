using System;

namespace AgentlabUtilityLibrary;

public class DateConvert
{
	public enum Format
	{
		SHORT = 1,
		LONG
	}

	public static int CalcAge(string birthDay, string toDay)
	{
		if (birthDay.Length == 8 && toDay.Length == 8)
		{
			int num = int.Parse(birthDay.Substring(0, 4));
			int num2 = int.Parse(birthDay.Substring(4, 2));
			int num3 = int.Parse(birthDay.Substring(6, 2));
			int num4 = int.Parse(toDay.Substring(0, 4));
			int num5 = int.Parse(toDay.Substring(4, 2));
			int num6 = int.Parse(toDay.Substring(6, 2));
			if (num2 < num5 || (num2 == num5 && num3 <= num6))
			{
				return num4 - num;
			}
			return num4 - num - 1;
		}
		return -1;
	}

	public static string DateFormat(string date, Format format)
	{
		string result = "";
		if (date.Length == 8)
		{
			switch (format)
			{
			case Format.LONG:
				result = date.Insert(4, "/").Insert(7, "/");
				break;
			case Format.SHORT:
				result = date.Substring(2, 6).Insert(2, "/").Insert(5, "/");
				break;
			}
		}
		return result;
	}

	public static string JConv(string orgDate)
	{
		string text = "";
		if (orgDate.Length == 8)
		{
			int num = int.Parse(orgDate.Substring(0, 4));
			int num2 = int.Parse(orgDate.Substring(4, 2));
			int num3 = int.Parse(orgDate.Substring(6, 2));
			if (num <= 1867)
			{
				text = "江戸時代";
			}
			else
			{
				switch (num)
				{
				case 1868:
					text = "明治元年";
					break;
				case 1869:
				case 1870:
				case 1871:
				case 1872:
				case 1873:
				case 1874:
				case 1875:
				case 1876:
				case 1877:
				case 1878:
				case 1879:
				case 1880:
				case 1881:
				case 1882:
				case 1883:
				case 1884:
				case 1885:
				case 1886:
				case 1887:
				case 1888:
				case 1889:
				case 1890:
				case 1891:
				case 1892:
				case 1893:
				case 1894:
				case 1895:
				case 1896:
				case 1897:
				case 1898:
				case 1899:
				case 1900:
				case 1901:
				case 1902:
				case 1903:
				case 1904:
				case 1905:
				case 1906:
				case 1907:
				case 1908:
				case 1909:
				case 1910:
				case 1911:
					text = "明治" + (num - 1867) + "年";
					break;
				default:
					switch (num)
					{
					case 1912:
						text = ((num2 >= 7 && (num2 != 7 || num3 > 29)) ? "大正元年" : "明治45年");
						break;
					case 1913:
					case 1914:
					case 1915:
					case 1916:
					case 1917:
					case 1918:
					case 1919:
					case 1920:
					case 1921:
					case 1922:
					case 1923:
					case 1924:
					case 1925:
						text = "大正" + (num - 1911) + "年";
						break;
					default:
						switch (num)
						{
						case 1926:
							text = ((num2 >= 12 && (num2 != 12 || num3 > 24)) ? "昭和元年" : "大正15年");
							break;
						case 1927:
						case 1928:
						case 1929:
						case 1930:
						case 1931:
						case 1932:
						case 1933:
						case 1934:
						case 1935:
						case 1936:
						case 1937:
						case 1938:
						case 1939:
						case 1940:
						case 1941:
						case 1942:
						case 1943:
						case 1944:
						case 1945:
						case 1946:
						case 1947:
						case 1948:
						case 1949:
						case 1950:
						case 1951:
						case 1952:
						case 1953:
						case 1954:
						case 1955:
						case 1956:
						case 1957:
						case 1958:
						case 1959:
						case 1960:
						case 1961:
						case 1962:
						case 1963:
						case 1964:
						case 1965:
						case 1966:
						case 1967:
						case 1968:
						case 1969:
						case 1970:
						case 1971:
						case 1972:
						case 1973:
						case 1974:
						case 1975:
						case 1976:
						case 1977:
						case 1978:
						case 1979:
						case 1980:
						case 1981:
						case 1982:
						case 1983:
						case 1984:
						case 1985:
						case 1986:
						case 1987:
						case 1988:
							text = "昭和" + (num - 1925) + "年";
							break;
						default:
							if (num == 1989)
							{
								text = ((num2 != 1 || num3 > 7) ? "平成元年" : "昭和64年");
							}
							else if (num >= 1990)
							{
								text = "平成" + (num - 1988) + "年";
							}
							break;
						}
						break;
					}
					break;
				}
			}
			object obj = text;
			return string.Concat(obj, num2, "月", num3, "日");
		}
		return "";
	}

	public static string JEConv(string orgDate)
	{
		string text = "";
		if (orgDate.Length == 8)
		{
			int num = int.Parse(orgDate.Substring(0, 4));
			int num2 = int.Parse(orgDate.Substring(4, 2));
			int num3 = int.Parse(orgDate.Substring(6, 2));
			if (num <= 1867)
			{
				text = "E";
			}
			else if (num >= 1868 && num <= 1911)
			{
				text = "M" + (num - 1867).ToString().PadLeft(2, '0') + "/";
			}
			else
			{
				switch (num)
				{
				case 1912:
					text = ((num2 >= 7 && (num2 != 7 || num3 > 29)) ? "T01/" : "M45/");
					break;
				case 1913:
				case 1914:
				case 1915:
				case 1916:
				case 1917:
				case 1918:
				case 1919:
				case 1920:
				case 1921:
				case 1922:
				case 1923:
				case 1924:
				case 1925:
					text = "T" + (num - 1911).ToString().PadLeft(2, '0') + "/";
					break;
				default:
					switch (num)
					{
					case 1926:
						text = ((num2 >= 12 && (num2 != 12 || num3 > 24)) ? "S01/" : "T15/");
						break;
					case 1927:
					case 1928:
					case 1929:
					case 1930:
					case 1931:
					case 1932:
					case 1933:
					case 1934:
					case 1935:
					case 1936:
					case 1937:
					case 1938:
					case 1939:
					case 1940:
					case 1941:
					case 1942:
					case 1943:
					case 1944:
					case 1945:
					case 1946:
					case 1947:
					case 1948:
					case 1949:
					case 1950:
					case 1951:
					case 1952:
					case 1953:
					case 1954:
					case 1955:
					case 1956:
					case 1957:
					case 1958:
					case 1959:
					case 1960:
					case 1961:
					case 1962:
					case 1963:
					case 1964:
					case 1965:
					case 1966:
					case 1967:
					case 1968:
					case 1969:
					case 1970:
					case 1971:
					case 1972:
					case 1973:
					case 1974:
					case 1975:
					case 1976:
					case 1977:
					case 1978:
					case 1979:
					case 1980:
					case 1981:
					case 1982:
					case 1983:
					case 1984:
					case 1985:
					case 1986:
					case 1987:
					case 1988:
						text = "S" + (num - 1925).ToString().PadLeft(2, '0') + "/";
						break;
					default:
						if (num == 1989)
						{
							text = ((num2 != 1 || num3 > 7) ? "H01/" : "S64/");
						}
						else if (num >= 1990)
						{
							text = "H" + (num - 1988).ToString().PadLeft(2, '0') + "/";
						}
						break;
					}
					break;
				}
			}
			return text + num2.ToString().PadLeft(2, '0') + "/" + num3.ToString().PadLeft(2, '0');
		}
		return "";
	}

	public static string JWeekday(string orgDate)
	{
		string result = "";
		if (orgDate.Length != 8)
		{
			return result;
		}
		if (!DateTime.TryParse(orgDate.Insert(4, "/").Insert(7, "/"), out var result2))
		{
			return result;
		}
		switch (result2.DayOfWeek)
		{
		case DayOfWeek.Sunday:
			result = "日";
			break;
		case DayOfWeek.Monday:
			result = "月";
			break;
		case DayOfWeek.Tuesday:
			result = "火";
			break;
		case DayOfWeek.Wednesday:
			result = "水";
			break;
		case DayOfWeek.Thursday:
			result = "木";
			break;
		case DayOfWeek.Friday:
			result = "金";
			break;
		case DayOfWeek.Saturday:
			result = "土";
			break;
		}
		return result;
	}

	public static int TimeAdd(int time, int add_minutes)
	{
		int result = 0;
		int.TryParse(new DateTime(2009, 1, 1, time / 100, time % 100, 0).AddMinutes(add_minutes).ToString("HHmm"), out result);
		return result;
	}
}
