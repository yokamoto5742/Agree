using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using AgentlabUtilityLibrary;

namespace Agree;

public class TmpAgree : Form
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

	private IContainer components;

	private TreeView tmpAgreeTree;

	private Label label1;

	private Label label15;

	private TextBox item3;

	private Label label13;

	private TextBox item2;

	private Label label10;

	private TextBox item1;

	private Label label9;

	private TextBox explanation;

	private Label label8;

	private TextBox ope;

	private Label label7;

	private TextBox diag;

	private Label label2;

	private TextBox temp_name;

	private Button regTmpButton;

	private Button delTmpButton;

	private Button closeButton;

	private Button applyTmpButton;

	private TextBox temp_id;

	private Label label4;

	private Button newTmpButton;

	private Button editTmpButton;

	private Button upButton;

	private Button downButton;

	private Button addParentButton;

	private bool editingParent;

	private ComboBox temp_parent;

	private Label label5;

	private Panel panel2;

	private Label label22;

	private Label label3;

	private TextBox item4;

	private ComboBox eye;

	private ComboBox sheetName;

	private Label label6;

	private Label label11;

	private TextBox anes;

	public TmpAgree(Form1 F1, bool applyButtonVisible)
	{
		InitializeComponent();
		f1 = F1;
		if (applyButtonVisible)
		{
			applyTmpButton.Visible = true;
		}
		else
		{
			applyTmpButton.Visible = false;
		}
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
		foreach (TextBox tmpBox in tmpBoxList)
		{
			tmpBox.BackColor = Color.LightGray;
		}
		foreach (ComboBox tmpCombo in tmpComboList)
		{
			tmpCombo.BackColor = Color.LightGray;
		}
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
			foreach (TextBox tmpBox in tmpBoxList)
			{
				tmpBox.Text = "";
				tmpBox.BackColor = Color.LightGray;
			}
			foreach (ComboBox tmpCombo in tmpComboList)
			{
				tmpCombo.Text = "";
				tmpCombo.BackColor = Color.LightGray;
			}
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
			text = ((temp_id.Text.Length <= 0) ? ("insert into AGREE_TEMPLATE (TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME, DELETE_FLAG) values (AGREE_TEMPLATE_SEQ.nextval, 0, 0, '" + temp_name.Text + "', 0)") : ("update AGREE_TEMPLATE set TEMP_NAME = '" + temp_name.Text + "' where TEMP_ID = " + temp_id.Text));
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
			text = ((temp_id.Text.Length <= 0) ? ("insert into AGREE_TEMPLATE (TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME, EYE , DIAG, ANES ,OPE, EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4,SHEET_NAME, DELETE_FLAG) values (AGREE_TEMPLATE_SEQ.nextval, 1, " + text2 + ", '" + temp_name.Text + "', '" + eye.Text + "', '" + diag.Text + "', '" + anes.Text + "', '" + ope.Text + "', '" + explanation.Text + "', '" + item1.Text + "', '" + item2.Text + "', '" + item3.Text + "', '" + item4.Text + "', '" + sheetName.Text + "', 0)") : ("update AGREE_TEMPLATE set TEMP_NAME = '" + temp_name.Text + "', TEMP_PARENT = " + text2 + ", EYE = '" + eye.Text + "', DIAG = '" + diag.Text + "', ANES = '" + anes.Text + "', OPE = '" + ope.Text + "', EXPLANATION = '" + explanation.Text + "', ITEM1 = '" + item1.Text + "', ITEM2 = '" + item2.Text + "', ITEM3 = '" + item3.Text + "', ITEM4 = '" + item4.Text + "', SHEET_NAME = '" + sheetName.Text + "' where TEMP_ID = " + temp_id.Text));
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
		foreach (TextBox tmpBox in tmpBoxList)
		{
			tmpBox.Text = "";
			tmpBox.BackColor = Color.LightGray;
		}
		foreach (ComboBox tmpCombo in tmpComboList)
		{
			tmpCombo.Text = "";
			tmpCombo.BackColor = Color.LightGray;
		}
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
		foreach (TextBox tmpBox in tmpBoxList)
		{
			tmpBox.Text = "";
			tmpBox.BackColor = Color.LightGray;
		}
		foreach (ComboBox tmpCombo in tmpComboList)
		{
			tmpCombo.Text = "";
			tmpCombo.BackColor = Color.LightGray;
		}
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
		bool flag = true;
		if (regTmpButton.Enabled)
		{
			switch (MessageBox.Show("編集中のテンプレートがあります。保存しますか？", "テンプレート編集中", MessageBoxButtons.YesNoCancel))
			{
			case DialogResult.Yes:
				regPlanTemplate();
				flag = true;
				break;
			case DialogResult.No:
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
		}
		if (!flag)
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
		foreach (TextBox tmpBox in tmpBoxList)
		{
			tmpBox.BackColor = Color.LightGray;
		}
		foreach (ComboBox tmpCombo in tmpComboList)
		{
			tmpCombo.BackColor = Color.LightGray;
		}
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
		foreach (TextBox tmpBox in tmpBoxList)
		{
			tmpBox.Text = "";
			tmpBox.BackColor = Color.White;
		}
		foreach (ComboBox tmpCombo in tmpComboList)
		{
			tmpCombo.Text = "";
			tmpCombo.BackColor = Color.White;
		}
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
			foreach (TextBox tmpBox in tmpBoxList)
			{
				tmpBox.BackColor = Color.LightGray;
			}
			foreach (ComboBox tmpCombo in tmpComboList)
			{
				tmpCombo.BackColor = Color.LightGray;
			}
			temp_name.BackColor = Color.White;
			temp_parent.Text = "";
			temp_parent.BackColor = Color.LightGray;
			temp_parent.Enabled = false;
			panel2.Enabled = false;
		}
		else
		{
			foreach (TextBox tmpBox in tmpBoxList)
			{
				tmpBox.BackColor = Color.White;
			}
			foreach (ComboBox tmpCombo in tmpComboList)
			{
				tmpCombo.BackColor = Color.White;
			}
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
		foreach (TextBox tmpBox in tmpBoxList)
		{
			tmpBox.Text = "";
			tmpBox.BackColor = Color.LightGray;
		}
		foreach (ComboBox tmpCombo in tmpComboList)
		{
			tmpCombo.Text = "";
			tmpCombo.BackColor = Color.LightGray;
		}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Agree.TmpAgree));
		this.tmpAgreeTree = new System.Windows.Forms.TreeView();
		this.label1 = new System.Windows.Forms.Label();
		this.label15 = new System.Windows.Forms.Label();
		this.item3 = new System.Windows.Forms.TextBox();
		this.label13 = new System.Windows.Forms.Label();
		this.item2 = new System.Windows.Forms.TextBox();
		this.label10 = new System.Windows.Forms.Label();
		this.item1 = new System.Windows.Forms.TextBox();
		this.label9 = new System.Windows.Forms.Label();
		this.explanation = new System.Windows.Forms.TextBox();
		this.label8 = new System.Windows.Forms.Label();
		this.ope = new System.Windows.Forms.TextBox();
		this.label7 = new System.Windows.Forms.Label();
		this.diag = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.temp_name = new System.Windows.Forms.TextBox();
		this.regTmpButton = new System.Windows.Forms.Button();
		this.delTmpButton = new System.Windows.Forms.Button();
		this.closeButton = new System.Windows.Forms.Button();
		this.applyTmpButton = new System.Windows.Forms.Button();
		this.temp_id = new System.Windows.Forms.TextBox();
		this.label4 = new System.Windows.Forms.Label();
		this.newTmpButton = new System.Windows.Forms.Button();
		this.editTmpButton = new System.Windows.Forms.Button();
		this.upButton = new System.Windows.Forms.Button();
		this.downButton = new System.Windows.Forms.Button();
		this.addParentButton = new System.Windows.Forms.Button();
		this.temp_parent = new System.Windows.Forms.ComboBox();
		this.label5 = new System.Windows.Forms.Label();
		this.panel2 = new System.Windows.Forms.Panel();
		this.label11 = new System.Windows.Forms.Label();
		this.anes = new System.Windows.Forms.TextBox();
		this.label6 = new System.Windows.Forms.Label();
		this.sheetName = new System.Windows.Forms.ComboBox();
		this.eye = new System.Windows.Forms.ComboBox();
		this.label3 = new System.Windows.Forms.Label();
		this.item4 = new System.Windows.Forms.TextBox();
		this.label22 = new System.Windows.Forms.Label();
		this.panel2.SuspendLayout();
		base.SuspendLayout();
		this.tmpAgreeTree.Location = new System.Drawing.Point(14, 24);
		this.tmpAgreeTree.Name = "tmpAgreeTree";
		this.tmpAgreeTree.Size = new System.Drawing.Size(233, 474);
		this.tmpAgreeTree.TabIndex = 0;
		this.tmpAgreeTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(tmpPlanTree_NodeMouseClick);
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(12, 9);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(175, 12);
		this.label1.TabIndex = 1;
		this.label1.Text = "リストからテンプレートを選んでください";
		this.label15.AutoSize = true;
		this.label15.Location = new System.Drawing.Point(11, 294);
		this.label15.Name = "label15";
		this.label15.Size = new System.Drawing.Size(53, 12);
		this.label15.TabIndex = 46;
		this.label15.Text = "検査内容";
		this.item3.Location = new System.Drawing.Point(91, 291);
		this.item3.MaxLength = 200;
		this.item3.Multiline = true;
		this.item3.Name = "item3";
		this.item3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.item3.Size = new System.Drawing.Size(400, 44);
		this.item3.TabIndex = 45;
		this.label13.AutoSize = true;
		this.label13.Location = new System.Drawing.Point(11, 245);
		this.label13.Name = "label13";
		this.label13.Size = new System.Drawing.Size(53, 12);
		this.label13.TabIndex = 43;
		this.label13.Text = "治療計画";
		this.item2.Location = new System.Drawing.Point(91, 240);
		this.item2.MaxLength = 200;
		this.item2.Multiline = true;
		this.item2.Name = "item2";
		this.item2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.item2.Size = new System.Drawing.Size(400, 45);
		this.item2.TabIndex = 42;
		this.label10.AutoSize = true;
		this.label10.Location = new System.Drawing.Point(11, 193);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(29, 12);
		this.label10.TabIndex = 40;
		this.label10.Text = "症状";
		this.item1.Location = new System.Drawing.Point(91, 189);
		this.item1.MaxLength = 200;
		this.item1.Multiline = true;
		this.item1.Name = "item1";
		this.item1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.item1.Size = new System.Drawing.Size(400, 45);
		this.item1.TabIndex = 39;
		this.label9.AutoSize = true;
		this.label9.Location = new System.Drawing.Point(11, 121);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(29, 12);
		this.label9.TabIndex = 38;
		this.label9.Text = "説明";
		this.explanation.Location = new System.Drawing.Point(91, 117);
		this.explanation.MaxLength = 1200;
		this.explanation.Multiline = true;
		this.explanation.Name = "explanation";
		this.explanation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.explanation.Size = new System.Drawing.Size(400, 66);
		this.explanation.TabIndex = 37;
		this.label8.AutoSize = true;
		this.label8.Location = new System.Drawing.Point(11, 84);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(71, 12);
		this.label8.TabIndex = 36;
		this.label8.Text = "手術・検査名";
		this.ope.Location = new System.Drawing.Point(91, 81);
		this.ope.MaxLength = 200;
		this.ope.Multiline = true;
		this.ope.Name = "ope";
		this.ope.Size = new System.Drawing.Size(400, 30);
		this.ope.TabIndex = 35;
		this.label7.AutoSize = true;
		this.label7.Location = new System.Drawing.Point(11, 31);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(29, 12);
		this.label7.TabIndex = 34;
		this.label7.Text = "病名";
		this.diag.Location = new System.Drawing.Point(91, 28);
		this.diag.MaxLength = 200;
		this.diag.Name = "diag";
		this.diag.Size = new System.Drawing.Size(400, 19);
		this.diag.TabIndex = 33;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(266, 65);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(71, 12);
		this.label2.TabIndex = 51;
		this.label2.Text = "テンプレート名";
		this.temp_name.BackColor = System.Drawing.SystemColors.Window;
		this.temp_name.Location = new System.Drawing.Point(346, 61);
		this.temp_name.MaxLength = 20;
		this.temp_name.Name = "temp_name";
		this.temp_name.Size = new System.Drawing.Size(211, 19);
		this.temp_name.TabIndex = 50;
		this.regTmpButton.Location = new System.Drawing.Point(413, 504);
		this.regTmpButton.Name = "regTmpButton";
		this.regTmpButton.Size = new System.Drawing.Size(70, 26);
		this.regTmpButton.TabIndex = 55;
		this.regTmpButton.Text = "登録";
		this.regTmpButton.UseVisualStyleBackColor = true;
		this.regTmpButton.Click += new System.EventHandler(regTmpButton_Click);
		this.delTmpButton.Location = new System.Drawing.Point(489, 504);
		this.delTmpButton.Name = "delTmpButton";
		this.delTmpButton.Size = new System.Drawing.Size(70, 26);
		this.delTmpButton.TabIndex = 56;
		this.delTmpButton.Text = "削除";
		this.delTmpButton.UseVisualStyleBackColor = true;
		this.delTmpButton.Click += new System.EventHandler(delTmpButton_Click);
		this.closeButton.Location = new System.Drawing.Point(658, 504);
		this.closeButton.Name = "closeButton";
		this.closeButton.Size = new System.Drawing.Size(84, 26);
		this.closeButton.TabIndex = 57;
		this.closeButton.Text = "閉じる";
		this.closeButton.UseVisualStyleBackColor = true;
		this.closeButton.Click += new System.EventHandler(closeButton_Click);
		this.applyTmpButton.Location = new System.Drawing.Point(14, 504);
		this.applyTmpButton.Name = "applyTmpButton";
		this.applyTmpButton.Size = new System.Drawing.Size(69, 26);
		this.applyTmpButton.TabIndex = 58;
		this.applyTmpButton.Text = "適用";
		this.applyTmpButton.UseVisualStyleBackColor = true;
		this.applyTmpButton.Click += new System.EventHandler(applyTmpButton_Click);
		this.temp_id.Location = new System.Drawing.Point(697, 6);
		this.temp_id.MaxLength = 25;
		this.temp_id.Name = "temp_id";
		this.temp_id.ReadOnly = true;
		this.temp_id.Size = new System.Drawing.Size(60, 19);
		this.temp_id.TabIndex = 59;
		this.temp_id.Visible = false;
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(266, 38);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(29, 12);
		this.label4.TabIndex = 63;
		this.label4.Text = "分類";
		this.newTmpButton.Location = new System.Drawing.Point(171, 504);
		this.newTmpButton.Name = "newTmpButton";
		this.newTmpButton.Size = new System.Drawing.Size(69, 26);
		this.newTmpButton.TabIndex = 65;
		this.newTmpButton.Text = "新規作成";
		this.newTmpButton.UseVisualStyleBackColor = true;
		this.newTmpButton.Click += new System.EventHandler(newTmpButton_Click);
		this.editTmpButton.Location = new System.Drawing.Point(93, 504);
		this.editTmpButton.Name = "editTmpButton";
		this.editTmpButton.Size = new System.Drawing.Size(69, 26);
		this.editTmpButton.TabIndex = 66;
		this.editTmpButton.Text = "編集";
		this.editTmpButton.UseVisualStyleBackColor = true;
		this.editTmpButton.Click += new System.EventHandler(editTmpButton_Click);
		this.upButton.Location = new System.Drawing.Point(250, 504);
		this.upButton.Name = "upButton";
		this.upButton.Size = new System.Drawing.Size(69, 26);
		this.upButton.TabIndex = 80;
		this.upButton.Text = "上へ";
		this.upButton.UseVisualStyleBackColor = true;
		this.upButton.Click += new System.EventHandler(upButton_Click);
		this.downButton.Location = new System.Drawing.Point(326, 504);
		this.downButton.Name = "downButton";
		this.downButton.Size = new System.Drawing.Size(69, 26);
		this.downButton.TabIndex = 81;
		this.downButton.Text = "下へ";
		this.downButton.UseVisualStyleBackColor = true;
		this.downButton.Click += new System.EventHandler(downButton_Click);
		this.addParentButton.Location = new System.Drawing.Point(565, 504);
		this.addParentButton.Name = "addParentButton";
		this.addParentButton.Size = new System.Drawing.Size(84, 26);
		this.addParentButton.TabIndex = 82;
		this.addParentButton.Text = "分類追加";
		this.addParentButton.UseVisualStyleBackColor = true;
		this.addParentButton.Click += new System.EventHandler(addParentButton_Click);
		this.temp_parent.FormattingEnabled = true;
		this.temp_parent.Location = new System.Drawing.Point(346, 35);
		this.temp_parent.Name = "temp_parent";
		this.temp_parent.Size = new System.Drawing.Size(148, 20);
		this.temp_parent.TabIndex = 67;
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(266, 9);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(254, 12);
		this.label5.TabIndex = 68;
		this.label5.Text = "変更するには左下の「編集」ボタンをクリックしてください";
		this.panel2.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
		this.panel2.Controls.Add(this.label11);
		this.panel2.Controls.Add(this.anes);
		this.panel2.Controls.Add(this.label6);
		this.panel2.Controls.Add(this.sheetName);
		this.panel2.Controls.Add(this.eye);
		this.panel2.Controls.Add(this.label3);
		this.panel2.Controls.Add(this.item4);
		this.panel2.Controls.Add(this.label22);
		this.panel2.Controls.Add(this.label15);
		this.panel2.Controls.Add(this.item3);
		this.panel2.Controls.Add(this.label13);
		this.panel2.Controls.Add(this.item2);
		this.panel2.Controls.Add(this.label10);
		this.panel2.Controls.Add(this.item1);
		this.panel2.Controls.Add(this.label9);
		this.panel2.Controls.Add(this.explanation);
		this.panel2.Controls.Add(this.label8);
		this.panel2.Controls.Add(this.ope);
		this.panel2.Controls.Add(this.label7);
		this.panel2.Controls.Add(this.diag);
		this.panel2.Location = new System.Drawing.Point(257, 86);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(505, 412);
		this.panel2.TabIndex = 70;
		this.label11.AutoSize = true;
		this.label11.Location = new System.Drawing.Point(11, 56);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(63, 12);
		this.label11.TabIndex = 78;
		this.label11.Text = "麻酔の形式";
		this.anes.Location = new System.Drawing.Point(91, 53);
		this.anes.MaxLength = 200;
		this.anes.Name = "anes";
		this.anes.Size = new System.Drawing.Size(400, 19);
		this.anes.TabIndex = 77;
		this.label6.AutoSize = true;
		this.label6.Font = new System.Drawing.Font("MS UI Gothic", 9f);
		this.label6.Location = new System.Drawing.Point(272, 9);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(33, 12);
		this.label6.TabIndex = 76;
		this.label6.Text = "シート";
		this.sheetName.FormattingEnabled = true;
		this.sheetName.Items.AddRange(new object[5] { "日帰り", "入院", "短期滞在", "注射", "検査同意書" });
		this.sheetName.Location = new System.Drawing.Point(332, 3);
		this.sheetName.MaxLength = 100;
		this.sheetName.Name = "sheetName";
		this.sheetName.Size = new System.Drawing.Size(159, 20);
		this.sheetName.TabIndex = 75;
		this.eye.FormattingEnabled = true;
		this.eye.Items.AddRange(new object[4] { "", "右", "左", "両" });
		this.eye.Location = new System.Drawing.Point(91, 3);
		this.eye.MaxLength = 40;
		this.eye.Name = "eye";
		this.eye.Size = new System.Drawing.Size(121, 20);
		this.eye.TabIndex = 74;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(11, 344);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(53, 12);
		this.label3.TabIndex = 73;
		this.label3.Text = "手術内容";
		this.item4.Location = new System.Drawing.Point(91, 341);
		this.item4.MaxLength = 500;
		this.item4.Multiline = true;
		this.item4.Name = "item4";
		this.item4.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.item4.Size = new System.Drawing.Size(400, 45);
		this.item4.TabIndex = 72;
		this.label22.AutoSize = true;
		this.label22.Font = new System.Drawing.Font("MS UI Gothic", 9f);
		this.label22.Location = new System.Drawing.Point(11, 9);
		this.label22.Name = "label22";
		this.label22.Size = new System.Drawing.Size(17, 12);
		this.label22.TabIndex = 71;
		this.label22.Text = "眼";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(769, 542);
		base.Controls.Add(this.panel2);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.temp_parent);
		base.Controls.Add(this.editTmpButton);
		base.Controls.Add(this.upButton);
		base.Controls.Add(this.downButton);
		base.Controls.Add(this.addParentButton);
		base.Controls.Add(this.newTmpButton);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.temp_id);
		base.Controls.Add(this.closeButton);
		base.Controls.Add(this.delTmpButton);
		base.Controls.Add(this.regTmpButton);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.applyTmpButton);
		base.Controls.Add(this.temp_name);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.tmpAgreeTree);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "TmpAgree";
		this.Text = "テンプレート";
		base.Load += new System.EventHandler(TmpPlan_Load);
		this.panel2.ResumeLayout(false);
		this.panel2.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
