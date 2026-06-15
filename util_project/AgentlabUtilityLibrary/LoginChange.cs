using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AgentlabUtilityLibrary;

public class LoginChange : Form
{
	private string login = "";

	private IContainer components;

	private Label label1;

	private Label UserLabel;

	public LoginChange()
	{
		InitializeComponent();
		UserLabel.Text = "現在のユーザーは " + LoginUser.Id + " " + LoginUser.Name + " です";
	}

	private void LoginChange_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			if (!LoginCheck())
			{
				MessageBox.Show("入力データが正しくありません");
			}
			else
			{
				LoginUser.SetUser(login.Substring(2).TrimStart('0'));
			}
			Dispose();
		}
		else if (e.KeyCode == Keys.Escape)
		{
			Dispose();
		}
	}

	private void LoginChange_KeyPress(object sender, KeyPressEventArgs e)
	{
		login += e.KeyChar;
	}

	private bool LoginCheck()
	{
		if (!login.StartsWith("MA"))
		{
			return false;
		}
		if (login.Length != 9)
		{
			return false;
		}
		return true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgentlabUtilityLibrary.LoginChange));
		this.label1 = new System.Windows.Forms.Label();
		this.UserLabel = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(51, 43);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(168, 24);
		this.label1.TabIndex = 0;
		this.label1.Text = "ログインユーザーを変更するには\r\n名札のバーコードを読んでください。";
		this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.UserLabel.BackColor = System.Drawing.Color.LightYellow;
		this.UserLabel.Location = new System.Drawing.Point(21, 17);
		this.UserLabel.Name = "UserLabel";
		this.UserLabel.Size = new System.Drawing.Size(223, 17);
		this.UserLabel.TabIndex = 1;
		this.UserLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(265, 78);
		base.Controls.Add(this.UserLabel);
		base.Controls.Add(this.label1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "LoginChange";
		this.Text = "ログインユーザー";
		base.KeyPress += new System.Windows.Forms.KeyPressEventHandler(LoginChange_KeyPress);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(LoginChange_KeyDown);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
