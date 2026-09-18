using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using AgentlabUtilityLibrary;

namespace Agree;

// 用語: dr_id / dr_name は画面上「入力者」と表示する。DB では AGREE.DR（M_USR のコード）に対応する。
public partial class Form1 : Form
{
	// 医師完了フラグ（AGREE.DR_OK）。true のとき登録完了後に印刷を促す。
	private bool doctorConfirmed = true;

	private PatCsvFields patCsv = new PatCsvFields();

	// 診療科コード → 略称。電子カルテの診療科マスタを起動時に読む。
	private readonly Dictionary<string, string> deptNames = new Dictionary<string, string>();

	private OleDbConnection oraConn;

	private readonly Ehr ehr;

	public Form1()
	{
		InitializeComponent();
		System.Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		this.Text = $"眼科同意書v{version.Major}.{version.Minor}.{version.Build}";
		applyWindowPosition();
		oraConn = DBConn.GetOpenDBConn();
		ehr = new Ehr(DBConn.GetEhrDBConn(), Env.DB_LINK, Env.LEGACY_HOME + "\\Pat.csv");
		try
		{
			foreach (KeyValuePair<string, string> d in ehr.LoadDepartments())
			{
				deptNames[d.Key] = d.Value;
				if (!d.Key.Equals("0"))
				{
					dept.Items.Add(d.Key + " " + deptNames[d.Key]);
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
	/// Pat.csv の先頭行から使う項目だけを patCsv に読み込む。ファイルが無い／空の場合は false を返し、
	/// patCsv は変更しない（呼び出し側はその場合 patCsv を参照しない）。
	/// </summary>
	private bool loadPatCsvFields()
	{
		PatCsvFields fields = ehr.ReadPatCsv();
		if (fields == null)
		{
			return false;
		}
		patCsv = fields;
		return true;
	}

	private void readPatCsv()
	{
		// コンストラクタから呼ばれるため、患者IDが数字でない Pat.csv は例外にせず読み飛ばす。
		if (!loadPatCsvFields() || patCsv.PtId == null)
		{
			return;
		}
		clearAgree();
		pt_id.Text = patCsv.PtId.ToString();
		applyPatientFromCsv();
		if (patCsv.PtId > 0)
		{
			showList();
		}
	}

	// Pat.csv の患者情報（氏名・カナ・性別）を画面に反映する。
	private void applyPatientFromCsv()
	{
		pt_name.Text = patCsv.PtName;
		pt_kana.Text = patCsv.PtKana;
		pt_sex.Text = patCsv.PtSex;
	}

	// Pat.csv の医師情報（入力者・診療科）を画面に反映する。
	private void applyDoctorFromPatCsv()
	{
		if (!loadPatCsvFields())
		{
			return;
		}
		if (patCsv.Field27 == "1")
		{
			dr_id.Text = int.Parse(patCsv.DrId).ToString();
			dr_name.Text = patCsv.DrName;
			if (!Program.OfflineMode)
			{
				if (Ehr.TryParseDeptCode(patCsv.DeptCode, out short deptCode) && patCsv.Field14.Length > 0 && deptNames.ContainsKey(deptCode.ToString()))
				{
					dept.Text = deptCode + " " + deptNames[deptCode.ToString()];
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
		if (dept.Text.Length > 0 && (dept.Text.Split(' ').Length != 2 || !Ehr.TryParseDeptCode(dept.Text.Split(' ')[0], out _)))
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
			string name = ehr.StaffName(dr_id.Text);
			if (name != null)
			{
				dr_name.Text = name;
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
		TmpStaff tmpStaff = new TmpStaff(ehr);
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
