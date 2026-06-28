using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AgentlabUtilityLibrary;
using Microsoft.VisualBasic.FileIO;

namespace Agree;

public partial class Form1
{
	private void showList()
	{
		if (Program.OfflineMode)
		{
			return;
		}
		if (pt_id.Text.Length > 0)
		{
			clearPlan();
			oraConn.Open();
			oraCmd.CommandText = "Select P_NAME,P_KANA,P_SEX from M_PATIENT" + Env.DB_LINK + " where P_ID = " + pt_id.Text.Trim();
			oraReader = oraCmd.ExecuteReader();
			string selectCommandText = "Select AGREE_ID, SAVE_DATE, AGREE.DEPT, Trim(M_DEPT.S_NAME), AGREE.DR, Trim(M_USR.NAME), '' ,  STAFF, EYE,  DIAG,  OPE, EXPLANATION,ITEM1,ITEM2,ITEM3,ITEM4, DR_OK ,SHEET_NAME,'',case when DR_OK = 1 then '○' else '-' end as 医師完了 , ANES  from AGREE inner join M_DEPT" + Env.DB_LINK + " on AGREE.DEPT = M_DEPT.CODE inner join M_USR" + Env.DB_LINK + " on AGREE.DR = M_USR.CODE where PATIENT_ID = " + pt_id.Text.Trim() + " and DELETE_FLAG = 0 order by SAVE_DATE desc";
			OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(selectCommandText, oraConn);
			if (oraReader.Read())
			{
				pt_name.Text = oraReader[0].ToString();
				pt_kana.Text = oraReader[1].ToString();
				if (oraReader[2].ToString() == "2")
				{
					pt_sex.Text = "女";
				}
				else
				{
					pt_sex.Text = "男";
				}
			}
			oraReader.Close();
			oraConn.Close();
			DataSet dataSet = new DataSet();
			oleDbDataAdapter.Fill(dataSet, "同意書");
			AgreeList.DataSource = dataSet.Tables["同意書"];
			AgreeList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			AgreeList.Columns[0].Visible = false;
			AgreeList.Columns[1].HeaderText = "作成日";
			AgreeList.Columns[1].Width = 80;
			AgreeList.Columns[1].DefaultCellStyle.Format = "0000/00/00";
			AgreeList.Columns[2].Visible = false;
			AgreeList.Columns[3].HeaderText = "診療科";
			AgreeList.Columns[3].Width = 80;
			AgreeList.Columns[4].Visible = false;
			AgreeList.Columns[5].HeaderText = "医師";
			AgreeList.Columns[5].Width = 80;
			AgreeList.Columns[6].Visible = false;
			AgreeList.Columns[7].Visible = false;
			AgreeList.Columns[8].HeaderText = "眼";
			AgreeList.Columns[8].Width = 40;
			AgreeList.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			AgreeList.Columns[9].Visible = false;
			AgreeList.Columns[10].HeaderText = "手術";
			AgreeList.Columns[10].Width = 150;
			AgreeList.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			AgreeList.Columns[11].Visible = false;
			AgreeList.Columns[12].Visible = false;
			AgreeList.Columns[13].Visible = false;
			AgreeList.Columns[14].Visible = false;
			AgreeList.Columns[15].Visible = false;
			AgreeList.Columns[16].Visible = false;
			AgreeList.Columns[17].Visible = false;
			AgreeList.Columns[18].Visible = false;
			AgreeList.Columns[19].Visible = false;
			AgreeList.Columns[20].Visible = false;
			clearPlan();
			printAgreeButton.Enabled = false;
			if (AgreeList.RowCount > 0)
			{
				agreePlanListLabel.Text = "同意書をクリックすると内容が表示されます。";
			}
			else
			{
				agreePlanListLabel.Text = "既存の同意書はありません。新規作成してください。";
			}
		}
		else
		{
			printAgreeButton.Enabled = false;
		}
	}

