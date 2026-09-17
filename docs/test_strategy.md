# テスト戦略 — 眼科同意書システム（Agree）

## 1. このアプリの性質と「テストしにくさ」

本体は **WinForms + 画面内インライン SQL + COM(Excel) + 外部DLL(AgentlabUtilityLibrary)**
で構成され、業務ロジックの大半はフォームのイベントハンドラ内に、TextBox 等の UI
フィールドへ直接張り付いている。そのため「アプリ本体を丸ごと参照して単体テストする」
のは現実的でない（COM Excel・外部DLL・OleDb プロバイダを test 側へ巻き込むことになる）。

そこで **テストピラミッドを3層に分け**、自動化できる層を確実に回し、できない層は
手動チェックリストに落とす方針とする。

```
        ┌─────────────────────────────┐
  L3    │ 手動スモーク（WinForms + Excel帳票）  │  ← 自動化対象外。§5 のチェックリスト
        ├─────────────────────────────┤
  L2    │ 結合テスト（ローカル Oracle / OleDb）    │  ← DB契約を検証。Oracle が在る所だけ実行
        ├─────────────────────────────┤
  L1    │ 単体テスト（純粋ロジック / ソースリンク） │  ← 依存ゼロ・高速。常時グリーン（CIの背骨）
        └─────────────────────────────┘
```

## 2. L1 — 純粋ロジックの単体テスト（依存ゼロ・高速）

- 対象: `Agree/AgreeSql.cs` の `SqlValue` / `CsvEscape`。
  これらは元々 `Form1` の private 実装だったものを **挙動を変えずに static 切り出し**し、
  Form1 はそれを呼ぶように変更した（呼び出し3+2箇所）。
- テスト: `Agree.Tests/AgreeSqlTests.cs`（12 ケース）。
  `'` の二重化・null→NULL・日本語無損失・CSV のカンマ/引用符/改行エスケープ等を固定する。
- **仕組み（重要）**: `Agree.Tests.csproj` は本体 `Agree.csproj` を参照せず、
  `AgreeSql.cs` **1ファイルだけを `<Compile Link>` で取り込む**。
  これにより COM(Excel)・外部DLL を test に持ち込まずに「本番と同一の実装」を検証できる。
  → 新たに純粋ロジックを切り出す時は同じく `AgreeSql.cs`（または同種の純粋クラス）へ置き、
     リンク対象に追加する。WinForms/COM/DLL に触れる関数はここへ移さないこと。

## 3. L2 — 結合テスト（ローカル Oracle / OleDb）

- 対象: `Agree.Tests/OracleIntegrationTests.cs`。アプリが依存する **SQL 契約**
  （AGREE / AGREE_TEMPLATE / AGREE_STAFF の往復、`regPlan` が書く全列、日本語・`'` の無損失格納）を検証する。
- 本体や外部DLL には依存しない（生 OleDb で完結）。前提は `docs/test_db_schema.sql` /
  `docs/test_db_seed.sql` の投入と、32bit ODAC(OraOLEDB.Oracle) + ローカル Oracle Free。
  接続文字列は環境変数 `AGREE_TEST_ORACLE` で上書き可。
- DB 未起動・スキーマ未投入なら各テストは `Assert.Ignore`（スキップ）。
  ビット不一致・プロバイダ未登録は **環境構築ミス**として `Assert.Fail` で表面化させる
  （「DB未起動」と取り違えないため）。

## 4. 実行方法

```sh
# L1 のみ（DB 不要・どこでも緑）
dotnet test Agree.Tests/Agree.Tests.csproj --filter "TestCategory=Unit"

# L2 のみ（ローカル Oracle が必要）
dotnet test Agree.Tests/Agree.Tests.csproj --filter "TestCategory=Integration"

# 全部
dotnet test Agree.Tests/Agree.Tests.csproj
```

- テストホストは 32bit OraOLEDB をロードするため **x86 固定**（`x86.runsettings`）。
- 本体 `Agree.csproj` の COM 参照は `$(MSBuildRuntimeType)` で分岐する。Visual Studio（フル版 MSBuild）は
  登録済み Excel の COM 参照を、`dotnet build` は同梱の Excel PIA を使うため、どちらでもビルドできる
  （テストプロジェクトは SDK 形式なので `dotnet test` で動く）。

## 5. L3 — 手動スモークチェックリスト（自動化対象外）

WinForms 描画と Excel 帳票（COM）は自動テスト対象外。リリース前に最低限:

1. 患者番号入力 → Enter で既存同意書一覧が表示される。
2. 新規作成 → 必須（患者/主治医/診療科）未入力時に各ガードが出る。
3. 登録 → 一覧へ反映、再度開いて内容一致。
4. **`病名`や`説明`に `'`（アポストロフィ）を含めて登録できる**（§6 の対応済み項目の回帰確認）。
5. 印刷 → Excel 帳票が生成され、プロセスが残らない（`ReleaseExcel`）。
6. CSV エクスポート/インポート（設定ボタンで表示）が往復する。
7. オフラインモード（DB 不通）で起動し、登録系が抑止される。

## 6. テストで判明した不具合（対応済み）

- **インライン SQL のエスケープ漏れ**: `Form1.regAgree` / `TmpStaff.saveButton_Click` /
  `TmpAgree.regAgreeTemplate` の自由記述（`diag`/`staff`/`ope`/`cont`/`temp_name` 等）は、
  CSV インポート経路（`MergeRow`）と同じく `AgreeSql.SqlValue`（`'`→`''`）経由で SQL に埋め込むよう修正済み。
  `Form1.getStaffRoom` は整数に変換した医師コードだけを連結する。
  - L1 の `SqlValue` テストと L2 の `Agree_StoresApostropheAndJapanese_RoundTrips` が
    「正しいエスケープなら無損失で往復する」契約を固定している。
  - 画面経路が `SqlValue` を通っていることは自動テストでは検出できないため、§5 の手順 4 で確認する。

- **重複した古い csproj**: リポジトリ直下にあった `Agree.Tests.csproj` は削除済み
  （テストプロジェクトは `Agree.Tests/Agree.Tests.csproj` のみ）。
