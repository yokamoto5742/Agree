using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using AgentlabUtilityLibrary;

namespace Agree;

public partial class TmpAgree : Form
{
	private struct TmpNode
	{
		public int temp_id;

		public int temp_level;

		public int temp_parent;

		public string temp_name;
	}

	private Form1 f1;

	private Hashtable nodeTable = new Hashtable();

	private Hashtable nodeNameTable = new Hashtable();

	private Hashtable parentTable = new Hashtable();

	private List<TmpNode> tmpNodeList = new List<TmpNode>();

	private List<TextBox> tmpBoxList = new List<TextBox>();

	private List<ComboBox> tmpComboList = new List<ComboBox>();

	private OleDbConnection oraConn;

	private OleDbCommand oraCmd = new OleDbCommand();

	private OleDbDataReader oraReader;

	private bool editingParent;

	public TmpAgree(Form1 F1, bool applyButtonVisible)
	{
		InitializeComponent();
		f1 = F1;
		applyTmpButton.Visible = applyButtonVisible;
	}

	private void TmpPlan_Load(object sender, EventArgs e)
	{
		tmpBoxList.Add(temp_id);
		tmpBoxList.Add(temp_name);
		tmpBoxList.Add(diag);
		tmpBoxList.Add(ope);
		tmpBoxList.Add(item1);
		tmpBoxList.Add(item2);
		tmpBoxList.Add(item3);
		tmpBoxList.Add(explanation);
		tmpBoxList.Add(item4);
		tmpComboList.Add(eye);
		tmpComboList.Add(sheetName);
		tmpBoxList.Add(anes);
		if (Program.OfflineMode)
		{
			applyTmpButton.Enabled = false;
			newTmpButton.Enabled = false;
			editTmpButton.Enabled = false;
			regTmpButton.Enabled = false;
			delTmpButton.Enabled = false;
			upButton.Enabled = false;
			downButton.Enabled = false;
			addParentButton.Enabled = false;
			temp_parent.Enabled = false;
			return;
		}
		oraConn = DBConn.GetOpenDBConn();
		oraCmd.Connection = oraConn;
		loadParents();
		initTree();
	}

	/// <summary>
	/// 入力欄（テキスト欄・コンボ）の背景色をまとめて設定する。
	/// clearText が true のときは内容も空にする。
	/// </summary>
	private void setTmpFields(Color backColor, bool clearText)
	{
		foreach (TextBox tmpBox in tmpBoxList)
		{
			if (clearText)
			{
				tmpBox.Text = "";
			}
			tmpBox.BackColor = backColor;
		}
		foreach (ComboBox tmpCombo in tmpComboList)
		{
			if (clearText)
			{
				tmpCombo.Text = "";
			}
			tmpCombo.BackColor = backColor;
		}
	}

	private void loadParents()
	{
		temp_parent.Items.Clear();
		parentTable.Clear();
		try
		{
			oraConn.Open();
			oraCmd.CommandText = "Select TEMP_ID, TEMP_NAME from AGREE_TEMPLATE where TEMP_LEVEL = 0 and DELETE_FLAG != 1 order by DISP_ORDER";
			oraReader = oraCmd.ExecuteReader();
			while (oraReader.Read())
			{
				temp_parent.Items.Add(oraReader["TEMP_NAME"].ToString());
				parentTable.Add(oraReader["TEMP_NAME"].ToString().Trim(), oraReader["TEMP_ID"].ToString());
			}
		}
		finally
		{
			if (oraReader != null && !oraReader.IsClosed)
			{
				oraReader.Close();
			}
			if (oraConn.State != System.Data.ConnectionState.Closed)
			{
				oraConn.Close();
			}
		}
	}

