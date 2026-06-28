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

public partial class Form1 : Form
{
	private bool staff1_ok = true;

	private string[] patCont = new string[50];

	private OleDbConnection oraConn;

	private OleDbCommand oraCmd = new OleDbCommand();

	private OleDbDataReader oraReader;

	public Form1()
	{
		InitializeComponent();
		System.Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		this.Text = $"眼科同意書v{version.Major}.{version.Minor}.{version.Build}";
		applyWindowPosition();
		oraConn = DBConn.GetOpenDBConn();
		oraCmd.Connection = oraConn;
		try
		{
			foreach (string key in Dict.DeptDict.Keys)
			{
				if (!key.Equals("0"))
				{
					dept.Items.Add(key + " " + Dict.DeptDict[key].ShortName);
				}
			}
		}
		catch (Exception ex)
		{
			Program.OfflineMode = true;
			MessageBox.Show("データベースに接続できません。オフラインモード（画面確認用）で起動します。\n" + ex.Message, "オフラインモード", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		agreePlanListLabel.Text = "患者IDを入力して Enter を押すと既存の同意書が表示されます";
		initShow();
	}

	private void initShow()
	{
		clearPlan();
		readPatCsv();
		printAgreeButton.Enabled = false;
		applySettingButtonVisibility();
	}

	private void applySettingButtonVisibility()
	{
		// 専用の外部設定ファイル EyeAgreeSettings.ini から設定ボタンの表示・非表示を読み込む。
		// ファイルが無い・読めない・値が不正な場合は安全のため非表示にする。
		settingButton.Visible = false;
		try
		{
			string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EyeAgreeSettings.ini");
			if (!File.Exists(configPath))
			{
				return;
			}

			string[] lines = File.ReadAllLines(configPath, Encoding.Default);
			foreach (string line in lines)
			{
				if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
				{
					continue;
				}

				if (line.Contains("SHOW_SETTING_BUTTON"))
				{
					string[] parts = line.Split('=');
					if (parts.Length == 2)
					{
						settingButton.Visible = (parts[1].Trim() == "1");
					}
					break;
				}
			}
		}
		catch (IOException)
		{
			settingButton.Visible = false;
		}
	}

	// ウィンドウの初期表示位置を毎回固定する。既定は画面左上 (0, 0)。
	// EyeAgreeSettings.ini の [WINDOW_SETTINGS] WINDOW_X / WINDOW_Y で上書きできる。
	// ファイルが無い・読めない・値が不正な場合は既定値を維持する。
	private void applyWindowPosition()
	{
		StartPosition = FormStartPosition.Manual;
		int x = 0;
		int y = 0;
		try
		{
			string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EyeAgreeSettings.ini");
			if (File.Exists(configPath))
			{
				string[] lines = File.ReadAllLines(configPath, Encoding.Default);
				foreach (string line in lines)
				{
					if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
					{
						continue;
					}
					string[] parts = line.Split('=');
					if (parts.Length != 2)
					{
						continue;
					}
					string key = parts[0].Trim();
					string val = parts[1].Trim();
					if (key == "WINDOW_X")
					{
						if (int.TryParse(val, out int n))
						{
							x = n;
						}
					}
					else if (key == "WINDOW_Y")
					{
						if (int.TryParse(val, out int n))
						{
							y = n;
						}
					}
				}
			}
		}
		catch (IOException)
		{
		}
		Location = new Point(x, y);
	}

	private void clearPlan()
	{
		Agree_id.Text = "";
		dr_id.Text = "";
		dr_name.Text = "";
		staff.Text = "";
		save_date.Text = "";
		dept.Text = "";
		diag.Text = "";
		anes.Text = "";
		ope.Text = "";
		item1.Text = "";
		item2.Text = "";
		item3.Text = "";
		item4.Text = "";
		explanation.Text = "";
		staff1_ok = true;
		eye.Text = "";
		printAgreeButton.Enabled = false;
		sheetName.Text = "";
	}

	/// <summary>
	/// Pat.csv の先頭行を patCont に読み込む。ファイルが無い／空の場合は false を返し、
	/// patCont は変更しない（呼び出し側はその場合 patCont を参照しない）。
	/// </summary>
	private bool loadPatCsvFields()
	{
		string path = Env.LEGACY_HOME + "\\Pat.csv";
		if (!File.Exists(path))
		{
			return false;
		}
		using (StreamReader reader = new StreamReader(path, Encoding.Default))
		{
			string line = reader.ReadLine();
			if (line == null)
			{
				return false;
			}
			string[] fields = line.Split(',');
			for (int i = 0; i < fields.Length && i < 50; i++)
			{
				patCont[i] = fields[i];
			}
		}
		return true;
	}

	private void readPatCsv()
	{
		if (!loadPatCsvFields())
		{
			return;
		}
		clearPlan();
		pt_id.Text = int.Parse(patCont[2]).ToString();
		pt_name.Text = patCont[3];
		pt_kana.Text = patCont[5];
		if (patCont[6] == "2")
		{
			pt_sex.Text = "女";
		}
		else if (patCont[6] == "1")
		{
			pt_sex.Text = "男";
		}
		if (int.Parse(pt_id.Text) > 0)
		{
			showList();
		}
	}

	private void readPatCsv2()
	{
		if (!loadPatCsvFields())
		{
			return;
		}
		if (patCont[27] == "1")
		{
			dr_id.Text = int.Parse(patCont[9]).ToString();
			dr_name.Text = patCont[10];
			if (!Program.OfflineMode)
			{
				if (short.Parse(patCont[13]) > 0 && short.Parse(patCont[13]) < 20 && patCont[14].Length > 0 && Dict.DeptDict.ContainsKey(short.Parse(patCont[13]).ToString()))
				{
					dept.Text = short.Parse(patCont[13]) + " " + Dict.DeptDict[short.Parse(patCont[13]).ToString()].ShortName;
				}
				getStaffRoom();
			}
		}
	}

	private void pt_id_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			showList();
		}
	}

