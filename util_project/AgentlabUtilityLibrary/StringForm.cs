using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AgentlabUtilityLibrary;

public class StringForm : Form
{
	private IContainer components;

	private Label StringLabel;

	private TextBox StringBox;

	private Button OKButton;

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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgentlabUtilityLibrary.StringForm));
		this.StringLabel = new System.Windows.Forms.Label();
		this.StringBox = new System.Windows.Forms.TextBox();
		this.OKButton = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.StringLabel.AutoSize = true;
		this.StringLabel.Location = new System.Drawing.Point(12, 9);
		this.StringLabel.Name = "StringLabel";
		this.StringLabel.Size = new System.Drawing.Size(0, 12);
		this.StringLabel.TabIndex = 0;
		this.StringBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.StringBox.Location = new System.Drawing.Point(12, 28);
		this.StringBox.Multiline = true;
		this.StringBox.Name = "StringBox";
		this.StringBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.StringBox.Size = new System.Drawing.Size(268, 68);
		this.StringBox.TabIndex = 1;
		this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.OKButton.Location = new System.Drawing.Point(105, 101);
		this.OKButton.Name = "OKButton";
		this.OKButton.Size = new System.Drawing.Size(75, 20);
		this.OKButton.TabIndex = 2;
		this.OKButton.Text = "OK";
		this.OKButton.UseVisualStyleBackColor = true;
		this.OKButton.Click += new System.EventHandler(OKButton_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(292, 123);
		base.Controls.Add(this.OKButton);
		base.Controls.Add(this.StringBox);
		base.Controls.Add(this.StringLabel);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "StringForm";
		this.Text = "StringForm";
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public StringForm(string title, string desc_label, string desc_box)
	{
		InitializeComponent();
		Text = title;
		StringLabel.Text = desc_label;
		StringBox.Text = desc_box;
		OKButton.Select();
	}

	private void OKButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}
}