	private void initTree()
	{
		tmpAgreeTree.Nodes.Clear();
		tmpNodeList.Clear();
		nodeTable.Clear();
		nodeNameTable.Clear();
		try
		{
			oraConn.Open();
			oraCmd.CommandText = "Select TEMP_ID, TEMP_LEVEL, TEMP_NAME, TEMP_PARENT from AGREE_TEMPLATE where DELETE_FLAG != 1 order by TEMP_LEVEL , DISP_ORDER , TEMP_ID";
			oraReader = oraCmd.ExecuteReader();
			while (oraReader.Read())
			{
				if (oraReader["TEMP_LEVEL"].ToString() == "0")
				{
					tmpAgreeTree.Nodes.Add(oraReader["TEMP_ID"].ToString(), oraReader["TEMP_NAME"].ToString());
					TmpNode tmpNode = new TmpNode
					{
						temp_id = int.Parse(oraReader["TEMP_ID"].ToString()),
						temp_level = int.Parse(oraReader["TEMP_LEVEL"].ToString()),
						temp_parent = int.Parse(oraReader["TEMP_PARENT"].ToString()),
						temp_name = oraReader["TEMP_NAME"].ToString()
					};
					tmpNodeList.Add(tmpNode);
					nodeTable.Add(tmpNode.temp_id, tmpNode);
					nodeNameTable.Add(tmpNode.temp_id, tmpNode.temp_name);
				}
				else if (oraReader["TEMP_ID"].ToString() != oraReader["TEMP_PARENT"].ToString())
				{
					string parentKey = oraReader["TEMP_PARENT"].ToString();
					if (tmpAgreeTree.Nodes.ContainsKey(parentKey))
					{
						tmpAgreeTree.Nodes[parentKey].Nodes.Add(oraReader["TEMP_ID"].ToString(), oraReader["TEMP_NAME"].ToString());
					}
					else
					{
						tmpAgreeTree.Nodes.Add(oraReader["TEMP_ID"].ToString(), oraReader["TEMP_NAME"].ToString());
					}
					TmpNode tmpNode2 = new TmpNode
					{
						temp_id = int.Parse(oraReader["TEMP_ID"].ToString()),
						temp_level = int.Parse(oraReader["TEMP_LEVEL"].ToString()),
						temp_parent = int.Parse(oraReader["TEMP_PARENT"].ToString()),
						temp_name = oraReader["TEMP_NAME"].ToString()
					};
					if (!nodeTable.ContainsKey(tmpNode2.temp_id))
					{
						tmpNodeList.Add(tmpNode2);
						nodeTable.Add(tmpNode2.temp_id, tmpNode2);
						nodeNameTable.Add(tmpNode2.temp_id, tmpNode2.temp_name);
					}
				}
			}
		}
		finally
		{
			if (oraReader != null && !oraReader.IsClosed)
			{
				oraReader.Close();
			}
			if (oraConn.State != System.Data.ConnectionState.Closed)
			{
				oraConn.Close();
			}
		}
		editingParent = false;
		panel2.Enabled = true;
		addParentButton.Enabled = true;
		applyTmpButton.Enabled = false;
		newTmpButton.Enabled = false;
		editTmpButton.Enabled = false;
		regTmpButton.Enabled = false;
		delTmpButton.Enabled = false;
		setTmpFields(Color.LightGray, clearText: false);
		temp_parent.Enabled = false;
	}

	private void showTemplate(int temp_id)
	{
		try
		{
			oraConn.Open();
			oraCmd.CommandText = "Select TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME,EYE , DIAG, ANES ,OPE, EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4 , SHEET_NAME from AGREE_TEMPLATE where TEMP_ID = " + temp_id;
			oraReader = oraCmd.ExecuteReader();
			if (oraReader.Read())
			{
				this.temp_id.Text = oraReader["TEMP_ID"].ToString();
				temp_name.Text = oraReader["TEMP_NAME"].ToString();
				if (oraReader["TEMP_LEVEL"].ToString() == "1")
				{
					temp_parent.Text = nodeNameTable[int.Parse(oraReader["TEMP_PARENT"].ToString())].ToString();
				}
				eye.Text = oraReader["EYE"].ToString();
				sheetName.Text = oraReader["SHEET_NAME"].ToString();
				diag.Text = oraReader["DIAG"].ToString();
				anes.Text = oraReader["ANES"].ToString();
				ope.Text = oraReader["OPE"].ToString();
				explanation.Text = oraReader["EXPLANATION"].ToString();
				item1.Text = oraReader["ITEM1"].ToString();
				item2.Text = oraReader["ITEM2"].ToString();
				item3.Text = oraReader["ITEM3"].ToString();
				item4.Text = oraReader["ITEM4"].ToString();
			}
		}
		finally
		{
			if (oraReader != null && !oraReader.IsClosed)
			{
				oraReader.Close();
			}
			if (oraConn.State != System.Data.ConnectionState.Closed)
			{
				oraConn.Close();
			}
		}
	}

