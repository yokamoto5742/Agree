using System;
using System.IO;
using System.Text;

namespace Agree;

/// <summary>
/// NuGet 依存のない軽量ファイルロガー。診断・障害解析用にスタックトレースを残す。
/// 個人情報（患者ID・氏名・SQL 全文・入力欄の値）は記録しない方針のため、呼び出し側は
/// context に論理名（例 "printAgree"）のみを渡し、患者データを渡さないこと。
/// ロガー自身は例外を投げない（書込み失敗で本来の処理を巻き込まない）。
/// </summary>
internal static class Logger
{
	private static readonly object writeLock = new object();

	private static string logDir;

	// 出力先: %LOCALAPPDATA%\EyeAgree\logs。常にユーザー書込み可能で、配置先が
	// Program Files やネットワーク共有でも権限エラーにならない。配置運用の都合で
	// AGENT_HOME 配下へ変えたい場合はここ1箇所を変更する。
	private static string ResolveLogDir()
	{
		if (logDir != null)
		{
			return logDir;
		}
		try
		{
			string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EyeAgree", "logs");
			Directory.CreateDirectory(dir);
			logDir = dir;
		}
		catch
		{
			logDir = "";
		}
		return logDir;
	}

	public static void Error(string context, Exception ex)
	{
		string detail = (ex == null)
			? ""
			: ex.GetType().FullName + ": " + ex.Message + Environment.NewLine + ex.StackTrace;
		Write("ERROR", context, detail);
	}

	public static void Info(string context, string message)
	{
		Write("INFO", context, message);
	}

	private static void Write(string level, string context, string detail)
	{
		try
		{
			string dir = ResolveLogDir();
			if (string.IsNullOrEmpty(dir))
			{
				return;
			}
			string file = Path.Combine(dir, "EyeAgree_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
			string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [" + level + "] " + context + ": " + detail + Environment.NewLine;
			lock (writeLock)
			{
				File.AppendAllText(file, line, Encoding.UTF8);
			}
		}
		catch
		{
			// ログ出力は本来の処理を妨げない。失敗しても握り潰す。
		}
	}

	/// <summary>
	/// 指定日数より古いログファイルを削除する。起動時に1回だけ呼ぶ想定。
	/// </summary>
	public static void Purge(int keepDays)
	{
		try
		{
			string dir = ResolveLogDir();
			if (string.IsNullOrEmpty(dir))
			{
				return;
			}
			DateTime threshold = DateTime.Now.AddDays(-keepDays);
			foreach (string file in Directory.GetFiles(dir, "EyeAgree_*.log"))
			{
				try
				{
					if (File.GetLastWriteTime(file) < threshold)
					{
						File.Delete(file);
					}
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
	}
}
