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

public class Form1 : Form
{
	private IContainer components;

	private TextBox pt_id;

	private Label label1;

	private TextBox pt_name;

	private DataGridView AgreeList;

	private ContextMenuStrip agreeListMenu;

	private ToolStripMenuItem copyNewMenuItem;

	private Label label2;

	private Button delAgreeButton;

	private Label label4;

	private Label label5;

	private Label label6;

	private TextBox dr_id;

	private TextBox staff;

	private Label label7;

	private TextBox diag;

	private Label label8;

	private TextBox ope;

	private Label label9;

	private TextBox explanation;

	private Label label10;

	private TextBox item1;

	private Label label12;

	private TextBox item2;

	private Label label15;

	private TextBox item3;

	private bool staff1_ok = true;

	private Label agreePlanListLabel;

	private Button regAgreeButton;

	private Button closeButton;

	private Button printAgreeButton;

	private TextBox dr_name;

	private ComboBox dept;

	private Label label19;

	private Button tmpAgreeButton;

	private DateTimePicker save_date;

	private Label label3;

	private Button newAgreeButton;

	private TextBox Agree_id;

	private Label label20;

	private Panel panel1;

	private Label label22;

	private Button tmpStaffButton;

	private Button findAgreeButton;

	private Button settingButton;

	private Button exportButton;

	private Button importButton;

	private TextBox item4;

	private ComboBox eye;

	private Label label14;

	private TextBox pt_kana;

	private TextBox pt_sex;

	private Label label13;

	private ComboBox sheetName;

	private CheckBox bothEye;

	private CheckBox leftEye;

	private CheckBox rightEye;

	private Label label11;

	private TextBox anes;

