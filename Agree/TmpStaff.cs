using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using AgentlabUtilityLibrary;

namespace Agree;

public partial class TmpStaff : Form
{
	private OleDbConnection oraConn;

	private OleDbCommand oraCmd = new OleDbCommand();

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
		oleDbDataAdapter.Fill(dataSet, "担当者");
		staffGridView.DataSource = dataSet.Tables["担当者"];
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
			DataGridViewRow row = staffGridView.Rows[rowId];
			id.Text = row.Cells[0].Value.ToString();
			staff_id.Text = row.Cells[1].Value.ToString();
			staff_name.Text = row.Cells[2].Value.ToString();
			cont.Text = row.Cells[3].Value.ToString();
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
				oraCmd.CommandText = "update AGREE_STAFF set CONT = " + AgreeSql.SqlValue(cont.Text.Trim()) + " where ID = " + id.Text.Trim();
			}
			else
			{
				oraCmd.CommandText = "insert into AGREE_STAFF (ID, STAFF, CONT) values (AGREE_STAFF_SEQ.nextval, " + staff_id.Text.Trim() + ", " + AgreeSql.SqlValue(cont.Text.Trim()) + ")";
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
		bool found = false;
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
					found = true;
					break;
				}
			}
			if (!found)
			{
				clearStaff(clearId: true, clearStaffId: false);
			}
		}
		else
		{
			MessageBox.Show("該当する担当者はありません");
			clearStaff(clearId: true, clearStaffId: true);
		}
	}

	private void closeButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}
}
