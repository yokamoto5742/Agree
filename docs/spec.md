# spec.md — 眼科同意書システム 仕様書

眼科の同意書を管理する C# WinForms デスクトップアプリ。患者・医師情報を Oracle DB から取得し、
WinForms 上で同意書を編集・登録し、Excel テンプレートで印刷用の同意書を生成する。UI 文字列はすべて日本語。

---

## 1. 起動に必要な前提条件

「正しく起動する」かどうかは、Visual Studio で開けるかではなく、以下のネイティブ／設定依存が満たされているかで決まる。

### 1.1 実行環境（OS・ランタイム）

| 項目 | 条件 | 根拠 |
|------|------|------|
| OS | Windows（32bit/64bit いずれも可） | WinForms / `Process.Kill` 等 |
| .NET | .NET Framework 4.8 | `App.config` の `supportedRuntime`、`TargetFrameworkVersion v4.8` |
| ビルド構成 | **x86（32bit）で実行すること** | 後述の Oracle OLE DB プロバイダ・Excel COM が 32bit のため。`Agree.slnx` は x86 構成を持つ |

> ビルド構成について: `Agree.csproj` の既定 PlatformTarget は `AnyCPU` だが、`Agree.slnx` でソリューションの
> x86 構成にマッピングされている。32bit の `Provider=MSDAORA.1`（Oracle OLE DB）と Excel COM を使うため、
> **必ず x86（32bit プロセス）で起動する**こと。64bit プロセスで起動すると DB 接続・Excel 連携が失敗する。

### 1.2 外部依存（これらが無いと起動・動作できない）

1. **32bit Oracle OLE DB プロバイダ（`MSDAORA.1`）**
   - DB 接続文字列は `Provider=MSDAORA.1;Data Source=...;User ID=...;Password=...`（`DBConn.GetOpenDBConn()`）。
   - 32bit Oracle クライアント（OLE DB プロバイダを含む）がインストールされていること。これが x86 ビルドを要求する理由。
   - `Data Source` に指定する Oracle ネットサービス名（TNS）が、クライアントの `tnsnames.ora` 等で解決できること。

2. **Oracle データベースと DB リンク**
   - マスタ参照クエリはすべてテーブル名に `Env.DB_LINK` を付加する（例: `M_DEPT@<dblink>`）。
   - 接続先 Oracle に、`AgentlabUtilityLibrary.ini` の `DB_LINK` で指定されたデータベースリンクが構成されていること。

3. **Microsoft Excel（COM / Interop）**
   - 同意書の生成・印刷は `Microsoft.Office.Interop.Excel`（COM）を使用（`ExcelControl`）。
   - Excel 本体がインストールされていること。COM オブジェクトは使用後に `Marshal.ReleaseComObject` で解放する設計（解放漏れで Excel プロセスが残る）。

4. **`AgentlabUtilityLibrary`（DBConn / Env / Dict / Enc 等）**
   - `DBConn`・`Env`・`Dict`・`Enc` 等を提供する外部ライブラリ。
   - **採用方針**: 別アプリケーションで**ビルド済みの `AgentlabUtilityLibrary.dll` をそのまま参照する**（リポジトリ直下の DLL）。
     `util_project/` 配下のソースは参照用であり、ビルドの正本ではない。
   - 現状の csproj はこの方針と矛盾する記述を含む（§6-1 参照）。

### 1.3 設定ファイル

1. **`AgentlabUtilityLibrary.ini`（必須・最重要）**
   - 探索順: ①実行ディレクトリ直下 → ②`c:\macs\utility\AgentlabUtilityLibrary.ini`（`Env.init()`）。
   - どちらにも無い場合は、`Env.init()` 内のハードコードされた既定値（`C:\macs` 等のテスト用値）にフォールバックする。
     本番 DB へ接続するには **exe と同じフォルダに正しい ini を配置する**こと。
   - 構造（リポジトリ同梱の例）:
     ```
     [HOME Config Start]
     LEGACY_HOME=C:\InnoKarte      ; Pat.csv の置き場所
     AGENT_HOME=C:\Shinseikai      ; Excel テンプレートの置き場所
     [HOME Config End]
     [DB Config Start]
     MAIN_DB / MAIN_USER / MAIN_PWD   ; 暗号化済み
     DB_LINK                          ; 暗号化済み
     OPEN_DB / OPEN_USER / OPEN_PWD   ; 暗号化済み（本アプリの接続に使用）
     [DB Config End]
     ```
   - DB セクションの値は **`Enc` クラスで暗号化**されており、読み込み時に `Enc.Decrypt()` で復号される（単純な文字置換方式、`Enc.cs`）。
   - 本アプリが実際に使うのは `OPEN_DB` / `OPEN_USER` / `OPEN_PWD`（`DBConn.GetOpenDBConn()`）と `DB_LINK`、`AGENT_HOME`、`LEGACY_HOME`。

2. **`macs.ini`（本アプリでは不要）**
   - `DBConn.Init()` が `C:\windows\macs.ini` を読むが、これを呼ぶのは `GetDBConn()`（本アプリ未使用）のみ。
     本アプリは `GetOpenDBConn()` しか使わず `Init()` を呼ばないため、**`macs.ini` は本アプリの前提条件ではない**。

