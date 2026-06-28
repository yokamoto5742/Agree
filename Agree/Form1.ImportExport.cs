using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AgentlabUtilityLibrary;
using Microsoft.VisualBasic.FileIO;

namespace Agree;

public partial class Form1
{
	private void exportButton_Click(object sender, EventArgs e)
	{
		exportButton.Visible = false;
		importButton.Visible = false;
		if (Program.OfflineMode)
		{
			MessageBox.Show("オフラインモードのため、データを出力できません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		using (FolderBrowserDialog fbd = new FolderBrowserDialog())
		{
			fbd.Description = "バックアップCSVを保存するフォルダを選択してください";
			if (fbd.ShowDialog() == DialogResult.OK)
			{
				try
				{
					string folder = fbd.SelectedPath;
					ExportTableToCsv("AGREE", Path.Combine(folder, "AGREE.csv"));
					ExportTableToCsv("AGREE_TEMPLATE", Path.Combine(folder, "AGREE_TEMPLATE.csv"));
					ExportTableToCsv("AGREE_STAFF", Path.Combine(folder, "AGREE_STAFF.csv"));
					MessageBox.Show("3つのテーブルの出力が完了しました。\n保存先: " + folder, "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show("データ出力中にエラーが発生しました:\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}

	private void importButton_Click(object sender, EventArgs e)
	{
		exportButton.Visible = false;
		importButton.Visible = false;
		if (Program.OfflineMode)
		{
			MessageBox.Show("オフラインモードのため、データを取り込めません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		if (MessageBox.Show("CSVデータを取り込みますか？\n同じIDのデータは上書き（マージ）されます。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		using (FolderBrowserDialog fbd = new FolderBrowserDialog())
		{
			fbd.Description = "取り込むCSVファイルがあるフォルダを選択してください";
			if (fbd.ShowDialog() == DialogResult.OK)
			{
				try
				{
					string folder = fbd.SelectedPath;
					StringBuilder report = new StringBuilder();
					ImportTable(folder, "AGREE.csv", "AGREE", "AGREE_ID", "AGREE_SEQ", report);
					ImportTable(folder, "AGREE_TEMPLATE.csv", "AGREE_TEMPLATE", "TEMP_ID", "AGREE_TEMPLATE_SEQ", report);
					ImportTable(folder, "AGREE_STAFF.csv", "AGREE_STAFF", "ID", "AGREE_STAFF_SEQ", report);
					MessageBox.Show("データの取り込みが完了しました。\n" + report.ToString(), "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
					showList();
				}
				catch (Exception ex)
				{
					MessageBox.Show("データ取り込み中にエラーが発生しました:\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}

	private void ExportTableToCsv(string tableName, string filePath)
	{
		oraConn.Open();
		try
		{
			using (OleDbCommand cmd = new OleDbCommand("select * from " + tableName, oraConn))
			using (OleDbDataReader reader = cmd.ExecuteReader())
			using (StreamWriter sw = new StreamWriter(filePath, append: false, Encoding.Default))
			{
				int fieldCount = reader.FieldCount;
				for (int i = 0; i < fieldCount; i++)
				{
					sw.Write(AgreeSql.CsvEscape(reader.GetName(i)));
					if (i < fieldCount - 1)
					{
						sw.Write(",");
					}
				}
				sw.WriteLine();
				while (reader.Read())
				{
					for (int i = 0; i < fieldCount; i++)
					{
						sw.Write(AgreeSql.CsvEscape(reader[i].ToString()));
						if (i < fieldCount - 1)
						{
							sw.Write(",");
						}
					}
					sw.WriteLine();
				}
			}
		}
		finally
		{
			if (oraConn.State != ConnectionState.Closed)
			{
				oraConn.Close();
			}
		}
	}

	private void ImportTable(string folder, string fileName, string tableName, string keyColumn, string seqName, StringBuilder report)
	{
		string path = Path.Combine(folder, fileName);
		if (!File.Exists(path))
		{
			report.AppendLine(fileName + " : スキップ（ファイルがありません）");
			return;
		}
		int count = MergeCsvToTable(tableName, keyColumn, path);
		ResyncSequence(seqName, tableName, keyColumn);
		report.AppendLine(fileName + " : " + count + "件 取り込みました");
	}

	private int MergeCsvToTable(string tableName, string keyColumn, string filePath)
	{
		int count = 0;
		oraConn.Open();
		OleDbTransaction tx = oraConn.BeginTransaction();
		try
		{
			using (TextFieldParser parser = new TextFieldParser(filePath, Encoding.Default))
			{
				parser.TextFieldType = FieldType.Delimited;
				parser.SetDelimiters(",");
				parser.HasFieldsEnclosedInQuotes = true;
				if (parser.EndOfData)
				{
					tx.Commit();
					return 0;
				}
				string[] columns = parser.ReadFields();
				for (int i = 0; i < columns.Length; i++)
				{
					columns[i] = columns[i].Trim();
				}
				int keyIndex = Array.FindIndex(columns, (string c) => c.Equals(keyColumn, StringComparison.OrdinalIgnoreCase));
				if (keyIndex < 0)
				{
					throw new Exception(tableName + " のCSVに " + keyColumn + " 列がありません。");
				}
				while (!parser.EndOfData)
				{
					string[] values = parser.ReadFields();
					if (values == null)
					{
						continue;
					}
					MergeRow(tableName, keyColumn, columns, values, keyIndex, tx);
					count++;
				}
			}
			tx.Commit();
		}
		catch
		{
			tx.Rollback();
			throw;
		}
		finally
		{
			if (oraConn.State != ConnectionState.Closed)
			{
				oraConn.Close();
			}
		}
		return count;
	}

	private void MergeRow(string tableName, string keyColumn, string[] columns, string[] values, int keyIndex, OleDbTransaction tx)
	{
		string keyValue = AgreeSql.SqlValue(values[keyIndex]);
		int exists;
		using (OleDbCommand chk = new OleDbCommand("select count(*) from " + tableName + " where " + keyColumn + " = " + keyValue, oraConn, tx))
		{
			exists = Convert.ToInt32(chk.ExecuteScalar());
		}
		StringBuilder sql = new StringBuilder();
		if (exists > 0)
		{
			sql.Append("update " + tableName + " set ");
			bool first = true;
			for (int i = 0; i < columns.Length; i++)
			{
				if (i == keyIndex)
				{
					continue;
				}
				if (!first)
				{
					sql.Append(", ");
				}
				sql.Append(columns[i] + " = " + AgreeSql.SqlValue(values[i]));
				first = false;
			}
			sql.Append(" where " + keyColumn + " = " + keyValue);
		}
		else
		{
			sql.Append("insert into " + tableName + " (");
			sql.Append(string.Join(", ", columns));
			sql.Append(") values (");
			for (int i = 0; i < values.Length; i++)
			{
				if (i > 0)
				{
					sql.Append(", ");
				}
				sql.Append(AgreeSql.SqlValue(values[i]));
			}
			sql.Append(")");
		}
		using (OleDbCommand cmd = new OleDbCommand(sql.ToString(), oraConn, tx))
		{
			cmd.ExecuteNonQuery();
		}
	}

	private void ResyncSequence(string seqName, string tableName, string keyColumn)
	{
		oraConn.Open();
		try
		{
			long maxId;
			using (OleDbCommand cmd = new OleDbCommand("select nvl(max(" + keyColumn + "), 0) from " + tableName, oraConn))
			{
				maxId = Convert.ToInt64(cmd.ExecuteScalar());
			}
			long current;
			using (OleDbCommand cmd = new OleDbCommand("select " + seqName + ".nextval from dual", oraConn))
			{
				current = Convert.ToInt64(cmd.ExecuteScalar());
			}
			// 次に発行される値を maxId+1 にしたい。現在の nextval は current を返したので、
			// 次の値は current+1。既に十分大きければ何もしない（シーケンスは後退させない）。
			long gap = maxId + 1 - (current + 1);
			if (gap > 0)
			{
				ExecuteDdl("alter sequence " + seqName + " increment by " + gap);
				using (OleDbCommand cmd = new OleDbCommand("select " + seqName + ".nextval from dual", oraConn))
				{
					cmd.ExecuteScalar();
				}
				ExecuteDdl("alter sequence " + seqName + " increment by 1");
			}
		}
		finally
		{
			if (oraConn.State != ConnectionState.Closed)
			{
				oraConn.Close();
			}
		}
	}

	private void ExecuteDdl(string sql)
	{
		using (OleDbCommand cmd = new OleDbCommand(sql, oraConn))
		{
			cmd.ExecuteNonQuery();
		}
	}

}
