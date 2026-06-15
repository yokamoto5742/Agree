using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AgentlabUtilityLibrary;

public class MacsProgram1
{
	private enum ShowWindowEnum
	{
		SW_HIDE,
		SW_NORMAL,
		SW_SHOWMINIMIZE,
		SW_MAXIMIZE,
		SW_SHOWNOACTIVATE,
		SW_SHOW,
		SW_MINIMIZE,
		SW_SHOWMINNOACTIVE,
		SW_SHOWNA,
		SW_RESTORE,
		SW_SHOWDEFAULT,
		SW_MAX
	}

	private static string PtId = "";

	public static bool KarteShow(string pt_id)
	{
		bool result = false;
		if (pt_id.Length == 0)
		{
			return result;
		}
		StringBuilder stringBuilder = new StringBuilder(100);
		IntPtr intPtr = WinAPI.GetForegroundWindow();
		PtId = pt_id;
		while (intPtr != IntPtr.Zero)
		{
			if (WinAPI.IsWindowVisible(intPtr))
			{
				WinAPI.GetWindowText(intPtr, stringBuilder, stringBuilder.Capacity);
				if (stringBuilder.ToString().IndexOf("外来受付一覧") != -1 || stringBuilder.ToString().IndexOf("入院情報（患者一覧）") != -1)
				{
					break;
				}
			}
			intPtr = WinAPI.GetWindow(intPtr, 2u);
		}
		WinAPI.EnumChildWindows(intPtr, EnumChildFunc1, intPtr);
		PtId = "";
		return result;
	}

	private static bool EnumChildFunc1(IntPtr hWnd, IntPtr lParam)
	{
		if (PtId.Length == 0)
		{
			return false;
		}
		if (hWnd != IntPtr.Zero)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			WinAPI.GetClassName(hWnd, stringBuilder, stringBuilder.Capacity);
			if (stringBuilder.Length > 0 && WinAPI.IsWindowVisible(hWnd))
			{
				StringBuilder stringBuilder2 = new StringBuilder(256);
				WinAPI.GetWindowText(hWnd, stringBuilder2, stringBuilder2.Capacity);
				if (stringBuilder.ToString() == "ThunderRT6TextBox" && stringBuilder2.ToString() == "123456789")
				{
					if (WinAPI.IsIconic(lParam))
					{
						WinAPI.ShowWindowAsync(lParam, WinAPI.ShowWindowEnum.SW_RESTORE);
						Thread.Sleep(100);
					}
					WinAPI.SetForegroundWindow(hWnd);
					Thread.Sleep(150);
					WinAPI.SetFocus(lParam);
					Thread.Sleep(150);
					SendKeys.SendWait("^A{DEL}" + PtId + "{ENTER}");
					return false;
				}
			}
			return true;
		}
		return false;
	}
}
