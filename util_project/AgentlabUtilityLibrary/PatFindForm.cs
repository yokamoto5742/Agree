using System;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace AgentlabUtilityLibrary;

public class PatFindForm : Form
{
	private TextBox RetBox;

	private DataSet PtSet = new DataSet();

	private IContainer components;

	private TextBox NameBox;

	private Button SelectButton;

	private DataGridView PatListView;

	private Label label1;

	private Label label2;

	private TextBox KanaBox;

	private Button CloseButton;

	private Button FindButton;

	private Label label3;

	private TextBox SexBox;

	private Label label4;

	private TextBox BirthBox1;

	private TextBox BirthBox2;

	private TextBox BirthBox3;

	private TextBox BirthBox4;

	public PatFindForm(TextBox retBox)
	{
		InitializeComponent();
		RetBox = retBox;
		DataTable dataTable = PtSet.Tables.Add("患者リスト");
		dataTable.Columns.Add("ID");
		dataTable.Columns.Add("氏名");
		dataTable.Columns.Add("カナ");
		dataTable.Columns.Add("P_SEX");
		dataTable.Columns.Add("生年月日");
		dataTable.Columns.Add("P_AGE");
		dataTable.Columns.Add("電話");
		dataTable.Columns.Add("郵便");
		dataTable.Columns.Add("住所");
	}

