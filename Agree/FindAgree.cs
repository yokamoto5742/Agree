using System;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using AgentlabUtilityLibrary;

namespace Agree;

public class FindAgree : Form
{
	private IContainer components;

	private Label label1;

	private Label label2;

	private DataGridView AgreementList;

	private Button findButton;

	private Button selectButton;

	private Button closeButton;

	private RadioButton dr_ok1;

	private RadioButton dr_ok2;

	private RadioButton dr_ok3;

	private Panel panel1;

	private DateTimePicker dateFrom;

	private DateTimePicker dateTo;

	private Label label3;

	private CheckBox fromToCheck;

	private Form1 f1;

	private OleDbConnection oraConn;

	private OleDbCommand oraCmd = new OleDbCommand();

	private OleDbDataReader oraReader;

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Agree.FindAgree));
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.AgreementList = new System.Windows.Forms.DataGridView();
		this.findButton = new System.Windows.Forms.Button();
		this.selectButton = new System.Windows.Forms.Button();
		this.closeButton = new System.Windows.Forms.Button();
		this.dr_ok1 = new System.Windows.Forms.RadioButton();
		this.dr_ok2 = new System.Windows.Forms.RadioButton();
		this.dr_ok3 = new System.Windows.Forms.RadioButton();
		this.panel1 = new System.Windows.Forms.Panel();
		this.dateFrom = new System.Windows.Forms.DateTimePicker();
		this.dateTo = new System.Windows.Forms.DateTimePicker();
		this.label3 = new System.Windows.Forms.Label();
		this.fromToCheck = new System.Windows.Forms.CheckBox();
		((System.ComponentModel.ISupportInitialize)this.AgreementList).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(12, 15);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(41, 12);
		this.label1.TabIndex = 2;
		this.label1.Text = "作成日";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(12, 44);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(29, 12);
		this.label2.TabIndex = 3;
		this.label2.Text = "医師";
		this.AgreementList.AllowUserToAddRows = false;
		this.AgreementList.AllowUserToDeleteRows = false;
		this.AgreementList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.AgreementList.Location = new System.Drawing.Point(14, 90);
		this.AgreementList.MultiSelect = false;
		this.AgreementList.Name = "AgreementList";
		this.AgreementList.ReadOnly = true;
		this.AgreementList.RowHeadersVisible = false;
		this.AgreementList.RowTemplate.Height = 21;
		this.AgreementList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.AgreementList.Size = new System.Drawing.Size(599, 493);
		this.AgreementList.TabIndex = 8;
		this.findButton.Location = new System.Drawing.Point(370, 36);
		this.findButton.Name = "findButton";
		this.findButton.Size = new System.Drawing.Size(69, 26);
		this.findButton.TabIndex = 9;
		this.findButton.Text = "検索";
		this.findButton.UseVisualStyleBackColor = true;
		this.findButton.Click += new System.EventHandler(findButton_Click);
		this.selectButton.Location = new System.Drawing.Point(456, 36);
		this.selectButton.Name = "selectButton";
		this.selectButton.Size = new System.Drawing.Size(69, 26);
		this.selectButton.TabIndex = 10;
		this.selectButton.Text = "選択";
		this.selectButton.UseVisualStyleBackColor = true;
		this.selectButton.Click += new System.EventHandler(selectButton_Click);
		this.closeButton.Location = new System.Drawing.Point(529, 589);
		this.closeButton.Name = "closeButton";
		this.closeButton.Size = new System.Drawing.Size(84, 26);
		this.closeButton.TabIndex = 11;
		this.closeButton.Text = "閉じる";
		this.closeButton.UseVisualStyleBackColor = true;
		this.closeButton.Click += new System.EventHandler(closeButton_Click);
		this.dr_ok1.AutoSize = true;
		this.dr_ok1.Location = new System.Drawing.Point(14, 5);
		this.dr_ok1.Name = "dr_ok1";
		this.dr_ok1.Size = new System.Drawing.Size(52, 16);
		this.dr_ok1.TabIndex = 12;
		this.dr_ok1.TabStop = true;
		this.dr_ok1.Text = "すべて";
		this.dr_ok1.UseVisualStyleBackColor = true;
		this.dr_ok2.AutoSize = true;
		this.dr_ok2.Location = new System.Drawing.Point(101, 5);
		this.dr_ok2.Name = "dr_ok2";
		this.dr_ok2.Size = new System.Drawing.Size(68, 16);
		this.dr_ok2.TabIndex = 13;
		this.dr_ok2.TabStop = true;
		this.dr_ok2.Text = "完成のみ";
		this.dr_ok2.UseVisualStyleBackColor = true;
		this.dr_ok3.AutoSize = true;
		this.dr_ok3.Location = new System.Drawing.Point(192, 5);
		this.dr_ok3.Name = "dr_ok3";
		this.dr_ok3.Size = new System.Drawing.Size(80, 16);
		this.dr_ok3.TabIndex = 14;
		this.dr_ok3.TabStop = true;
		this.dr_ok3.Text = "未完成のみ";
		this.dr_ok3.UseVisualStyleBackColor = true;
		this.panel1.Controls.Add(this.dr_ok3);
		this.panel1.Controls.Add(this.dr_ok2);
		this.panel1.Controls.Add(this.dr_ok1);
		this.panel1.Location = new System.Drawing.Point(72, 36);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(287, 24);
		this.panel1.TabIndex = 15;
		this.dateFrom.Location = new System.Drawing.Point(72, 11);
		this.dateFrom.Name = "dateFrom";
		this.dateFrom.Size = new System.Drawing.Size(136, 19);
		this.dateFrom.TabIndex = 16;
		this.dateFrom.ValueChanged += new System.EventHandler(dateFrom_ValueChanged);
		this.dateTo.Location = new System.Drawing.Point(250, 12);
		this.dateTo.Name = "dateTo";
		this.dateTo.Size = new System.Drawing.Size(136, 19);
		this.dateTo.TabIndex = 17;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(227, 15);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(17, 12);
		this.label3.TabIndex = 18;
		this.label3.Text = "～";
		this.fromToCheck.AutoSize = true;
		this.fromToCheck.Location = new System.Drawing.Point(413, 13);
		this.fromToCheck.Name = "fromToCheck";
		this.fromToCheck.Size = new System.Drawing.Size(91, 16);
		this.fromToCheck.TabIndex = 19;
		this.fromToCheck.Text = "範囲指定する";
		this.fromToCheck.UseVisualStyleBackColor = true;
		this.fromToCheck.CheckedChanged += new System.EventHandler(fromToCheck_CheckedChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(625, 617);
		base.Controls.Add(this.fromToCheck);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.dateTo);
		base.Controls.Add(this.dateFrom);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.closeButton);
		base.Controls.Add(this.selectButton);
		base.Controls.Add(this.findButton);
		base.Controls.Add(this.AgreementList);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "FindAgree";
		this.Text = "同意書検索";
		base.Load += new System.EventHandler(FindPlan_Load);
		((System.ComponentModel.ISupportInitialize)this.AgreementList).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public FindAgree(Form1 F1)
	{
		InitializeComponent();
		f1 = F1;
	}

	private void FindPlan_Load(object sender, EventArgs e)
	{
		oraConn = DBConn.GetOpenDBConn();
		oraCmd.Connection = oraConn;
		dateTo.Enabled = false;
		dr_ok1.Checked = true;
		showList();
	}

	private void showList()
	{
		string text = "Select AGREE.AGREE_ID, AGREE.SAVE_DATE, AGREE.PATIENT_ID, Trim(M_PATIENT.P_NAME), AGREE.DEPT, Trim(M_DEPT.S_NAME), AGREE.DR, Trim(M_USR.NAME), case when AGREE.DR_OK = 1 then '○' else '-' end as 医師完了 from AGREE inner join M_PATIENT" + Env.DB_LINK + " on M_PATIENT.P_ID = AGREE.PATIENT_ID inner join M_DEPT" + Env.DB_LINK + " on AGREE.DEPT = M_DEPT.CODE inner join M_USR" + Env.DB_LINK + " on AGREE.DR = M_USR.CODE where DELETE_FLAG = 0 ";
		string text2 = text;
		text = text2 + " and SAVE_DATE >= " + dateFrom.Value.ToString("yyyyMMdd") + " and SAVE_DATE <= " + dateTo.Value.ToString("yyyyMMdd");
		if (dr_ok2.Checked)
		{
			text += " and DR_OK = 1";
		}
		else if (dr_ok3.Checked)
		{
			text += " and DR_OK = 0";
		}
		oraCmd.CommandText = text;
		OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oraCmd);
		DataSet dataSet = new DataSet();
		oleDbDataAdapter.Fill(dataSet, "同意書");
		AgreementList.DataSource = dataSet.Tables["同意書"];
		AgreementList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		AgreementList.Columns[0].Visible = false;
		AgreementList.Columns[1].HeaderText = "作成日";
		AgreementList.Columns[1].Width = 70;
		AgreementList.Columns[1].DefaultCellStyle.Format = "0000/00/00";
		AgreementList.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		AgreementList.Columns[2].HeaderText = "ID";
		AgreementList.Columns[2].Width = 50;
		AgreementList.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
		AgreementList.Columns[3].HeaderText = "氏名";
		AgreementList.Columns[3].Width = 100;
		AgreementList.Columns[4].Visible = false;
		AgreementList.Columns[5].HeaderText = "診療科";
		AgreementList.Columns[5].Width = 70;
		AgreementList.Columns[6].Visible = false;
		AgreementList.Columns[7].HeaderText = "医師";
		AgreementList.Columns[7].Width = 80;
		AgreementList.Columns[8].HeaderText = "完了";
		AgreementList.Columns[8].Width = 40;
		AgreementList.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		oraConn.Close();
	}

	private void findButton_Click(object sender, EventArgs e)
	{
		showList();
	}

	private void closeButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void selectButton_Click(object sender, EventArgs e)
	{
		f1.showPlan(AgreementList.SelectedRows[0].Cells[0].Value.ToString(), AgreementList.SelectedRows[0].Cells[2].Value.ToString());
		Dispose();
	}

	private void fromToCheck_CheckedChanged(object sender, EventArgs e)
	{
		if (fromToCheck.Checked)
		{
			dateTo.Enabled = true;
			return;
		}
		dateTo.Value = dateFrom.Value;
		dateTo.Enabled = false;
	}

	private void dateFrom_ValueChanged(object sender, EventArgs e)
	{
		dateTo.Value = dateFrom.Value;
	}
}
