# 変更履歴

このプロジェクトのすべての重要な変更は、このファイルに記録されます。

フォーマットは [Keep a Changelog](https://keepachangelog.com/ja/1.1.0/) に基づいており、
バージョン番号は [Semantic Versioning](https://semver.org/lang/ja/) に従っています。

## [Unreleased]

### 変更
- 電子カルテ（マスタ M_PATIENT / M_USR / M_DEPT と Pat.csv）へのアクセスを Ehr クラスに集約し、同意書テーブルとマスタを SQL で JOIN しないよう変更（将来の電子カルテ乗り換えに備えた内部構造の変更。画面の表示・操作は変更なし）
- 電子カルテのマスタを、同意書側とは別の接続（`DBConn.GetEhrDBConn()`）で読むよう変更。`AgentlabUtilityLibrary.ini` に `EHR_DB` / `EHR_USER` / `EHR_PWD` / `EHR_PROVIDER` を書くと別の接続先にでき、書かなければ従来と同じ接続先を使う
- 同梱の `AgentlabUtilityLibrary.dll` を最新版（3.0.0 + `GetEhrDBConn` 追加）に更新。**この DLL は `AgentlabUtilityLibrary.ini` が無い場合の DB 接続情報の既定値を持たないため、配置先に ini が必要**

## [1.1.2] - 2026-09-17

### 削除
- 使われていないアセンブリ参照（System.Net.Http / System.Deployment / System.Xml.Linq / System.Data.DataSetExtensions / System.Xml）と COM 参照（Microsoft.Office.Core / VBIDE）を削除
- 中身が空で使われていない Properties の Settings.settings / Resources.resx と、その自動生成コードを削除
- Form1 の常に非表示で使われていない眼の選択チェックボックス（両・左・右）を削除（画面の見た目・操作は変更なし）

### 変更
- ローカルテスト用の接続設定ファイル test.udl（認証情報を含む）を git の管理対象から外し、.gitignore に追加

## [1.1.1] - 2026-09-17

### 変更
- CSV 取り込みが途中で失敗した場合、エラーメッセージに失敗前に取り込み済みのテーブル（ロールバックされない分）を表示するよう変更

## [1.1.0] - 2026-09-15

### 修正
- 同意書（共通情報B4）の性別が常に「男」で出力されていた問題を修正
- DB エラー（例: 患者IDに数字以外を入力して Enter）の後に接続が開いたまま残り、アプリを再起動するまで一覧表示・登録・印刷ができなくなる問題を修正。患者ID・入力者IDは数字であることを検証する
- 「記載中の内容を保存しますか？」で「はい」を選び、入力チェックで保存が中断した場合に入力内容が消えていた問題を修正（テンプレート画面の切替時も同様）
- 同じ分類に同名のテンプレートがあると、別のテンプレートが表示・適用される問題を修正
- Pat.csv の患者IDや診療科コードが数字でない場合に起動・新規作成が例外で失敗する問題を修正

### 変更
- EyeAgreeSettings.ini の読込を AppSettings に統一し、起動後に1回だけ読み込むよう変更（設定の変更はアプリの再起動後に反映）
- 診療科コードの許容範囲を 1〜20 に統一（Pat.csv からの反映は従来 1〜19）
- ビルド構成を x86 のみに整理（AnyCPU 構成を削除。出力先は bin\x86\Debug / bin\x86\Release）
- 内部構造を整理（DB アクセスを Db ヘルパーに集約、Form1.Plan.cs を Form1.Agree.cs に改名し Plan→Agree に名称統一、一覧グリッドを列名で参照、ExcelControl を IDisposable 化、未使用コード・ファイルを削除、例外時のログ記録を統一）

## [1.0.3] - 2026-09-14

### 修正
- テンプレート未配置時に「Excelが起動しているか確認してください」と誤案内していた問題を修正。ExcelControl.Open で FileNotFoundException に探索パスを含めるようにし、Form1.Plan.cs の printAgree で個別に捕捉してテンプレートの配置先パスを表示するようにした

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
