# 眼科同意書システム（Agree）

眼科の手術・検査に関する**同意書を作成・管理し、Excel 帳票として出力する** Windows デスクトップアプリです。電子カルテ側から渡された患者情報をもとに、患者・主治医・病名・術式・説明内容を入力し、定型テンプレートを差し込んだ同意書を生成します。

## 目次

- [特徴](#特徴)
- [動作環境](#動作環境)
- [インストール方法](#インストール方法)
- [使い方](#使い方)
- [設定](#設定)
- [トラブルシューティング](#トラブルシューティング)
- [開発者向け情報](#開発者向け情報)
- [ライセンス](#ライセンス)
- [更新履歴](#更新履歴)

## 特徴

汎用の文書作成ツールではなく、**眼科の同意書業務の流れに特化**しています。

- **電子カルテ連携** — 受付済みの患者情報（`Pat.csv`）を読み込み、患者番号・氏名・診療科・主治医を自動セットします。患者番号を入力して Enter を押すと、その患者の既存同意書が一覧表示されます。
- **Excel 帳票の自動生成** — テンプレート `Agree_眼科同意書.xlsm` にデータを差し込み、患者ごとの同意書を生成します。入院・日帰り・通常などのシート切り替えに対応します。
- **テンプレート管理** — 病名・術式・説明文などの定型文をテンプレートとして登録・並び替え・バージョン管理でき、入力の手間を削減します。説明医・立会者などの**スタッフテンプレート**も別途管理します。
- **コピーして新規作成** — 既存の同意書を複製して新しい同意書を素早く作成できます。
- **CSV エクスポート / インポート** — 同意書データを CSV で入出力できます（設定で表示）。
- **オフラインモード** — データベースに接続できない場合は画面確認用のオフラインモードで起動し、登録系の操作を抑止します。
- **多重起動防止** — すでに起動中のインスタンスがあれば確認のうえ終了させ、二重起動を防ぎます。

<div align="right"><a href="#目次">▲ 目次へ戻る</a></div>

## 動作環境

- Windows（32bit / 64bit いずれの OS でも、アプリは **x86 専用ビルド**で動作）
- .NET Framework 4.8
- **Microsoft Excel**（COM 連携で帳票を生成するため必須）
- Oracle データベースへの接続環境（OleDb / `OraOLEDB.Oracle` 32bit プロバイダ）

### 外部依存（重要）

- **`AgentlabUtilityLibrary.dll`** — DB 接続（`DBConn`）や患者情報取得を提供する外部 DLL。これが無いとビルド・実行とも失敗します。接続文字列・認証情報はこの DLL 側で管理され、リポジトリ内に設定はありません。
- **Excel テンプレート** — `Agree_眼科同意書.xlsm` が `AGENT_HOME` 配下に必要です。

<div align="right"><a href="#目次">▲ 目次へ戻る</a></div>

## インストール方法

開発・ビルド手順は次のとおりです。

1. リポジトリを取得します。
   ```sh
   git clone <repository-url>
   cd Agree
   ```
2. Visual Studio で `Agree.slnx`（または `Agree.csproj`）を開きます。
3. 外部依存 `AgentlabUtilityLibrary.dll` と Excel テンプレート `Agree_眼科同意書.xlsm`、患者情報受け渡し用の `Pat.csv` が、配置先（`AGENT_HOME` / `LEGACY_HOME`）に揃っていることを確認します。
4. **F5** で実行します（プラットフォームは x86）。

CLI でビルドの検証のみ行う場合:

```sh
dotnet build Agree.slnx
```

> COM 参照は `$(MSBuildRuntimeType)` で分岐し、`dotnet`（.NET Core 版 MSBuild）では同梱の Excel PIA を、Visual Studio のフル版 MSBuild では登録済み Excel の COM 参照を使います。CLI ビルド時のみ参照され、VS の成果物には影響しません。

<div align="right"><a href="#目次">▲ 目次へ戻る</a></div>

## 使い方

1. アプリを起動します（通常は電子カルテのランチャーから、患者を選択した状態で呼び出されます）。
2. **患者番号を入力して Enter** を押すと、その患者の既存同意書が一覧に表示されます。
3. 一覧から開く、または **新規作成**で同意書を作成します。
   - 必須項目（患者・主治医・診療科）が未入力の場合はガードが表示されます。
   - **コピーして新規作成**（一覧の右クリックメニュー）で既存の同意書を複製できます。
4. 病名・術式・説明内容などを入力します。**テンプレート**ボタンから定型文を呼び出せます。
5. **登録**で保存します。保存後は一覧へ反映されます。
6. **印刷**で Excel 帳票（同意書）を生成します。Excel が起動し、対象シートが選択された状態で開きます。

<div align="right"><a href="#目次">▲ 目次へ戻る</a></div>

## 設定

UI の挙動はリポジトリ同梱の `AgreeSettings.ini` で制御します。

```ini
[UI_SETTINGS]
; 1 = 設定ボタンを表示する, 0 = 非表示にする
SHOW_SETTING_BUTTON=0
```

`SHOW_SETTING_BUTTON=1` にすると、CSV エクスポート／インポートなどの設定機能を呼び出す**設定ボタン**が画面に表示されます。

<div align="right"><a href="#目次">▲ 目次へ戻る</a></div>

## トラブルシューティング

| 症状 | 原因と対処 |
| --- | --- |
| 「データベースに接続できません。オフラインモードで起動します」と表示される | DB に接続できない状態です。画面確認は可能ですが登録系は使えません。ネットワーク／DB の稼働、`AgentlabUtilityLibrary.dll` の接続設定、32bit OleDb プロバイダ（`OraOLEDB.Oracle`）の登録を確認してください。 |
| ビルド・起動が失敗する | `AgentlabUtilityLibrary.dll` が見つからない可能性があります。配置を確認してください（ソースには含まれません）。 |
| 印刷時にエラー／帳票が出ない | Excel がインストールされているか、テンプレート `Agree_眼科同意書.xlsm` が `AGENT_HOME` 配下にあるかを確認してください。 |
| 印刷後に Excel プロセスが残る | COM オブジェクトの解放漏れが原因です（通常は `ReleaseExcel` で解放されます）。残った Excel プロセスを終了してください。 |
| 患者情報が自動で入らない | `Pat.csv` が `LEGACY_HOME` 配下に存在し、内容が空でないかを確認してください。 |
| 起動時に「すでに起動している同意書システムを終了してよろしいですか？」と出る | 多重起動防止の確認です。OK で既存インスタンスを終了して起動します。 |
| `'`（アポストロフィ）を含む病名・説明で登録が不安定 | 一部の保存経路でエスケープ未対応の既知事項です。詳細は `docs/test_strategy.md` を参照してください。 |

<div align="right"><a href="#目次">▲ 目次へ戻る</a></div>

## 開発者向け情報

- 技術スタック: C# / .NET Framework 4.8 / Windows Forms（**x86 専用ビルド**）
- エントリポイント: `Agree/Program.cs` → `Form1`
- DB アクセス: `AgentlabUtilityLibrary.DBConn.GetOpenDBConn()`（OleDb）。主なテーブルは `AGREE` / `AGREE_TEMPLATE` / `AGREE_STAFF` / `M_PATIENT` / `M_DEPT` / `M_USR`。
- Excel 生成: `Microsoft.Office.Interop.Excel`（COM）。使用後は `ExcelControl.ReleaseExcel` で必ず解放します。

テスト（詳細は `docs/test_strategy.md`）:

```sh
# 単体テスト（DB 不要・どこでも実行可）
dotnet test Agree.Tests/Agree.Tests.csproj --filter "TestCategory=Unit"

# 結合テスト（ローカル Oracle が必要）
dotnet test Agree.Tests/Agree.Tests.csproj --filter "TestCategory=Integration"
```

コミット規約は `.claude/rules/commit.md`、コーディング指針は `.claude/rules/coding-guidelines.md` を参照してください。

<div align="right"><a href="#目次">▲ 目次へ戻る</a></div>

## ライセンス

このプロジェクトのライセンス情報については、[LICENSE](docs/LICENSE) を参照してください。

<div align="right"><a href="#目次">▲ 目次へ戻る</a></div>

## 更新履歴

更新履歴は [CHANGELOG.md](docs/CHANGELOG.md) を参照してください。

<div align="right"><a href="#目次">▲ 目次へ戻る</a></div>
