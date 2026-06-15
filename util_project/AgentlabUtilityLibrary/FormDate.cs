using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AgentlabUtilityLibrary;

public class FormDate : Form
{
	private IContainer components;

	private MonthCalendar Cal;

	private TextBox ReturnBox;

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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgentlabUtilityLibrary.FormDate));
		this.Cal = new System.Windows.Forms.MonthCalendar();
		base.SuspendLayout();
		this.Cal.Location = new System.Drawing.Point(6, 2);
		this.Cal.MaxSelectionCount = 1;
		this.Cal.Name = "Cal";
		this.Cal.TabIndex = 0;
		this.Cal.DateSelected += new System.Windows.Forms.DateRangeEventHandler(Cal_DateSelected);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(150, 150);
		base.Controls.Add(this.Cal);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.KeyPreview = true;
		base.Name = "FormDate";
		this.Text = "日付選択";
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(FormDate_KeyDown);
		base.ResumeLayout(false);
	}

	public FormDate(TextBox box)
	{
		InitializeComponent();
		ReturnBox = box;
	}

	private void Cal_DateSelected(object sender, DateRangeEventArgs e)
	{
		ReturnBox.Text = Cal.SelectionStart.ToString("yyyy/MM/dd");
		Dispose();
	}

	private void FormDate_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Escape)
		{
			Dispose();
		}
	}
}
