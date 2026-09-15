using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using AgentlabUtilityLibrary;
using Microsoft.Office.Interop.Excel;

namespace Agree;

internal class ExcelControl : IDisposable
{
	private Application exApp;

	private _Workbook exWorkbook;

	private _Worksheet exWorksheet;

	// バーコード画像の解像度設定。既定値は EyeAgreeSettings.ini の
	// [BARCODE_SETTINGS] で上書きできる。1モジュール=barcodeLineWidth px。
	private float barcodeLineWidth = 3f;

	private float barcodeHeight = 80f;

	private int barcodeQuietModules = 10;

	// バーコードに埋め込む文書コード
	// 既定値は EyeAgreeSettings.ini の [BARCODE_SETTINGS] DOCUMENT_CODE で上書きできる。
	private string documentCode = "39911";

	public void Dispose()
	{
		ReleaseExcel();
	}

	private void ReleaseExcel()
	{
		// Open() 失敗時は各 COM フィールドが null のため、Dispose から無条件に呼べるよう
		// null ガードする。解放漏れによる Excel プロセス残留を防ぐ。
		if (exWorksheet != null)
		{
			Marshal.ReleaseComObject(exWorksheet);
			exWorksheet = null;
		}
		if (exWorkbook != null)
		{
			Marshal.ReleaseComObject(exWorkbook);
			exWorkbook = null;
		}
		if (exApp != null)
		{
			Marshal.ReleaseComObject(exApp);
			exApp = null;
		}
	}

	// テンプレートを開き、指定シートを exWorksheet に設定する。
	// ファイルが無い場合は FileNotFoundException（FileName に探したパス）、Excel の起動・シート取得の失敗は例外がそのまま伝播する。
	private void Open(string fileName, string sheetName)
	{
		if (!File.Exists(fileName))
		{
			throw new FileNotFoundException("ファイルが存在しません", fileName);
		}
		exApp = new Application();
		exApp.Visible = true;
		exApp.EnableEvents = false;
		Workbooks workbooks = exApp.Workbooks;
		try
		{
			exWorkbook = workbooks.Open(fileName);
		}
		finally
		{
			Marshal.ReleaseComObject(workbooks);
		}
		exWorksheet = (_Worksheet)exWorkbook.Sheets[sheetName];
	}

	/// <param name="values">共通情報シートの行番号→値（すべて B 列に書き込む）。</param>
	/// <param name="patientId">バーコード値・保存ファイル名に使う患者ID。</param>
	/// <param name="saveDate">バーコード値の日付に使う作成日（yyyyMMdd）。</param>
	public void MakeEyeAgree(string sheetName, Dictionary<int, string> values, string patientId, string saveDate)
	{
		Open(Env.AGENT_HOME + "\\EyeAgree\\EyeAgree.xlsm", "共通情報");
		// シート切替・セル書込み・バーコード挿入の途中経過を画面に見せないため、
		// 自動処理中は描画を凍結する。例外時に凍結・イベント無効のまま Excel が
		// 残らないよう、finally で必ず復元する。
		exApp.ScreenUpdating = false;
		try
		{
			activateAllSheets();
			setValue(values);
			// バーコード値の日付は作成日を使う。時刻は印刷時刻を1回だけ取得し、
			// セル・バーコード値・ファイル名で共用する。
			DateTime now = DateTime.Now;
			string hms = now.ToString("HHmmss");
			exWorksheet.Cells[9, 2] = hms;   // B9: 作成時刻
			// バーコード解像度と文書コードを INI から読み込む（文書コードはバーコード値構築で使う）。
			loadBarcodeSettings();
			string barcodeValue = buildBarcodeValue(patientId, saveDate, hms);
			// B11 は入力者氏名に使うため、バーコード値は B10 へ出力する。
			exWorksheet.Cells[10, 2] = barcodeValue;
			insertBarcodeToFormSheets(barcodeValue);
			// ファイル名は再印刷時の重複を避けるため印刷日時で付ける。
			saveWorkbook(patientId, now.ToString("yyyyMMdd"), hms);
			selectTargetSheet(sheetName);
		}
		finally
		{
			// 自動処理を終えたのでイベントを再開し、描画凍結を解除する。
			exApp.EnableEvents = true;
			exApp.ScreenUpdating = true;
		}
	}