	private void showTemplate(TreeNode tnode)
	{
		if (tnode.Level == 0)
		{
			applyTmpButton.Enabled = false;
			setTmpFields(Color.LightGray, clearText: true);
			this.temp_id.Text = tnode.Name;
			temp_name.Text = tnode.Text;
			temp_parent.Text = "";
			temp_parent.BackColor = Color.LightGray;
			temp_parent.Enabled = false;
		}
		else
		{
			applyTmpButton.Enabled = true;
			showTemplate(getTempIdFromNode(tnode));
		}
	}

	private void regPlanTemplate()
	{
		if (temp_name.Text.Length == 0)
		{
			MessageBox.Show(editingParent ? "分類名を入力してください" : "テンプレート名を入力してください");
			return;
		}
		string text;
		if (editingParent)
		{
			object duplicateParent = parentTable[temp_name.Text.Trim()];
			if (duplicateParent != null && (temp_id.Text.Length == 0 || duplicateParent.ToString() != temp_id.Text))
			{
				MessageBox.Show("同じ名前の分類が既に存在します");
				return;
			}
			text = ((temp_id.Text.Length <= 0) ? ("insert into AGREE_TEMPLATE (TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME, DELETE_FLAG) values (AGREE_TEMPLATE_SEQ.nextval, 0, 0, " + AgreeSql.SqlValue(temp_name.Text) + ", 0)") : ("update AGREE_TEMPLATE set TEMP_NAME = " + AgreeSql.SqlValue(temp_name.Text) + " where TEMP_ID = " + temp_id.Text));
		}
		else
		{
			object parentValue = parentTable[temp_parent.Text.Trim()];
			if (parentValue == null)
			{
				MessageBox.Show("分類をリストから選んでください");
				return;
			}
			string text2 = parentValue.ToString();
			text = ((temp_id.Text.Length <= 0) ? ("insert into AGREE_TEMPLATE (TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME, EYE , DIAG, ANES ,OPE, EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4,SHEET_NAME, DELETE_FLAG) values (AGREE_TEMPLATE_SEQ.nextval, 1, " + text2 + ", " + AgreeSql.SqlValue(temp_name.Text) + ", " + AgreeSql.SqlValue(eye.Text) + ", " + AgreeSql.SqlValue(diag.Text) + ", " + AgreeSql.SqlValue(anes.Text) + ", " + AgreeSql.SqlValue(ope.Text) + ", " + AgreeSql.SqlValue(explanation.Text) + ", " + AgreeSql.SqlValue(item1.Text) + ", " + AgreeSql.SqlValue(item2.Text) + ", " + AgreeSql.SqlValue(item3.Text) + ", " + AgreeSql.SqlValue(item4.Text) + ", " + AgreeSql.SqlValue(sheetName.Text) + ", 0)") : ("update AGREE_TEMPLATE set TEMP_NAME = " + AgreeSql.SqlValue(temp_name.Text) + ", TEMP_PARENT = " + text2 + ", EYE = " + AgreeSql.SqlValue(eye.Text) + ", DIAG = " + AgreeSql.SqlValue(diag.Text) + ", ANES = " + AgreeSql.SqlValue(anes.Text) + ", OPE = " + AgreeSql.SqlValue(ope.Text) + ", EXPLANATION = " + AgreeSql.SqlValue(explanation.Text) + ", ITEM1 = " + AgreeSql.SqlValue(item1.Text) + ", ITEM2 = " + AgreeSql.SqlValue(item2.Text) + ", ITEM3 = " + AgreeSql.SqlValue(item3.Text) + ", ITEM4 = " + AgreeSql.SqlValue(item4.Text) + ", SHEET_NAME = " + AgreeSql.SqlValue(sheetName.Text) + " where TEMP_ID = " + temp_id.Text));
		}
		try
		{
			oraConn.Open();
			oraCmd.CommandText = text;
			oraCmd.ExecuteNonQuery();
		}
		finally
		{
			if (oraConn.State != System.Data.ConnectionState.Closed)
			{
				oraConn.Close();
			}
		}
		setTmpFields(Color.LightGray, clearText: true);
		temp_parent.Text = "";
		temp_parent.BackColor = Color.LightGray;
		temp_parent.Enabled = false;
		if (editingParent)
		{
			loadParents();
		}
		MessageBox.Show("登録しました");
		initTree();
	}

