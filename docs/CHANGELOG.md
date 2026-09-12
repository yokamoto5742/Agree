# 変更履歴

このプロジェクトのすべての重要な変更は、このファイルに記録されます。

フォーマットは [Keep a Changelog](https://keepachangelog.com/ja/1.1.0/) に基づいており、
バージョン番号は [Semantic Versioning](https://semver.org/lang/ja/) に従っています。

## [Unreleased]

### 変更
- DB接続を OLE DB（`MSDAORA.1` / `OraOLEDB.Oracle`）から **ODP.NET マネージド・ドライバ**
  （`Oracle.ManagedDataAccess` 4.122.19.1）へ移行。**Oracle クライアントのインストールが不要**になった
  （電子カルテ端末にOracleクライアントを導入しない方針への対応。DBはOracleのまま）
- 外部DLL `AgentlabUtilityLibrary.dll` のDB機能への依存を解消。接続生成を `Agree/OracleDb.cs`、
  診療科・職員マスタの読込を `Agree/MasterDict.cs` としてリポジトリ内に実装（旧 `DBConn` / `Dict` の置き換え）。
  外部DLLは `Env`（ini読込）と `Barcode128`（バーコード描画）のみ使用する
- 結合テストもマネージド・ドライバへ移行。32bit `OraOLEDB` の regsvr32 登録が不要になった

### 注意
- 配布時は `Oracle.ManagedDataAccess.dll` と **`tnsnames.ora`** をexeと同じフォルダに配置すること。
  Oracleクライアントを撤去する場合は、撤去前に `%ORACLE_HOME%\network\admin\tnsnames.ora` を退避しておく

## [1.0.2] - 2026-09-11

### 変更
- Excel共通情報の入力者ID/氏名（B7/B11）を Pat.csv の操作者からDB保存値（AGREE.DR）に変更。誰が印刷しても同意書作成時の入力者で出力される（36桁バーコードの医師5桁も連動）
- 職員マスタに存在しないコード（退職者等）の同意書を選択した際、入力者ID/氏名を空にせず一覧のDB値を表示するよう変更

### 修正
- 36桁バーコードの日付8桁と共通情報B8が印刷日になっていたため、同意書の作成日を使うよう修正（時刻6桁は印刷時刻のまま）

## [1.0.1] - 2026-06-28

### 追加
- フォーム UI を複数ファイルに分割（Form1.Designer.cs, Form1.ImportExport.cs, Form1.Plan.cs）し、保守性を向上

### 変更
- バーコード文書コード（5桁）の取得をExcelシートから EyeAgreeSettings.ini の BARCODE_SETTINGS セクションに変更
- Excel出力のセル配置を B 列中心に整理

### 修正
- Form1.Plan.cs および Form1.ImportExport.cs がデザイナで正しく認識されるよう SubType 要素を追加
- IContainer の初期化と未使用フィールド（OleDbDataReader）をクリーンアップ

## [1.0.0] - 2026-06-19
- 初版リリース
