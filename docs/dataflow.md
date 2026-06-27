# EyeAgree データフロー

眼科同意書システムにおける患者情報の読込からExcel同意書生成までのデータフローを実装に沿って整理する。

## 概要

```
【起動時】Form1.cs:857-882
1. Pat.csv 読込（起点）        → 患者ID・氏名・性別を取得   Form1.cs:1029-1051
2. その患者IDでOracle照会      → 氏名(正式値)・既存同意書一覧    Form1.cs:1074-1151

【印刷時】"印刷"ボタン or 登録後  Form1.cs:1445-1514
3. 画面入力＋Pat.csv医師情報をDictionaryに集約
4. テンプレEyeAgree.xlsmを開く（原本名）  ExcelControl.cs:98
5. 共通情報シートへ書込             ExcelControl.cs:125-147
6. バーコード生成→各シートに画像挿入   ExcelControl.cs:150-162
7. %TEMP%に別名で保存（ここで改名）   ExcelControl.cs:163-165
```

正確な順序は **「pat.csv（起点）→ Oracle照会 → Excelテンプレートを開く → 共通情報書込 → バーコード挿入 → 別名保存」** である。

## 各ステップの詳細

### 1. Pat.csv の読込が起点 (`readPatCsv` Form1.cs:1029)

`Env.LEGACY_HOME\Pat.csv` の先頭行をカンマ分割し `patCont[]` へ格納する。`patCont[2]`=患者ID、`[3]`=氏名、`[5]`=カナ、`[6]`=性別。**このCSVが患者IDの供給源**で、電子カルテ側が書き出したファイルを介して連携する。
 
### 2. Oracle照会 (`showList` Form1.cs:1074)

CSVで得た患者IDをキーに、`AgentlabUtilityLibrary.DBConn.GetOpenDBConn()`（起動時 Form1.cs:863）で開いたOleDb接続を使う。

- `M_PATIENT` から氏名・カナ・性別を取得し、CSV値を**正式値で上書き** (Form1.cs:1084-1100)
- `AGREE`（`M_DEPT`/`M_USR`結合）から既存同意書一覧を取得しグリッド表示 (Form1.cs:1086-1135)

Oracleは患者情報だけでなく、**診療科マスタ**(`Dict.DeptDict` Form1.cs:867)、**医師/担当者文例**(`AGREE_STAFF` `getStaffRoom` Form1.cs:1652)、同意書レコードのCRUD（`regPlan`/`delPlan`）にも使われる。接続失敗時はオフラインモード（画面確認用、DB読込スキップ）になる (Form1.cs:877)。

### 3. 共通情報シートへの書込データ準備 (`printAgree` Form1.cs:1445)

`"行,列"→値` のDictionaryを構築する。患者情報(3行)・診療科(5行)に加え、**医師ID/氏名はPat.csvを再度開いて** `patCont[9]/[10]` から取得 (Form1.cs:1462-1487)、術眼・病名・麻酔・説明・項目1〜4を27〜35行へ割当てる。

### 4-5. Excelテンプレートを開いてセル書込 (`MakeEyeAgree`/`Open` ExcelControl.cs:94,51)

`Env.AGENT_HOME\EyeAgree\EyeAgree.xlsm` を**原本名のまま**開き「共通情報」シートを選択する。描画凍結(`ScreenUpdating=false`)後、`setValue` でDictionaryをセルに展開し、日付`[8,2]`・時刻`[9,2]`も書込む。

### 6. バーコード生成・挿入 (ExcelControl.cs:139-162)

共通情報シートのセル値（患者9桁+医師5桁+科3桁+医師5桁+日付+時刻）から**36桁のCODE128-C値**を組立て`[11,2]/[23,2]`に書込む。さらに「共通情報」以外の全フォームシートをループし、`Barcode128`でPNG画像を生成してD1セルに貼付ける (ExcelControl.cs:243-321)。**保存より前**に行うことで保存ファイルに画像を残す。

### 7. 別名で保存＝改名 (ExcelControl.cs:163-165)

`%TEMP%\{患者ID}_{yyyyMMdd}{HHmmss}_EyeAgree.xlsm` として`SaveAs`（xlsm形式）。**改名はこの保存時点**で、開いた原本テンプレートはそのまま残る。最後に対象シート（`入院/日帰り`→`通常`に解決 ExcelControl.cs:74）を選択し描画を復帰する。
