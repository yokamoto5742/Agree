using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AgentlabUtilityLibrary;

public class TableData
{
	public enum TitlePrint
	{
		No,
		Yes
	}

	public enum FileDialogShow
	{
		No,
		Yes
	}

	public List<string> Title = new List<string>();

	public List<TableDataRecord> RecordList = new List<TableDataRecord>();

	public TableData()
	{
	}

	public TableData(DataGridView view)
	{
		foreach (DataGridViewColumn column in view.Columns)
		{
			Title.Add(column.HeaderText);
		}
		foreach (DataGridViewRow item in (IEnumerable)view.Rows)
		{
			TableDataRecord tableDataRecord = new TableDataRecord();
			foreach (DataGridViewCell cell in item.Cells)
			{
				tableDataRecord.DataList.Add(cell.Value.ToString());
			}
			RecordList.Add(tableDataRecord);
		}
	}

	public bool CSVSave(string file, bool append, TitlePrint title_print, FileDialogShow dialog_show)
	{
		string path = file;
		if (dialog_show == FileDialogShow.Yes)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			if (file.Length > 0)
			{
				saveFileDialog.FileName = file;
			}
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return false;
			}
			path = saveFileDialog.FileName;
		}
		try
		{
			StreamWriter streamWriter = new StreamWriter(path, append, Encoding.GetEncoding("shift-jis"));
			if (title_print == TitlePrint.Yes)
			{
				foreach (string item in Title)
				{
					streamWriter.Write("\"" + item.Replace("\"", "\"\"") + "\",");
				}
				streamWriter.WriteLine();
			}
			foreach (TableDataRecord record in RecordList)
			{
				foreach (string data in record.DataList)
				{
					streamWriter.Write("\"" + data.Replace("\"", "\"\"") + "\",");
				}
				streamWriter.WriteLine();
			}
			streamWriter.Close();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
