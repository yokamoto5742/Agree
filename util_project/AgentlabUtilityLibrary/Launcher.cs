using System.Diagnostics;

namespace AgentlabUtilityLibrary;

public class Launcher
{
	public enum DicomKind
	{
		PRO = 1,
		EV
	}

	public static void Dicom(string pt_id, DicomKind kind)
	{
		switch (kind)
		{
		case DicomKind.PRO:
			Process.Start("explorer.exe", "\"http://dicomweb/EXtViewer_pro/cgi/login.cgi?USER=test&PASSWORD=test&ID=" + pt_id + "&DATE=\"\"\"\"\"");
			break;
		case DicomKind.EV:
			Process.Start("explorer.exe", "\"http://dicomweb/EVService/EVService.dll?clientCall&USER=shinseikai&ID=" + pt_id);
			break;
		}
	}

	public static void Dicom(string pt_id)
	{
		Dicom(pt_id, DicomKind.EV);
	}
}
