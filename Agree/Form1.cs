using System;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AgentlabUtilityLibrary;

namespace Agree;

// 用語: dr_id / dr_name は画面上「入力者」と表示する。DB では AGREE.DR（M_USR のコード）に対応する。
public partial class Form1 : Form
{
	// 医師完了フラグ（AGREE.DR_OK）。true のとき登録完了後に印刷を促す。
	private bool doctorConfirmed = true;

	private string[] patCont = new string[50];

	private OleDbConnection oraConn;

	public Form1()
	{
		InitializeComponent();
		System.Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		this.Text = $"眼科同意書v{version.Major}.{version.Minor}.{version.Build}";
		applyWindowPosition();
		oraConn = DBConn.GetOpenDBConn();
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
			Logger.Error("Form1", ex);
			Program.OfflineMode = true;
			MessageBox.Show("データベースに接続できません。オフラインモード（画面確認用）で起動します。\n" + ex.Message, "オフラインモード", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		agreeListLabel.Text = "患者IDを入力して Enter を押すと既存の同意書が表示されます";
		initShow();
	}

	private void initShow()
	{
		clearAgree();
		readPatCsv();
		printAgreeButton.Enabled = false;
		applySettingButtonVisibility();
	}

	// EyeAgreeSettings.ini の SHOW_SETTING_BUTTON=1 のときだけ設定ボタンを表示する。
	// ファイルが無い・読めない・値が不正な場合は安全のため非表示にする。
	private void applySettingButtonVisibility()
	{
		settingButton.Visible = AppSettings.Get("SHOW_SETTING_BUTTON", "0") == "1";
	}

	// ウィンドウの初期表示位置を毎回固定する。既定は画面左上 (0, 0)。
	// EyeAgreeSettings.ini の [WINDOW_SETTINGS] WINDOW_X / WINDOW_Y で上書きできる。
	// ファイルが無い・読めない・値が不正な場合は既定値を維持する。
	private void applyWindowPosition()
	{
		StartPosition = FormStartPosition.Manual;
		Location = new Point(AppSettings.GetInt("WINDOW_X", 0), AppSettings.GetInt("WINDOW_Y", 0));
	}

	private void clearAgree()
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
		doctorConfirmed = true;
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
		// コンストラクタから呼ばれるため、患者IDが数字でない Pat.csv は例外にせず読み飛ばす。
		if (!loadPatCsvFields() || !int.TryParse(patCont[2], out int ptId))
		{
			return;
		}
		clearAgree();
		pt_id.Text = ptId.ToString();
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
		if (ptId > 0)
		{
			showList();
		}
	}

	// Pat.csv の医師情報（入力者・診療科）を画面に反映する。
	private void applyDoctorFromPatCsv()
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
				if (tryParseDeptCode(patCont[13], out short deptCode) && patCont[14].Length > 0 && Dict.DeptDict.ContainsKey(deptCode.ToString()))
				{
					dept.Text = deptCode + " " + Dict.DeptDict[deptCode.ToString()].ShortName;
				}
				getStaffRoom();
			}
		}
	}

	// 診療科コード（1〜20）として解釈できる場合だけ true を返す。
	private static bool tryParseDeptCode(string text, out short code)
	{
		return short.TryParse(text, out code) && code >= 1 && code <= 20;
	}

	private void pt_id_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			showList();
		}
	}

	private void AgreeList_RowEnter(object sender, DataGridViewCellEventArgs e)
	{
		showAgree(e.RowIndex);
	}

	private void AgreeList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
		{
			AgreeList.ClearSelection();
			AgreeList.Rows[e.RowIndex].Selected = true;
			AgreeList.CurrentCell = AgreeList.Rows[e.RowIndex].Cells["SAVE_DATE"];
		}
	}

	private void closeButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void dept_Leave(object sender, EventArgs e)
	{
		if (dept.Text.Length > 0 && (dept.Text.Split(' ').Length != 2 || !tryParseDeptCode(dept.Text.Split(' ')[0], out _)))
		{
			MessageBox.Show("診療科はリストから選んでください");
			dept.Text = "";
		}
	}

	private void dr_id_Leave(object sender, EventArgs e)
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

	private void dr_id_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			save_date.Focus();
		}
	}

	private void tmpAgreeButton_Click(object sender, EventArgs e)
	{
		new TmpAgree(this, applyButtonVisible: true).Show();
	}

	private void newAgreeButton_Click(object sender, EventArgs e)
	{
		if (Agree_id.Text.Length > 0)
		{
			switch (MessageBox.Show("記載中の内容を保存しますか？", "保存", MessageBoxButtons.YesNoCancel))
			{
			case DialogResult.Yes:
				// 保存が入力チェックで中断した場合は、入力内容を残すため新規作成に進まない。
				if (!regAgree())
				{
					return;
				}
				clearAgree();
				break;
			case DialogResult.No:
				clearAgree();
				break;
			}
		}
		else
		{
			clearAgree();
		}
		sheetName.Text = "通常";
		eye.Text = "";
		applyDoctorFromPatCsv();
	}

	public void getStaffRoom()
	{
		if (Program.OfflineMode || !int.TryParse(dr_id.Text.Trim(), out int drId))
		{
			return;
		}
		object cont = Db.Scalar(oraConn, "select CONT from AGREE_STAFF where STAFF = " + drId);
		if (cont != null && (staff.Text.Length == 0 || MessageBox.Show("担当者が既に入力されています。上書きしますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes))
		{
			staff.Text = cont.ToString().Trim();
		}
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
}