	private void delPlanTemplate()
	{
		if (editingParent && temp_id.Text.Length > 0 && countChildTemplates(temp_id.Text) > 0)
		{
			MessageBox.Show("子テンプレートが登録されているため削除できません。\n先に子テンプレートを削除してください。");
			return;
		}
		if (MessageBox.Show("削除しますか？", "削除", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
		{
			return;
		}
		if (temp_id.Text.Length > 0)
		{
			try
			{
				oraConn.Open();
				oraCmd.CommandText = "update AGREE_TEMPLATE set DELETE_FLAG = 1 where TEMP_ID = " + temp_id.Text;
				oraCmd.ExecuteNonQuery();
			}
			finally
			{
				if (oraConn.State != System.Data.ConnectionState.Closed)
				{
					oraConn.Close();
				}
			}
		}
		setTmpFields(Color.LightGray, clearText: true);
		temp_parent.Text = "";
		temp_parent.BackColor = Color.LightGray;
		temp_parent.Enabled = false;
		if (editingParent)
		{
			loadParents();
		}
		MessageBox.Show("削除しました");
		initTree();
	}

	private int countChildTemplates(string parentId)
	{
		try
		{
			oraConn.Open();
			oraCmd.CommandText = "Select count(*) from AGREE_TEMPLATE where TEMP_PARENT = " + parentId + " and TEMP_LEVEL = 1 and DELETE_FLAG != 1";
			return Convert.ToInt32(oraCmd.ExecuteScalar());
		}
		finally
		{
			if (oraConn.State != System.Data.ConnectionState.Closed)
			{
				oraConn.Close();
			}
		}
	}

	private void applyTmpButton_Click(object sender, EventArgs e)
	{
		f1.applyTemplate(getTempIdFromNode(tmpAgreeTree.SelectedNode));
		Dispose();
	}

	private void regTmpButton_Click(object sender, EventArgs e)
	{
		regPlanTemplate();
	}

	private void delTmpButton_Click(object sender, EventArgs e)
	{
		delPlanTemplate();
	}

	private void closeButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void tmpPlanTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
	{
		bool proceed = true;
		if (regTmpButton.Enabled)
		{
			switch (MessageBox.Show("編集中のテンプレートがあります。保存しますか？", "テンプレート編集中", MessageBoxButtons.YesNoCancel))
			{
			case DialogResult.Yes:
				regPlanTemplate();
				proceed = true;
				break;
			case DialogResult.No:
				proceed = true;
				break;
			default:
				proceed = false;
				break;
			}
		}
		if (!proceed)
		{
			return;
		}
		editingParent = false;
		panel2.Enabled = true;
		addParentButton.Enabled = true;
		if (e.Node.Level == 0)
		{
			applyTmpButton.Enabled = false;
			newTmpButton.Enabled = true;
			editTmpButton.Enabled = true;
			regTmpButton.Enabled = false;
			delTmpButton.Enabled = false;
		}
		else
		{
			applyTmpButton.Enabled = true;
			newTmpButton.Enabled = false;
			editTmpButton.Enabled = true;
			regTmpButton.Enabled = false;
			delTmpButton.Enabled = false;
		}
		setTmpFields(Color.LightGray, clearText: false);
		temp_parent.BackColor = Color.LightGray;
		temp_parent.Enabled = false;
		showTemplate(e.Node);
	}

	private void newTmpButton_Click(object sender, EventArgs e)
	{
		if (tmpAgreeTree.SelectedNode == null)
		{
			return;
		}
		editingParent = false;
		panel2.Enabled = true;
		setTmpFields(Color.White, clearText: true);
		temp_parent.Text = tmpAgreeTree.SelectedNode.Text;
		temp_parent.BackColor = Color.White;
		temp_parent.Enabled = true;
		addParentButton.Enabled = false;
		applyTmpButton.Enabled = false;
		newTmpButton.Enabled = false;
		editTmpButton.Enabled = false;
		regTmpButton.Enabled = true;
		delTmpButton.Enabled = true;
	}

	private void editTmpButton_Click(object sender, EventArgs e)
	{
		TreeNode selectedNode = tmpAgreeTree.SelectedNode;
		editingParent = selectedNode != null && selectedNode.Level == 0;
		if (editingParent)
		{
			setTmpFields(Color.LightGray, clearText: false);
			temp_name.BackColor = Color.White;
			temp_parent.Text = "";
			temp_parent.BackColor = Color.LightGray;
			temp_parent.Enabled = false;
			panel2.Enabled = false;
		}
		else
		{
			setTmpFields(Color.White, clearText: false);
			temp_parent.BackColor = Color.White;
			temp_parent.Enabled = true;
			panel2.Enabled = true;
		}
		addParentButton.Enabled = false;
		applyTmpButton.Enabled = false;
		newTmpButton.Enabled = false;
		editTmpButton.Enabled = false;
		regTmpButton.Enabled = true;
		delTmpButton.Enabled = true;
	}

	private void addParentButton_Click(object sender, EventArgs e)
	{
		editingParent = true;
		setTmpFields(Color.LightGray, clearText: true);
		temp_name.BackColor = Color.White;
		temp_parent.Text = "";
		temp_parent.BackColor = Color.LightGray;
		temp_parent.Enabled = false;
		panel2.Enabled = false;
		addParentButton.Enabled = false;
		applyTmpButton.Enabled = false;
		newTmpButton.Enabled = false;
		editTmpButton.Enabled = false;
		regTmpButton.Enabled = true;
		delTmpButton.Enabled = false;
		temp_name.Focus();
	}

	private void upButton_Click(object sender, EventArgs e)
	{
		moveNode(-1);
	}

	private void downButton_Click(object sender, EventArgs e)
	{
		moveNode(1);
	}

	private void moveNode(int direction)
	{
		TreeNode selectedNode = tmpAgreeTree.SelectedNode;
		if (selectedNode == null)
		{
			return;
		}
		TreeNodeCollection siblings = ((selectedNode.Parent == null) ? tmpAgreeTree.Nodes : selectedNode.Parent.Nodes);
		int target = selectedNode.Index + direction;
		if (target < 0 || target >= siblings.Count)
		{
			return;
		}
		List<string> ids = new List<string>();
		foreach (TreeNode sibling in siblings)
		{
			ids.Add(sibling.Name);
		}
		string movingId = ids[selectedNode.Index];
		ids.RemoveAt(selectedNode.Index);
		ids.Insert(target, movingId);
		try
		{
			oraConn.Open();
			for (int i = 0; i < ids.Count; i++)
			{
				oraCmd.CommandText = "update AGREE_TEMPLATE set DISP_ORDER = " + i + " where TEMP_ID = " + ids[i];
				oraCmd.ExecuteNonQuery();
			}
		}
		finally
		{
			if (oraConn.State != System.Data.ConnectionState.Closed)
			{
				oraConn.Close();
			}
		}
		initTree();
		TreeNode[] movedNodes = tmpAgreeTree.Nodes.Find(movingId, searchAllChildren: true);
		if (movedNodes.Length > 0)
		{
			tmpAgreeTree.SelectedNode = movedNodes[0];
			movedNodes[0].EnsureVisible();
		}
	}

	private int getTempIdFromNode(TreeNode tnode)
	{
		int result = -1;
		if (tnode.Level == 0)
		{
			result = int.Parse(parentTable[tnode.Text].ToString());
		}
		else
		{
			int num = int.Parse(parentTable[tnode.Parent.Text].ToString());
			for (int i = 0; i < tmpNodeList.Count; i++)
			{
				if (tnode.Text == tmpNodeList[i].temp_name && num == tmpNodeList[i].temp_parent)
				{
					result = tmpNodeList[i].temp_id;
					break;
				}
			}
		}
		return result;
	}
}