	private void agreePlanList_RowEnter(object sender, DataGridViewCellEventArgs e)
	{
		showPlan(e.RowIndex);
	}

	private void AgreeList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
		{
			AgreeList.ClearSelection();
			AgreeList.Rows[e.RowIndex].Selected = true;
			AgreeList.CurrentCell = AgreeList.Rows[e.RowIndex].Cells[1];
		}
	}

	private void closeButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void dept_Leave(object sender, EventArgs e)
	{
		if (dept.Text.Length > 0)
		{
			if (dept.Text.Split(' ').Length != 2)
			{
				MessageBox.Show("診療科はリストから選んでください");
				dept.Text = "";
			}
			else if (short.Parse(dept.Text.Split(' ')[0]) < 1 || short.Parse(dept.Text.Split(' ')[0]) > 20)
			{
				MessageBox.Show("診療科はリストから選んでください");
				dept.Text = "";
			}
		}
	}

	private void staff1_id_Leave(object sender, EventArgs e)
	{
		if (Program.OfflineMode)
		{
			return;
		}
		if (dr_id.Text.Length > 0)
		{
			if (Dict.StaffDict.ContainsKey(dr_id.Text.Trim()))
			{
				dr_name.Text = Dict.StaffDict[dr_id.Text.Trim()].Name;
				getStaffRoom();
			}
			else
			{
				MessageBox.Show("該当する医師はありません");
				dr_id.Text = "";
				dr_name.Text = "";
			}
		}
		else
		{
			dr_id.Text = "";
			dr_name.Text = "";
		}
	}

	private void staff1_id_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			save_date.Focus();
		}
	}

	private void tmpPlanButton_Click(object sender, EventArgs e)
	{
		if (regAgreeButton.Enabled)
		{
			TmpAgree tmpAgree = new TmpAgree(this, applyButtonVisible: true);
			tmpAgree.Show();
		}
		else
		{
			TmpAgree tmpAgree2 = new TmpAgree(this, applyButtonVisible: false);
			tmpAgree2.Show();
		}
	}

	private void newPlanButton_Click(object sender, EventArgs e)
	{
		if (Agree_id.Text.Length > 0)
		{
			switch (MessageBox.Show("記載中の内容を保存しますか？", "保存", MessageBoxButtons.YesNoCancel))
			{
			case DialogResult.Yes:
				regPlan();
				clearPlan();
				break;
			case DialogResult.No:
				clearPlan();
				break;
			}
		}
		else
		{
			clearPlan();
		}
		sheetName.Text = "通常";
		eye.Text = "";
		readPatCsv2();
	}

	public void getStaffRoom()
	{
		if (Program.OfflineMode)
		{
			return;
		}
		oraConn.Open();
		oraCmd.CommandText = "select CONT from AGREE_STAFF where STAFF = " + dr_id.Text.Trim();
		oraReader = oraCmd.ExecuteReader();
		if (oraReader.Read() && (staff.Text.Length == 0 || MessageBox.Show("担当者が既に入力されています。上書きしますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes))
		{
			staff.Text = oraReader["CONT"].ToString().Trim();
		}
		oraReader.Close();
		oraConn.Close();
	}

	private void tmpStaffButton_Click(object sender, EventArgs e)
	{
		TmpStaff tmpStaff = new TmpStaff();
		tmpStaff.Show();
	}

	private void settingButton_Click(object sender, EventArgs e)
	{
		bool show = !exportButton.Visible;
		exportButton.Visible = show;
		importButton.Visible = show;
		if (show)
		{
			exportButton.BringToFront();
			importButton.BringToFront();
		}
	}

    private void label5_Click(object sender, EventArgs e)
    {

    }

    private void agreePlanListLabel_Click(object sender, EventArgs e)
    {

    }

    private void panel1_Paint(object sender, PaintEventArgs e)
    {

    }

    private void explanation_TextChanged(object sender, EventArgs e)
    {

    }
}
