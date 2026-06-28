# 変更履歴

このプロジェクトのすべての重要な変更は、このファイルに記録されます。

フォーマットは [Keep a Changelog](https://keepachangelog.com/ja/1.1.0/) に基づいており、
バージョン番号は [Semantic Versioning](https://semver.org/lang/ja/) に従っています。

## [Unreleased]

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
