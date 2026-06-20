# 変更履歴

このプロジェクトのすべての重要な変更は、このファイルに記録されます。

フォーマットは [Keep a Changelog](https://keepachangelog.com/ja/1.1.0/) に基づいており、
バージョン番号は [Semantic Versioning](https://semver.org/lang/ja/) に従っています。

## [Unreleased]

### リファクタリング（可読性・メンテナンス性向上、挙動は不変）
- デッドコード削除: `ExcelControl` の未使用メンバ（`FileName`/`SheetName`/`Save`/`RunVBA` と関連フィールド）、`Form1` の未使用フィールド（`tempPlanId`/`printPage`/`printOK`/`agreeDateList`/`agreeBoxList`）と未使用の印刷インフラ（`printDocument1`/`printPreviewDialog1` と空ハンドラ）、`TmpStaff` の空 `Load` ハンドラを削除。
- 重複の集約: `TmpAgree` の入力欄リセット処理（9 箇所）を `setTmpFields` ヘルパーに統合。`Form1` の Pat.csv 読込（`readPatCsv`/`readPatCsv2`）を `loadPatCsvFields` に統合。
- 可読性: 逆コンパイル由来の不明瞭なローカル変数名を改善（`text`/`flag` 等 → 意味のある名前）。

## [1.0.0] - 2026-04-24
- 初版リリース
