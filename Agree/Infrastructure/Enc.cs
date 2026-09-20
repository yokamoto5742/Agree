using System.Collections.Generic;

namespace AgentlabUtilityLibrary;

public static class Enc
{
	private static char[] keys = new char[94]
	{
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
		'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
		'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
		'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
		'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
		'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7',
		'8', '9', '!', '"', '#', '$', '%', '&', '\'', '(',
		')', '-', '+', '*', '/', '\\', '=', '^', '~', '|',
		'`', '@', '[', ']', '{', '}', ';', ':', '_', '?',
		',', '.', '<', '>'
	};

	private static char[] vals = new char[94]
	{
		'?', '.', 'u', ',', '[', '<', '8', 'z', ';', 'y',
		'+', 'J', 'Q', 'K', 'C', 'H', 'T', 'B', 'R', 'F',
		'U', 'S', 'D', 'O', 'Y', 'E', 'I', 'X', 'N', 'L',
		'A', 'W', 'Z', 'V', 'P', 'M', 'G', '%', '1', '|',
		'6', '/', 'c', '"', '{', 'f', '0', 'g', 'h', '*',
		'i', 'x', 'k', 'l', '7', 'n', 'j', '~', 'r', 's',
		'>', '5', 't', 'a', 'v', 'w', '3', '&', '9', '!',
		'p', '#', '$', 'd', '\'', '(', ')', '-', '\\', 'q',
		'=', '^', 'm', '`', '4', '@', 'b', ']', 'o', '}',
		'e', '2', ':', '_'
	};

	private static readonly Dictionary<char, char> decDict = CharMap.Create(vals, keys);

	public static string Decrypt(string s)
	{
		return CharMap.Convert(s, decDict);
	}
}
