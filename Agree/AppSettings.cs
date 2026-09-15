using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Agree;

/// <summary>
/// 専用設定ファイル EyeAgreeSettings.ini（key=value 形式）を起動後に1回だけ読み込む。
/// ファイルが無い・読めない場合は空として扱い、各項目は呼び出し側の既定値になる。
/// WinForms・COM・外部DLL に依存しないため、テストプロジェクトへソースリンクして単体テストできる。
/// </summary>
internal static class AppSettings
{
	private static readonly Dictionary<string, string> values = load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EyeAgreeSettings.ini"));

	public static string Get(string key, string defaultValue)
	{
		return values.TryGetValue(key, out string value) ? value : defaultValue;
	}

	public static int GetInt(string key, int defaultValue)
	{
		return int.TryParse(Get(key, null), NumberStyles.Integer, CultureInfo.InvariantCulture, out int n) ? n : defaultValue;
	}

	public static float GetFloat(string key, float defaultValue)
	{
		return float.TryParse(Get(key, null), NumberStyles.Float, CultureInfo.InvariantCulture, out float f) ? f : defaultValue;
	}

	private static Dictionary<string, string> load(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				return Parse(File.ReadAllLines(path, Encoding.Default));
			}
		}
		catch (Exception)
		{
			// フォーム生成前（Application.Run より前）に失敗すると起動できなくなるため、既定値で続行する。
		}
		return new Dictionary<string, string>();
	}

	/// <summary>
	/// key=value 行を辞書にする。空行・; で始まるコメント・[セクション] など = が1つでない行は無視する。
	/// キーと値は前後の空白を除く。同じキーが複数ある場合は後の行を採用する。
	/// </summary>
	internal static Dictionary<string, string> Parse(IEnumerable<string> lines)
	{
		Dictionary<string, string> result = new Dictionary<string, string>();
		foreach (string line in lines)
		{
			if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
			{
				continue;
			}
			string[] parts = line.Split('=');
			if (parts.Length != 2)
			{
				continue;
			}
			result[parts[0].Trim()] = parts[1].Trim();
		}
		return result;
	}
}
