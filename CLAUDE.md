# CLAUDE.md

このファイルは、本リポジトリ内のコードを扱う際に Claude Code（claude.ai/code）に対するガイダンスを提供します。
必ず日本語で回答してください。

## プロジェクト概要

眼科の同意書を管理するC# WinFormsデスクトップアプリ（医師の同意書システム）。
患者・医師情報を扱い、Excelで同意書を生成する。UI文字列はすべて日本語。

## 技術スタック

- C# / .NET Framework 4.8 / Windows Forms
- **32bit (x86) 専用ビルド** — PlatformTargetがx86のため64bit環境では設定に注意
- エントリポイント: `Program.cs` → `Form1`（起動時に多重起動チェックあり）

## ビルドと実行

- Visual Studioで `Agree.csproj` を開き、F5で実行する（通常の開発手順）
- **CLIでのビルド検証**: `dotnet build Agree.slnx`（または VS Code の build タスク）。
  COM参照は `$(MSBuildRuntimeType)` で分岐し、フル版MSBuild(VS)は登録済みExcelの
  `COMReference`、.NET Core版MSBuild(`dotnet`)はリポジトリ同梱の Excel PIA
  (`Microsoft.Office.Interop.Excel.dll`, `EmbedInteropTypes`)を使う。
  resx生成用に `System.Resources.Extensions.dll` も同梱。いずれもCLIビルド時のみ参照され、
  VSの成果物には影響しない。

## 外部依存（重要）

- **`Agree/Infrastructure/`** は旧 `AgentlabUtilityLibrary.dll` のソースを取り込んだもの
  （`Env` / `DBConn` / `Enc` / `CharMap` / `Barcode128`。名前空間は `AgentlabUtilityLibrary` のまま）。
  外部DLLの配置は不要になったが、実行時には設定ファイル `AgentlabUtilityLibrary.ini` が必要。
- DB接続は `AgentlabUtilityLibrary.DBConn.GetOpenDBConn()`（同意書側のテーブル）と
  `GetEhrDBConn()`（電子カルテのマスタ。`Agree/Ehr.cs` だけが使う）の OleDb 接続経由。
  接続文字列・認証情報は `AgentlabUtilityLibrary.ini`（暗号化された値）から読み、
  この ini は認証情報を含むため git 管理外（各環境で配置する）。
- Excel生成は `Microsoft.Office.Interop.Excel`（COM）を使用。Excelのインストールが必要で、
  COMオブジェクトは使用後に確実に解放すること（解放漏れでExcelプロセスが残る）。

## テスト・CI

- ユニットテスト・CIパイプラインは未整備。

## コミット

- `.claude/rules/commit.md` の規約に従う（絵文字プレフィックス + 日本語）。
