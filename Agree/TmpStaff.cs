using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using AgentlabUtilityLibrary;

namespace Agree;

public class TmpStaff : Form
{
	private OleDbConnection oraConn;

	private OleDbCommand oraCmd = new OleDbCommand();

	private OleDbDataReader oraReader;

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

	public TmpStaff()
	{
		InitializeComponent();
		oraConn = DBConn.GetOpenDBConn();
		oraCmd.Connection = oraConn;
		if (Program.OfflineMode)
		{
			saveButton.Enabled = false;
			deleteButton.Enabled = false;
			return;
		}
		initList();
	}

	private void initList()
	{
		oraConn.Open();
		oraCmd.CommandText = "Select ID, STAFF, Trim(NAME), CONT from AGREE_STAFF inner join M_USR" + Env.DB_LINK + " on AGREE_STAFF.STAFF = CODE";
		OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oraCmd);
		oraConn.Close();
		DataSet dataSet = new DataSet();
		oleDbDataAdapter.Fill(dataSet, "スタッフ");
		staffGridView.DataSource = dataSet.Tables["スタッフ"];
		staffGridView.Columns[0].Visible = false;
		staffGridView.Columns[1].HeaderText = "ID";
		staffGridView.Columns[1].Width = 40;
		staffGridView.Columns[2].HeaderText = "入力者";
		staffGridView.Columns[2].Width = 80;
		staffGridView.Columns[3].HeaderText = "担当者";
		staffGridView.Columns[3].Width = 160;
	}

	private void showStaff(int rowId)
	{
		if (rowId >= 0 && rowId < staffGridView.Rows.Count)
		{
			string text = staffGridView.Rows[rowId].Cells[0].Value.ToString();
			string text2 = staffGridView.Rows[rowId].Cells[1].Value.ToString();
			string text3 = staffGridView.Rows[rowId].Cells[2].Value.ToString();
			string text4 = staffGridView.Rows[rowId].Cells[3].Value.ToString();
			id.Text = text;
			staff_id.Text = text2;
			staff_name.Text = text3;
			cont.Text = text4;
		}
	}

	private void clearStaff(bool clearId, bool clearStaffId)
	{
		if (clearId)
		{
			id.Text = "";
		}
		if (clearStaffId)
		{
			staff_id.Text = "";
			staff_name.Text = "";
		}
		cont.Text = "";
	}

	private void saveButton_Click(object sender, EventArgs e)
	{
		if (staff_id.Text.Length > 0)
		{
			oraConn.Open();
			if (id.Text.Length > 0)
			{
				oraCmd.CommandText = "update AGREE_STAFF set CONT = '" + cont.Text.Trim() + "' where ID = " + id.Text.Trim();
			}
			else
			{
				oraCmd.CommandText = "insert into AGREE_STAFF (ID, STAFF, CONT) values (AGREE_STAFF_SEQ.nextval, " + staff_id.Text.Trim() + ", '" + cont.Text.Trim() + "')";
			}
			oraCmd.ExecuteNonQuery();
			oraConn.Close();
			initList();
			clearStaff(clearId: true, clearStaffId: true);
			MessageBox.Show("登録完了しました");
		}
		else
		{
			MessageBox.Show("入力者のIDを入力してください");
		}
	}

	private void deleteButton_Click(object sender, EventArgs e)
	{
		if (id.Text.Length <= 0)
		{
			MessageBox.Show("削除する行を一覧から選択してください");
			return;
		}
		if (MessageBox.Show(staff_name.Text + " を削除しますか？", "削除確認", MessageBoxButtons.YesNo) != DialogResult.Yes)
		{
			return;
		}
		oraConn.Open();
		oraCmd.CommandText = "delete from AGREE_STAFF where ID = " + id.Text.Trim();
		oraCmd.ExecuteNonQuery();
		oraConn.Close();
		initList();
		clearStaff(clearId: true, clearStaffId: true);
		MessageBox.Show("削除完了しました");
	}

	private void staffGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
	{
		showStaff(e.RowIndex);
	}

	private void staff_id_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			cont.Focus();
		}
	}

	private void staff_id_Leave(object sender, EventArgs e)
	{
		if (Program.OfflineMode)
		{
			return;
		}
		bool flag = false;
		if (staff_id.Text.Length <= 0)
		{
			return;
		}
		if (Dict.StaffDict.ContainsKey(staff_id.Text))
		{
			staff_name.Text = Dict.StaffDict[staff_id.Text].Name;
			foreach (DataGridViewRow item in (IEnumerable)staffGridView.Rows)
			{
				if (staff_id.Text == item.Cells[1].Value.ToString())
				{
					id.Text = item.Cells[0].Value.ToString();
					cont.Text = item.Cells[3].Value.ToString();
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				clearStaff(clearId: true, clearStaffId: false);
			}
		}
		else
		{
			MessageBox.Show("該当するスタッフはありません");
			clearStaff(clearId: true, clearStaffId: true);
		}
	}

	private void closeButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void TmpStaff_Load(object sender, EventArgs e)
	{
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
		base.Load += new System.EventHandler(TmpStaff_Load);
		((System.ComponentModel.ISupportInitialize)this.staffGridView).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
