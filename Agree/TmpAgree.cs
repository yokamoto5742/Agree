using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using AgentlabUtilityLibrary;

namespace Agree;

public partial class TmpAgree : Form
{
	private Form1 f1;

	// 分類（TEMP_LEVEL = 0）の 名前→ID と ID→名前。名前→ID のキーは前後の空白を除いて登録する。
	private Dictionary<string, int> parentIds = new Dictionary<string, int>();

	private Dictionary<int, string> parentNames = new Dictionary<int, string>();

	private List<TextBox> tmpBoxList = new List<TextBox>();

	private List<ComboBox> tmpComboList = new List<ComboBox>();

	private OleDbConnection oraConn;

	private bool editingParent;

	public TmpAgree(Form1 F1, bool applyButtonVisible)
	{
		InitializeComponent();
		f1 = F1;
		applyTmpButton.Visible = applyButtonVisible;
	}

	private void TmpAgree_Load(object sender, EventArgs e)
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
			setButtons(apply: false, create: false, edit: false, register: false, delete: false, addParent: false);
			upButton.Enabled = false;
			downButton.Enabled = false;
			temp_parent.Enabled = false;
			return;
		}
		oraConn = DBConn.GetOpenDBConn();
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

	private void setButtons(bool apply, bool create, bool edit, bool register, bool delete, bool addParent)
	{
		applyTmpButton.Enabled = apply;
		newTmpButton.Enabled = create;
		editTmpButton.Enabled = edit;
		regTmpButton.Enabled = register;
		delTmpButton.Enabled = delete;
		addParentButton.Enabled = addParent;
	}

	private void lockParentCombo()
	{
		temp_parent.Text = "";
		temp_parent.BackColor = Color.LightGray;
		temp_parent.Enabled = false;
	}

	private void loadParents()
	{
		temp_parent.Items.Clear();
		parentIds.Clear();
		parentNames.Clear();
		Db.Read(oraConn, "Select TEMP_ID, TEMP_NAME from AGREE_TEMPLATE where TEMP_LEVEL = 0 and DELETE_FLAG != 1 order by DISP_ORDER", r =>
		{
			int id = int.Parse(r["TEMP_ID"].ToString());
			string name = r["TEMP_NAME"].ToString();
			temp_parent.Items.Add(name);
			parentIds.Add(name.Trim(), id);
			parentNames.Add(id, name);
		});
	}

	private void initTree()
	{
		tmpAgreeTree.Nodes.Clear();
		// ノードの Name に TEMP_ID を持たせる（getTempIdFromNode で使う）。
		Db.Read(oraConn, "Select TEMP_ID, TEMP_LEVEL, TEMP_NAME, TEMP_PARENT from AGREE_TEMPLATE where DELETE_FLAG != 1 order by TEMP_LEVEL , DISP_ORDER , TEMP_ID", r =>
		{
			string id = r["TEMP_ID"].ToString();
			string name = r["TEMP_NAME"].ToString();
			string parentKey = r["TEMP_PARENT"].ToString();
			if (r["TEMP_LEVEL"].ToString() == "0")
			{
				tmpAgreeTree.Nodes.Add(id, name);
			}
			else if (id != parentKey)
			{
				if (tmpAgreeTree.Nodes.ContainsKey(parentKey))
				{
					tmpAgreeTree.Nodes[parentKey].Nodes.Add(id, name);
				}
				else
				{
					tmpAgreeTree.Nodes.Add(id, name);
				}
			}
		});
		editingParent = false;
		panel2.Enabled = true;
		setButtons(apply: false, create: false, edit: false, register: false, delete: false, addParent: true);
		setTmpFields(Color.LightGray, clearText: false);
		temp_parent.Enabled = false;
	}

	private void showTemplate(int temp_id)
	{
		Db.Read(oraConn, "Select TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME,EYE , DIAG, ANES ,OPE, EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4 , SHEET_NAME from AGREE_TEMPLATE where TEMP_ID = " + temp_id, r =>
		{
			this.temp_id.Text = r["TEMP_ID"].ToString();
			temp_name.Text = r["TEMP_NAME"].ToString();
			if (r["TEMP_LEVEL"].ToString() == "1")
			{
				temp_parent.Text = parentNames[int.Parse(r["TEMP_PARENT"].ToString())];
			}
			eye.Text = r["EYE"].ToString();
			sheetName.Text = r["SHEET_NAME"].ToString();
			diag.Text = r["DIAG"].ToString();
			anes.Text = r["ANES"].ToString();
			ope.Text = r["OPE"].ToString();
			explanation.Text = r["EXPLANATION"].ToString();
			item1.Text = r["ITEM1"].ToString();
			item2.Text = r["ITEM2"].ToString();
			item3.Text = r["ITEM3"].ToString();
			item4.Text = r["ITEM4"].ToString();
		});
	}

	private void showTemplate(TreeNode tnode)
	{
		if (tnode.Level == 0)
		{
			applyTmpButton.Enabled = false;
			setTmpFields(Color.LightGray, clearText: true);
			this.temp_id.Text = tnode.Name;
			temp_name.Text = tnode.Text;
			lockParentCombo();
		}
		else
		{
			applyTmpButton.Enabled = true;
			showTemplate(getTempIdFromNode(tnode));
		}
	}

	/// <summary>
	/// 入力内容を AGREE_TEMPLATE に登録する。入力チェックで中断した場合は false を返す。
	/// </summary>
	private bool regAgreeTemplate()
	{
		if (temp_name.Text.Length == 0)
		{
			MessageBox.Show(editingParent ? "分類名を入力してください" : "テンプレート名を入力してください");
			return false;
		}
		string sql;
		if (editingParent)
		{
			if (parentIds.TryGetValue(temp_name.Text.Trim(), out int duplicateId) && (temp_id.Text.Length == 0 || duplicateId.ToString() != temp_id.Text))
			{
				MessageBox.Show("同じ名前の分類が既に存在します");
				return false;
			}
			sql = ((temp_id.Text.Length <= 0) ? ("insert into AGREE_TEMPLATE (TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME, DELETE_FLAG) values (AGREE_TEMPLATE_SEQ.nextval, 0, 0, " + AgreeSql.SqlValue(temp_name.Text) + ", 0)") : ("update AGREE_TEMPLATE set TEMP_NAME = " + AgreeSql.SqlValue(temp_name.Text) + " where TEMP_ID = " + temp_id.Text));
		}
		else
		{
			if (!parentIds.TryGetValue(temp_parent.Text.Trim(), out int parentId))
			{
				MessageBox.Show("分類をリストから選んでください");
				return false;
			}
			sql = ((temp_id.Text.Length <= 0) ? ("insert into AGREE_TEMPLATE (TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME, EYE , DIAG, ANES ,OPE, EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4,SHEET_NAME, DELETE_FLAG) values (AGREE_TEMPLATE_SEQ.nextval, 1, " + parentId + ", " + AgreeSql.SqlValue(temp_name.Text) + ", " + AgreeSql.SqlValue(eye.Text) + ", " + AgreeSql.SqlValue(diag.Text) + ", " + AgreeSql.SqlValue(anes.Text) + ", " + AgreeSql.SqlValue(ope.Text) + ", " + AgreeSql.SqlValue(explanation.Text) + ", " + AgreeSql.SqlValue(item1.Text) + ", " + AgreeSql.SqlValue(item2.Text) + ", " + AgreeSql.SqlValue(item3.Text) + ", " + AgreeSql.SqlValue(item4.Text) + ", " + AgreeSql.SqlValue(sheetName.Text) + ", 0)") : ("update AGREE_TEMPLATE set TEMP_NAME = " + AgreeSql.SqlValue(temp_name.Text) + ", TEMP_PARENT = " + parentId + ", EYE = " + AgreeSql.SqlValue(eye.Text) + ", DIAG = " + AgreeSql.SqlValue(diag.Text) + ", ANES = " + AgreeSql.SqlValue(anes.Text) + ", OPE = " + AgreeSql.SqlValue(ope.Text) + ", EXPLANATION = " + AgreeSql.SqlValue(explanation.Text) + ", ITEM1 = " + AgreeSql.SqlValue(item1.Text) + ", ITEM2 = " + AgreeSql.SqlValue(item2.Text) + ", ITEM3 = " + AgreeSql.SqlValue(item3.Text) + ", ITEM4 = " + AgreeSql.SqlValue(item4.Text) + ", SHEET_NAME = " + AgreeSql.SqlValue(sheetName.Text) + " where TEMP_ID = " + temp_id.Text));
		}
		Db.Execute(oraConn, sql);
		setTmpFields(Color.LightGray, clearText: true);
		lockParentCombo();
		if (editingParent)
		{
			loadParents();
		}
		MessageBox.Show("登録しました");
		initTree();
		return true;
	}

	private void delAgreeTemplate()
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
			Db.Execute(oraConn, "update AGREE_TEMPLATE set DELETE_FLAG = 1 where TEMP_ID = " + temp_id.Text);
		}
		setTmpFields(Color.LightGray, clearText: true);
		lockParentCombo();
		if (editingParent)
		{
			loadParents();
		}
		MessageBox.Show("削除しました");
		initTree();
	}

	private int countChildTemplates(string parentId)
	{
		return Convert.ToInt32(Db.Scalar(oraConn, "Select count(*) from AGREE_TEMPLATE where TEMP_PARENT = " + parentId + " and TEMP_LEVEL = 1 and DELETE_FLAG != 1"));
	}

	private void applyTmpButton_Click(object sender, EventArgs e)
	{
		f1.applyTemplate(getTempIdFromNode(tmpAgreeTree.SelectedNode));
		Dispose();
	}

	private void regTmpButton_Click(object sender, EventArgs e)
	{
		regAgreeTemplate();
	}

	private void delTmpButton_Click(object sender, EventArgs e)
	{
		delAgreeTemplate();
	}

	private void closeButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void tmpAgreeTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
	{
		if (regTmpButton.Enabled)
		{
			switch (MessageBox.Show("編集中のテンプレートがあります。保存しますか？", "テンプレート編集中", MessageBoxButtons.YesNoCancel))
			{
			case DialogResult.Yes:
				// 保存が入力チェックで中断した場合は、編集内容を残すため切り替えない。
				if (!regAgreeTemplate())
				{
					return;
				}
				break;
			case DialogResult.No:
				break;
			default:
				return;
			}
		}
		editingParent = false;
		panel2.Enabled = true;
		bool isParent = e.Node.Level == 0;
		setButtons(apply: !isParent, create: isParent, edit: true, register: false, delete: false, addParent: true);
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
		setButtons(apply: false, create: false, edit: false, register: true, delete: true, addParent: false);
	}

	private void editTmpButton_Click(object sender, EventArgs e)
	{
		TreeNode selectedNode = tmpAgreeTree.SelectedNode;
		editingParent = selectedNode != null && selectedNode.Level == 0;
		if (editingParent)
		{
			setTmpFields(Color.LightGray, clearText: false);
			temp_name.BackColor = Color.White;
			lockParentCombo();
			panel2.Enabled = false;
		}
		else
		{
			setTmpFields(Color.White, clearText: false);
			temp_parent.BackColor = Color.White;
			temp_parent.Enabled = true;
			panel2.Enabled = true;
		}
		setButtons(apply: false, create: false, edit: false, register: true, delete: true, addParent: false);
	}

	private void addParentButton_Click(object sender, EventArgs e)
	{
		editingParent = true;
		setTmpFields(Color.LightGray, clearText: true);
		temp_name.BackColor = Color.White;
		lockParentCombo();
		panel2.Enabled = false;
		setButtons(apply: false, create: false, edit: false, register: true, delete: false, addParent: false);
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
		for (int i = 0; i < ids.Count; i++)
		{
			Db.Execute(oraConn, "update AGREE_TEMPLATE set DISP_ORDER = " + i + " where TEMP_ID = " + ids[i]);
		}
		initTree();
		TreeNode[] movedNodes = tmpAgreeTree.Nodes.Find(movingId, searchAllChildren: true);
		if (movedNodes.Length > 0)
		{
			tmpAgreeTree.SelectedNode = movedNodes[0];
			movedNodes[0].EnsureVisible();
		}
	}

	// ノードの Name には initTree で TEMP_ID を入れている。表示名で探すと同名テンプレートを取り違えるため使わない。
	private int getTempIdFromNode(TreeNode tnode)
	{
		return int.Parse(tnode.Name);
	}
}
