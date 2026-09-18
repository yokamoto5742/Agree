# EyeAgree データフロー

眼科同意書システムにおける患者情報の読込からExcel同意書生成までのデータフローを実装に沿って整理する。
行番号はすぐに古くなるため、コードは「クラス.メソッド名」で参照する（`Form1` は `Form1.cs` / `Form1.Agree.cs` / `Form1.ImportExport.cs` に分割）。

## 概要

```
【起動時】Form1 コンストラクタ → initShow
1. Pat.csv 読込（起点）        → 患者ID・氏名・性別を取得        Form1.readPatCsv
2. その患者IDでOracle照会      → 既存同意書一覧（手入力の別患者は氏名も）  Form1.showList

【印刷時】"印刷"ボタン or 登録後  Form1.printAgree
3. 画面の入力内容を 行番号→値 の Dictionary に集約
4. テンプレEyeAgree.xlsmを開く（原本名）   ExcelControl.Open
5. 共通情報シートへ書込                    ExcelControl.setValue
6. バーコード生成→各シートに画像挿入      ExcelControl.buildBarcodeValue / insertBarcodeToFormSheets
7. %TEMP%に別名で保存（ここで改名）       ExcelControl.saveWorkbook
```

正確な順序は **「pat.csv（起点）→ Oracle照会 → Excelテンプレートを開く → 共通情報書込 → バーコード挿入 → 別名保存」** である。

## 各ステップの詳細

### 1. Pat.csv の読込が起点 (`Form1.readPatCsv`)

`Env.LEGACY_HOME\Pat.csv` の先頭行をカンマ分割し、**使う項目だけ**を `patCsv`（`PatCsvFields`）へ格納する（`loadPatCsvFields`）。`[2]`=患者ID、`[3]`=氏名、`[5]`=カナ、`[6]`=性別。**このCSVが患者IDと患者氏名の供給源**で、電子カルテ側が書き出したファイルを介して連携する。患者IDが数字でない場合は読み飛ばす。

新規作成時は `Form1.applyDoctorFromPatCsv` が同じCSVの医師情報（`[9]`=入力者ID、`[10]`=氏名、`[13]`=診療科コード）を画面に反映する。`[14]`・`[27]` は用途不明だが反映条件の判定に使うため保持する。

### 2. Oracle照会 (`Form1.showList`)

CSVで得た患者IDをキーに、`AgentlabUtilityLibrary.DBConn.GetOpenDBConn()`（`Form1` コンストラクタ）で取得したOleDb接続を使う。接続の Open/Close は `Db` ヘルパー（`Db.Execute` / `Scalar` / `Read`）が行い、例外時も必ず閉じる。

- 患者IDが Pat.csv と同じなら氏名・カナ・性別は CSV の値を使い、`M_PATIENT` には問い合わせない。**手入力で別の患者IDを検索したときだけ** `M_PATIENT` から氏名・カナ・性別を取得する（取り違え防止のため、取得前に氏名欄を空にする）
- `AGREE`（`M_DEPT`/`M_USR`結合）から既存同意書一覧を取得しグリッド表示する。グリッドの列はSELECTの列名・別名（`DEPT_NAME` / `DR_NAME` など）で参照する（`Form1.showAgree`）

Oracleは患者情報だけでなく、**診療科マスタ**（`M_DEPT` の CODE・S_NAME のみ、`Form1` コンストラクタ）、**入力者氏名**（`M_USR` から該当1人の NAME のみ、`Ehr.StaffName`）、**担当者文例**（`AGREE_STAFF`、`Form1.getStaffRoom`）、同意書レコードのCRUD（`Form1.regAgree` / `delAgree`）にも使われる。接続失敗時はオフラインモード（画面確認用、DB読込スキップ）になる。

### 3. 共通情報シートへの書込データ準備 (`Form1.printAgree`)

`行番号→値` の Dictionary を構築する（すべて B 列に書き込む）。患者情報（B1〜B4）・診療科（B5〜B6）に加え、**入力者ID/氏名は選択中の同意書のDB値(`AGREE.DR`)を `dr_id`/`dr_name` 経由でB7/B11へ出力**する（一覧選択時に `showAgree` がセットする。Pat.csv の操作者情報は使わないため、誰が印刷しても同じ入力者になる）。作成日をB8、担当者・眼・手術名・麻酔・病名・説明・症状・治療計画・手術内容をB12〜B20へ割当てる。

患者IDと作成日はバーコード値・ファイル名にも使うため、`MakeEyeAgree` の引数としても渡す。

### 4-5. Excelテンプレートを開いてセル書込 (`ExcelControl.MakeEyeAgree` / `Open`)

`Env.AGENT_HOME\EyeAgree\EyeAgree.xlsm` を**原本名のまま**開き「共通情報」シートを選択する。描画凍結(`ScreenUpdating=false`)後、`setValue` でDictionaryをセルに展開し、印刷時刻をB9へ書込む。

### 6. バーコード生成・挿入 (`ExcelControl.buildBarcodeValue` / `insertBarcodeToFormSheets`)

患者ID9桁（引数）+ **文書コード5桁** + 診療科3桁（B5）+ 入力者5桁（B7）+ 作成日8桁（引数）+ 印刷時刻6桁 から**36桁のCODE128-C値**を組立ててB10に書込む。文書コードは`EyeAgreeSettings.ini`の`[BARCODE_SETTINGS] DOCUMENT_CODE`（既定39911）から取得する（`loadBarcodeSettings`、INIの読込は `AppSettings`）。さらに「共通情報」以外の全フォームシートをループし、`Barcode128`でPNG画像を生成してD1セルに貼付ける（`insertBarcode`）。**保存より前**に行うことで保存ファイルに画像を残す。

### 7. 別名で保存＝改名 (`ExcelControl.saveWorkbook`)

`%TEMP%\{患者ID}_{印刷日yyyyMMdd}{HHmmss}_EyeAgree.xlsm` として`SaveAs`（xlsm形式）。**改名はこの保存時点**で、開いた原本テンプレートはそのまま残る。最後に対象シート（`入院/日帰り`→`通常`に解決、`resolveSheetName`）を選択し描画を復帰する。`ExcelControl` は `IDisposable` で、`printAgree` の `using` を抜けるときに COM を解放する。