	private void showPlan(int rowIndex)
	{
		if (Program.OfflineMode)
		{
			return;
		}
		try
		{
			if (rowIndex >= 0)
			{
				Agree_id.Text = AgreeList.Rows[rowIndex].Cells[0].Value.ToString().Trim();
				if (Dict.StaffDict.ContainsKey(AgreeList.Rows[rowIndex].Cells[4].Value.ToString().Trim()))
				{
					dr_id.Text = AgreeList.Rows[rowIndex].Cells[4].Value.ToString().Trim();
					dr_name.Text = Dict.StaffDict[dr_id.Text].Name;
				}
				else
				{
					dr_id.Text = "";
					dr_name.Text = "";
				}
				string text = "";
				if (AgreeList.Rows[rowIndex].Cells[1].Value.ToString().Length == 8)
				{
					text = AgreeList.Rows[rowIndex].Cells[1].Value.ToString();
					save_date.Text = text.Substring(0, 4) + "/" + text.Substring(4, 2) + "/" + text.Substring(6, 2);
				}
				dept.Text = AgreeList.Rows[rowIndex].Cells[2].Value.ToString().Trim() + " " + AgreeList.Rows[rowIndex].Cells[3].Value.ToString().Trim();
				eye.Text = AgreeList.Rows[rowIndex].Cells[8].Value.ToString().Trim();
				sheetName.Text = AgreeList.Rows[rowIndex].Cells[17].Value.ToString().Trim();
				staff.Text = AgreeList.Rows[rowIndex].Cells[7].Value.ToString().Trim();
				diag.Text = AgreeList.Rows[rowIndex].Cells[9].Value.ToString().Trim();
				anes.Text = AgreeList.Rows[rowIndex].Cells[20].Value.ToString().Trim();
				ope.Text = AgreeList.Rows[rowIndex].Cells[10].Value.ToString().Trim();
				explanation.Text = AgreeList.Rows[rowIndex].Cells[11].Value.ToString().Trim();
				item1.Text = AgreeList.Rows[rowIndex].Cells[12].Value.ToString().Trim();
				item2.Text = AgreeList.Rows[rowIndex].Cells[13].Value.ToString().Trim();
				item3.Text = AgreeList.Rows[rowIndex].Cells[14].Value.ToString().Trim();
				item4.Text = AgreeList.Rows[rowIndex].Cells[15].Value.ToString().Trim();
				if (AgreeList.Rows[rowIndex].Cells[16].Value.ToString().Trim() == "1")
				{
					staff1_ok = true;
				}
				else
				{
					staff1_ok = false;
				}
				printAgreeButton.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			MessageBox.Show(message);
		}
	}

	public void showPlan(string planId, string ptId)
	{
		int num = -1;
		pt_id.Text = ptId;
		showList();
		for (int i = 0; i < AgreeList.Rows.Count; i++)
		{
			if (planId == AgreeList.Rows[i].Cells[0].Value.ToString())
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			AgreeList.Rows[num].Selected = true;
			showPlan(num);
		}
	}

	private void copyNewMenuItem_Click(object sender, EventArgs e)
	{
		if (Program.OfflineMode)
		{
			return;
		}
		if (AgreeList.SelectedRows.Count == 0)
		{
			return;
		}
		copyAsNew(AgreeList.SelectedRows[0].Index);
	}

	private void copyAsNew(int rowIndex)
	{
		showPlan(rowIndex);
		Agree_id.Text = "";
		staff1_ok = true;
		if (int.TryParse(patCont[9], out int drCode))
		{
			dr_id.Text = drCode.ToString();
			dr_name.Text = patCont[10];
		}
		else
		{
			dr_id.Text = "";
			dr_name.Text = "";
		}
		save_date.Value = DateTime.Today;
		if (regPlan() != -1)
		{
			oraConn.Open();
			oraCmd.CommandText = "select max(AGREE_ID) from AGREE where PATIENT_ID = " + pt_id.Text.Trim() + " and DELETE_FLAG = 0";
			string newId = oraCmd.ExecuteScalar().ToString();
			oraConn.Close();
			showPlan(newId, pt_id.Text);
			MessageBox.Show("コピーして作成しました");
		}
	}

	private void regPlanButton_Click(object sender, EventArgs e)
	{
		switch (regPlan())
		{
		case 0:
			if (MessageBox.Show("登録が完了しました。印刷しますか？", "完了", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				printAgree();
			}
			showList();
			break;
		case 1:
			MessageBox.Show("登録が完了しました");
			showList();
			break;
		}
	}

	private void delPlanButton_Click(object sender, EventArgs e)
	{
		if (delPlan() == 0)
		{
			showList();
			MessageBox.Show("削除しました");
		}
	}

	private int regPlan()
	{
		if (Program.OfflineMode)
		{
			MessageBox.Show("オフラインモードのため登録できません");
			return -1;
		}
		if (pt_id.Text.Length == 0)
		{
			MessageBox.Show("患者IDを入力してください");
			return -1;
		}
		if (dr_id.Text.Length == 0)
		{
			MessageBox.Show("入力者IDを入力してください");
			return -1;
		}
		if (dept.Text.Length == 0)
		{
			MessageBox.Show("診療科を入力してください");
			return -1;
		}
		string text = "";
		string text2 = "";
		string text3 = save_date.Value.ToString("yyyyMMdd");
		text2 = ((!staff1_ok) ? "0" : "1");
		text = ((Agree_id.Text.Length <= 0) ? ("insert into AGREE (AGREE_ID, PATIENT_ID, SAVE_DATE, DEPT, DR, STAFF, EYE, DIAG, ANES, OPE, EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DR_OK,  DELETE_FLAG, SAVE_TIME )  values ( AGREE_SEQ.nextval, " + pt_id.Text + " , " + text3 + " , " + dept.Text.Split(' ')[0] + " , " + dr_id.Text + " , " + AgreeSql.SqlValue(staff.Text) + " , " + AgreeSql.SqlValue(eye.Text) + ", " + AgreeSql.SqlValue(diag.Text) + ", " + AgreeSql.SqlValue(anes.Text) + ", " + AgreeSql.SqlValue(ope.Text) + ", " + AgreeSql.SqlValue(explanation.Text) + ", " + AgreeSql.SqlValue(item1.Text) + ", " + AgreeSql.SqlValue(item2.Text) + ", " + AgreeSql.SqlValue(item3.Text) + ", " + AgreeSql.SqlValue(item4.Text) + ", " + AgreeSql.SqlValue(sheetName.Text) + "," + text2 + ",0," + DateTime.Now.ToString("HHmmss") + ")") : ("update AGREE set SAVE_DATE = " + text3 + ", DEPT = " + dept.Text.Split(' ')[0].Trim() + ", DR = " + dr_id.Text.Trim() + ", STAFF = " + AgreeSql.SqlValue(staff.Text) + ", EYE = " + AgreeSql.SqlValue(eye.Text) + ", DIAG = " + AgreeSql.SqlValue(diag.Text) + ", ANES = " + AgreeSql.SqlValue(anes.Text) + ", OPE = " + AgreeSql.SqlValue(ope.Text) + ", EXPLANATION = " + AgreeSql.SqlValue(explanation.Text) + ", ITEM1 = " + AgreeSql.SqlValue(item1.Text) + ", ITEM2 = " + AgreeSql.SqlValue(item2.Text) + ", ITEM3 = " + AgreeSql.SqlValue(item3.Text) + ", ITEM4 = " + AgreeSql.SqlValue(item4.Text) + ", SHEET_NAME = " + AgreeSql.SqlValue(sheetName.Text) + ", DR_OK = " + text2 + ", SAVE_TIME = " + DateTime.Now.ToString("HHmmss") + " where AGREE_ID = " + Agree_id.Text));
		oraConn.Open();
		oraCmd.CommandText = text;
		oraCmd.ExecuteNonQuery();
		oraConn.Close();
		if (staff1_ok)
		{
			return 0;
		}
		return 1;
	}

	private int delPlan()
	{
		if (Program.OfflineMode)
		{
			MessageBox.Show("オフラインモードのため削除できません");
			return -1;
		}
		if (MessageBox.Show("削除しますか？", "削除", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
		{
			if (Agree_id.Text.Length > 0)
			{
				oraConn.Open();
				oraCmd.CommandText = "update AGREE set DELETE_FLAG = 1 where AGREE_ID = " + Agree_id.Text;
				oraCmd.ExecuteNonQuery();
				oraConn.Close();
			}
			else
			{
				clearPlan();
			}
			return 0;
		}
		return -1;
	}

	private void printPlanButton_Click(object sender, EventArgs e)
	{
		printAgree();
	}

	private void printAgree()
	{
		ExcelControl excelControl = new ExcelControl();
		// キーは "行, 列" 形式で、ExcelControl 側で Cells[行, 列] に書き込む。すべて列2=B に出力する。
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["1, 2"] = pt_id.Text;      // B1: 患者ID
		dictionary["2, 2"] = pt_kana.Text;    // B2: 患者カナ氏名
		dictionary["3, 2"] = pt_name.Text;    // B3: 患者氏名
		if (pt_sex.Equals("2"))
		{
			dictionary["4, 2"] = "女";        // B4: 性別（女）
		}
		else
		{
			dictionary["4, 2"] = "男";        // B4: 性別（男）
		}
		dictionary["5, 2"] = dept.Text.Split(' ')[0];   // B5: 診療科コード（"コード 科名" の前半）
		dictionary["6, 2"] = dept.Text.Split(' ')[1];   // B6: 診療科名（"コード 科名" の後半）
		string path = Env.LEGACY_HOME + "\\Pat.csv";
		if (File.Exists(path))
		{
			dictionary["7, 2"] = patCont[9].PadLeft(5, '0');  // B7: 入力者ID（Pat.csv由来・5桁ゼロ埋め）
			dictionary["11, 2"] = patCont[10];                // B11: 入力者氏名（Pat.csv由来）
		}
		else
		{
			dictionary["7, 2"] = dr_id.Text.PadLeft(5, '0');  // B7: 入力者ID（画面入力・5桁ゼロ埋め）
			dictionary["11, 2"] = dr_name.Text;               // B11: 入力者氏名（画面入力）
		}
		dictionary["8, 2"] = save_date.Value.ToString("yyyyMMdd");  // B8: 作成日
		dictionary["9, 2"] = DateTime.Now.ToString("HHmmss");       // B9: 作成時刻
		// B10 は ExcelControl.MakeEyeAgree が36桁バーコード値を書き込むため、ここでは設定しない。
		dictionary["12, 2"] = staff.Text;         // B12: 担当者
		dictionary["13, 2"] = eye.Text;           // B13: 眼
		dictionary["14, 2"] = ope.Text;           // B14: 手術名
		dictionary["15, 2"] = anes.Text;          // B15: 麻酔
		dictionary["16, 2"] = diag.Text;          // B16: 病名
		dictionary["17, 2"] = explanation.Text;   // B17: 説明
		dictionary["18, 2"] = item1.Text;         // B18: 症状
		dictionary["19, 2"] = item2.Text;         // B19: 治療計画
		dictionary["20, 2"] = item4.Text;         // B20: 手術内容
		excelControl.ValueList = dictionary;
		try
		{
			excelControl.MakeEyeAgree(sheetName.Text);
		}
		catch (Exception ex)
		{
			Logger.Error("printAgree", ex);
			MessageBox.Show("同意書の作成中にエラーが発生しました。\nExcelが起動しているか確認してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			// 正常・異常いずれの経路でも COM を解放し、EXCEL.EXE の残留を防ぐ。
			excelControl.ReleaseExcel();
		}
	}

	public void applyTemplate(int temp_id)
	{
		if (Program.OfflineMode)
		{
			return;
		}
		oraConn.Open();
		oraCmd.CommandText = "select * from AGREE_TEMPLATE where TEMP_ID = " + temp_id;
		oraReader = oraCmd.ExecuteReader();
		if (oraReader.Read())
		{
			if (eye.Text.Length > 0)
			{
				ComboBox comboBox = eye;
				comboBox.Text = comboBox.Text + " " + oraReader["EYE"].ToString();
			}
			else
			{
				eye.Text = oraReader["EYE"].ToString();
			}
			if (diag.Text.Length > 0)
			{
				TextBox textBox = diag;
				textBox.Text = textBox.Text + " " + oraReader["DIAG"].ToString();
			}
			else
			{
				diag.Text = oraReader["DIAG"].ToString();
			}
			if (anes.Text.Length > 0)
			{
				TextBox textBox2 = anes;
				textBox2.Text = textBox2.Text + " " + oraReader["ANES"].ToString();
			}
			else
			{
				anes.Text = oraReader["ANES"].ToString();
			}
			if (ope.Text.Length > 0)
			{
				TextBox textBox3 = ope;
				textBox3.Text = textBox3.Text + " " + oraReader["OPE"].ToString();
			}
			else
			{
				ope.Text = oraReader["OPE"].ToString();
			}
			if (explanation.Text.Length > 0)
			{
				TextBox textBox4 = explanation;
				textBox4.Text = textBox4.Text + " " + oraReader["EXPLANATION"].ToString();
			}
			else
			{
				explanation.Text = oraReader["EXPLANATION"].ToString();
			}
			if (item1.Text.Length > 0)
			{
				TextBox textBox5 = item1;
				textBox5.Text = textBox5.Text + " " + oraReader["ITEM1"].ToString();
			}
			else
			{
				item1.Text = oraReader["ITEM1"].ToString();
			}
			if (item2.Text.Length > 0)
			{
				TextBox textBox6 = item2;
				textBox6.Text = textBox6.Text + " " + oraReader["ITEM2"].ToString();
			}
			else
			{
				item2.Text = oraReader["ITEM2"].ToString();
			}
			if (item3.Text.Length > 0)
			{
				TextBox textBox7 = item3;
				textBox7.Text = textBox7.Text + " " + oraReader["ITEM3"].ToString();
			}
			else
			{
				item3.Text = oraReader["ITEM3"].ToString();
			}
			if (item4.Text.Length > 0)
			{
				TextBox textBox8 = item4;
				textBox8.Text = textBox8.Text + " " + oraReader["ITEM4"].ToString();
			}
			else
			{
				item4.Text = oraReader["ITEM4"].ToString();
			}
			sheetName.Text = oraReader["SHEET_NAME"].ToString();
		}
		oraReader.Close();
		oraConn.Close();
	}

}
