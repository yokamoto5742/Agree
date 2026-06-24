using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using AgentlabUtilityLibrary;
using Microsoft.Office.Interop.Excel;

internal class ExcelControl
{
	private Dictionary<string, string> valueList = new Dictionary<string, string>();

	private Application exApp;

	private _Workbook exWorkbook;

	private _Worksheet exWorksheet;

	public Dictionary<string, string> ValueList
	{
		set
		{
			valueList = value;
		}
	}

	public void ReleaseExcel()
	{
		Marshal.ReleaseComObject(exWorksheet);
		Marshal.ReleaseComObject(exWorkbook);
		Marshal.ReleaseComObject(exApp);
	}

	public string Open(string fileName, string sheetName)
	{
		string result = "";
		if (!File.Exists(fileName))
		{
			return "ファイルが存在しません";
		}
		try
		{
			exApp = (Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
			exApp.Visible = true;
			// テンプレート(.xlsm)のイベントマクロ(Workbook_Open/Worksheet_Change/BeforeSave等)を
			// 自動処理中は発火させない。対象シート以外のボタンを消すクリーンアップが
			// マクロ側にある場合、これで抑止する。MakeEyeAgree の SaveAs 後に true へ戻す。
			exApp.EnableEvents = false;
			exWorkbook = exApp.Workbooks.Open(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
			exWorksheet = (_Worksheet)(dynamic)exWorkbook.Sheets[sheetName];
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			result = message;
		}
		return result;
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

	private void setValue(Dictionary<string, string> valueToCell)
	{
		foreach (string key in valueToCell.Keys)
		{
			int num = int.Parse(key.Split(',')[0]);
			int num2 = int.Parse(key.Split(',')[1]);
			exWorksheet.Cells[num, num2] = valueToCell[key];
		}
	}

	public void MakeEyeAgree(string sheetName)
	{
		Open(Env.AGENT_HOME + "\\Agree_眼科同意書.xlsm", "共通情報");
		// シート切替・セル書込み・バーコード挿入の途中経過を画面に見せないため、
		// 自動処理中は描画を凍結する。最終シートを Select した後に true へ戻す。
		exApp.ScreenUpdating = false;
		// 自動保存時に「セッション中アクティブにされていないシートのフォームコントロール
		// （ボタン）が脱落する」既知の挙動を回避するため、全シートを一度アクティブ化して
		// 描画レイヤーを確実に読み込ませる。EnableEvents=false 中なのでイベントは発火しない。
		Sheets activateSheets = exWorkbook.Sheets;
		int activateCount = activateSheets.Count;
		for (int i = 1; i <= activateCount; i++)
		{
			_Worksheet ws = (_Worksheet)(dynamic)activateSheets[i];
			try
			{
				ws.Activate();
			}
			catch
			{
				// 非表示シートはアクティブ化できないためスキップ
			}
			Marshal.ReleaseComObject(ws);
		}
		Marshal.ReleaseComObject(activateSheets);
		setValue(valueList);
		// 日付・時刻は1回だけ取得し、セル・バーコード値・ファイル名で共用する。
		DateTime now = DateTime.Now;
		string ymd = now.ToString("yyyyMMdd");
		string hms = now.ToString("HHmmss");
		exWorksheet.Cells[8, 2] = ymd;
		exWorksheet.Cells[9, 2] = hms;
		Range range = (Range)(dynamic)exWorksheet.Cells[3, 2];
		Range range2 = (Range)(dynamic)exWorksheet.Cells[7, 2];
		Range range3 = (Range)(dynamic)exWorksheet.Cells[4, 2];
		Range range4 = (Range)(dynamic)exWorksheet.Cells[22, 2];
		Range range5 = (Range)(dynamic)exWorksheet.Cells[5, 2];
		// 36桁バーコード値を構築する。日付・時刻は Value2 経由だと数値化で先頭ゼロが落ちる
		// （例: 093948→93948）ため、上で確定した文字列をそのまま使う。
		string patient = ((dynamic)range.Value2).ToString().PadLeft(9, '0');
		string dept = ((dynamic)range5.Value2).ToString().PadLeft(3, '0');
		string doctor = ((dynamic)range2.Value2).ToString().PadLeft(5, '0');
		string doc1 = ((dynamic)range3.Value2).ToString().PadLeft(5, '0');
		string doc2 = ((dynamic)range4.Value2).ToString().PadLeft(5, '0');
		string barcode11 = patient + doc1 + dept + doctor + ymd + hms;
		string barcode23 = patient + doc2 + dept + doctor + ymd + hms;
		exWorksheet.Cells[11, 2] = (object)barcode11;
		exWorksheet.Cells[23, 2] = (object)barcode23;
		// 「バーコード印刷」フラグ（共通情報!B17）が "1" のときのみ、全フォームシートへ
		// バーコード画像を挿入する（SaveAs より前に行うことで保存ファイルへ残す）。
		Range flagRange = (Range)(dynamic)exWorksheet.Cells[17, 2];
		object flagValue = flagRange.Value2;
		string barcodeFlag = (flagValue == null) ? "" : ((dynamic)flagValue).ToString();
		Marshal.ReleaseComObject(flagRange);
		if (barcodeFlag == "1")
		{
			Sheets sheets = exWorkbook.Sheets;
			int sheetCount = sheets.Count;
			for (int i = 1; i <= sheetCount; i++)
			{
				_Worksheet formSheet = (_Worksheet)(dynamic)sheets[i];
                if (formSheet.Name != "共通情報")
                {
                    // 同意書の文書番号をB11から取得(39911)
                    string barcodeText = barcode11;
                    insertBarcode(formSheet, barcodeText);
                }
                Marshal.ReleaseComObject(formSheet);
			}
			Marshal.ReleaseComObject(sheets);
		}
		string filename = Environment.GetEnvironmentVariable("TEMP") + "\\" + ((dynamic)range.Value2).ToString() + "_" + ymd + hms + "_" + "眼科同意書.xlsm";
		// マクロ有効形式(.xlsm)を明示して保存する。FileFormat未指定だとマクロ無効形式へ変換され、
		// 全シートのボタン（フォームコントロール／ActiveX）が一時ファイルから消える。
		exWorkbook.SaveAs(filename, XlFileFormat.xlOpenXMLWorkbookMacroEnabled, Missing.Value, Missing.Value, Missing.Value, Missing.Value, XlSaveAsAccessMode.xlExclusive, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
		exWorksheet = (_Worksheet)(dynamic)exWorkbook.Sheets[resolveSheetName(sheetName)];
		exWorksheet.Select(true);
		// 自動処理が終わったのでイベントを元に戻す（ユーザーの手動印刷操作用）。
		exApp.EnableEvents = true;
		// 描画凍結を解除し、最終シートを再描画させる。
		exApp.ScreenUpdating = true;
		Marshal.ReleaseComObject(range);
		Marshal.ReleaseComObject(range2);
		Marshal.ReleaseComObject(range3);
		Marshal.ReleaseComObject(range4);
		Marshal.ReleaseComObject(range5);
	}

	// 定数（バーコード画像の解像度）。1モジュール=BARCODE_LINE_WIDTH px。
	private const float BARCODE_LINE_WIDTH = 3f;

	private const float BARCODE_HEIGHT = 80f;

	private const int BARCODE_QUIET_MODULES = 10;

	private void insertBarcode(_Worksheet sheet, string barcodeText)
	{
		if (string.IsNullOrEmpty(barcodeText))
		{
			return;
		}
		// CODE128-C は数字を2桁ずつ符号化するため、36桁（偶数・全数字）でないと
		// 桁ずれを無言で誤エンコードする。満たさない場合は挿入しない。
		if (barcodeText.Length != 36 || !isAllDigits(barcodeText))
		{
			System.Diagnostics.Debug.WriteLine("バーコード値が36桁の数字でないため挿入をスキップ: " + barcodeText);
			return;
		}
		string tempPath = null;
		Range anchor = null;
		object shapes = null;
		object picture = null;
		try
		{
			tempPath = generateBarcodeImage(barcodeText);
			anchor = (Range)(dynamic)sheet.Cells[1, 4]; // D1
			float left = (float)(double)anchor.Left;
			float top = (float)(double)anchor.Top;
			shapes = sheet.Shapes;
			// AddPicture(filename, LinkToFile=msoFalse(0), SaveWithDocument=msoTrue(-1), left, top, width, height)。
			// MsoTriState は CLI ビルドでは未参照のため、遅延バインドで int(0/-1) を渡す。
			picture = ((dynamic)shapes).AddPicture(tempPath, 0, -1, left, top, 250f, 30f);
			((dynamic)picture).Placement = (int)XlPlacement.xlMove;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine("バーコード挿入エラー: " + ex.Message);
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
		int totalModules = symbols * 11 + 13 + BARCODE_QUIET_MODULES * 2;
		int widthPx = (int)Math.Ceiling(totalModules * BARCODE_LINE_WIDTH);
		int heightPx = (int)Math.Ceiling(BARCODE_HEIGHT);
		string tempPath = Path.Combine(Path.GetTempPath(), "barcode_" + Guid.NewGuid().ToString("N") + ".png");
		using (Bitmap bitmap = new Bitmap(widthPx, heightPx))
		{
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				g.Clear(Color.White);
				Barcode128 barcode = new Barcode128();
				float left = BARCODE_QUIET_MODULES * BARCODE_LINE_WIDTH;
				barcode.Draw(Barcode128.CODE.C, barcodeText, g, left, 0f, BARCODE_HEIGHT, BARCODE_LINE_WIDTH);
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
