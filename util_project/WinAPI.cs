using System;
using System.Runtime.InteropServices;
using System.Text;

public static class WinAPI
{
	public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr lParam);

	public enum ShowWindowEnum
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

	public struct COPYDATASTRUCT
	{
		public IntPtr dwData;

		public IntPtr cbData;

		public string lpData;
	}

	public struct SHFILEINFO
	{
		public IntPtr hIcon;

		public IntPtr iIcon;

		public uint dwAttributes;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szDisplayName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string szTypeName;
	}

	public const int VK_CAPITAL = 20;

	public const int VK_KANA = 21;

	public const int VK_INSERT = 45;

	public const int VK_NUMLOCK = 144;

	public const int VK_SCROLL = 145;

	public const uint SHGFI_ICON = 256u;

	public const uint SHGFI_LARGEICON = 0u;

	public const uint SHGFI_SMALLICON = 1u;

	[DllImport("user32.dll")]
	public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowProc lpEnumFunc, IntPtr lParam);

	[DllImport("user32.dll")]
	public static extern int EnumWindows(EnumWindowProc lpEnumFunc, IntPtr lParam);

	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	public static extern int GetClassName(IntPtr hWnd, [Out] StringBuilder lpClassName, int nMaxCount);

	[DllImport("user32.dll")]
	public static extern int GetDesktopWindow();

	[DllImport("user32.dll")]
	public static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	public static extern int GetParent(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

	[DllImport("user32.dll")]
	public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpStr, int nMaxCount);

	[DllImport("user32.dll")]
	public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, int bRepaint);

	[DllImport("user32.dll")]
	public static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern bool IsIconic(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern bool SetFocus(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern bool SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	public static extern int SetWindowText(IntPtr hWnd, string strText);

	[DllImport("user32.dll")]
	public static extern bool ShowWindowAsync(IntPtr hWnd, ShowWindowEnum nCmdShow);

	[DllImport("user32.dll")]
	public static extern IntPtr SendMessage(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll")]
	public static extern IntPtr SendMessage(IntPtr hWnd, IntPtr Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

	[DllImport("user32.dll")]
	public static extern int GetKeyState(int nVirtKey);

	[DllImport("kernel32.dll")]
	public static extern bool Beep(uint dwFreq, uint dwDuration);

	[DllImport("comctl32.dll")]
	public static extern bool InitCommonControls();

	[DllImport("comctl32.dll", CharSet = CharSet.Auto)]
	public static extern bool ImageList_BeginDrag(IntPtr himlTrack, int iTrack, int dxHotspot, int dyHotspot);

	[DllImport("comctl32.dll", CharSet = CharSet.Auto)]
	public static extern bool ImageList_DragMove(int x, int y);

	[DllImport("comctl32.dll", CharSet = CharSet.Auto)]
	public static extern void ImageList_EndDrag();

	[DllImport("comctl32.dll", CharSet = CharSet.Auto)]
	public static extern bool ImageList_DragEnter(IntPtr hwndLock, int x, int y);

	[DllImport("comctl32.dll", CharSet = CharSet.Auto)]
	public static extern bool ImageList_DragLeave(IntPtr hwndLock);

	[DllImport("shell32.dll")]
	public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
}