### 1.4 データファイル

| ファイル | 場所 | 必須 | 用途 |
|----------|------|------|------|
| `Agree_眼科同意書.xls` | `AGENT_HOME\`（例: `C:\Shinseikai\Agree_眼科同意書.xls`） | 印刷時に必須 | Excel 同意書テンプレート。**実行時に読むのは `AGENT_HOME` 配下であり、リポジトリ直下のコピーではない** |
| `Pat.csv` | `LEGACY_HOME\`（例: `C:\InnoKarte\Pat.csv`） | 任意 | 電子カルテからの患者連携。無くても起動・動作する（存在すれば起動時に患者情報を自動取り込み） |

---

## 2. 起動シーケンス

```
Program.Main()
 ├─ カレントディレクトリを exe の場所に設定
 ├─ 多重起動チェック: 同名プロセスが既に存在する場合、確認のうえ既存プロセスを Kill
 └─ Application.Run(new Form1())
       │
   Form1() コンストラクタ
     ├─ InitializeComponent()  … 画面構築
     ├─ oraConn = DBConn.GetOpenDBConn()  … OleDb 接続オブジェクト生成
     ├─ Dict.DeptDict を参照して診療科コンボボックスを構築  ←★ ここで初めて DB アクセス
     │    └─ 例外発生時 → Program.OfflineMode = true にして警告表示（オフラインモードで継続起動）
     ├─ 入力欄リスト（agreeBoxList）を初期化
     └─ initShow()
          ├─ clearPlan()        … 入力欄クリア
          ├─ readPatCsv()       … LEGACY_HOME\Pat.csv があれば患者情報を読み込み、一覧表示
          └─ 印刷ボタンを無効化
