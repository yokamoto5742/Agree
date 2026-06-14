using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Agree;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
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
					process.CloseMainWindow();
				}
			}
		}
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Application.Run(new Form1());
	}
}