	// 自動保存時に「セッション中アクティブにされていないシートのフォームコントロール
	// （ボタン）が脱落する」既知の挙動を回避するため、全シートを一度アクティブ化して
	// 描画レイヤーを確実に読み込ませる。EnableEvents=false 中なのでイベントは発火しない。
	private void activateAllSheets()
	{
		Sheets sheets = exWorkbook.Sheets;
		int sheetCount = sheets.Count;
		for (int i = 1; i <= sheetCount; i++)
		{
			_Worksheet sheet = (_Worksheet)sheets[i];
			try
			{
				sheet.Activate();
			}
			catch
			{
				// 非表示シートはアクティブ化できないためスキップ
			}
			Marshal.ReleaseComObject(sheet);
		}
		Marshal.ReleaseComObject(sheets);
	}

	private void setValue(Dictionary<int, string> rowToValue)
	{
		// キーは行番号（Form1.printAgree 側で構築）。すべて B 列（列2）に書き込む。
		foreach (KeyValuePair<int, string> pair in rowToValue)
		{
			exWorksheet.Cells[pair.Key, 2] = pair.Value;
		}
	}

	private string getCellText(int row, int col)
	{
		Range cell = (Range)exWorksheet.Cells[row, col];
		try
		{
			object value = cell.Value2;
			return value.ToString();
		}
		finally
		{
			Marshal.ReleaseComObject(cell);
		}
	}

	// 36桁バーコード値を構築する。日付・時刻は Value2 経由だと数値化で先頭ゼロが落ちる
	// （例: 093948→93948）ため、セルへ書き込んだ文字列をそのまま使う。
	private string buildBarcodeValue(string patientId, string ymd, string hms)
	{
		string deptCode = getCellText(5, 2).PadLeft(3, '0');   // B5: 診療科コード
		string doctorId = getCellText(7, 2).PadLeft(5, '0');   // B7: 入力者ID
		return patientId.PadLeft(9, '0') + documentCode.PadLeft(5, '0') + deptCode + doctorId + ymd + hms;
	}

	// 全フォームシートへバーコード画像を挿入する（SaveAs より前に行うことで保存ファイルへ残す）。
	private void insertBarcodeToFormSheets(string barcodeValue)
	{
		Sheets sheets = exWorkbook.Sheets;
		int sheetCount = sheets.Count;
		for (int i = 1; i <= sheetCount; i++)
		{
			_Worksheet formSheet = (_Worksheet)sheets[i];
			if (formSheet.Name != "共通情報")
			{
				insertBarcode(formSheet, barcodeValue);
			}
			Marshal.ReleaseComObject(formSheet);
		}
		Marshal.ReleaseComObject(sheets);
	}

	private void saveWorkbook(string patientId, string ymd, string hms)
	{
		string filename = Environment.GetEnvironmentVariable("TEMP") + "\\" + patientId + "_" + ymd + hms + "_" + "EyeAgree.xlsm";
		// マクロ有効形式(.xlsm)を明示して保存する
		exWorkbook.SaveAs(filename, XlFileFormat.xlOpenXMLWorkbookMacroEnabled, AccessMode: XlSaveAsAccessMode.xlExclusive);
	}

	private void selectTargetSheet(string sheetName)
	{
		// 共通情報シートの COM 参照を解放してから、表示対象シートへ差し替える。
		Marshal.ReleaseComObject(exWorksheet);
		exWorksheet = (_Worksheet)exWorkbook.Sheets[resolveSheetName(sheetName)];
		exWorksheet.Select(true);
	}

	private string resolveSheetName(string sheetName)
	{
		// 入院・日帰り は通常のエイリアス。該当シートが無い場合は通常シートを開く。
		if (sheetName == "入院" || sheetName == "日帰り")
		{
			return "通常";
		}
		return sheetName;
	}

