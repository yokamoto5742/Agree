using System.Collections.Generic;
using System.Text;

namespace AgentlabUtilityLibrary;

/// <summary>並行する2つの char 配列による1文字単位の変換表(Enc で使う)。</summary>
internal static class CharMap
{
	public static Dictionary<char, char> Create(char[] from, char[] to)
	{
		Dictionary<char, char> map = new Dictionary<char, char>();
		for (int i = 0; i < from.Length; i++)
		{
			map.Add(from[i], to[i]);
		}
		return map;
	}

	/// <summary>変換表にない文字はそのまま残す。</summary>
	public static string Convert(string s, Dictionary<char, char> map)
	{
		StringBuilder builder = new StringBuilder(s.Length);
		foreach (char c in s)
		{
			builder.Append(map.TryGetValue(c, out char converted) ? converted : c);
		}
		return builder.ToString();
	}
}