	private string[] patCont = new string[50];

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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Agree.Form1));
		this.pt_id = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.pt_name = new System.Windows.Forms.TextBox();
		this.AgreeList = new System.Windows.Forms.DataGridView();
		this.components = new System.ComponentModel.Container();
		this.agreeListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.copyNewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.label2 = new System.Windows.Forms.Label();
		this.delAgreeButton = new System.Windows.Forms.Button();
		this.label4 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.dr_id = new System.Windows.Forms.TextBox();
		this.staff = new System.Windows.Forms.TextBox();
		this.label7 = new System.Windows.Forms.Label();
		this.diag = new System.Windows.Forms.TextBox();
		this.label8 = new System.Windows.Forms.Label();
		this.ope = new System.Windows.Forms.TextBox();
		this.label9 = new System.Windows.Forms.Label();
		this.explanation = new System.Windows.Forms.TextBox();
		this.label10 = new System.Windows.Forms.Label();
		this.item1 = new System.Windows.Forms.TextBox();
		this.label12 = new System.Windows.Forms.Label();
		this.item2 = new System.Windows.Forms.TextBox();
		this.label15 = new System.Windows.Forms.Label();
		this.item3 = new System.Windows.Forms.TextBox();
		this.agreePlanListLabel = new System.Windows.Forms.Label();
		this.regAgreeButton = new System.Windows.Forms.Button();
		this.closeButton = new System.Windows.Forms.Button();
		this.printAgreeButton = new System.Windows.Forms.Button();
		this.dr_name = new System.Windows.Forms.TextBox();
		this.dept = new System.Windows.Forms.ComboBox();
		this.label19 = new System.Windows.Forms.Label();
		this.tmpAgreeButton = new System.Windows.Forms.Button();
		this.save_date = new System.Windows.Forms.DateTimePicker();
		this.label3 = new System.Windows.Forms.Label();
		this.newAgreeButton = new System.Windows.Forms.Button();
		this.Agree_id = new System.Windows.Forms.TextBox();
		this.label20 = new System.Windows.Forms.Label();
		this.panel1 = new System.Windows.Forms.Panel();
		this.label11 = new System.Windows.Forms.Label();
		this.anes = new System.Windows.Forms.TextBox();
		this.bothEye = new System.Windows.Forms.CheckBox();
		this.leftEye = new System.Windows.Forms.CheckBox();
		this.rightEye = new System.Windows.Forms.CheckBox();
		this.label13 = new System.Windows.Forms.Label();
		this.sheetName = new System.Windows.Forms.ComboBox();
		this.label14 = new System.Windows.Forms.Label();
		this.eye = new System.Windows.Forms.ComboBox();
		this.item4 = new System.Windows.Forms.TextBox();
		this.label22 = new System.Windows.Forms.Label();
		this.tmpStaffButton = new System.Windows.Forms.Button();
		this.findAgreeButton = new System.Windows.Forms.Button();
		this.settingButton = new System.Windows.Forms.Button();
		this.exportButton = new System.Windows.Forms.Button();
		this.importButton = new System.Windows.Forms.Button();
		this.pt_kana = new System.Windows.Forms.TextBox();
		this.pt_sex = new System.Windows.Forms.TextBox();
		((System.ComponentModel.ISupportInitialize)this.AgreeList).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.pt_id.ImeMode = System.Windows.Forms.ImeMode.Disable;
		this.pt_id.Location = new System.Drawing.Point(75, 11);
		this.pt_id.Name = "pt_id";
		this.pt_id.Size = new System.Drawing.Size(77, 19);
		this.pt_id.TabIndex = 0;
		this.pt_id.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.pt_id.KeyDown += new System.Windows.Forms.KeyEventHandler(pt_id_KeyDown);
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(18, 14);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(53, 12);
		this.label1.TabIndex = 15;
		this.label1.Text = "患者番号";
		this.pt_name.BackColor = System.Drawing.SystemColors.ScrollBar;
		this.pt_name.Location = new System.Drawing.Point(158, 11);
		this.pt_name.Name = "pt_name";
		this.pt_name.ReadOnly = true;
		this.pt_name.Size = new System.Drawing.Size(100, 19);
		this.pt_name.TabIndex = 15;
		this.AgreeList.AllowUserToAddRows = false;
		this.AgreeList.AllowUserToDeleteRows = false;
		this.AgreeList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.AgreeList.Location = new System.Drawing.Point(15, 83);
		this.AgreeList.MultiSelect = false;
		this.AgreeList.Name = "AgreeList";
		this.AgreeList.ReadOnly = true;
		dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
		dataGridViewCellStyle.Font = new System.Drawing.Font("MS UI Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
		dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
		dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.AgreeList.RowHeadersDefaultCellStyle = dataGridViewCellStyle;
		this.AgreeList.RowHeadersVisible = false;
		this.AgreeList.RowTemplate.Height = 21;
		this.AgreeList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.AgreeList.Size = new System.Drawing.Size(575, 122);
		this.AgreeList.TabIndex = 2;
		this.AgreeList.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(agreePlanList_RowEnter);
		this.AgreeList.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(AgreeList_CellMouseDown);
		this.AgreeList.ContextMenuStrip = this.agreeListMenu;
		this.agreeListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.copyNewMenuItem });
		this.agreeListMenu.Name = "agreeListMenu";
		this.copyNewMenuItem.Name = "copyNewMenuItem";
		this.copyNewMenuItem.Text = "コピーして作成";
		this.copyNewMenuItem.Click += new System.EventHandler(copyNewMenuItem_Click);
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(18, 66);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(65, 12);
		this.label2.TabIndex = 15;
		this.label2.Text = "同意書一覧";
		this.delAgreeButton.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.delAgreeButton.Location = new System.Drawing.Point(340, 705);
		this.delAgreeButton.Name = "delAgreeButton";
		this.delAgreeButton.Size = new System.Drawing.Size(75, 25);
		this.delAgreeButton.TabIndex = 5;
		this.delAgreeButton.Text = "削除";
		this.delAgreeButton.UseVisualStyleBackColor = false;
		this.delAgreeButton.Click += new System.EventHandler(delPlanButton_Click);
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(8, 82);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(17, 12);
		this.label4.TabIndex = 29;
		this.label4.Text = "眼";
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(126, 8);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(85, 12);
		this.label5.TabIndex = 21;
		this.label5.Text = "主治医(入力者)";
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(273, 61);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(111, 12);
		this.label6.TabIndex = 26;
		this.label6.Text = "主治医以外の担当者";
		this.dr_id.ImeMode = System.Windows.Forms.ImeMode.Disable;
		this.dr_id.Location = new System.Drawing.Point(210, 3);
		this.dr_id.MaxLength = 5;
		this.dr_id.Name = "dr_id";
		this.dr_id.Size = new System.Drawing.Size(48, 19);
		this.dr_id.TabIndex = 1;
		this.dr_id.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.dr_id.KeyDown += new System.Windows.Forms.KeyEventHandler(staff1_id_KeyDown);
		this.dr_id.Leave += new System.EventHandler(staff1_id_Leave);
		this.staff.Location = new System.Drawing.Point(275, 79);
		this.staff.MaxLength = 120;
		this.staff.Name = "staff";
		this.staff.Size = new System.Drawing.Size(277, 19);
		this.staff.TabIndex = 5;
		this.label7.AutoSize = true;
		this.label7.Location = new System.Drawing.Point(8, 108);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(29, 12);
		this.label7.TabIndex = 30;
		this.label7.Text = "病名";
		this.diag.Location = new System.Drawing.Point(88, 104);
		this.diag.MaxLength = 200;
		this.diag.Name = "diag";
		this.diag.Size = new System.Drawing.Size(464, 19);
		this.diag.TabIndex = 6;
		this.label8.AutoSize = true;
		this.label8.Location = new System.Drawing.Point(8, 157);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(71, 12);
		this.label8.TabIndex = 31;
		this.label8.Text = "手術・検査名";
		this.ope.Location = new System.Drawing.Point(88, 154);
		this.ope.MaxLength = 200;
		this.ope.Multiline = true;
		this.ope.Name = "ope";
		this.ope.Size = new System.Drawing.Size(464, 19);
		this.ope.TabIndex = 7;
		this.label9.AutoSize = true;
		this.label9.Location = new System.Drawing.Point(10, 179);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(29, 12);
		this.label9.TabIndex = 32;
		this.label9.Text = "説明";
		this.explanation.Location = new System.Drawing.Point(88, 179);
		this.explanation.MaxLength = 1200;
		this.explanation.Multiline = true;
		this.explanation.Name = "explanation";
		this.explanation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.explanation.Size = new System.Drawing.Size(464, 110);
		this.explanation.TabIndex = 8;
		this.label10.AutoSize = true;
		this.label10.Location = new System.Drawing.Point(7, 295);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(29, 12);
		this.label10.TabIndex = 6;
		this.label10.Text = "症状";
		this.item1.Location = new System.Drawing.Point(88, 295);
		this.item1.MaxLength = 200;
		this.item1.Multiline = true;
		this.item1.Name = "item1";
		this.item1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.item1.Size = new System.Drawing.Size(464, 32);
		this.item1.TabIndex = 9;
		this.label12.AutoSize = true;
		this.label12.Location = new System.Drawing.Point(8, 333);
		this.label12.Name = "label12";
		this.label12.Size = new System.Drawing.Size(53, 12);
		this.label12.TabIndex = 3;
		this.label12.Text = "治療計画";
		this.item2.Location = new System.Drawing.Point(87, 333);
		this.item2.MaxLength = 200;
		this.item2.Multiline = true;
		this.item2.Name = "item2";
		this.item2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.item2.Size = new System.Drawing.Size(464, 29);
		this.item2.TabIndex = 10;
		this.label15.AutoSize = true;
		this.label15.Location = new System.Drawing.Point(8, 372);
		this.label15.Name = "label15";
		this.label15.Size = new System.Drawing.Size(53, 12);
		this.label15.TabIndex = 7;
		this.label15.Text = "検査内容";
		this.label15.Visible = false;
		this.item3.Location = new System.Drawing.Point(88, 368);
		this.item3.MaxLength = 200;
		this.item3.Multiline = true;
		this.item3.Name = "item3";
		this.item3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.item3.Size = new System.Drawing.Size(464, 41);
		this.item3.TabIndex = 11;
		this.item3.Visible = false;
		this.agreePlanListLabel.AutoSize = true;
		this.agreePlanListLabel.Location = new System.Drawing.Point(130, 67);
		this.agreePlanListLabel.Name = "agreePlanListLabel";
		this.agreePlanListLabel.Size = new System.Drawing.Size(320, 12);
		this.agreePlanListLabel.TabIndex = 15;
		this.agreePlanListLabel.Text = "患者番号を入力して Enter を押すと既存の同意書が表示されます";
		this.regAgreeButton.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.regAgreeButton.Location = new System.Drawing.Point(259, 705);
		this.regAgreeButton.Name = "regAgreeButton";
		this.regAgreeButton.Size = new System.Drawing.Size(75, 25);
		this.regAgreeButton.TabIndex = 4;
		this.regAgreeButton.Text = "登録";
		this.regAgreeButton.UseVisualStyleBackColor = false;
		this.regAgreeButton.Click += new System.EventHandler(regPlanButton_Click);
		this.closeButton.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.closeButton.Location = new System.Drawing.Point(502, 705);
		this.closeButton.Name = "closeButton";
		this.closeButton.Size = new System.Drawing.Size(75, 25);
		this.closeButton.TabIndex = 7;
		this.closeButton.Text = "閉じる";
		this.closeButton.UseVisualStyleBackColor = false;
		this.closeButton.Click += new System.EventHandler(closeButton_Click);
		this.printAgreeButton.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.printAgreeButton.Location = new System.Drawing.Point(421, 705);
		this.printAgreeButton.Name = "printAgreeButton";
		this.printAgreeButton.Size = new System.Drawing.Size(75, 25);
		this.printAgreeButton.TabIndex = 6;
		this.printAgreeButton.Text = "印刷";
		this.printAgreeButton.UseVisualStyleBackColor = false;
		this.printAgreeButton.Click += new System.EventHandler(printPlanButton_Click);
		this.dr_name.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
		this.dr_name.Location = new System.Drawing.Point(275, 5);
		this.dr_name.Name = "dr_name";
		this.dr_name.ReadOnly = true;
		this.dr_name.Size = new System.Drawing.Size(80, 19);
		this.dr_name.TabIndex = 22;
		this.dr_name.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.dept.FormattingEnabled = true;
		this.dept.Location = new System.Drawing.Point(88, 53);
		this.dept.Name = "dept";
		this.dept.Size = new System.Drawing.Size(109, 20);
		this.dept.TabIndex = 3;
		this.dept.Leave += new System.EventHandler(dept_Leave);
		this.label19.AutoSize = true;
		this.label19.Location = new System.Drawing.Point(8, 56);
		this.label19.Name = "label19";
		this.label19.Size = new System.Drawing.Size(41, 12);
		this.label19.TabIndex = 28;
		this.label19.Text = "診療科";
		this.tmpAgreeButton.Location = new System.Drawing.Point(242, 36);
		this.tmpAgreeButton.Name = "tmpAgreeButton";
		this.tmpAgreeButton.Size = new System.Drawing.Size(128, 25);
		this.tmpAgreeButton.TabIndex = 12;
		this.tmpAgreeButton.Text = "同意書テンプレート";
		this.tmpAgreeButton.UseVisualStyleBackColor = true;
		this.tmpAgreeButton.Click += new System.EventHandler(tmpPlanButton_Click);
		this.save_date.Format = System.Windows.Forms.DateTimePickerFormat.Short;
		this.save_date.Location = new System.Drawing.Point(88, 28);
		this.save_date.Name = "save_date";
		this.save_date.Size = new System.Drawing.Size(109, 19);
		this.save_date.TabIndex = 2;
		this.save_date.Value = new System.DateTime(2007, 6, 6, 0, 0, 0, 0);
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(8, 31);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(41, 12);
		this.label3.TabIndex = 27;
		this.label3.Text = "作成日";
		this.newAgreeButton.Location = new System.Drawing.Point(15, 36);
		this.newAgreeButton.Name = "newAgreeButton";
		this.newAgreeButton.Size = new System.Drawing.Size(98, 25);
		this.newAgreeButton.TabIndex = 10;
		this.newAgreeButton.Text = "新規作成";
		this.newAgreeButton.UseVisualStyleBackColor = true;
		this.newAgreeButton.Click += new System.EventHandler(newPlanButton_Click);
		this.Agree_id.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
		this.Agree_id.Location = new System.Drawing.Point(444, 8);
		this.Agree_id.Name = "Agree_id";
		this.Agree_id.ReadOnly = true;
		this.Agree_id.Size = new System.Drawing.Size(43, 19);
		this.Agree_id.TabIndex = 24;
		this.Agree_id.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.Agree_id.Visible = false;
		this.label20.AutoSize = true;
		this.label20.Location = new System.Drawing.Point(410, 11);
		this.label20.Name = "label20";
		this.label20.Size = new System.Drawing.Size(29, 12);
		this.label20.TabIndex = 23;
		this.label20.Text = "番号";
		this.label20.Visible = false;
		this.panel1.AutoScroll = true;
		this.panel1.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
		this.panel1.Controls.Add(this.label11);
		this.panel1.Controls.Add(this.anes);
		this.panel1.Controls.Add(this.bothEye);
		this.panel1.Controls.Add(this.leftEye);
		this.panel1.Controls.Add(this.rightEye);
		this.panel1.Controls.Add(this.label13);
		this.panel1.Controls.Add(this.sheetName);
		this.panel1.Controls.Add(this.label14);
		this.panel1.Controls.Add(this.eye);
		this.panel1.Controls.Add(this.item4);
		this.panel1.Controls.Add(this.label22);
		this.panel1.Controls.Add(this.label3);
		this.panel1.Controls.Add(this.save_date);
		this.panel1.Controls.Add(this.label20);
		this.panel1.Controls.Add(this.Agree_id);
		this.panel1.Controls.Add(this.label19);
		this.panel1.Controls.Add(this.dept);
		this.panel1.Controls.Add(this.dr_name);
		this.panel1.Controls.Add(this.label15);
		this.panel1.Controls.Add(this.item3);
		this.panel1.Controls.Add(this.label12);
		this.panel1.Controls.Add(this.item2);
		this.panel1.Controls.Add(this.label10);
		this.panel1.Controls.Add(this.item1);
		this.panel1.Controls.Add(this.label9);
		this.panel1.Controls.Add(this.explanation);
		this.panel1.Controls.Add(this.label8);
		this.panel1.Controls.Add(this.ope);
		this.panel1.Controls.Add(this.label7);
		this.panel1.Controls.Add(this.diag);
		this.panel1.Controls.Add(this.staff);
		this.panel1.Controls.Add(this.dr_id);
		this.panel1.Controls.Add(this.label6);
		this.panel1.Controls.Add(this.label5);
		this.panel1.Controls.Add(this.label4);
		this.panel1.Location = new System.Drawing.Point(15, 213);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(575, 486);
		this.panel1.TabIndex = 3;
		this.label11.AutoSize = true;
		this.label11.Location = new System.Drawing.Point(8, 132);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(63, 12);
		this.label11.TabIndex = 67;
		this.label11.Text = "麻酔の形式";
		this.anes.Location = new System.Drawing.Point(88, 129);
		this.anes.MaxLength = 200;
		this.anes.Name = "anes";
		this.anes.Size = new System.Drawing.Size(464, 19);
		this.anes.TabIndex = 66;
		this.bothEye.AutoSize = true;
		this.bothEye.Location = new System.Drawing.Point(526, 35);
		this.bothEye.Name = "bothEye";
		this.bothEye.Size = new System.Drawing.Size(36, 16);
		this.bothEye.TabIndex = 65;
		this.bothEye.Text = "両";
		this.bothEye.UseVisualStyleBackColor = true;
		this.bothEye.Visible = false;
		this.leftEye.AutoSize = true;
		this.leftEye.Location = new System.Drawing.Point(526, 55);
		this.leftEye.Name = "leftEye";
		this.leftEye.Size = new System.Drawing.Size(36, 16);
		this.leftEye.TabIndex = 64;
		this.leftEye.Text = "左";
		this.leftEye.UseVisualStyleBackColor = true;
		this.leftEye.Visible = false;
		this.rightEye.AutoSize = true;
		this.rightEye.Location = new System.Drawing.Point(485, 56);
		this.rightEye.Name = "rightEye";
		this.rightEye.Size = new System.Drawing.Size(36, 16);
		this.rightEye.TabIndex = 63;
		this.rightEye.Text = "右";
		this.rightEye.UseVisualStyleBackColor = true;
		this.rightEye.Visible = false;
		this.label13.AutoSize = true;
		this.label13.Location = new System.Drawing.Point(225, 35);
		this.label13.Name = "label13";
		this.label13.Size = new System.Drawing.Size(33, 12);
		this.label13.TabIndex = 62;
		this.label13.Text = "シート";
		this.sheetName.FormattingEnabled = true;
		this.sheetName.Items.AddRange(new object[4] { "日帰り", "入院", "短期滞在", "検査同意書" });
		this.sheetName.Location = new System.Drawing.Point(275, 31);
		this.sheetName.MaxLength = 50;
		this.sheetName.Name = "sheetName";
		this.sheetName.Size = new System.Drawing.Size(160, 20);
		this.sheetName.TabIndex = 33;
		this.label14.AutoSize = true;
		this.label14.Location = new System.Drawing.Point(7, 418);
		this.label14.Name = "label14";
		this.label14.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.label14.Size = new System.Drawing.Size(53, 12);
		this.label14.TabIndex = 8;
		this.label14.Text = "手術内容";
		this.eye.FormattingEnabled = true;
		this.eye.Items.AddRange(new object[3] { "右", "左", "両" });
		this.eye.Location = new System.Drawing.Point(87, 79);
		this.eye.MaxLength = 40;
		this.eye.Name = "eye";
		this.eye.Size = new System.Drawing.Size(109, 20);
		this.eye.TabIndex = 4;
		this.item4.Location = new System.Drawing.Point(87, 415);
		this.item4.MaxLength = 500;
		this.item4.Multiline = true;
		this.item4.Name = "item4";
		this.item4.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.item4.Size = new System.Drawing.Size(465, 39);
		this.item4.TabIndex = 12;
		this.label22.AutoSize = true;
		this.label22.Font = new System.Drawing.Font("MS UI Gothic", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
		this.label22.Location = new System.Drawing.Point(9, 8);
		this.label22.Name = "label22";
		this.label22.Size = new System.Drawing.Size(77, 14);
		this.label22.TabIndex = 20;
		this.label22.Text = "医師記入欄";
		this.tmpStaffButton.Location = new System.Drawing.Point(376, 36);
		this.tmpStaffButton.Name = "tmpStaffButton";
		this.tmpStaffButton.Size = new System.Drawing.Size(138, 25);
		this.tmpStaffButton.TabIndex = 13;
		this.tmpStaffButton.Text = "スタッフテンプレート";
		this.tmpStaffButton.UseVisualStyleBackColor = true;
		this.tmpStaffButton.Click += new System.EventHandler(tmpStaffButton_Click);
		this.findAgreeButton.Location = new System.Drawing.Point(119, 36);
		this.findAgreeButton.Name = "findAgreeButton";
		this.findAgreeButton.Size = new System.Drawing.Size(117, 25);
		this.findAgreeButton.TabIndex = 11;
		this.findAgreeButton.Text = "同意書検索";
		this.findAgreeButton.UseVisualStyleBackColor = true;
		this.findAgreeButton.Click += new System.EventHandler(findPlanButton_Click);
		this.settingButton.Location = new System.Drawing.Point(518, 36);
		this.settingButton.Name = "settingButton";
		this.settingButton.Size = new System.Drawing.Size(70, 25);
		this.settingButton.TabIndex = 14;
		this.settingButton.Text = "設定";
		this.settingButton.UseVisualStyleBackColor = true;
		this.settingButton.Click += new System.EventHandler(settingButton_Click);
		this.exportButton.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.exportButton.Location = new System.Drawing.Point(478, 62);
		this.exportButton.Name = "exportButton";
		this.exportButton.Size = new System.Drawing.Size(110, 25);
		this.exportButton.TabIndex = 15;
		this.exportButton.Text = "CSVエクスポート";
		this.exportButton.UseVisualStyleBackColor = false;
		this.exportButton.Visible = false;
		this.exportButton.Click += new System.EventHandler(exportButton_Click);
		this.importButton.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.importButton.Location = new System.Drawing.Point(478, 88);
		this.importButton.Name = "importButton";
		this.importButton.Size = new System.Drawing.Size(110, 25);
		this.importButton.TabIndex = 16;
		this.importButton.Text = "CSVインポート";
		this.importButton.UseVisualStyleBackColor = false;
		this.importButton.Visible = false;
		this.importButton.Click += new System.EventHandler(importButton_Click);
		this.pt_kana.BackColor = System.Drawing.SystemColors.ScrollBar;
		this.pt_kana.Location = new System.Drawing.Point(270, 11);
		this.pt_kana.Name = "pt_kana";
		this.pt_kana.ReadOnly = true;
		this.pt_kana.Size = new System.Drawing.Size(100, 19);
		this.pt_kana.TabIndex = 60;
		this.pt_sex.BackColor = System.Drawing.SystemColors.ScrollBar;
		this.pt_sex.Location = new System.Drawing.Point(376, 11);
		this.pt_sex.Name = "pt_sex";
		this.pt_sex.ReadOnly = true;
		this.pt_sex.Size = new System.Drawing.Size(26, 19);
		this.pt_sex.TabIndex = 61;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
        this.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		base.ClientSize = new System.Drawing.Size(604, 835);
		base.Controls.Add(this.pt_sex);
		base.Controls.Add(this.pt_kana);
		base.Controls.Add(this.exportButton);
		base.Controls.Add(this.importButton);
		base.Controls.Add(this.settingButton);
		base.Controls.Add(this.findAgreeButton);
		base.Controls.Add(this.tmpStaffButton);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.newAgreeButton);
		base.Controls.Add(this.tmpAgreeButton);
		base.Controls.Add(this.printAgreeButton);
		base.Controls.Add(this.closeButton);
		base.Controls.Add(this.regAgreeButton);
		base.Controls.Add(this.agreePlanListLabel);
		base.Controls.Add(this.delAgreeButton);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.AgreeList);
		base.Controls.Add(this.pt_name);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.pt_id);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "Form1";
		this.Text = "同意書";
		((System.ComponentModel.ISupportInitialize)this.AgreeList).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public Form1()
	{
		InitializeComponent();
		System.Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		this.Text = $"眼科同意書v{version.Major}.{version.Minor}.{version.Build}";
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
		agreePlanListLabel.Text = "患者番号を入力して Enter を押すと既存の同意書が表示されます";
		initShow();
	}

	private void initShow()
	{
		clearPlan();
		readPatCsv();
		printAgreeButton.Enabled = false;
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

	private void showList()
	{
		if (Program.OfflineMode)
		{
			return;
		}
		if (pt_id.Text.Length > 0)
		{
			clearPlan();
			oraConn.Open();
			oraCmd.CommandText = "Select P_NAME,P_KANA,P_SEX from M_PATIENT" + Env.DB_LINK + " where P_ID = " + pt_id.Text.Trim();
			oraReader = oraCmd.ExecuteReader();
			string selectCommandText = "Select AGREE_ID, SAVE_DATE, AGREE.DEPT, Trim(M_DEPT.S_NAME), AGREE.DR, Trim(M_USR.NAME), '' ,  STAFF, EYE,  DIAG,  OPE, EXPLANATION,ITEM1,ITEM2,ITEM3,ITEM4, DR_OK ,SHEET_NAME,'',case when DR_OK = 1 then '○' else '-' end as 医師完了 , ANES  from AGREE inner join M_DEPT" + Env.DB_LINK + " on AGREE.DEPT = M_DEPT.CODE inner join M_USR" + Env.DB_LINK + " on AGREE.DR = M_USR.CODE where PATIENT_ID = " + pt_id.Text.Trim() + " and DELETE_FLAG = 0 order by SAVE_DATE desc";
			OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(selectCommandText, oraConn);
			if (oraReader.Read())
			{
				pt_name.Text = oraReader[0].ToString();
				pt_kana.Text = oraReader[1].ToString();
				if (oraReader[2].ToString() == "2")
				{
					pt_sex.Text = "女";
				}
				else
				{
					pt_sex.Text = "男";
				}
			}
			oraReader.Close();
			oraConn.Close();
			DataSet dataSet = new DataSet();
			oleDbDataAdapter.Fill(dataSet, "同意書");
			AgreeList.DataSource = dataSet.Tables["同意書"];
			AgreeList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			AgreeList.Columns[0].Visible = false;
			AgreeList.Columns[1].HeaderText = "作成日";
			AgreeList.Columns[1].Width = 80;
			AgreeList.Columns[1].DefaultCellStyle.Format = "0000/00/00";
			AgreeList.Columns[2].Visible = false;
			AgreeList.Columns[3].HeaderText = "診療科";
			AgreeList.Columns[3].Width = 80;
			AgreeList.Columns[4].Visible = false;
			AgreeList.Columns[5].HeaderText = "医師";
			AgreeList.Columns[5].Width = 80;
			AgreeList.Columns[6].Visible = false;
			AgreeList.Columns[7].Visible = false;
			AgreeList.Columns[8].HeaderText = "眼";
			AgreeList.Columns[8].Width = 40;
			AgreeList.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			AgreeList.Columns[9].Visible = false;
			AgreeList.Columns[10].HeaderText = "手術";
			AgreeList.Columns[10].Width = 150;
			AgreeList.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			AgreeList.Columns[11].Visible = false;
			AgreeList.Columns[12].Visible = false;
			AgreeList.Columns[13].Visible = false;
			AgreeList.Columns[14].Visible = false;
			AgreeList.Columns[15].Visible = false;
			AgreeList.Columns[16].Visible = false;
			AgreeList.Columns[17].Visible = false;
			AgreeList.Columns[18].Visible = false;
			AgreeList.Columns[19].Visible = false;
			AgreeList.Columns[20].Visible = false;
			clearPlan();
			printAgreeButton.Enabled = false;
			if (AgreeList.RowCount > 0)
			{
				agreePlanListLabel.Text = "同意書をクリックすると内容が表示されます。";
			}
			else
			{
				agreePlanListLabel.Text = "既存の同意書はありません。新規作成してください。";
			}
		}
		else
		{
			printAgreeButton.Enabled = false;
		}
	}

	private void showPlan(int rowIndex)
	{
		if (Program.OfflineMode)
		{
			return;
		}
		try
		{
			if (rowIndex >= 0)
			{
				Agree_id.Text = AgreeList.Rows[rowIndex].Cells[0].Value.ToString().Trim();
				if (Dict.StaffDict.ContainsKey(AgreeList.Rows[rowIndex].Cells[4].Value.ToString().Trim()))
				{
					dr_id.Text = AgreeList.Rows[rowIndex].Cells[4].Value.ToString().Trim();
					dr_name.Text = Dict.StaffDict[dr_id.Text].Name;
				}
				else
				{
					dr_id.Text = "";
					dr_name.Text = "";
				}
				string text = "";
				if (AgreeList.Rows[rowIndex].Cells[1].Value.ToString().Length == 8)
				{
					text = AgreeList.Rows[rowIndex].Cells[1].Value.ToString();
					save_date.Text = text.Substring(0, 4) + "/" + text.Substring(4, 2) + "/" + text.Substring(6, 2);
				}
				dept.Text = AgreeList.Rows[rowIndex].Cells[2].Value.ToString().Trim() + " " + AgreeList.Rows[rowIndex].Cells[3].Value.ToString().Trim();
				eye.Text = AgreeList.Rows[rowIndex].Cells[8].Value.ToString().Trim();
				sheetName.Text = AgreeList.Rows[rowIndex].Cells[17].Value.ToString().Trim();
				staff.Text = AgreeList.Rows[rowIndex].Cells[7].Value.ToString().Trim();
				diag.Text = AgreeList.Rows[rowIndex].Cells[9].Value.ToString().Trim();
				anes.Text = AgreeList.Rows[rowIndex].Cells[20].Value.ToString().Trim();
				ope.Text = AgreeList.Rows[rowIndex].Cells[10].Value.ToString().Trim();
				explanation.Text = AgreeList.Rows[rowIndex].Cells[11].Value.ToString().Trim();
				item1.Text = AgreeList.Rows[rowIndex].Cells[12].Value.ToString().Trim();
				item2.Text = AgreeList.Rows[rowIndex].Cells[13].Value.ToString().Trim();
				item3.Text = AgreeList.Rows[rowIndex].Cells[14].Value.ToString().Trim();
				item4.Text = AgreeList.Rows[rowIndex].Cells[15].Value.ToString().Trim();
				if (AgreeList.Rows[rowIndex].Cells[16].Value.ToString().Trim() == "1")
				{
					staff1_ok = true;
				}
				else
				{
					staff1_ok = false;
				}
				printAgreeButton.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			MessageBox.Show(message);
		}
	}

	public void showPlan(string planId, string ptId)
	{
		int num = -1;
		pt_id.Text = ptId;
		showList();
		for (int i = 0; i < AgreeList.Rows.Count; i++)
		{
			if (planId == AgreeList.Rows[i].Cells[0].Value.ToString())
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			AgreeList.Rows[num].Selected = true;
			showPlan(num);
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

	private void copyNewMenuItem_Click(object sender, EventArgs e)
	{
		if (Program.OfflineMode)
		{
			return;
		}
		if (AgreeList.SelectedRows.Count == 0)
		{
			return;
		}
		copyAsNew(AgreeList.SelectedRows[0].Index);
	}

	private void copyAsNew(int rowIndex)
	{
		showPlan(rowIndex);
		Agree_id.Text = "";
		staff1_ok = true;
		if (int.TryParse(patCont[9], out int drCode))
		{
			dr_id.Text = drCode.ToString();
			dr_name.Text = patCont[10];
		}
		else
		{
			dr_id.Text = "";
			dr_name.Text = "";
		}
		save_date.Value = DateTime.Today;
		if (regPlan() != -1)
		{
			showList();
			MessageBox.Show("コピーして作成しました");
		}
	}

	private void closeButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void regPlanButton_Click(object sender, EventArgs e)
	{
		switch (regPlan())
		{
		case 0:
			if (MessageBox.Show("登録が完了しました。印刷しますか？", "完了", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				printAgree();
			}
			showList();
			break;
		case 1:
			MessageBox.Show("登録が完了しました");
			showList();
			break;
		}
	}

	private void delPlanButton_Click(object sender, EventArgs e)
	{
		if (delPlan() == 0)
		{
			showList();
			MessageBox.Show("削除しました");
		}
	}

	private int regPlan()
	{
		if (Program.OfflineMode)
		{
			MessageBox.Show("オフラインモードのため登録できません");
			return -1;
		}
		if (pt_id.Text.Length == 0)
		{
			MessageBox.Show("患者番号を入力してください");
			return -1;
		}
		if (dr_id.Text.Length == 0)
		{
			MessageBox.Show("主治医を入力してください");
			return -1;
		}
		if (dept.Text.Length == 0)
		{
			MessageBox.Show("診療科を入力してください");
			return -1;
		}
		string text = "";
		string text2 = "";
		string text3 = save_date.Value.ToString("yyyyMMdd");
		text2 = ((!staff1_ok) ? "0" : "1");
		text = ((Agree_id.Text.Length <= 0) ? ("insert into AGREE (AGREE_ID, PATIENT_ID, SAVE_DATE, DEPT, DR, STAFF, EYE, DIAG, ANES, OPE, EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DR_OK,  DELETE_FLAG, SAVE_TIME )  values ( AGREE_SEQ.nextval, " + pt_id.Text + " , " + text3 + " , " + dept.Text.Split(' ')[0] + " , " + dr_id.Text + " , " + AgreeSql.SqlValue(staff.Text) + " , " + AgreeSql.SqlValue(eye.Text) + ", " + AgreeSql.SqlValue(diag.Text) + ", " + AgreeSql.SqlValue(anes.Text) + ", " + AgreeSql.SqlValue(ope.Text) + ", " + AgreeSql.SqlValue(explanation.Text) + ", " + AgreeSql.SqlValue(item1.Text) + ", " + AgreeSql.SqlValue(item2.Text) + ", " + AgreeSql.SqlValue(item3.Text) + ", " + AgreeSql.SqlValue(item4.Text) + ", " + AgreeSql.SqlValue(sheetName.Text) + "," + text2 + ",0," + DateTime.Now.ToString("HHmmss") + ")") : ("update AGREE set SAVE_DATE = " + text3 + ", DEPT = " + dept.Text.Split(' ')[0].Trim() + ", DR = " + dr_id.Text.Trim() + ", STAFF = " + AgreeSql.SqlValue(staff.Text) + ", EYE = " + AgreeSql.SqlValue(eye.Text) + ", DIAG = " + AgreeSql.SqlValue(diag.Text) + ", ANES = " + AgreeSql.SqlValue(anes.Text) + ", OPE = " + AgreeSql.SqlValue(ope.Text) + ", EXPLANATION = " + AgreeSql.SqlValue(explanation.Text) + ", ITEM1 = " + AgreeSql.SqlValue(item1.Text) + ", ITEM2 = " + AgreeSql.SqlValue(item2.Text) + ", ITEM3 = " + AgreeSql.SqlValue(item3.Text) + ", ITEM4 = " + AgreeSql.SqlValue(item4.Text) + ", SHEET_NAME = " + AgreeSql.SqlValue(sheetName.Text) + ", DR_OK = " + text2 + ", SAVE_TIME = " + DateTime.Now.ToString("HHmmss") + " where AGREE_ID = " + Agree_id.Text));
		oraConn.Open();
		oraCmd.CommandText = text;
		oraCmd.ExecuteNonQuery();
		oraConn.Close();
		if (staff1_ok)
		{
			return 0;
		}
		return 1;
	}

	private int delPlan()
	{
		if (Program.OfflineMode)
		{
			MessageBox.Show("オフラインモードのため削除できません");
			return -1;
		}
		if (MessageBox.Show("削除しますか？", "削除", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
		{
			if (Agree_id.Text.Length > 0)
			{
				oraConn.Open();
				oraCmd.CommandText = "update AGREE set DELETE_FLAG = 1 where AGREE_ID = " + Agree_id.Text;
				oraCmd.ExecuteNonQuery();
				oraConn.Close();
			}
			else
			{
				clearPlan();
			}
			return 0;
		}
		return -1;
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

	private void printPlanButton_Click(object sender, EventArgs e)
	{
		printAgree();
	}

	private void printAgree()
	{
		ExcelControl excelControl = new ExcelControl();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["3, 2"] = pt_id.Text;
		dictionary["3, 3"] = pt_kana.Text;
		dictionary["3, 4"] = pt_name.Text;
		if (pt_sex.Equals("2"))
		{
			dictionary["3, 7"] = "女";
		}
		else
		{
			dictionary["3, 7"] = "男";
		}
		dictionary["5, 2"] = dept.Text.Split(' ')[0];
		dictionary["5,3"] = dept.Text.Split(' ')[1];
		string path = Env.LEGACY_HOME + "\\Pat.csv";
		string text = "";
		string value = "";
		if (File.Exists(path))
		{
			StreamReader streamReader = new StreamReader(path, Encoding.Default);
			string text2;
			if ((text2 = streamReader.ReadLine()) != null)
			{
				string[] fields = text2.Split(',');
				for (int i = 0; i < fields.Length && i < 50; i++)
				{
					patCont[i] = fields[i];
				}
				text = patCont[9];
				value = patCont[10];
			}
			streamReader.Dispose();
			dictionary["7, 2"] = text.PadLeft(5, '0');
			dictionary["7, 3"] = value;
		}
		else
		{
			dictionary["7,2"] = dr_id.Text.PadLeft(5, '0');
			dictionary["7, 3"] = dr_name.Text;
		}
		dictionary["27, 2"] = dr_name.Text;
		dictionary["28, 2"] = eye.Text;
		dictionary["28, 4"] = staff.Text;
		dictionary["29, 2"] = ope.Text;
		dictionary["30, 2"] = diag.Text;
		dictionary["30, 4"] = anes.Text;
		dictionary["31, 2"] = explanation.Text;
		dictionary["32, 2"] = item1.Text;
		dictionary["33, 2"] = item2.Text;
		dictionary["34, 2"] = item3.Text;
		dictionary["35, 2"] = item4.Text;
		excelControl.ValueList = dictionary;
		excelControl.MakeEyeAgree(sheetName.Text);
		excelControl.ReleaseExcel();
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

	public void applyTemplate(int temp_id)
	{
		if (Program.OfflineMode)
		{
			return;
		}
		oraConn.Open();
		oraCmd.CommandText = "select * from AGREE_TEMPLATE where TEMP_ID = " + temp_id;
		oraReader = oraCmd.ExecuteReader();
		if (oraReader.Read())
		{
			if (eye.Text.Length > 0)
			{
				ComboBox comboBox = eye;
				comboBox.Text = comboBox.Text + " " + oraReader["EYE"].ToString();
			}
			else
			{
				eye.Text = oraReader["EYE"].ToString();
			}
			if (diag.Text.Length > 0)
			{
				TextBox textBox = diag;
				textBox.Text = textBox.Text + " " + oraReader["DIAG"].ToString();
			}
			else
			{
				diag.Text = oraReader["DIAG"].ToString();
			}
			if (anes.Text.Length > 0)
			{
				TextBox textBox2 = anes;
				textBox2.Text = textBox2.Text + " " + oraReader["ANES"].ToString();
			}
			else
			{
				anes.Text = oraReader["ANES"].ToString();
			}
			if (ope.Text.Length > 0)
			{
				TextBox textBox3 = ope;
				textBox3.Text = textBox3.Text + " " + oraReader["OPE"].ToString();
			}
			else
			{
				ope.Text = oraReader["OPE"].ToString();
			}
			if (explanation.Text.Length > 0)
			{
				TextBox textBox4 = explanation;
				textBox4.Text = textBox4.Text + " " + oraReader["EXPLANATION"].ToString();
			}
			else
			{
				explanation.Text = oraReader["EXPLANATION"].ToString();
			}
			if (item1.Text.Length > 0)
			{
				TextBox textBox5 = item1;
				textBox5.Text = textBox5.Text + " " + oraReader["ITEM1"].ToString();
			}
			else
			{
				item1.Text = oraReader["ITEM1"].ToString();
			}
			if (item2.Text.Length > 0)
			{
				TextBox textBox6 = item2;
				textBox6.Text = textBox6.Text + " " + oraReader["ITEM2"].ToString();
			}
			else
			{
				item2.Text = oraReader["ITEM2"].ToString();
			}
			if (item3.Text.Length > 0)
			{
				TextBox textBox7 = item3;
				textBox7.Text = textBox7.Text + " " + oraReader["ITEM3"].ToString();
			}
			else
			{
				item3.Text = oraReader["ITEM3"].ToString();
			}
			if (item4.Text.Length > 0)
			{
				TextBox textBox8 = item4;
				textBox8.Text = textBox8.Text + " " + oraReader["ITEM4"].ToString();
			}
			else
			{
				item4.Text = oraReader["ITEM4"].ToString();
			}
			sheetName.Text = oraReader["SHEET_NAME"].ToString();
		}
		oraReader.Close();
		oraConn.Close();
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
		eye.Text = "右";
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
		if (oraReader.Read() && (staff.Text.Length == 0 || MessageBox.Show("スタッフが既に入力されています。上書きしますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes))
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

	private void findPlanButton_Click(object sender, EventArgs e)
	{
		FindAgree findAgree = new FindAgree(this);
		findAgree.Show();
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
