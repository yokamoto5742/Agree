using System;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace AgentlabUtilityLibrary;

public class SendMessageForm : Form
{
	private string Qual1 = "";

	private string Send1 = "";

	private string Qual2 = "";

	private string Send2 = "";

	private string Qual3 = "";

	private string Send3 = "";

	private OleDbConnection oraConn = DBConn.GetOpenDBConn();

	private OleDbCommand oraCmd = new OleDbCommand();

	private OleDbDataReader oraReader;

	private IContainer components;

	private ComboBox SendToBox1;

	private Label label1;

	private TextBox TitleBox;

	private Button SendButton;

	private Button CloseButton;

	private TextBox ContBox;

	private Label label2;

	private ComboBox SendToBox2;

	private ComboBox SendToBox3;

	private TextBox SendToBox4;

	private TextBox SendToBox5;

	private Label label3;

	private TextBox PtIdBox;

	private TextBox SendToNameBox4;

	private TextBox SendToNameBox5;

	private Label label4;

	private TextBox PtInfoBox;

	private Label label5;

	private ComboBox PriorityBox;

	public SendMessageForm()
	{
		InitializeComponent();
	}

	public SendMessageForm(string ptId)
	{
		InitializeComponent();
		PtIdBox.Text = ptId;
		ShowPtInfo();
	}

	public SendMessageForm(string ptId, string qual1, string send1, string qual2, string send2, string qual3, string send3)
	{
		InitializeComponent();
		PtIdBox.Text = ptId;
		ShowPtInfo();
		Qual1 = qual1;
		Send1 = send1;
		Qual2 = qual2;
		Send2 = send2;
		Qual3 = qual3;
		Send3 = send3;
	}

	private void SendMessageForm_Load(object sender, EventArgs e)
	{
		oraConn.Open();
		oraCmd.Connection = oraConn;
		SendToBox1.Items.Add("");
		SendToBox2.Items.Add("");
		SendToBox3.Items.Add("");
		if (Qual1.Length > 0)
		{
			oraCmd.CommandText = "select IM90RC_F01, Trim(IM90RC_F03) from IM90RC" + Env.DB_LINK + " where IM90RC_F04 = " + Qual1;
			oraReader = oraCmd.ExecuteReader();
			while (oraReader.Read())
			{
				SendToBox1.Items.Add(oraReader[0].ToString() + " " + oraReader[1].ToString());
			}
			oraReader.Close();
			if (Dict.StaffDict.ContainsKey(Send1) && SendToBox1.Items.Contains(Send1 + " " + Dict.StaffDict[Send1].Name))
			{
				SendToBox1.Text = Send1 + " " + Dict.StaffDict[Send1].Name;
			}
		}
		else
		{
			SendToBox1.Enabled = false;
		}
		if (Qual2.Length > 0)
		{
			oraCmd.CommandText = "select IM90RC_F01, Trim(IM90RC_F03) from IM90RC" + Env.DB_LINK + " where IM90RC_F04 = " + Qual2;
			oraReader = oraCmd.ExecuteReader();
			while (oraReader.Read())
			{
				SendToBox2.Items.Add(oraReader[0].ToString() + " " + oraReader[1].ToString());
			}
			oraReader.Close();
			if (Dict.StaffDict.ContainsKey(Send2) && SendToBox2.Items.Contains(Send2 + " " + Dict.StaffDict[Send2].Name))
			{
				SendToBox2.Text = Send2 + " " + Dict.StaffDict[Send2].Name;
			}
		}
		else
		{
			SendToBox2.Enabled = false;
		}
		if (Qual3.Length > 0)
		{
			oraCmd.CommandText = "select IM90RC_F01, Trim(IM90RC_F03) from IM90RC" + Env.DB_LINK + " where IM90RC_F04 = " + Qual3;
			oraReader = oraCmd.ExecuteReader();
			while (oraReader.Read())
			{
				SendToBox3.Items.Add(oraReader[0].ToString() + " " + oraReader[1].ToString());
			}
			oraReader.Close();
			if (Dict.StaffDict.ContainsKey(Send3) && SendToBox3.Items.Contains(Send3 + " " + Dict.StaffDict[Send3].Name))
			{
				SendToBox3.Text = Send3 + " " + Dict.StaffDict[Send3].Name;
			}
		}
		else
		{
			SendToBox3.Enabled = false;
		}
		PriorityBox.Items.Add("0 中");
		PriorityBox.Items.Add("1 高");
		PriorityBox.Items.Add("2 低");
		PriorityBox.Text = "0 中";
		oraConn.Close();
	}