	private void Find()
	{
		string text = "";
		if (BirthBox1.Text.Length > 0 && BirthBox2.Text.Length > 0 && BirthBox3.Text.Length > 0 && BirthBox4.Text.Length > 0)
		{
			int result = 0;
			if (int.TryParse(BirthBox2.Text, out result))
			{
				if (BirthBox1.Text.Equals("1"))
				{
					text = 1867 + result + BirthBox3.Text.PadLeft(2, '0') + BirthBox4.Text.PadLeft(2, '0');
				}
				else if (BirthBox1.Text.Equals("2"))
				{
					text = 1911 + result + BirthBox3.Text.PadLeft(2, '0') + BirthBox4.Text.PadLeft(2, '0');
				}
				else if (BirthBox1.Text.Equals("3"))
				{
					text = 1925 + result + BirthBox3.Text.PadLeft(2, '0') + BirthBox4.Text.PadLeft(2, '0');
				}
				else if (BirthBox1.Text.Equals("4"))
				{
					text = 1988 + result + BirthBox3.Text.PadLeft(2, '0') + BirthBox4.Text.PadLeft(2, '0');
				}
			}
		}
		if (NameBox.Text.Length == 0 && KanaBox.Text.Length == 0 && text.Length == 0)
		{
			MessageBox.Show("検索条件を入力してください");
			return;
		}
		string text2 = "";
		if (NameBox.Text.Length > 0)
		{
			text2 = "P_NAME like '%" + NameBox.Text + "%'";
		}
		if (KanaBox.Text.Length > 0)
		{
			if (text2.Length > 0)
			{
				text2 += " and ";
			}
			text2 = text2 + "P_KANA like '%" + KanaBox.Text + "%'";
		}
		if (SexBox.Text.Length > 0 && (SexBox.Text.Equals("1") || SexBox.Text.Equals("2")))
		{
			if (text2.Length > 0)
			{
				text2 += " and ";
			}
			text2 = text2 + "P_SEX = " + SexBox.Text;
		}
		if (text.Length > 0)
		{
			if (text2.Length > 0)
			{
				text2 += " and ";
			}
			text2 = text2 + "P_BIRTHDAY_AD = " + text;
		}
		DataTable dataTable = PtSet.Tables["患者リスト"];
		dataTable.Clear();
		OleDbConnection openDBConn = DBConn.GetOpenDBConn();
		openDBConn.Open();
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = openDBConn;
		oleDbCommand.CommandText = "select P_ID as ID, Trim(P_KANA) as カナ, Trim(P_NAME) as 氏名, P_SEX as P_SEX, TEL as 電話, P_BIRTHDAY_AD as 生年月日, POST as 郵便, Trim(ADDR_1) || Trim(ADDR_2) as 住所 from M_PATIENT" + Env.DB_LINK + " where " + text2 + " order by P_ID";
		OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
		while (oleDbDataReader.Read())
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["ID"] = oleDbDataReader["ID"].ToString();
			dataRow["氏名"] = oleDbDataReader["氏名"].ToString();
			dataRow["カナ"] = oleDbDataReader["カナ"].ToString();
			if (oleDbDataReader["P_SEX"].ToString().Equals("1"))
			{
				dataRow["P_SEX"] = "M";
			}
			else if (oleDbDataReader["P_SEX"].ToString().Equals("2"))
			{
				dataRow["P_SEX"] = "F";
			}
			if (oleDbDataReader["生年月日"].ToString().Length == 8)
			{
				dataRow["生年月日"] = oleDbDataReader["生年月日"].ToString().Insert(4, "/").Insert(7, "/");
				dataRow["P_AGE"] = DateConvert.CalcAge(oleDbDataReader["生年月日"].ToString(), DateTime.Now.ToString("yyyyMMdd")).ToString();
			}
			dataRow["電話"] = oleDbDataReader["電話"].ToString();
			dataRow["郵便"] = oleDbDataReader["郵便"].ToString();
			dataRow["住所"] = oleDbDataReader["住所"].ToString();
			dataTable.Rows.Add(dataRow);
		}
		openDBConn.Close();
		PatListView.DataSource = new DataView(dataTable);
		PatListView.Columns["ID"].Width = 60;
		PatListView.Columns["ID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
		PatListView.Columns["氏名"].Width = 75;
		PatListView.Columns["カナ"].Width = 65;
		PatListView.Columns["P_SEX"].Width = 35;
		PatListView.Columns["P_SEX"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		PatListView.Columns["生年月日"].Width = 70;
		PatListView.Columns["生年月日"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		PatListView.Columns["P_AGE"].Width = 35;
		PatListView.Columns["P_AGE"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		PatListView.Columns["電話"].Width = 70;
		PatListView.Columns["郵便"].Width = 55;
		PatListView.Columns["住所"].Width = 120;
	}

	private void FindButton_Click(object sender, EventArgs e)
	{
		Find();
	}

	private void SelectButton_Click(object sender, EventArgs e)
	{
		if (PatListView.SelectedRows.Count > 0)
		{
			RetBox.Text = PatListView.SelectedRows[0].Cells["ID"].Value.ToString();
		}
		Dispose();
	}

	private void CloseButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void PatListView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
	{
		RetBox.Text = PatListView.Rows[e.RowIndex].Cells["ID"].Value.ToString();
		Dispose();
	}

	private void NameBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			KanaBox.Select();
		}
	}

	private void KanaBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			SexBox.Select();
		}
	}

	private void SexBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			BirthBox1.Select();
		}
	}

	private void BirthBox1_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			BirthBox2.Select();
		}
	}

	private void BirthBox2_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			BirthBox3.Select();
		}
	}

	private void BirthBox3_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			BirthBox4.Select();
		}
	}

	private void BirthBox4_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			FindButton.Select();
		}
	}

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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgentlabUtilityLibrary.PatFindForm));
		this.NameBox = new System.Windows.Forms.TextBox();
		this.SelectButton = new System.Windows.Forms.Button();
		this.PatListView = new System.Windows.Forms.DataGridView();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.KanaBox = new System.Windows.Forms.TextBox();
		this.CloseButton = new System.Windows.Forms.Button();
		this.FindButton = new System.Windows.Forms.Button();
		this.label3 = new System.Windows.Forms.Label();
		this.SexBox = new System.Windows.Forms.TextBox();
		this.label4 = new System.Windows.Forms.Label();
		this.BirthBox1 = new System.Windows.Forms.TextBox();
		this.BirthBox2 = new System.Windows.Forms.TextBox();
		this.BirthBox3 = new System.Windows.Forms.TextBox();
		this.BirthBox4 = new System.Windows.Forms.TextBox();
		((System.ComponentModel.ISupportInitialize)this.PatListView).BeginInit();
		base.SuspendLayout();
		this.NameBox.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
		this.NameBox.Location = new System.Drawing.Point(39, 12);
		this.NameBox.MaxLength = 20;
		this.NameBox.Name = "NameBox";
		this.NameBox.Size = new System.Drawing.Size(90, 19);
		this.NameBox.TabIndex = 0;
		this.NameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(NameBox_KeyDown);
		this.SelectButton.Location = new System.Drawing.Point(173, 427);
		this.SelectButton.Name = "SelectButton";
		this.SelectButton.Size = new System.Drawing.Size(66, 23);
		this.SelectButton.TabIndex = 1;
		this.SelectButton.Text = "選択";
		this.SelectButton.UseVisualStyleBackColor = true;
		this.SelectButton.Click += new System.EventHandler(SelectButton_Click);
		this.PatListView.AllowUserToAddRows = false;
		this.PatListView.AllowUserToDeleteRows = false;
		this.PatListView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
		dataGridViewCellStyle.Font = new System.Drawing.Font("MS UI Gothic", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
		dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
		dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.PatListView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
		this.PatListView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
		dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.PatListView.DefaultCellStyle = dataGridViewCellStyle2;
		this.PatListView.Location = new System.Drawing.Point(9, 39);
		this.PatListView.MultiSelect = false;
		this.PatListView.Name = "PatListView";
		this.PatListView.RowHeadersVisible = false;
		this.PatListView.RowTemplate.Height = 21;
		this.PatListView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.PatListView.Size = new System.Drawing.Size(611, 382);
		this.PatListView.TabIndex = 2;
		this.PatListView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(PatListView_CellDoubleClick);
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(7, 15);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(29, 12);
		this.label1.TabIndex = 3;
		this.label1.Text = "氏名";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(140, 15);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(24, 12);
		this.label2.TabIndex = 5;
		this.label2.Text = "カナ";
		this.KanaBox.ImeMode = System.Windows.Forms.ImeMode.KatakanaHalf;
		this.KanaBox.Location = new System.Drawing.Point(165, 12);
		this.KanaBox.MaxLength = 20;
		this.KanaBox.Name = "KanaBox";
		this.KanaBox.Size = new System.Drawing.Size(90, 19);
		this.KanaBox.TabIndex = 1;
		this.KanaBox.KeyDown += new System.Windows.Forms.KeyEventHandler(KanaBox_KeyDown);
		this.CloseButton.Location = new System.Drawing.Point(318, 427);
		this.CloseButton.Name = "CloseButton";
		this.CloseButton.Size = new System.Drawing.Size(66, 23);
		this.CloseButton.TabIndex = 6;
		this.CloseButton.Text = "閉じる";
		this.CloseButton.UseVisualStyleBackColor = true;
		this.CloseButton.Click += new System.EventHandler(CloseButton_Click);
		this.FindButton.Location = new System.Drawing.Point(477, 10);
		this.FindButton.Name = "FindButton";
		this.FindButton.Size = new System.Drawing.Size(66, 23);
		this.FindButton.TabIndex = 7;
		this.FindButton.Text = "検索";
		this.FindButton.UseVisualStyleBackColor = true;
		this.FindButton.Click += new System.EventHandler(FindButton_Click);
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(264, 15);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(29, 12);
		this.label3.TabIndex = 9;
		this.label3.Text = "性別";
		this.SexBox.ImeMode = System.Windows.Forms.ImeMode.Off;
		this.SexBox.Location = new System.Drawing.Point(295, 12);
		this.SexBox.MaxLength = 1;
		this.SexBox.Name = "SexBox";
		this.SexBox.Size = new System.Drawing.Size(18, 19);
		this.SexBox.TabIndex = 2;
		this.SexBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.SexBox.KeyDown += new System.Windows.Forms.KeyEventHandler(SexBox_KeyDown);
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(323, 15);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(53, 12);
		this.label4.TabIndex = 11;
		this.label4.Text = "生年月日";
		this.BirthBox1.ImeMode = System.Windows.Forms.ImeMode.Off;
		this.BirthBox1.Location = new System.Drawing.Point(376, 12);
		this.BirthBox1.MaxLength = 1;
		this.BirthBox1.Name = "BirthBox1";
		this.BirthBox1.Size = new System.Drawing.Size(18, 19);
		this.BirthBox1.TabIndex = 3;
		this.BirthBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.BirthBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(BirthBox1_KeyDown);
		this.BirthBox2.ImeMode = System.Windows.Forms.ImeMode.Off;
		this.BirthBox2.Location = new System.Drawing.Point(395, 12);
		this.BirthBox2.MaxLength = 2;
		this.BirthBox2.Name = "BirthBox2";
		this.BirthBox2.Size = new System.Drawing.Size(22, 19);
		this.BirthBox2.TabIndex = 4;
		this.BirthBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.BirthBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(BirthBox2_KeyDown);
		this.BirthBox3.ImeMode = System.Windows.Forms.ImeMode.Off;
		this.BirthBox3.Location = new System.Drawing.Point(418, 12);
		this.BirthBox3.MaxLength = 2;
		this.BirthBox3.Name = "BirthBox3";
		this.BirthBox3.Size = new System.Drawing.Size(22, 19);
		this.BirthBox3.TabIndex = 5;
		this.BirthBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.BirthBox3.KeyDown += new System.Windows.Forms.KeyEventHandler(BirthBox3_KeyDown);
		this.BirthBox4.ImeMode = System.Windows.Forms.ImeMode.Off;
		this.BirthBox4.Location = new System.Drawing.Point(441, 12);
		this.BirthBox4.MaxLength = 2;
		this.BirthBox4.Name = "BirthBox4";
		this.BirthBox4.Size = new System.Drawing.Size(22, 19);
		this.BirthBox4.TabIndex = 6;
		this.BirthBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.BirthBox4.KeyDown += new System.Windows.Forms.KeyEventHandler(BirthBox4_KeyDown);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(632, 453);
		base.Controls.Add(this.BirthBox4);
		base.Controls.Add(this.BirthBox3);
		base.Controls.Add(this.BirthBox2);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.BirthBox1);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.SexBox);
		base.Controls.Add(this.FindButton);
		base.Controls.Add(this.CloseButton);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.KanaBox);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.PatListView);
		base.Controls.Add(this.SelectButton);
		base.Controls.Add(this.NameBox);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "PatFindForm";
		this.Text = "患者検索";
		((System.ComponentModel.ISupportInitialize)this.PatListView).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
