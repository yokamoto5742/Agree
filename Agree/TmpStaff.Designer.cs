using System.ComponentModel;
using System.Windows.Forms;

namespace Agree;

partial class TmpStaff
{
	private IContainer components;

	private DataGridView staffGridView;

	private Button saveButton;

	private Button deleteButton;

	private TextBox staff_id;

	private TextBox staff_name;

	private TextBox cont;

	private TextBox id;

	private Label label1;

	private Label label2;

	private Button closeButton;

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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Agree.TmpStaff));
		this.staffGridView = new System.Windows.Forms.DataGridView();
		this.saveButton = new System.Windows.Forms.Button();
		this.deleteButton = new System.Windows.Forms.Button();
		this.staff_id = new System.Windows.Forms.TextBox();
		this.staff_name = new System.Windows.Forms.TextBox();
		this.cont = new System.Windows.Forms.TextBox();
		this.id = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.closeButton = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.staffGridView).BeginInit();
		base.SuspendLayout();
		this.staffGridView.AllowUserToAddRows = false;
		this.staffGridView.AllowUserToDeleteRows = false;
		this.staffGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.staffGridView.Location = new System.Drawing.Point(15, 17);
		this.staffGridView.MultiSelect = false;
		this.staffGridView.Name = "staffGridView";
		this.staffGridView.ReadOnly = true;
		this.staffGridView.RowHeadersVisible = false;
		this.staffGridView.RowTemplate.Height = 21;
		this.staffGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.staffGridView.Size = new System.Drawing.Size(402, 365);
		this.staffGridView.TabIndex = 0;
		this.staffGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(staffGridView_RowEnter);
		this.saveButton.Location = new System.Drawing.Point(264, 471);
		this.saveButton.Name = "saveButton";
		this.saveButton.Size = new System.Drawing.Size(73, 24);
		this.saveButton.TabIndex = 1;
		this.saveButton.Text = "保存";
		this.saveButton.UseVisualStyleBackColor = true;
		this.saveButton.Click += new System.EventHandler(saveButton_Click);
		this.deleteButton.Location = new System.Drawing.Point(15, 471);
		this.deleteButton.Name = "deleteButton";
		this.deleteButton.Size = new System.Drawing.Size(73, 24);
		this.deleteButton.TabIndex = 5;
		this.deleteButton.Text = "削除";
		this.deleteButton.UseVisualStyleBackColor = true;
		this.deleteButton.Click += new System.EventHandler(deleteButton_Click);
		this.staff_id.ImeMode = System.Windows.Forms.ImeMode.Disable;
		this.staff_id.Location = new System.Drawing.Point(84, 395);
		this.staff_id.MaxLength = 5;
		this.staff_id.Name = "staff_id";
		this.staff_id.Size = new System.Drawing.Size(54, 19);
		this.staff_id.TabIndex = 2;
		this.staff_id.KeyDown += new System.Windows.Forms.KeyEventHandler(staff_id_KeyDown);
		this.staff_id.Leave += new System.EventHandler(staff_id_Leave);
		this.staff_name.BackColor = System.Drawing.SystemColors.ScrollBar;
		this.staff_name.Location = new System.Drawing.Point(144, 395);
		this.staff_name.Name = "staff_name";
		this.staff_name.Size = new System.Drawing.Size(96, 19);
		this.staff_name.TabIndex = 3;
		this.cont.Location = new System.Drawing.Point(84, 420);
		this.cont.MaxLength = 30;
		this.cont.Name = "cont";
		this.cont.Size = new System.Drawing.Size(333, 19);
		this.cont.TabIndex = 4;
		this.id.Location = new System.Drawing.Point(363, 395);
		this.id.Name = "id";
		this.id.ReadOnly = true;
		this.id.Size = new System.Drawing.Size(54, 19);
		this.id.TabIndex = 6;
		this.id.Visible = false;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(15, 398);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(41, 12);
		this.label1.TabIndex = 7;
		this.label1.Text = "入力者";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(15, 423);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(63, 12);
		this.label2.TabIndex = 8;
		this.label2.Text = "他の担当者";
		this.closeButton.Location = new System.Drawing.Point(343, 471);
		this.closeButton.Name = "closeButton";
		this.closeButton.Size = new System.Drawing.Size(74, 24);
		this.closeButton.TabIndex = 10;
		this.closeButton.Text = "閉じる";
		this.closeButton.UseVisualStyleBackColor = true;
		this.closeButton.Click += new System.EventHandler(closeButton_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(429, 502);
		base.Controls.Add(this.closeButton);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.id);
		base.Controls.Add(this.cont);
		base.Controls.Add(this.staff_name);
		base.Controls.Add(this.staff_id);
		base.Controls.Add(this.saveButton);
		base.Controls.Add(this.deleteButton);
		base.Controls.Add(this.staffGridView);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "TmpStaff";
		this.Text = "担当者テンプレート";
		((System.ComponentModel.ISupportInitialize)this.staffGridView).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
