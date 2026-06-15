using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AgentlabUtilityLibrary;

public class InfoShareForm : Form
{
	private IContainer components;

	private TextBox ContBox;

	private TextBox PtInfoBox;

	private Label label1;

	private TextBox PtIdBox;

	private Button RegButton;

	private TextBox StaffBox;

	private Label label2;

	private TextBox SaveDateBox;

	private Label label3;

	private Button CloseButton;

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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgentlabUtilityLibrary.InfoShareForm));
		this.ContBox = new System.Windows.Forms.TextBox();
		this.PtInfoBox = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.PtIdBox = new System.Windows.Forms.TextBox();
		this.RegButton = new System.Windows.Forms.Button();
		this.StaffBox = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.SaveDateBox = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.CloseButton = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.ContBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ContBox.Location = new System.Drawing.Point(8, 62);
		this.ContBox.Multiline = true;
		this.ContBox.Name = "ContBox";
		this.ContBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.ContBox.Size = new System.Drawing.Size(400, 208);
		this.ContBox.TabIndex = 7;
		this.PtInfoBox.BackColor = System.Drawing.Color.LightYellow;
		this.PtInfoBox.ImeMode = System.Windows.Forms.ImeMode.Off;
		this.PtInfoBox.Location = new System.Drawing.Point(118, 12);
		this.PtInfoBox.MaxLength = 100;
		this.PtInfoBox.Name = "PtInfoBox";
		this.PtInfoBox.ReadOnly = true;
		this.PtInfoBox.Size = new System.Drawing.Size(290, 19);
		this.PtInfoBox.TabIndex = 6;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(6, 15);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(40, 12);
		this.label1.TabIndex = 5;
		this.label1.Text = "患者ID";
		this.PtIdBox.ImeMode = System.Windows.Forms.ImeMode.Off;
		this.PtIdBox.Location = new System.Drawing.Point(48, 12);
		this.PtIdBox.MaxLength = 9;
		this.PtIdBox.Name = "PtIdBox";
		this.PtIdBox.Size = new System.Drawing.Size(68, 19);
		this.PtIdBox.TabIndex = 4;
		this.PtIdBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.PtIdBox.KeyDown += new System.Windows.Forms.KeyEventHandler(PtIdBox_KeyDown);
		this.RegButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.RegButton.Location = new System.Drawing.Point(119, 276);
		this.RegButton.Name = "RegButton";
		this.RegButton.Size = new System.Drawing.Size(75, 23);
		this.RegButton.TabIndex = 8;
		this.RegButton.Text = "登録";
		this.RegButton.UseVisualStyleBackColor = true;
		this.RegButton.Click += new System.EventHandler(RegButton_Click);
		this.StaffBox.BackColor = System.Drawing.Color.LightYellow;
		this.StaffBox.ImeMode = System.Windows.Forms.ImeMode.Off;
		this.StaffBox.Location = new System.Drawing.Point(76, 37);
		this.StaffBox.MaxLength = 100;
		this.StaffBox.Name = "StaffBox";
		this.StaffBox.ReadOnly = true;
		this.StaffBox.Size = new System.Drawing.Size(99, 19);
		this.StaffBox.TabIndex = 10;
		this.StaffBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(6, 40);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(65, 12);
		this.label2.TabIndex = 9;
		this.label2.Text = "最終登録者";
		this.SaveDateBox.BackColor = System.Drawing.Color.LightYellow;
		this.SaveDateBox.ImeMode = System.Windows.Forms.ImeMode.Off;
		this.SaveDateBox.Location = new System.Drawing.Point(265, 37);
		this.SaveDateBox.MaxLength = 100;
		this.SaveDateBox.Name = "SaveDateBox";
		this.SaveDateBox.ReadOnly = true;
		this.SaveDateBox.Size = new System.Drawing.Size(143, 19);
		this.SaveDateBox.TabIndex = 12;
		this.SaveDateBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(195, 40);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(65, 12);
		this.label3.TabIndex = 11;
		this.label3.Text = "最終登録日";
		this.CloseButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.CloseButton.Location = new System.Drawing.Point(220, 276);
		this.CloseButton.Name = "CloseButton";
		this.CloseButton.Size = new System.Drawing.Size(75, 23);
		this.CloseButton.TabIndex = 13;
		this.CloseButton.Text = "閉じる";
		this.CloseButton.UseVisualStyleBackColor = true;
		this.CloseButton.Click += new System.EventHandler(CloseButton_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(412, 303);
		base.Controls.Add(this.CloseButton);
		base.Controls.Add(this.SaveDateBox);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.StaffBox);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.RegButton);
		base.Controls.Add(this.ContBox);
		base.Controls.Add(this.PtInfoBox);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.PtIdBox);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "InfoShareForm";
		this.Text = "カルテ伝達情報";
		base.Load += new System.EventHandler(InfoShareForm_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public InfoShareForm()
	{
		InitializeComponent();
	}

	public InfoShareForm(string pt_id)
	{
		InitializeComponent();
		PtShow(pt_id);
	}

	private void InfoShareForm_Load(object sender, EventArgs e)
	{
		if ((LoginUser.Id == null || LoginUser.Id.Length == 0) && !LoginUser.Init())
		{
			Dispose();
		}
		CloseButton.Select();
	}

	private void Clear()
	{
		PtIdBox.Clear();
		PtInfoBox.Clear();
		StaffBox.Clear();
		SaveDateBox.Clear();
		ContBox.Clear();
	}

	public void PtShow(string pt_id)
	{
		int result = 0;
		if (pt_id.Length == 0 || !int.TryParse(pt_id, out result))
		{
			return;
		}
		PtIdBox.Text = pt_id;
		PtInfoBox.Text = Pat.GetPatInfo(pt_id);
		InfoShare infoShare = InfoShare.Load(pt_id);
		if (infoShare.PtId.Length > 0)
		{
			ContBox.Text = infoShare.Cont;
			if (Dict.StaffDict.ContainsKey(infoShare.Staff))
			{
				StaffBox.Text = Dict.StaffDict[infoShare.Staff].Name;
			}
			if (infoShare.SaveDate.Length == 8)
			{
				SaveDateBox.Text = infoShare.SaveDate.Substring(2, 6).Insert(2, "/").Insert(5, "/");
				TextBox saveDateBox = SaveDateBox;
				saveDateBox.Text = saveDateBox.Text + " " + infoShare.SaveTime.PadLeft(6, '0').Substring(0, 4).Insert(2, ":");
			}
		}
	}

	private void PtIdBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			int result = 0;
			if (PtIdBox.Text.Length > 0 && int.TryParse(PtIdBox.Text, out result))
			{
				PtShow(PtIdBox.Text);
			}
		}
		else if (e.KeyCode == Keys.F3)
		{
			PatFindForm patFindForm = new PatFindForm(PtIdBox);
			patFindForm.ShowDialog();
			int result2 = 0;
			if (PtIdBox.Text.Length > 0 && int.TryParse(PtIdBox.Text, out result2))
			{
				PtShow(PtIdBox.Text);
			}
		}
	}

	private void RegButton_Click(object sender, EventArgs e)
	{
		int result = 0;
		if (PtIdBox.Text.Length != 0 && int.TryParse(PtIdBox.Text, out result))
		{
			InfoShare infoShare = new InfoShare();
			infoShare.PtId = PtIdBox.Text;
			infoShare.InOut = "0";
			infoShare.Cont = ContBox.Text;
			infoShare.Staff = LoginUser.Id;
			infoShare.Save();
			Clear();
			MessageBox.Show("登録しました");
		}
	}

	private void CloseButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}
}
