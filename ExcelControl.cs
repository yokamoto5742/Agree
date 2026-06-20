using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using AgentlabUtilityLibrary;
using Microsoft.Office.Interop.Excel;

internal class ExcelControl
{
	private Dictionary<string, string> valueList = new Dictionary<string, string>();

	private Application exApp;

	private _Workbook exWorkbook;

	private _Worksheet exWorksheet;

	public Dictionary<string, string> ValueList
	{
		set
		{
			valueList = value;
		}
	}

	public void ReleaseExcel()
	{
		Marshal.ReleaseComObject(exWorksheet);
		Marshal.ReleaseComObject(exWorkbook);
		Marshal.ReleaseComObject(exApp);
	}

	public string Open(string fileName, string sheetName)
	{
		string result = "";
		if (!File.Exists(fileName))
		{
			return "ファイルが存在しません";
		}
		try
		{
			exApp = (Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
			exApp.Visible = true;
			exWorkbook = exApp.Workbooks.Open(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
			exWorksheet = (_Worksheet)(dynamic)exWorkbook.Sheets[sheetName];
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			result = message;
		}
		return result;
	}

	private void setValue(Dictionary<string, string> valueToCell)
	{
		foreach (string key in valueToCell.Keys)
		{
			int num = int.Parse(key.Split(',')[0]);
			int num2 = int.Parse(key.Split(',')[1]);
			exWorksheet.Cells[num, num2] = valueToCell[key];
		}
	}

	public void MakeEyeAgree(string sheetName)
	{
		Open(Env.AGENT_HOME + "\\Agree_眼科同意書.xlsm", "共通情報");
		setValue(valueList);
		exWorksheet.Cells[8, 2] = DateTime.Now.ToString("yyyyMMdd");
		exWorksheet.Cells[9, 2] = DateTime.Now.ToString("HHmmss");
		Range range = (Range)(dynamic)exWorksheet.Cells[3, 2];
		Range range2 = (Range)(dynamic)exWorksheet.Cells[7, 2];
		Range range3 = (Range)(dynamic)exWorksheet.Cells[4, 2];
		Range range4 = (Range)(dynamic)exWorksheet.Cells[22, 2];
		Range range5 = (Range)(dynamic)exWorksheet.Cells[5, 2];
		Range range6 = (Range)(dynamic)exWorksheet.Cells[8, 2];
		Range range7 = (Range)(dynamic)exWorksheet.Cells[9, 2];
		exWorksheet.Cells[11, 2] = (object)(((dynamic)range.Value2).ToString().PadLeft(9, '0') + ((dynamic)range3.Value2).ToString() + ((dynamic)range5.Value2).ToString().PadLeft(3, '0') + ((dynamic)range2.Value2).ToString().PadLeft(5, '0') + ((dynamic)range6.Value2).ToString() + ((dynamic)range7.Value2).ToString());
		exWorksheet.Cells[23, 2] = (object)(((dynamic)range.Value2).ToString().PadLeft(9, '0') + ((dynamic)range4.Value2).ToString() + ((dynamic)range5.Value2).ToString().PadLeft(3, '0') + ((dynamic)range2.Value2).ToString().PadLeft(5, '0') + ((dynamic)range6.Value2).ToString() + ((dynamic)range7.Value2).ToString());
		string filename = Environment.GetEnvironmentVariable("TEMP") + "\\" + ((dynamic)range.Value2).ToString() + "_" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmmss") + "_" + "眼科同意書";
		exWorkbook.SaveAs(filename, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, XlSaveAsAccessMode.xlExclusive, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
		exWorksheet = (_Worksheet)(dynamic)exWorkbook.Sheets[sheetName];
		exWorksheet.Select(true);
		Marshal.ReleaseComObject(range);
		Marshal.ReleaseComObject(range2);
		Marshal.ReleaseComObject(range3);
		Marshal.ReleaseComObject(range4);
		Marshal.ReleaseComObject(range5);
		Marshal.ReleaseComObject(range6);
		Marshal.ReleaseComObject(range7);
	}
}
