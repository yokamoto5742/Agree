using System.ComponentModel;
using System.Windows.Forms;

namespace Agree;

partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pt_id = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pt_name = new System.Windows.Forms.TextBox();
            this.AgreeList = new System.Windows.Forms.DataGridView();
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
            this.settingButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.pt_kana = new System.Windows.Forms.TextBox();
            this.pt_sex = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.AgreeList)).BeginInit();
            this.agreeListMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pt_id
            // 
            this.pt_id.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.pt_id.Location = new System.Drawing.Point(75, 11);
            this.pt_id.Name = "pt_id";
            this.pt_id.Size = new System.Drawing.Size(77, 19);
            this.pt_id.TabIndex = 0;
            this.pt_id.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pt_id.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pt_id_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "患者ID";
            // 
            // pt_name
            // 
            this.pt_name.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.pt_name.Location = new System.Drawing.Point(158, 11);
            this.pt_name.Name = "pt_name";
            this.pt_name.ReadOnly = true;
            this.pt_name.Size = new System.Drawing.Size(100, 19);
            this.pt_name.TabIndex = 15;
            // 
            // AgreeList
            // 
            this.AgreeList.AllowUserToAddRows = false;
            this.AgreeList.AllowUserToDeleteRows = false;
            this.AgreeList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AgreeList.ContextMenuStrip = this.agreeListMenu;
            this.AgreeList.Location = new System.Drawing.Point(15, 83);
            this.AgreeList.MultiSelect = false;
            this.AgreeList.Name = "AgreeList";
            this.AgreeList.ReadOnly = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.AgreeList.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.AgreeList.RowHeadersVisible = false;
            this.AgreeList.RowHeadersWidth = 51;
            this.AgreeList.RowTemplate.Height = 21;
            this.AgreeList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.AgreeList.Size = new System.Drawing.Size(575, 122);
            this.AgreeList.TabIndex = 2;
            this.AgreeList.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.AgreeList_CellMouseDown);
            this.AgreeList.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.agreePlanList_RowEnter);
            // 
            // agreeListMenu
            // 
            this.agreeListMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.agreeListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyNewMenuItem});
            this.agreeListMenu.Name = "agreeListMenu";
            this.agreeListMenu.Size = new System.Drawing.Size(157, 26);
            // 
            // copyNewMenuItem
            // 
            this.copyNewMenuItem.Name = "copyNewMenuItem";
            this.copyNewMenuItem.Size = new System.Drawing.Size(156, 22);
            this.copyNewMenuItem.Text = "コピーして作成";
            this.copyNewMenuItem.Click += new System.EventHandler(this.copyNewMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 15;
            this.label2.Text = "同意書一覧";
            // 
            // delAgreeButton
            // 
            this.delAgreeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.delAgreeButton.Location = new System.Drawing.Point(346, 755);
            this.delAgreeButton.Name = "delAgreeButton";
            this.delAgreeButton.Size = new System.Drawing.Size(75, 25);
            this.delAgreeButton.TabIndex = 5;
            this.delAgreeButton.Text = "削除";
            this.delAgreeButton.UseVisualStyleBackColor = false;
            this.delAgreeButton.Click += new System.EventHandler(this.delPlanButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 29;
            this.label4.Text = "眼";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(163, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 21;
            this.label5.Text = "入力者";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(273, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 26;
            this.label6.Text = "担当者";
            // 
            // dr_id
            // 
            this.dr_id.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dr_id.Location = new System.Drawing.Point(210, 3);
            this.dr_id.MaxLength = 5;
            this.dr_id.Name = "dr_id";
            this.dr_id.Size = new System.Drawing.Size(48, 19);
            this.dr_id.TabIndex = 1;
            this.dr_id.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.dr_id.KeyDown += new System.Windows.Forms.KeyEventHandler(this.staff1_id_KeyDown);
            this.dr_id.Leave += new System.EventHandler(this.staff1_id_Leave);
            // 
            // staff
            // 
            this.staff.Location = new System.Drawing.Point(275, 79);
            this.staff.MaxLength = 120;
            this.staff.Name = "staff";
            this.staff.Size = new System.Drawing.Size(277, 19);
            this.staff.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 30;
            this.label7.Text = "病名";
            // 
            // diag
            // 
            this.diag.Location = new System.Drawing.Point(88, 104);
            this.diag.MaxLength = 200;
            this.diag.Name = "diag";
            this.diag.Size = new System.Drawing.Size(464, 19);
            this.diag.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 157);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 12);
            this.label8.TabIndex = 31;
            this.label8.Text = "手術・検査名";
            // 
            // ope
            // 
            this.ope.Location = new System.Drawing.Point(88, 154);
            this.ope.MaxLength = 200;
            this.ope.Multiline = true;
            this.ope.Name = "ope";
            this.ope.Size = new System.Drawing.Size(464, 19);
            this.ope.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 179);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 32;
            this.label9.Text = "説明";
            // 
            // explanation
            // 
            this.explanation.Location = new System.Drawing.Point(88, 179);
            this.explanation.MaxLength = 1200;
            this.explanation.Multiline = true;
            this.explanation.Name = "explanation";
            this.explanation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.explanation.Size = new System.Drawing.Size(464, 220);
            this.explanation.TabIndex = 8;
            this.explanation.TextChanged += new System.EventHandler(this.explanation_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 405);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 6;
            this.label10.Text = "症状";
            // 
            // item1
            // 
            this.item1.Location = new System.Drawing.Point(88, 405);
            this.item1.MaxLength = 200;
            this.item1.Multiline = true;
            this.item1.Name = "item1";
            this.item1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.item1.Size = new System.Drawing.Size(464, 32);
            this.item1.TabIndex = 9;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 443);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 3;
            this.label12.Text = "治療計画";
            // 
            // item2
            // 
            this.item2.Location = new System.Drawing.Point(87, 443);
            this.item2.MaxLength = 200;
            this.item2.Multiline = true;
            this.item2.Name = "item2";
            this.item2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.item2.Size = new System.Drawing.Size(464, 29);
            this.item2.TabIndex = 10;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 482);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 12);
            this.label15.TabIndex = 7;
            this.label15.Text = "検査内容";
            this.label15.Visible = false;
            // 
            // item3
            // 
            this.item3.Location = new System.Drawing.Point(88, 478);
            this.item3.MaxLength = 200;
            this.item3.Multiline = true;
            this.item3.Name = "item3";
            this.item3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.item3.Size = new System.Drawing.Size(464, 41);
            this.item3.TabIndex = 11;
            this.item3.Visible = false;
            // 
            // agreePlanListLabel
            // 
            this.agreePlanListLabel.AutoSize = true;
            this.agreePlanListLabel.Location = new System.Drawing.Point(130, 67);
            this.agreePlanListLabel.Name = "agreePlanListLabel";
            this.agreePlanListLabel.Size = new System.Drawing.Size(307, 12);
            this.agreePlanListLabel.TabIndex = 15;
            this.agreePlanListLabel.Text = "患者IDを入力して Enter を押すと既存の同意書が表示されます";
            this.agreePlanListLabel.Click += new System.EventHandler(this.agreePlanListLabel_Click);
            // 
            // regAgreeButton
            // 
            this.regAgreeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.regAgreeButton.Location = new System.Drawing.Point(265, 755);
            this.regAgreeButton.Name = "regAgreeButton";
            this.regAgreeButton.Size = new System.Drawing.Size(75, 25);
            this.regAgreeButton.TabIndex = 4;
            this.regAgreeButton.Text = "登録";
            this.regAgreeButton.UseVisualStyleBackColor = false;
            this.regAgreeButton.Click += new System.EventHandler(this.regPlanButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.closeButton.Location = new System.Drawing.Point(508, 755);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 25);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "閉じる";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // printAgreeButton
            // 
            this.printAgreeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.printAgreeButton.Location = new System.Drawing.Point(427, 755);
            this.printAgreeButton.Name = "printAgreeButton";
            this.printAgreeButton.Size = new System.Drawing.Size(75, 25);
            this.printAgreeButton.TabIndex = 6;
            this.printAgreeButton.Text = "印刷";
            this.printAgreeButton.UseVisualStyleBackColor = false;
            this.printAgreeButton.Click += new System.EventHandler(this.printPlanButton_Click);
            // 
            // dr_name
            // 
            this.dr_name.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.dr_name.Location = new System.Drawing.Point(275, 5);
            this.dr_name.Name = "dr_name";
            this.dr_name.ReadOnly = true;
            this.dr_name.Size = new System.Drawing.Size(80, 19);
            this.dr_name.TabIndex = 22;
            this.dr_name.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // dept
            // 
            this.dept.FormattingEnabled = true;
            this.dept.Location = new System.Drawing.Point(88, 53);
            this.dept.Name = "dept";
            this.dept.Size = new System.Drawing.Size(109, 20);
            this.dept.TabIndex = 3;
            this.dept.Leave += new System.EventHandler(this.dept_Leave);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(8, 56);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(41, 12);
            this.label19.TabIndex = 28;
            this.label19.Text = "診療科";
            // 
            // tmpAgreeButton
            // 
            this.tmpAgreeButton.Location = new System.Drawing.Point(119, 36);
            this.tmpAgreeButton.Name = "tmpAgreeButton";
            this.tmpAgreeButton.Size = new System.Drawing.Size(128, 25);
            this.tmpAgreeButton.TabIndex = 12;
            this.tmpAgreeButton.Text = "同意書テンプレート";
            this.tmpAgreeButton.UseVisualStyleBackColor = true;
            this.tmpAgreeButton.Click += new System.EventHandler(this.tmpPlanButton_Click);
            // 
            // save_date
            // 
            this.save_date.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.save_date.Location = new System.Drawing.Point(88, 28);
            this.save_date.Name = "save_date";
            this.save_date.Size = new System.Drawing.Size(109, 19);
            this.save_date.TabIndex = 2;
            this.save_date.Value = new System.DateTime(2007, 6, 6, 0, 0, 0, 0);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 27;
            this.label3.Text = "作成日";
            // 
            // newAgreeButton
            // 
            this.newAgreeButton.Location = new System.Drawing.Point(15, 36);
            this.newAgreeButton.Name = "newAgreeButton";
            this.newAgreeButton.Size = new System.Drawing.Size(98, 25);
            this.newAgreeButton.TabIndex = 10;
            this.newAgreeButton.Text = "新規作成";
            this.newAgreeButton.UseVisualStyleBackColor = true;
            this.newAgreeButton.Click += new System.EventHandler(this.newPlanButton_Click);
            // 
            // Agree_id
            // 
            this.Agree_id.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Agree_id.Location = new System.Drawing.Point(444, 8);
            this.Agree_id.Name = "Agree_id";
            this.Agree_id.ReadOnly = true;
            this.Agree_id.Size = new System.Drawing.Size(43, 19);
            this.Agree_id.TabIndex = 24;
            this.Agree_id.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Agree_id.Visible = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(410, 11);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(29, 12);
            this.label20.TabIndex = 23;
            this.label20.Text = "番号";
            this.label20.Visible = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
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
            this.panel1.Size = new System.Drawing.Size(575, 536);
            this.panel1.TabIndex = 3;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 132);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 12);
            this.label11.TabIndex = 67;
            this.label11.Text = "麻酔の形式";
            // 
            // anes
            // 
            this.anes.Location = new System.Drawing.Point(88, 129);
            this.anes.MaxLength = 200;
            this.anes.Name = "anes";
            this.anes.Size = new System.Drawing.Size(464, 19);
            this.anes.TabIndex = 66;
            // 
            // bothEye
            // 
            this.bothEye.AutoSize = true;
            this.bothEye.Location = new System.Drawing.Point(526, 35);
            this.bothEye.Name = "bothEye";
            this.bothEye.Size = new System.Drawing.Size(36, 16);
            this.bothEye.TabIndex = 65;
            this.bothEye.Text = "両";
            this.bothEye.UseVisualStyleBackColor = true;
            this.bothEye.Visible = false;
            // 
            // leftEye
            // 
            this.leftEye.AutoSize = true;
            this.leftEye.Location = new System.Drawing.Point(526, 55);
            this.leftEye.Name = "leftEye";
            this.leftEye.Size = new System.Drawing.Size(36, 16);
            this.leftEye.TabIndex = 64;
            this.leftEye.Text = "左";
            this.leftEye.UseVisualStyleBackColor = true;
            this.leftEye.Visible = false;
            // 
            // rightEye
            // 
            this.rightEye.AutoSize = true;
            this.rightEye.Location = new System.Drawing.Point(485, 56);
            this.rightEye.Name = "rightEye";
            this.rightEye.Size = new System.Drawing.Size(36, 16);
            this.rightEye.TabIndex = 63;
            this.rightEye.Text = "右";
            this.rightEye.UseVisualStyleBackColor = true;
            this.rightEye.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(225, 35);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(33, 12);
            this.label13.TabIndex = 62;
            this.label13.Text = "シート";
            // 
            // sheetName
            // 
            this.sheetName.FormattingEnabled = true;
            this.sheetName.Items.AddRange(new object[] {
            "通常",
            "短期滞在",
            "注射",
            "検査同意書"});
            this.sheetName.Location = new System.Drawing.Point(275, 31);
            this.sheetName.MaxLength = 50;
            this.sheetName.Name = "sheetName";
            this.sheetName.Size = new System.Drawing.Size(160, 20);
            this.sheetName.TabIndex = 33;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 481);
            this.label14.Name = "label14";
            this.label14.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label14.Size = new System.Drawing.Size(53, 12);
            this.label14.TabIndex = 8;
            this.label14.Text = "手術内容";
            // 
            // eye
            // 
            this.eye.FormattingEnabled = true;
            this.eye.Items.AddRange(new object[] {
            "右",
            "左",
            "両"});
            this.eye.Location = new System.Drawing.Point(87, 79);
            this.eye.MaxLength = 40;
            this.eye.Name = "eye";
            this.eye.Size = new System.Drawing.Size(109, 20);
            this.eye.TabIndex = 4;
            // 
            // item4
            // 
            this.item4.Location = new System.Drawing.Point(87, 478);
            this.item4.MaxLength = 500;
            this.item4.Multiline = true;
            this.item4.Name = "item4";
            this.item4.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.item4.Size = new System.Drawing.Size(465, 39);
            this.item4.TabIndex = 12;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label22.Location = new System.Drawing.Point(9, 8);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(77, 14);
            this.label22.TabIndex = 20;
            this.label22.Text = "医師記入欄";
            // 
            // tmpStaffButton
            // 
            this.tmpStaffButton.Location = new System.Drawing.Point(253, 36);
            this.tmpStaffButton.Name = "tmpStaffButton";
            this.tmpStaffButton.Size = new System.Drawing.Size(138, 25);
            this.tmpStaffButton.TabIndex = 13;
            this.tmpStaffButton.Text = "担当者テンプレート";
            this.tmpStaffButton.UseVisualStyleBackColor = true;
            this.tmpStaffButton.Click += new System.EventHandler(this.tmpStaffButton_Click);
            // 
            // settingButton
            // 
            this.settingButton.Location = new System.Drawing.Point(518, 36);
            this.settingButton.Name = "settingButton";
            this.settingButton.Size = new System.Drawing.Size(70, 25);
            this.settingButton.TabIndex = 14;
            this.settingButton.Text = "設定";
            this.settingButton.UseVisualStyleBackColor = true;
            this.settingButton.Click += new System.EventHandler(this.settingButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.exportButton.Location = new System.Drawing.Point(478, 62);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(110, 25);
            this.exportButton.TabIndex = 15;
            this.exportButton.Text = "CSVエクスポート";
            this.exportButton.UseVisualStyleBackColor = false;
            this.exportButton.Visible = false;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // importButton
            // 
            this.importButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.importButton.Location = new System.Drawing.Point(478, 88);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(110, 25);
            this.importButton.TabIndex = 16;
            this.importButton.Text = "CSVインポート";
            this.importButton.UseVisualStyleBackColor = false;
            this.importButton.Visible = false;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // pt_kana
            // 
            this.pt_kana.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.pt_kana.Location = new System.Drawing.Point(270, 11);
            this.pt_kana.Name = "pt_kana";
            this.pt_kana.ReadOnly = true;
            this.pt_kana.Size = new System.Drawing.Size(100, 19);
            this.pt_kana.TabIndex = 60;
            // 
            // pt_sex
            // 
            this.pt_sex.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.pt_sex.Location = new System.Drawing.Point(376, 11);
            this.pt_sex.Name = "pt_sex";
            this.pt_sex.ReadOnly = true;
            this.pt_sex.Size = new System.Drawing.Size(26, 19);
            this.pt_sex.TabIndex = 61;
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(604, 799);
            this.Controls.Add(this.pt_sex);
            this.Controls.Add(this.pt_kana);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.settingButton);
            this.Controls.Add(this.tmpStaffButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.newAgreeButton);
            this.Controls.Add(this.tmpAgreeButton);
            this.Controls.Add(this.printAgreeButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.regAgreeButton);
            this.Controls.Add(this.agreePlanListLabel);
            this.Controls.Add(this.delAgreeButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AgreeList);
            this.Controls.Add(this.pt_name);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pt_id);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "眼科同意書";
            ((System.ComponentModel.ISupportInitialize)(this.AgreeList)).EndInit();
            this.agreeListMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

	}
}
