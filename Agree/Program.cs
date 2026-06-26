using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Agree;

internal static class Program
{
	public static bool OfflineMode;

	[STAThread]
	private static void Main()
	{
		Logger.Purge(30);
		// UI スレッドの未処理例外を WinForms に捕捉させ、標準クラッシュダイアログの代わりに
		// ログ記録＋日本語メッセージで継続する。フォーム生成前に設定する必要がある。
		Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
		Application.ThreadException += (sender, e) =>
		{
			Logger.Error("UI", e.Exception);
			MessageBox.Show("予期しないエラーが発生しました。操作をやり直してください。\n問題が続く場合は管理者に連絡してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
		};
		AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
		{
			Logger.Error("Fatal", e.ExceptionObject as Exception);
		};
		Directory.SetCurrentDirectory(Application.StartupPath);
		Process currentProcess = Process.GetCurrentProcess();
		Process[] processesByName = Process.GetProcessesByName(currentProcess.ProcessName);
		if (processesByName.Length > 1)
		{
			if (DialogResult.OK != MessageBox.Show("すでに起動している同意書システムを終了してよろしいですか？", "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
			{
				return;
			}
			Process[] array = processesByName;
			foreach (Process process in array)
			{
				if (process.Id != currentProcess.Id)
				{
					try
					{
						process.Kill();
						process.WaitForExit(3000);
					}
					catch (Exception ex)
					{
						Logger.Error("KillPrev", ex);
					}
				}
			}
		}
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Application.Run(new Form1());
	}
}