	private void ClearForm()
	{
		PtIdBox.Text = "";
		PtInfoBox.Text = "";
		SendToBox1.Text = "";
		SendToBox2.Text = "";
		SendToBox3.Text = "";
		SendToBox4.Text = "";
		SendToNameBox4.Text = "";
		SendToBox5.Text = "";
		SendToNameBox5.Text = "";
		TitleBox.Text = "";
		PriorityBox.Text = "";
		ContBox.Text = "";
	}

	private void CloseButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void SendToBox4_Leave(object sender, EventArgs e)
	{
		ShowSendToNameBox4();
	}

	private void SendToBox4_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			ShowSendToNameBox4();
		}
	}

	private void ShowSendToNameBox4()
	{
		if (Dict.StaffDict.ContainsKey(SendToBox4.Text))
		{
			SendToNameBox4.Text = Dict.StaffDict[SendToBox4.Text].Name;
			return;
		}
		SendToBox4.Text = "";
		SendToNameBox4.Text = "";
	}

	private void SendToBox5_Leave(object sender, EventArgs e)
	{
		ShowSendToNameBox5();
	}

	private void SendToBox5_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			ShowSendToNameBox5();
		}
	}

	private void ShowSendToNameBox5()
	{
		if (Dict.StaffDict.ContainsKey(SendToBox5.Text))
		{
			SendToNameBox5.Text = Dict.StaffDict[SendToBox5.Text].Name;
			return;
		}
		SendToBox5.Text = "";
		SendToNameBox5.Text = "";
	}

	private void PtIdBox_Leave(object sender, EventArgs e)
	{
		ShowPtInfo();
	}

	private void PtIdBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			ShowPtInfo();
		}
	}

	private void ShowPtInfo()
	{
		if (PtIdBox.Text.Length > 0)
		{
			Pat.SetPat(PtIdBox.Text);
			PtInfoBox.Text = Pat.Name + " 様";
		}
		else
		{
			PtIdBox.Text = "";
			PtInfoBox.Text = "";
		}
	}

	private void SendButton_Click(object sender, EventArgs e)
	{
		if (SendToBox1.Text.Contains(" "))
		{
			KarteMessage.Send(SendToBox1.Text.Split(' ')[0], LoginUser.Id, TitleBox.Text, ContBox.Text, PtIdBox.Text, PriorityBox.Text.Split(' ')[0]);
		}
		if (SendToBox2.Text.Contains(" "))
		{
			KarteMessage.Send(SendToBox2.Text.Split(' ')[0], LoginUser.Id, TitleBox.Text, ContBox.Text, PtIdBox.Text, PriorityBox.Text.Split(' ')[0]);
		}
		if (SendToBox3.Text.Contains(" "))
		{
			KarteMessage.Send(SendToBox3.Text.Split(' ')[0], LoginUser.Id, TitleBox.Text, ContBox.Text, PtIdBox.Text, PriorityBox.Text.Split(' ')[0]);
		}
		if (SendToBox4.Text.Length > 0)
		{
			KarteMessage.Send(SendToBox4.Text, LoginUser.Id, TitleBox.Text, ContBox.Text, PtIdBox.Text, PriorityBox.Text.Split(' ')[0]);
		}
		if (SendToBox5.Text.Length > 0)
		{
			KarteMessage.Send(SendToBox5.Text, LoginUser.Id, TitleBox.Text, ContBox.Text, PtIdBox.Text, PriorityBox.Text.Split(' ')[0]);
		}
		ClearForm();
		MessageBox.Show("送信しました");
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgentlabUtilityLibrary.SendMessageForm));
		this.SendToBox1 = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.TitleBox = new System.Windows.Forms.TextBox();
		this.SendButton = new System.Windows.Forms.Button();
		this.CloseButton = new System.Windows.Forms.Button();
		this.ContBox = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.SendToBox2 = new System.Windows.Forms.ComboBox();
		this.SendToBox3 = new System.Windows.Forms.ComboBox();
		this.SendToBox4 = new System.Windows.Forms.TextBox();
		this.SendToBox5 = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.PtIdBox = new System.Windows.Forms.TextBox();
		this.SendToNameBox4 = new System.Windows.Forms.TextBox();
		this.SendToNameBox5 = new System.Windows.Forms.TextBox();
		this.label4 = new System.Windows.Forms.Label();
		this.PtInfoBox = new System.Windows.Forms.TextBox();
		this.label5 = new System.Windows.Forms.Label();
		this.PriorityBox = new System.Windows.Forms.ComboBox();
		base.SuspendLayout();
		this.SendToBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.SendToBox1.FormattingEnabled = true;
		this.SendToBox1.Location = new System.Drawing.Point(6, 71);
		this.SendToBox1.Name = "SendToBox1";
		this.SendToBox1.Size = new System.Drawing.Size(138, 20);
		this.SendToBox1.TabIndex = 0;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(3, 54);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(41, 12);
		this.label1.TabIndex = 1;
		this.label1.Text = "送信先";
		this.TitleBox.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
		this.TitleBox.Location = new System.Drawing.Point(198, 8);
		this.TitleBox.MaxLength = 30;
		this.TitleBox.Name = "TitleBox";
		this.TitleBox.Size = new System.Drawing.Size(223, 19);
		this.TitleBox.TabIndex = 11;
		this.SendButton.Location = new System.Drawing.Point(287, 163);
		this.SendButton.Name = "SendButton";
		this.SendButton.Size = new System.Drawing.Size(75, 23);
		this.SendButton.TabIndex = 3;
		this.SendButton.Text = "送信";
		this.SendButton.UseVisualStyleBackColor = true;
		this.SendButton.Click += new System.EventHandler(SendButton_Click);
		this.CloseButton.Location = new System.Drawing.Point(368, 163);
		this.CloseButton.Name = "CloseButton";
		this.CloseButton.Size = new System.Drawing.Size(75, 23);
		this.CloseButton.TabIndex = 4;
		this.CloseButton.Text = "閉じる";
		this.CloseButton.UseVisualStyleBackColor = true;
		this.CloseButton.Click += new System.EventHandler(CloseButton_Click);
		this.ContBox.Location = new System.Drawing.Point(198, 30);
		this.ContBox.MaxLength = 30;
		this.ContBox.Multiline = true;
		this.ContBox.Name = "ContBox";
		this.ContBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.ContBox.Size = new System.Drawing.Size(335, 129);
		this.ContBox.TabIndex = 12;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(154, 11);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(29, 12);
		this.label2.TabIndex = 6;
		this.label2.Text = "件名";
		this.SendToBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.SendToBox2.FormattingEnabled = true;
		this.SendToBox2.Location = new System.Drawing.Point(6, 94);
		this.SendToBox2.Name = "SendToBox2";
		this.SendToBox2.Size = new System.Drawing.Size(138, 20);
		this.SendToBox2.TabIndex = 7;
		this.SendToBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.SendToBox3.FormattingEnabled = true;
		this.SendToBox3.Location = new System.Drawing.Point(6, 117);
		this.SendToBox3.Name = "SendToBox3";
		this.SendToBox3.Size = new System.Drawing.Size(138, 20);
		this.SendToBox3.TabIndex = 8;
		this.SendToBox4.ImeMode = System.Windows.Forms.ImeMode.Disable;
		this.SendToBox4.Location = new System.Drawing.Point(6, 140);
		this.SendToBox4.MaxLength = 5;
		this.SendToBox4.Name = "SendToBox4";
		this.SendToBox4.Size = new System.Drawing.Size(44, 19);
		this.SendToBox4.TabIndex = 9;
		this.SendToBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.SendToBox4.KeyDown += new System.Windows.Forms.KeyEventHandler(SendToBox4_KeyDown);
		this.SendToBox4.Leave += new System.EventHandler(SendToBox4_Leave);
		this.SendToBox5.ImeMode = System.Windows.Forms.ImeMode.Disable;
		this.SendToBox5.Location = new System.Drawing.Point(6, 162);
		this.SendToBox5.MaxLength = 5;
		this.SendToBox5.Name = "SendToBox5";
		this.SendToBox5.Size = new System.Drawing.Size(44, 19);
		this.SendToBox5.TabIndex = 10;
		this.SendToBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.SendToBox5.KeyDown += new System.Windows.Forms.KeyEventHandler(SendToBox5_KeyDown);
		this.SendToBox5.Leave += new System.EventHandler(SendToBox5_Leave);
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(4, 11);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(40, 12);
		this.label3.TabIndex = 11;
		this.label3.Text = "患者ID";
		this.PtIdBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
		this.PtIdBox.Location = new System.Drawing.Point(48, 8);
		this.PtIdBox.MaxLength = 9;
		this.PtIdBox.Name = "PtIdBox";
		this.PtIdBox.Size = new System.Drawing.Size(68, 19);
		this.PtIdBox.TabIndex = 1;
		this.PtIdBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.PtIdBox.KeyDown += new System.Windows.Forms.KeyEventHandler(PtIdBox_KeyDown);
		this.PtIdBox.Leave += new System.EventHandler(PtIdBox_Leave);
		this.SendToNameBox4.Location = new System.Drawing.Point(56, 140);
		this.SendToNameBox4.MaxLength = 5;
		this.SendToNameBox4.Name = "SendToNameBox4";
		this.SendToNameBox4.ReadOnly = true;
		this.SendToNameBox4.Size = new System.Drawing.Size(88, 19);
		this.SendToNameBox4.TabIndex = 13;
		this.SendToNameBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.SendToNameBox5.Location = new System.Drawing.Point(56, 162);
		this.SendToNameBox5.MaxLength = 5;
		this.SendToNameBox5.Name = "SendToNameBox5";
		this.SendToNameBox5.ReadOnly = true;
		this.SendToNameBox5.Size = new System.Drawing.Size(88, 19);
		this.SendToNameBox5.TabIndex = 14;
		this.SendToNameBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(154, 33);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(29, 12);
		this.label4.TabIndex = 15;
		this.label4.Text = "内容";
		this.PtInfoBox.Location = new System.Drawing.Point(48, 30);
		this.PtInfoBox.MaxLength = 5;
		this.PtInfoBox.Name = "PtInfoBox";
		this.PtInfoBox.ReadOnly = true;
		this.PtInfoBox.Size = new System.Drawing.Size(96, 19);
		this.PtInfoBox.TabIndex = 2;
		this.PtInfoBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(427, 11);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(41, 12);
		this.label5.TabIndex = 17;
		this.label5.Text = "重要度";
		this.PriorityBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.PriorityBox.FormattingEnabled = true;
		this.PriorityBox.Location = new System.Drawing.Point(473, 7);
		this.PriorityBox.Name = "PriorityBox";
		this.PriorityBox.Size = new System.Drawing.Size(60, 20);
		this.PriorityBox.TabIndex = 18;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(541, 187);
		base.Controls.Add(this.PriorityBox);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.PtInfoBox);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.SendToNameBox5);
		base.Controls.Add(this.SendToNameBox4);
		base.Controls.Add(this.PtIdBox);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.SendToBox5);
		base.Controls.Add(this.SendToBox4);
		base.Controls.Add(this.SendToBox3);
		base.Controls.Add(this.SendToBox2);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.ContBox);
		base.Controls.Add(this.CloseButton);
		base.Controls.Add(this.SendButton);
		base.Controls.Add(this.TitleBox);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.SendToBox1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "SendMessageForm";
		this.Text = "メッセージ送信";
		base.Load += new System.EventHandler(SendMessageForm_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
