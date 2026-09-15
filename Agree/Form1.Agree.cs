using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows.Forms;
using AgentlabUtilityLibrary;

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
			if (!int.TryParse(pt_id.Text.Trim(), out int ptId))
			{
				MessageBox.Show("患者IDは数字で入力してください");
				return;
			}
			Db.Read(oraConn, "select P_NAME, P_KANA, P_SEX from M_PATIENT" + Env.DB_LINK + " where P_ID = " + ptId, r =>
			{
				pt_name.Text = r["P_NAME"].ToString();
				pt_kana.Text = r["P_KANA"].ToString();
				pt_sex.Text = r["P_SEX"].ToString() == "2" ? "女" : "男";
			});
			// 一覧の列は列名・別名で参照する（showAgree も同じ名前を使う）。
			string sql = "select AGREE_ID, SAVE_DATE, AGREE.DEPT, Trim(M_DEPT.S_NAME) as DEPT_NAME, AGREE.DR, Trim(M_USR.NAME) as DR_NAME,"
				+ " STAFF, EYE, DIAG, OPE, EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, DR_OK, SHEET_NAME, ANES"
				+ " from AGREE inner join M_DEPT" + Env.DB_LINK + " on AGREE.DEPT = M_DEPT.CODE inner join M_USR" + Env.DB_LINK + " on AGREE.DR = M_USR.CODE"
				+ " where PATIENT_ID = " + ptId + " and DELETE_FLAG = 0 order by SAVE_DATE desc";
			DataSet dataSet = new DataSet();
			using (OleDbDataAdapter adapter = new OleDbDataAdapter(sql, oraConn))
			{
				adapter.Fill(dataSet, "同意書");
			}
			AgreeList.DataSource = dataSet.Tables["同意書"];
			AgreeList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			foreach (DataGridViewColumn column in AgreeList.Columns)
			{
				// 表示する列は一度も非表示にしない（先頭行のカレントセルを保つため）。
				column.Visible = column.Name is "SAVE_DATE" or "DEPT_NAME" or "DR_NAME" or "EYE" or "OPE";
			}
			setListColumn("SAVE_DATE", "作成日", 80, format: "0000/00/00");
			setListColumn("DEPT_NAME", "診療科", 80);
			setListColumn("DR_NAME", "医師", 80);
			setListColumn("EYE", "眼", 40, center: true);
			setListColumn("OPE", "手術", 150, center: true);
			// DataSource 設定時の RowEnter で先頭行が入力欄に表示されるため、ここで空に戻す。
			clearAgree();
			if (AgreeList.RowCount > 0)
			{
				agreeListLabel.Text = "同意書をクリックすると内容が表示されます。";
			}
			else
			{
				agreeListLabel.Text = "既存の同意書はありません。新規作成してください。";
			}
		}
		else
		{
			printAgreeButton.Enabled = false;
		}
	}

	private void setListColumn(string name, string headerText, int width, string format = null, bool center = false)
	{
		DataGridViewColumn column = AgreeList.Columns[name];
		column.HeaderText = headerText;
		column.Width = width;
		if (format != null)
		{
			column.DefaultCellStyle.Format = format;
		}
		if (center)
		{
			column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		}
	}

	private void showAgree(int rowIndex)
	{
		if (Program.OfflineMode)
		{
			return;
		}
		try
		{
			if (rowIndex >= 0)
			{
				DataGridViewRow row = AgreeList.Rows[rowIndex];
				string cell(string name) => row.Cells[name].Value.ToString().Trim();
				Agree_id.Text = cell("AGREE_ID");
				dr_id.Text = cell("DR");
				// 職員マスタに無いコード（退職者等）でも、DBに保存された入力者を欠落させない。
				dr_name.Text = Dict.StaffDict.ContainsKey(dr_id.Text) ? Dict.StaffDict[dr_id.Text].Name : cell("DR_NAME");
				string saveDate = cell("SAVE_DATE");
				if (saveDate.Length == 8)
				{
					save_date.Text = saveDate.Substring(0, 4) + "/" + saveDate.Substring(4, 2) + "/" + saveDate.Substring(6, 2);
				}
				dept.Text = cell("DEPT") + " " + cell("DEPT_NAME");
				eye.Text = cell("EYE");
				sheetName.Text = cell("SHEET_NAME");
				staff.Text = cell("STAFF");
				diag.Text = cell("DIAG");
				anes.Text = cell("ANES");
				ope.Text = cell("OPE");
				explanation.Text = cell("EXPLANATION");
				item1.Text = cell("ITEM1");
				item2.Text = cell("ITEM2");
				item3.Text = cell("ITEM3");
				item4.Text = cell("ITEM4");
				doctorConfirmed = cell("DR_OK") == "1";
				printAgreeButton.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("showAgree", ex);
			MessageBox.Show(ex.Message);
		}
	}

	public void showAgree(string agreeId, string ptId)
	{
		int rowIndex = -1;
		pt_id.Text = ptId;
		showList();
		for (int i = 0; i < AgreeList.Rows.Count; i++)
		{
			if (agreeId == AgreeList.Rows[i].Cells["AGREE_ID"].Value.ToString())
			{
				rowIndex = i;
				break;
			}
		}
		if (rowIndex >= 0)
		{
			AgreeList.Rows[rowIndex].Selected = true;
			showAgree(rowIndex);
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
		showAgree(rowIndex);
		Agree_id.Text = "";
		doctorConfirmed = true;
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
		if (regAgree())
		{
			// 患者IDが数字であることは regAgree で検証済み。
			string newId = Db.Scalar(oraConn, "select max(AGREE_ID) from AGREE where PATIENT_ID = " + int.Parse(pt_id.Text.Trim()) + " and DELETE_FLAG = 0").ToString();
			showAgree(newId, pt_id.Text);
			MessageBox.Show("コピーして作成しました");
		}
	}

	private void regAgreeButton_Click(object sender, EventArgs e)
	{
		if (!regAgree())
		{
			return;
		}
		if (doctorConfirmed)
		{
			if (MessageBox.Show("登録が完了しました。印刷しますか？", "完了", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				printAgree();
			}
		}
		else
		{
			MessageBox.Show("登録が完了しました");
		}
		showList();
	}

	private void delAgreeButton_Click(object sender, EventArgs e)
	{
		if (delAgree())
		{
			showList();
			MessageBox.Show("削除しました");
		}
	}

	/// <summary>
	/// 入力内容を AGREE に登録する（同意書IDが無ければ INSERT、あれば UPDATE）。
	/// オフライン・入力チェックで中断した場合は false を返す。
	/// </summary>
	private bool regAgree()
	{
		if (Program.OfflineMode)
		{
			MessageBox.Show("オフラインモードのため登録できません");
			return false;
		}
		if (!int.TryParse(pt_id.Text.Trim(), out int ptId))
		{
			MessageBox.Show(pt_id.Text.Length == 0 ? "患者IDを入力してください" : "患者IDは数字で入力してください");
			return false;
		}
		if (!int.TryParse(dr_id.Text.Trim(), out int drId))
		{
			MessageBox.Show(dr_id.Text.Length == 0 ? "入力者IDを入力してください" : "入力者IDは数字で入力してください");
			return false;
		}
		if (dept.Text.Length == 0)
		{
			MessageBox.Show("診療科を入力してください");
			return false;
		}
		if (!tryParseDeptCode(dept.Text.Split(' ')[0], out short deptCode))
		{
			MessageBox.Show("診療科はリストから選んでください");
			return false;
		}
		// INSERT と UPDATE で同じ列定義を使い、列の追加漏れや位置ずれを防ぐ。
		(string Name, string Value)[] columns =
		{
			("SAVE_DATE", save_date.Value.ToString("yyyyMMdd")),
			("DEPT", deptCode.ToString()),
			("DR", drId.ToString()),
			("STAFF", AgreeSql.SqlValue(staff.Text)),
			("EYE", AgreeSql.SqlValue(eye.Text)),
			("DIAG", AgreeSql.SqlValue(diag.Text)),
			("ANES", AgreeSql.SqlValue(anes.Text)),
			("OPE", AgreeSql.SqlValue(ope.Text)),
			("EXPLANATION", AgreeSql.SqlValue(explanation.Text)),
			("ITEM1", AgreeSql.SqlValue(item1.Text)),
			("ITEM2", AgreeSql.SqlValue(item2.Text)),
			("ITEM3", AgreeSql.SqlValue(item3.Text)),
			("ITEM4", AgreeSql.SqlValue(item4.Text)),
			("SHEET_NAME", AgreeSql.SqlValue(sheetName.Text)),
			("DR_OK", doctorConfirmed ? "1" : "0"),
			("SAVE_TIME", DateTime.Now.ToString("HHmmss")),
		};
		string sql;
		if (int.TryParse(Agree_id.Text, out int agreeId))
		{
			sql = "update AGREE set " + string.Join(", ", columns.Select(c => c.Name + " = " + c.Value)) + " where AGREE_ID = " + agreeId;
		}
		else
		{
			sql = "insert into AGREE (AGREE_ID, PATIENT_ID, DELETE_FLAG, " + string.Join(", ", columns.Select(c => c.Name)) + ")"
				+ " values (AGREE_SEQ.nextval, " + ptId + ", 0, " + string.Join(", ", columns.Select(c => c.Value)) + ")";
		}
		Db.Execute(oraConn, sql);
		return true;
	}

	private bool delAgree()
	{
		if (Program.OfflineMode)
		{
			MessageBox.Show("オフラインモードのため削除できません");
			return false;
		}
		if (MessageBox.Show("削除しますか？", "削除", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
		{
			return false;
		}
		if (int.TryParse(Agree_id.Text, out int agreeId))
		{
			Db.Execute(oraConn, "update AGREE set DELETE_FLAG = 1 where AGREE_ID = " + agreeId);
		}
		else
		{
			clearAgree();
		}
		return true;
	}

	private void printAgreeButton_Click(object sender, EventArgs e)
	{
		printAgree();
	}

	private void printAgree()
	{
		string patientId = pt_id.Text.Trim();
		string saveDate = save_date.Value.ToString("yyyyMMdd");
		// キーは共通情報シートの行番号。ExcelControl 側ですべて B 列に書き込む。
		Dictionary<int, string> values = new Dictionary<int, string>();
		values[1] = patientId;                 // B1: 患者ID
		values[2] = pt_kana.Text;              // B2: 患者カナ氏名
		values[3] = pt_name.Text;              // B3: 患者氏名
		values[4] = pt_sex.Text;               // B4: 性別（showList / readPatCsv で「女」「男」に変換済み）
		values[5] = dept.Text.Split(' ')[0];   // B5: 診療科コード（"コード 科名" の前半）
		values[6] = dept.Text.Split(' ')[1];   // B6: 診療科名（"コード 科名" の後半）
		// 入力者はDBに保存された値（AGREE.DR）を出力する。一覧選択時に showAgree が
		// dr_id/dr_name へセットするため、スタッフが印刷しても医師の入力者IDで出力される。
		values[7] = dr_id.Text.Trim().PadLeft(5, '0');  // B7: 入力者ID（DB由来・5桁ゼロ埋め）
		values[11] = dr_name.Text.Trim();               // B11: 入力者氏名（DB由来）
		values[8] = saveDate;                  // B8: 作成日
		// B9（作成時刻）と B10（36桁バーコード値）は ExcelControl.MakeEyeAgree が印刷時に書き込む。
		values[12] = staff.Text;               // B12: 担当者
		values[13] = eye.Text;                 // B13: 眼
		values[14] = ope.Text;                 // B14: 手術名
		values[15] = anes.Text;                // B15: 麻酔
		values[16] = diag.Text;                // B16: 病名
		values[17] = explanation.Text;         // B17: 説明
		values[18] = item1.Text;               // B18: 症状
		values[19] = item2.Text;               // B19: 治療計画
		values[20] = item4.Text;               // B20: 手術内容
		try
		{
			// 正常・異常いずれの経路でも Dispose で COM を解放し、EXCEL.EXE の残留を防ぐ。
			using (ExcelControl excelControl = new ExcelControl())
			{
				excelControl.MakeEyeAgree(sheetName.Text, values, patientId, saveDate);
			}
		}
		catch (System.IO.FileNotFoundException ex)
		{
			// テンプレート未配置を「Excel未起動」と誤案内しないよう、探したパスを示す。
			Logger.Error("printAgree", ex);
			MessageBox.Show("同意書のテンプレートが見つかりません。\n次の場所に配置してください。\n" + ex.FileName, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		catch (Exception ex)
		{
			Logger.Error("printAgree", ex);
			MessageBox.Show("同意書の作成中にエラーが発生しました。\nExcelが起動しているか確認してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	public void applyTemplate(int temp_id)
	{
		if (Program.OfflineMode)
		{
			return;
		}
		Db.Read(oraConn, "select EYE, DIAG, ANES, OPE, EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME from AGREE_TEMPLATE where TEMP_ID = " + temp_id, r =>
		{
			appendText(eye, r["EYE"].ToString());
			appendText(diag, r["DIAG"].ToString());
			appendText(anes, r["ANES"].ToString());
			appendText(ope, r["OPE"].ToString());
			appendText(explanation, r["EXPLANATION"].ToString());
			appendText(item1, r["ITEM1"].ToString());
			appendText(item2, r["ITEM2"].ToString());
			appendText(item3, r["ITEM3"].ToString());
			appendText(item4, r["ITEM4"].ToString());
			sheetName.Text = r["SHEET_NAME"].ToString();
		});
	}

	// 入力済みの欄にはテンプレートの値を空白区切りで追記し、空の欄にはそのまま入れる。
	private static void appendText(Control box, string value)
	{
		box.Text = box.Text.Length > 0 ? box.Text + " " + value : value;
	}
}