	// EyeAgreeSettings.ini からバーコード解像度と文書コードを読み込む。
	// 値が無い・不正な場合は既定値を維持する。
	private void loadBarcodeSettings()
	{
		float lineWidth = AppSettings.GetFloat("BARCODE_LINE_WIDTH", 0f);
		if (lineWidth > 0f)
		{
			barcodeLineWidth = lineWidth;
		}
		float height = AppSettings.GetFloat("BARCODE_HEIGHT", 0f);
		if (height > 0f)
		{
			barcodeHeight = height;
		}
		int quietModules = AppSettings.GetInt("BARCODE_QUIET_MODULES", -1);
		if (quietModules >= 0)
		{
			barcodeQuietModules = quietModules;
		}
		// バーコードの桁数を保つため、数字のみ・空でない値だけ採用する。
		string code = AppSettings.Get("DOCUMENT_CODE", "");
		if (code.Length > 0 && isAllDigits(code))
		{
			documentCode = code;
		}
	}

	private void insertBarcode(_Worksheet sheet, string barcodeText)
	{
		if (string.IsNullOrEmpty(barcodeText))
		{
			return;
		}
		if (barcodeText.Length != 36 || !isAllDigits(barcodeText))
		{
			// バーコード値は患者IDを含むため値そのものは記録せず、桁数のみ残す。
			Logger.Info("insertBarcode", "バーコード値が36桁の数字でないため挿入をスキップ（桁数=" + barcodeText.Length + "）");
			return;
		}
		string tempPath = null;
		Range anchor = null;
		object shapes = null;
		object picture = null;
		try
		{
			tempPath = generateBarcodeImage(barcodeText);
			anchor = (Range)sheet.Cells[1, 4]; // D1
			float left = (float)(double)anchor.Left;
			float top = (float)(double)anchor.Top;
			shapes = sheet.Shapes;
			// AddPicture の引数型 MsoTriState は office.dll(PIA) 由来だが、CLI ビルド用には
			// Excel PIA しか同梱していないため、この呼び出しのみ dynamic 経由で遅延バインドする。
			picture = ((dynamic)shapes).AddPicture(tempPath, 0, -1, left, top, 250f, 30f);
			((dynamic)picture).Placement = (int)XlPlacement.xlMove;
		}
		catch (Exception ex)
		{
			Logger.Error("insertBarcode", ex);
		}
		finally
		{
			if (picture != null)
			{
				Marshal.ReleaseComObject(picture);
			}
			if (shapes != null)
			{
				Marshal.ReleaseComObject(shapes);
			}
			if (anchor != null)
			{
				Marshal.ReleaseComObject(anchor);
			}
			if (tempPath != null && File.Exists(tempPath))
			{
				try
				{
					File.Delete(tempPath);
				}
				catch
				{
				}
			}
		}
	}

	private string generateBarcodeImage(string barcodeText)
	{
		// CODE128-C のモジュール数: (開始1 + データ(桁/2) + チェック1)*11 + 停止13 + クワイエットゾーン両側。
		int symbols = 2 + barcodeText.Length / 2;
		int totalModules = symbols * 11 + 13 + barcodeQuietModules * 2;
		int widthPx = (int)Math.Ceiling(totalModules * barcodeLineWidth);
		int heightPx = (int)Math.Ceiling(barcodeHeight);
		string tempPath = Path.Combine(Path.GetTempPath(), "barcode_" + Guid.NewGuid().ToString("N") + ".png");
		using (Bitmap bitmap = new Bitmap(widthPx, heightPx))
		{
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				g.Clear(Color.White);
				Barcode128 barcode = new Barcode128();
				float left = barcodeQuietModules * barcodeLineWidth;
				barcode.Draw(Barcode128.CODE.C, barcodeText, g, left, 0f, barcodeHeight, barcodeLineWidth);
			}
			bitmap.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
		}
		return tempPath;
	}

	private static bool isAllDigits(string s)
	{
		foreach (char c in s)
		{
			if (c < '0' || c > '9')
			{
				return false;
			}
		}
		return true;
	}
}
