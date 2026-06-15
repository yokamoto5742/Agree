using System;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using InnoUketsukeLib.Entity;
using InnoUketsukeLib.Utility;

namespace AgentlabUtilityLibrary;

public class LoginPrompt : Form
{
	private bool suc;

	private IContainer components;

	private TextBox idBox;

	private Label label1;

	private Label label2;

	private TextBox pwdBox;

	private Button loginButton;

	private Button closeButton;

	private Label errLabel;

	public LoginPrompt()
	{
		InitializeComponent();
	}

	private void login()
	{
		AppInit.g_AppInit.Init();
		if (idBox.Text.Length == 0)
		{
			MessageBox.Show("IDが入力されていません");
			return;
		}
		Hide();
		OleDbConnection dBConn = DBConn.GetDBConn();
		dBConn.Open();
		int result = 0;
		int.TryParse(idBox.Text, out result);
		suc = M_USR.g_Usr1.GetData(result, pwdBox.Text);
		dBConn.Close();
		if (suc)
		{
			LoginUser.SetUser(idBox.Text);
		}
		else
		{
			errLabel.Text = "ログインに失敗しました";
			idBox.Text = "";
			pwdBox.Text = "";
			idBox.Focus();
			Show();
		}
		if (suc)
		{
			Dispose();
		}
	}

	private void loginButton_Click(object sender, EventArgs e)
	{
		login();
	}

	private void closeButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void idBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			pwdBox.Focus();
		}
	}

	private void pwdBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			loginButton.Focus();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgentlabUtilityLibrary.LoginPrompt));
		this.idBox = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.pwdBox = new System.Windows.Forms.TextBox();
		this.loginButton = new System.Windows.Forms.Button();
		this.closeButton = new System.Windows.Forms.Button();
		this.errLabel = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.idBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
		this.idBox.Location = new System.Drawing.Point(98, 30);
		this.idBox.MaxLength = 50;
		this.idBox.Name = "idBox";
		this.idBox.Size = new System.Drawing.Size(92, 19);
		this.idBox.TabIndex = 0;
		this.idBox.KeyDown += new System.Windows.Forms.KeyEventHandler(idBox_KeyDown);
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(32, 33);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(16, 12);
		this.label1.TabIndex = 1;
		this.label1.Text = "ID";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(32, 58);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(52, 12);
		this.label2.TabIndex = 3;
		this.label2.Text = "パスワード";
		this.pwdBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
		this.pwdBox.Location = new System.Drawing.Point(98, 55);
		this.pwdBox.MaxLength = 50;
		this.pwdBox.Name = "pwdBox";
		this.pwdBox.PasswordChar = '*';
		this.pwdBox.Size = new System.Drawing.Size(92, 19);
		this.pwdBox.TabIndex = 2;
		this.pwdBox.KeyDown += new System.Windows.Forms.KeyEventHandler(pwdBox_KeyDown);
		this.loginButton.Location = new System.Drawing.Point(28, 89);
		this.loginButton.Name = "loginButton";
		this.loginButton.Size = new System.Drawing.Size(74, 23);
		this.loginButton.TabIndex = 4;
		this.loginButton.Text = "ログイン";
		this.loginButton.UseVisualStyleBackColor = true;
		this.loginButton.Click += new System.EventHandler(loginButton_Click);
		this.closeButton.Location = new System.Drawing.Point(123, 89);
		this.closeButton.Name = "closeButton";
		this.closeButton.Size = new System.Drawing.Size(74, 23);
		this.closeButton.TabIndex = 5;
		this.closeButton.Text = "閉じる";
		this.closeButton.UseVisualStyleBackColor = true;
		this.closeButton.Click += new System.EventHandler(closeButton_Click);
		this.errLabel.AutoSize = true;
		this.errLabel.Font = new System.Drawing.Font("MS UI Gothic", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
		this.errLabel.ForeColor = System.Drawing.Color.Red;
		this.errLabel.Location = new System.Drawing.Point(45, 9);
		this.errLabel.Name = "errLabel";
		this.errLabel.Size = new System.Drawing.Size(0, 12);
		this.errLabel.TabIndex = 6;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(225, 133);
		base.Controls.Add(this.errLabel);
		base.Controls.Add(this.closeButton);
		base.Controls.Add(this.loginButton);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.pwdBox);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.idBox);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "LoginPrompt";
		this.Text = "ログイン";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
