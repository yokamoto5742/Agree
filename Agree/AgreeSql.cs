using System;

namespace Agree;

/// <summary>
/// DB / CSV 入出力で使う純粋な文字列変換ヘルパー。
/// WinForms・COM・外部DLL に依存しないため、テストプロジェクトへソースリンクして単体テストできる。
/// （Form1 の private 実装をそのまま static 化して切り出したもの。挙動は不変。）
/// </summary>
internal static class AgreeSql
{
	/// <summary>
	/// SQL リテラル化。null / 空文字は NULL、それ以外はシングルクォートで囲み、
	/// 値中の ' を '' にエスケープする（SQL 文字列リテラルの破壊・注入を防ぐ）。
	/// </summary>
	public static string SqlValue(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return "NULL";
		}
		return "'" + value.Replace("'", "''") + "'";
	}

	/// <summary>
	/// CSV 1 フィールドのエスケープ。区切り( , )・引用符( " )・改行を含む場合のみ
	/// " で囲み、値中の " を "" に二重化する。null は空文字として扱う。
	/// </summary>
	public static string CsvEscape(string value)
	{
		if (value == null)
		{
			value = "";
		}
		if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
		{
			value = "\"" + value.Replace("\"", "\"\"") + "\"";
		}
		return value;
	}
}
