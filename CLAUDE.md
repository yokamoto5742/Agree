# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

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

- **`AgentlabUtilityLibrary.dll`** はソースに含まれない外部DLL。`DBConn` クラスを提供し、
  これが無いとビルド・実行とも失敗する。
- DB接続は `AgentlabUtilityLibrary.DBConn.GetOpenDBConn()`（OleDb）経由。
  接続文字列・認証情報は外部DLL側で管理され、リポジトリ内に設定ファイルは無い。
- Excel生成は `Microsoft.Office.Interop.Excel`（COM）を使用。Excelのインストールが必要で、
  COMオブジェクトは使用後に確実に解放すること（解放漏れでExcelプロセスが残る）。

## テスト・CI

- ユニットテスト・CIパイプラインは未整備。

## コミット

- `.claude/rules/commit.md` の規約に従う（絵文字プレフィックス + 日本語）。
