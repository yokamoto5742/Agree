# 設定ファイル・設定キー一覧

アプリが読む設定ファイルと、キーの用途・既定値をまとめる。

> **機密情報の扱い**：`AgentlabUtilityLibrary.ini` には DB の接続先と認証情報が入る。
> 本書にはキー名と用途だけを載せ、**値は載せない**。設定値をドキュメント・ログ・チケットに転記しないこと。

## 1. 設定ファイル一覧

| ファイル | 読むクラス | 探す場所 | 形式 | 読込タイミング |
| --- | --- | --- | --- | --- |
| `EyeAgreeSettings.ini` | `Agree.AppSettings` | exe と同じフォルダ（`AppDomain.CurrentDomain.BaseDirectory`） | `key=value`（セクション名は無視） | 初回参照時に 1 回 |
| `AgentlabUtilityLibrary.ini` | `AgentlabUtilityLibrary.Env` | カレントディレクトリ（`Program` が exe のフォルダに変更）、無ければ `c:\macs\utility\` | 開始行・終了行で囲んだブロックの中に `key=value` | 初回参照時に 1 回 |
| `App.config` | .NET ランタイム | exe と同じフォルダ | XML | 起動時 |

## 2. EyeAgreeSettings.ini（アプリ専用）

ファイルが無い・読めない・値が不正なときは、コードの既定値を使う。
セクション名は読込時に無視するため、**キー名はファイル全体で一意にする**必要がある（重複したら後の行が有効）。

| セクション（見出し） | キー | 型 | コードの既定値 | 同梱ファイルの値 | 用途 | 参照箇所 |
| --- | --- | --- | --- | --- | --- | --- |
| `[UI_SETTINGS]` | `SHOW_SETTING_BUTTON` | "1" / その他 | "0"（非表示） | 0 | "1" のとき、CSV 入出力を開く「設定」ボタンを表示 | `Form1.applySettingButtonVisibility` |
| `[BARCODE_SETTINGS]` | `BARCODE_LINE_WIDTH` | 小数 > 0 | 3 | 3 | バーコード 1 モジュールの幅（px） | `ExcelControl.loadBarcodeSettings` |
| 〃 | `BARCODE_HEIGHT` | 小数 > 0 | 80 | 80 | バーコード画像の高さ（px） | 〃 |
| 〃 | `BARCODE_QUIET_MODULES` | 整数 ≥ 0 | 10 | 10 | 両側の余白（モジュール数） | 〃 |
| 〃 | `DOCUMENT_CODE` | 数字のみ | 39911 | 39911 | バーコード値に入れる文書コード（5 桁想定） | 〃 |
| `[WINDOW_SETTINGS]` | `WINDOW_X` | 整数 | 0 | 100 | 画面の初期位置 X（px） | `Form1.applyWindowPosition` |
| 〃 | `WINDOW_Y` | 整数 | 0 | 100 | 画面の初期位置 Y（px） | 〃 |

`BARCODE_*` と `DOCUMENT_CODE` は印刷のたびに `ExcelControl` が参照する（ファイル自体の読込は 1 回だけ）。

## 3. AgentlabUtilityLibrary.ini（DB 接続・連携フォルダの設定）

`Env` が読み、Agree は `DBConn.GetOpenDBConn()` / `DBConn.GetEhrDBConn()` と `Env` のプロパティを通して間接的に使う。
値の一部は暗号化されていて、`Enc.Decrypt` で復号してから使う。

| ブロック | キー | 暗号化 | 用途 | Agree での利用 |
| --- | --- | --- | --- | --- |
| `[HOME Config Start]`〜`[HOME Config End]` | `LEGACY_HOME` | なし | 電子カルテ連携フォルダ | `Pat.csv` の場所（`Ehr.ReadPatCsv`） |
| 〃 | `AGENT_HOME` | なし | アプリ資材フォルダ | `EyeAgree\EyeAgree.xlsm` の場所（`ExcelControl.MakeEyeAgree`） |
| `[DB Config Start]`〜`[DB Config End]` | `OPEN_DB` | あり | 同意書側のテーブルの接続先（Data Source） | `DBConn.GetOpenDBConn`（`AGREE` / `AGREE_TEMPLATE` / `AGREE_STAFF`） |
| 〃 | `OPEN_USER` | あり | 接続ユーザー | 〃 |
| 〃 | `OPEN_PWD` | あり | パスワード | 〃 |
| 〃 | `PROVIDER` | なし | OleDb プロバイダ名（例：`OraOLEDB.Oracle`）。空なら `MSDAORA.1` | 〃 |
| 〃 | `EHR_DB` | あり | 電子カルテのマスタの接続先。無ければ `OPEN_DB` と同じ | `DBConn.GetEhrDBConn`（`Ehr` が使う） |
| 〃 | `EHR_USER` | あり | 接続ユーザー。無ければ `OPEN_USER` と同じ | 〃 |
| 〃 | `EHR_PWD` | あり | パスワード。無ければ `OPEN_PWD` と同じ | 〃 |
| 〃 | `EHR_PROVIDER` | なし | OleDb プロバイダ名。無ければ `PROVIDER` と同じ | 〃 |
| 〃 | `DB_LINK` | あり | マスタ参照用の DB リンク接尾辞（例：`@リンク名`）。空ならローカルのテーブル | `M_PATIENT` / `M_DEPT` / `M_USR` の参照 |
| 〃 | `MAIN_DB` / `MAIN_USER` / `MAIN_PWD` | あり | メイン DB の接続情報 | **使わない**（現在の `Env` は読まない） |

- `AgentlabUtilityLibrary.ini` は認証情報を含むため、`.gitignore` で git の管理から外している（各環境で配置する）。
  以前は git で管理していたので、過去のコミットには残っている。
- ini がどちらの場所にも無い場合は、`LEGACY_HOME = C:\macs`、`AGENT_HOME = C:\macs\utility`、`PROVIDER = MSDAORA.1` になり、接続情報は空になる。
- アプリ用テーブル（`AGREE` など）は `DB_LINK` を付けず、`OPEN_USER` のスキーマで参照する。
- `EHR_*` を書かなければ、同意書側と電子カルテ側は同じ接続先になる（従来と同じ動作）。起動時のオフライン判定は電子カルテ側（診療科一覧の読込）で行うため、接続先を分けた場合、同意書側に接続できないことは操作時のエラーで分かる。

## 4. App.config・ビルド設定・環境変数

| 項目 | 場所 | 値・内容 | 用途 |
| --- | --- | --- | --- |
| `supportedRuntime` | `App.config` | `v4.0` | .NET Framework 4.x で起動 |
| `PlatformTarget` | `Agree.csproj` / `Agree.Tests.csproj` | `x86` | 32bit OleDb プロバイダを使うため |
| `EyeAgreeSettings.ini` のコピー | `Agree.csproj`（Content） | 出力フォルダへコピー | 実行時に exe の隣に置く |
| `AgentlabUtilityLibrary.ini` のコピー | `Agree.Tests.csproj`（Content） | 出力フォルダへコピー | テスト実行時 |
| `x86.runsettings` | `Agree.Tests/` | テストホストを x86 に固定 | 結合テスト |
| `AGREE_TEST_ORACLE` | 環境変数 | 結合テスト用の接続文字列 | 未設定なら `OracleIntegrationTests` の既定値（ローカルの FREEPDB1） |

## 5. 固定パス・命名規則（コードに直接書かれているもの）

| 項目 | 値 | 参照箇所 |
| --- | --- | --- |
| Pat.csv | `{LEGACY_HOME}\Pat.csv` | `Ehr.ReadPatCsv` |
| 帳票テンプレート | `{AGENT_HOME}\EyeAgree\EyeAgree.xlsm` | `ExcelControl.MakeEyeAgree` |
| 共通情報シート名 | `共通情報` | 〃 |
| 帳票の出力先 | `%TEMP%\{患者ID}_{yyyyMMdd}{HHmmss}_EyeAgree.xlsm` | `ExcelControl.saveWorkbook` |
| バーコード一時画像 | `%TEMP%\barcode_{GUID}.png`（貼り付け後に削除） | `ExcelControl.generateBarcodeImage` |
| ログ | `%LOCALAPPDATA%\EyeAgree\logs\EyeAgree_yyyyMMdd.log`（30 日保持） | `Logger` / `Program.Main` |
| バックアップ CSV | 選んだフォルダの `AGREE.csv` / `AGREE_TEMPLATE.csv` / `AGREE_STAFF.csv` | `Form1.exportButton_Click` / `importButton_Click` |
| 診療科コードの範囲 | 1〜20 | `Form1.tryParseDeptCode` |