```

### オフラインモード（起動可否の分岐の要）

- **トリガー**: `Form1` コンストラクタ内で `Dict.DeptDict`（＝マスタの DB ロード）にアクセスした際に**例外が発生**すると、
  `Program.OfflineMode = true` となり、「データベースに接続できません。オフラインモード（画面確認用）で起動します。」と表示して**起動は継続**する。
- **オフラインモードで無効化される機能**（いずれも先頭で `if (Program.OfflineMode) return;` 等で抑止）:
  - 同意書一覧の取得・表示（`showList`）
  - 同意書の登録・更新（`regPlan` → 「オフラインモードのため登録できません」）
  - 同意書の削除（`delPlan` → 「オフラインモードのため削除できません」）
  - 主治医氏名の解決・スタッフ定型文の取得（`staff1_id_Leave` / `getStaffRoom`）
  - テンプレート適用（`applyTemplate`）、検索画面の一覧（`FindAgree.showList`）
  - 選択行の内容表示（`showPlan`）
- つまりオフラインモードは「画面は出るが DB 連携機能は使えない」状態。**正しくフル機能で起動する条件＝DB 接続（§1.2/§1.3）がすべて成立していること**。

---

## 3. 機能の仕組み

### 3.1 患者の特定

- **手入力**: 患者番号を入力して Enter（`pt_id_KeyDown` → `showList`）。`M_PATIENT@dblink` から氏名・カナ・性別を取得。
- **カルテ連携**: 起動時 `readPatCsv()` が `LEGACY_HOME\Pat.csv` の 1 行目（CSV 50 列）を読み、患者番号・氏名・カナ・性別を画面へ反映。
  「新規作成」時の `readPatCsv2()` は主治医・診療科も取り込む（27 列目が `1` のとき）。

### 3.2 同意書一覧の表示（`showList`）

- `AGREE` テーブルを `M_DEPT` / `M_USR`（いずれも `@dblink`）と内部結合し、`PATIENT_ID = <患者番号>` かつ `DELETE_FLAG = 0` の行を作成日降順で取得。
- DataGridView に表示。多くの列は非表示にし、作成日・診療科・医師・眼・手術・完了のみ見せる。
- 行を選択（`agreePlanList_RowEnter` → `showPlan`）すると、その同意書の全項目が下部の入力欄に展開される。

### 3.3 登録・更新（`regPlan`）

- 必須チェック: 患者番号・主治医 ID・診療科。
- `Agree_id`（画面の隠し番号）が空なら **INSERT**（`AGREE_SEQ.nextval` で採番）、値があれば **UPDATE**。
- 保存項目: 作成日・診療科・主治医・スタッフ・眼・病名・麻酔・術式・説明・症状/計画/検査/手術内容（item1〜4）・シート名・医師完了フラグ（`DR_OK`）・保存時刻。
- 「作成完了」チェック（`staff1_ok` → `DR_OK`）が ON のとき戻り値 0（印刷を促す）、OFF のとき 1（登録のみ）。
- ※ SQL は文字列連結で生成しており、入力値のエスケープは行っていない（既存仕様。シングルクォート等で構文崩れの恐れ）。

### 3.4 削除（`delPlan`）

- 物理削除ではなく `UPDATE AGREE SET DELETE_FLAG = 1`（論理削除）。確認ダイアログあり。

### 3.5 テンプレート

- **同意書テンプレート**（`tmpPlanButton_Click` → `TmpAgree` 画面 → `applyTemplate`）:
  `AGREE_TEMPLATE` テーブルから 1 件読み、眼・病名・麻酔・術式・説明・item1〜4・シート名を、
  既存入力があれば追記、なければ設定する。
- **スタッフテンプレート**（`tmpStaffButton_Click` → `TmpStaff` 画面）／主治医に紐づく定型スタッフ文（`getStaffRoom`）:
  `AGREE_STAFF` テーブルの `CONT` を主治医 ID で引き、スタッフ欄に反映。

### 3.6 検索（`FindAgree`）

- 作成日（単一日／範囲指定）と医師完了状態（すべて／完成のみ／未完成のみ）で `AGREE` を横断検索。
- 「選択」で元画面（`Form1.showPlan(planId, ptId)`）に該当同意書を読み込む。

### 3.7 Excel 同意書の生成・印刷（`printAgree` → `ExcelControl.MakeEyeAgree`）

1. 画面の値をセル座標→文字列の辞書（`ValueList`）に詰める（患者情報・医師・術式・説明・各 item 等）。
2. `AGENT_HOME\Agree_眼科同意書.xls` を開き（シート「共通情報」）、辞書の値を各セルへ書き込む。
3. 現在日付・時刻、および連結したバーコード用文字列（患者番号・医師・日時等をゼロ埋め連結）をセルに設定。
4. `%TEMP%\<患者番号>_<日時>_眼科同意書` として保存し、`sheetName`（「日帰り」「入院」「短期滞在」「検査同意書」等）のシートを選択して可視化。
5. `ReleaseExcel()` で COM オブジェクトを解放。
- 印刷は Excel を可視化（`exApp.Visible = true`）してユーザーに渡す方式。WinForms 側の `PrintDocument` は実体の描画を持たない。

---

## 4. 使用するデータベースオブジェクト

| 種別 | オブジェクト | 用途 |
|------|--------------|------|
| テーブル | `AGREE` | 同意書本体（CRUD 対象） |
| シーケンス | `AGREE_SEQ` | `AGREE_ID` の採番 |
| テーブル | `AGREE_TEMPLATE` | 同意書テンプレート |
| テーブル | `AGREE_STAFF` | 主治医別スタッフ定型文 |
| マスタ（@dblink） | `M_PATIENT` | 患者 |
| マスタ（@dblink） | `M_DEPT` | 診療科 |
| マスタ（@dblink） | `M_USR` | 職員（ユーザー） |
| マスタ（@dblink） | `M_DR` / `M_SHINKU` / `M_SEKOU` / `M_SYOZOKU` / `M_SHIKAKU` | `Dict` 初期化で参照 |

`AGREE` 系は接続先 DB に直接、`M_*` マスタは `DB_LINK` 経由で参照される。

---

## 5. デプロイ時のチェックリスト

- [ ] x86（32bit）でビルド／実行している
- [ ] 32bit Oracle クライアント（`MSDAORA.1` OLE DB プロバイダ）導入済み・TNS 解決可
- [ ] 接続先 Oracle に `DB_LINK` のデータベースリンクが存在
- [ ] exe と同フォルダに正しい `AgentlabUtilityLibrary.ini`（暗号化済み DB 情報）を配置
- [ ] `AGENT_HOME\Agree_眼科同意書.xls` を配置（印刷に必須）
- [ ] Excel インストール済み
- [ ] （任意）`LEGACY_HOME\Pat.csv` を配置すればカルテ連携可

---

## 6. 未確認事項・要確認点

1. **ビルド構成: 採用方針は「ビルド済み DLL を参照」だが、現状 csproj がそれと矛盾している。**
   - **方針（確定）**: 別アプリでビルド済みの `AgentlabUtilityLibrary.dll` をそのまま参照する。
   - **対応済み**: `Agree.csproj` から `util_project/AgentlabUtilityLibrary/*.cs`・`util_project/WinAPI.cs`・
     `util_project/Properties/AssemblyInfo.cs` の `Compile` 項目を除外し、`Reference`（`AgentlabUtilityLibrary.dll`）
     のみでビルドする構成に変更した。これにより同名型のソース/DLL 二重定義（CS0436）とアセンブリ属性重複（CS0579）を解消。
   - **残存**: `util_project/*.resx` の `EmbeddedResource` と `None` 項目は残しているが、DLL 側フォームのリソースであり
     ビルドには影響しない（Agree.exe に未使用リソースとして埋め込まれるのみ）。気になる場合は別途除外可。
2. SQL は文字列連結で生成され、入力値のエスケープ／パラメータ化がされていない（既存仕様。記録のみ）。
3. `Env.init()` のフォールバック既定値（`C:\macs` / `WGS_ODBC_ORCL` 等）はテスト用と思われる。ini 不在時に
   この値で起動してしまう点は要注意（本番では必ず ini を配置）。
