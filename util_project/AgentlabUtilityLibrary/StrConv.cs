namespace AgentlabUtilityLibrary;

public class StrConv
{
	private static char[] zen = new char[94]
	{
		'０', '１', '２', '３', '４', '５', '６', '７', '８', '９',
		'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', 'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ',
		'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ', 'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ',
		'Ｕ', 'Ｖ', 'Ｗ', 'Ｘ', 'Ｙ', 'Ｚ', 'ａ', 'ｂ', 'ｃ', 'ｄ',
		'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ', 'ｊ', 'ｋ', 'ｌ', 'ｍ', 'ｎ',
		'ｏ', 'ｐ', 'ｑ', 'ｒ', 'ｓ', 'ｔ', 'ｕ', 'ｖ', 'ｗ', 'ｘ',
		'ｙ', 'ｚ', '！', '”', '＃', '＄', '％', '＆', '’', '（',
		'）', '＝', '－', '＋', '＊', '\uff3e', '～', '￥', '｜', '「',
		'」', '｛', '｝', '＠', '‘', '；', '：', '、', '．', '＜',
		'＞', '？', '／', '\uff3f'
	};

	private static char[] han = new char[94]
	{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
		'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
		'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',
		'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
		'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
		'y', 'z', '!', '"', '#', '$', '%', '&', '\'', '(',
		')', '=', '-', '+', '*', '^', '~', '\\', '|', '[',
		']', '{', '}', '@', '`', ';', ':', ',', '.', '<',
		'>', '?', '/', '_'
	};

	public static string ZenToHan(string s)
	{
		string text = "";
		for (int i = 0; i < s.Length; i++)
		{
			for (int j = 0; j < zen.Length; j++)
			{
				if (s[i].Equals(zen[j]))
				{
					text += han[j];
					break;
				}
				if (j == zen.Length - 1)
				{
					text += s[i];
				}
			}
		}
		return text;
	}

	public static string HanToZen(string s)
	{
		string text = "";
		for (int i = 0; i < s.Length; i++)
		{
			for (int j = 0; j < han.Length; j++)
			{
				if (s[i].Equals(han[j]))
				{
					text += zen[j];
					break;
				}
				if (j == han.Length - 1)
				{
					text += s[i];
				}
			}
		}
		return text;
	}
}
