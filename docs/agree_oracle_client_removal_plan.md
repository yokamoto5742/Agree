# EyeAgree Oracle クライアント非依存化 計画

作成日: 2026-09-12 / 最終更新: 2026-09-12

> **状況（2026-09-12）**: **Phase 1〜4, 6 のコード改修は完了**。開発機での検証（ビルド・テスト18件全合格・
> ローカルOracleへの実接続）まで到達した。残るのは本番環境での機能検証（Phase 7）と配布（Phase 8）。
> 詳細は **7. 実施記録** を参照。

電子カルテ端末に今後 Oracle クライアントを導入しない方針のため、本アプリ（EyeAgree / 眼科同意書）を
**Oracle クライアントのインストール無しで動作する構成**へ移行する。データベースは Oracle のまま使用する前提。

先行事例として [`eyedata_oracle_client_removal_plan.md`](eyedata_oracle_client_removal_plan.md)（EyeCenter / EyeData）があるが、
**本アプリの構造はそれとは決定的に異なる**。EyeData は ODP.NET 非管理版を使っており `using` の付け替えで済んだのに対し、
本アプリは **OLE DB 経由**であり、かつ **接続生成が外部DLL側にある**。移行先は同じ `Oracle.ManagedDataAccess` だが、作業内容は別物になる。

## 0. 前提（確認済み）

| 事項 | 値 |
|---|---|
| 接続先DB | EyeData と**同じ本番Oracle**（`11.2.0.1.0` / SJIS系）。19.x マネージドでの疎通実績あり |
| `OPEN_DB`（復号後） | **TNSエイリアス名** → `tnsnames.ora` の配置が必要 |
| 本番配置先 | `C:\Shinseikai\EyeAgree`（`C:\Shinseikai\EyeData` とは**別フォルダ**） |
| 外部DLLの扱い | **A案: DLLのDB機能を使わず、接続生成とマスタ読込をリポジトリ内に自前実装する** |

---

## 1. 現状の依存

Oracle クライアントへの依存は **2層** に分かれている。片方だけ直しても外れない。

| 層 | 依存の実体 |
|---|---|
| **アプリ本体** | `System.Data.OleDb`（`OleDbConnection` / `OleDbCommand` / `OleDbDataReader` / `OleDbDataAdapter` / `OleDbTransaction`） |
| **外部DLL** `AgentlabUtilityLibrary.dll` | `DBConn.GetOpenDBConn()` が `OleDbConnection` を生成し、`Dict` が内部でマスタ表を OleDb で読む |

OLE DB プロバイダは ini の `PROVIDER` キーで切り替わる（未指定なら本番既定 `MSDAORA.1`、ローカル開発は `OraOLEDB.Oracle`）。

**重要**: `MSDAORA.1`（Microsoft OLE DB Provider for Oracle）も `OraOLEDB.Oracle`（Oracle Provider for OLE DB）も
**ネイティブ COM コンポーネントであり、Oracle クライアントのインストールと `regsvr32` による HKLM 登録（管理者権限）が必須**である。
詳細は [`oracle_oledb_32bit_setup.md`](oracle_oledb_32bit_setup.md) に実戦記録がある。
つまり **OLE DB を使い続ける限り、どのプロバイダを選んでもクライアント非依存にはできない。**

### アプリ本体の OleDb 使用箇所（全 25 行）

| ファイル | 行 |
|---|---|
| `Agree/Form1.cs` | 5, 21, 23, 25 |
| `Agree/Form1.ImportExport.cs` | 5, 88, 89, 142, 193, 197, 236, 248, 253, 263, 281 |
| `Agree/Form1.Plan.cs` | 5, 29 |
| `Agree/TmpAgree.cs` | 5, 39, 41, 43 |
| `Agree/TmpStaff.cs` | 5, 14, 16, 36 |
| `Agree.Tests/OracleIntegrationTests.cs` | 2, 39, 42, 74, 153, 221, 223, 228, 230, 234, 236, 240, 242 |

**`OleDbParameter` の使用は 0 件**。SQL は全て文字列連結（自由記述列は `AgreeSql.SqlValue` でエスケープ済み）で組み立てられている。
したがって `?` → `:name` のバインド変数変換は**不要**で、移行は純粋な型の置き換えで済む。

### 外部DLLへの依存（アプリが実際に呼ぶ API のみ）

| API | DB接続 | 置換方針 |
|---|---|---|
| `DBConn.GetOpenDBConn()` — `Form1.cs:33`, `TmpAgree.cs:81`, `TmpStaff.cs:21` | **あり** | 自前の接続ファクトリへ |
| `Dict.DeptDict` — `Form1.cs:37,41,235,237` | **あり** | 自前のマスタ読込へ |
| `Dict.StaffDict` — `Form1.cs:297,299`, `Form1.Plan.cs:106,109`, `TmpStaff.cs:145,147` | **あり** | 自前のマスタ読込へ |
| `Env.OPEN_DB` / `OPEN_USER` / `OPEN_PWD` / `DB_LINK` / `AGENT_HOME` / `LEGACY_HOME` | なし（ini 読込のみ） | **そのまま使う** |
| `Barcode128` — `ExcelControl.cs:346,348` | なし（純粋な描画） | **そのまま使う** |

**`Env` は復号済みの平文を返すことを実機確認済み**（`Env.OPEN_DB` → `localhost:1521/FREEPDB1` が返る）。
よって接続文字列はアプリ側で組み立てられ、DLL の復号ロジックを再実装する必要はない。

---

## 2. 移行先の選定

`Oracle.ManagedDataAccess`（100% マネージド ADO.NET プロバイダ）**一択**。

**管理版の OLE DB プロバイダは存在しない**ため、`System.Data.OleDb` を維持したままクライアントを外す方法は無い。
`OracleConnection` / `OracleCommand` ベースの ADO.NET へ移すのが唯一の解。

### 却下した選択肢

| 案 | 却下理由 |
|---|---|
| Instant Client Basic Lite を xcopy 配置して `OraOLEDB.Oracle` を継続 | `regsvr32` による **HKLM 登録（管理者権限）が必須**。「インストールしない」という要件を実質満たさない |
| `MSDAORA.1` を継続 | 同上に加え、Microsoft が非推奨化済み・32bit 限定 |
| ODP.NET 非管理版（`Oracle.DataAccess`） | ネイティブDLL依存でクライアント必須。移行の意味がない |

### バージョン

**`4.122.19.1`（19c）を採用する。** 根拠:

- EyeData が同一DB（`11.2.0.1.0`）に対しこのバージョンで**本番疎通済み**。同DB・同ネットワークでの実績が最強の根拠
- 本アプリは `C:\Shinseikai\EyeAgree` に単独で配置されるため、EyeData とのバージョン統一は**技術的には不要**だが、
  院内で扱う DLL を 1 種類に保つほうが運用事故が少ない
- .NET Framework 4.8 は 19.x のサポート範囲内（4.6.2 以上）

退避先は EyeData と同じく `12.1.0.2`（`4.121.2.0`、11.2.0.1 を正式サポートする最後のマネージド版）。DLL 差し替えと `HintPath` の修正のみで戻せる。

---

## 3. 作業手順

### Phase 0 — 事前確認

EyeData の調査で大半は判明済み。本アプリ固有で確認が必要なのは 2 点のみ。

1. **本番 ini の `OPEN_DB`（TNSエイリアス名）の実値**と、それに対応する `tnsnames.ora` のエントリ内容
   （ホスト・ポート・サービス名）。Oracle クライアント撤去前に控える
2. `sqlnet.ora` にネイティブ暗号化（ANO）や `SQLNET.AUTHENTICATION_SERVICES` の指定が無いこと

EyeData で確認済みのため**再確認不要**: サーバーバージョン / `password_versions` に `11G` / 端末→DB の TCP 1521 直結可否。

### Phase 1 — DLL の配置とプロジェクト参照

`Oracle.ManagedDataAccess.dll`（4.122.19.1, MSIL）を**リポジトリ直下**に置く。
`AgentlabUtilityLibrary.dll` / `Microsoft.Office.Interop.Excel.dll` と同じ流儀で、NuGet 復元は本体ビルドに持ち込まない。

`Agree.csproj`:
```xml
<Reference Include="Oracle.ManagedDataAccess">
  <HintPath>Oracle.ManagedDataAccess.dll</HintPath>
  <Private>true</Private>
</Reference>
```

`Agree.Tests.csproj` も **同一ファイル**を `HintPath` 参照する（`PackageReference` にしない）。
両プロジェクトが同じ物理ファイルを見るため、バージョン不一致は原理的に起こらず `bindingRedirect` は不要。

> `Oracle.ManagedDataAccessDTC.dll` は分散トランザクション（`TransactionScope`）用。本アプリは未使用のため配置不要。

### Phase 2 — 接続生成の自前化（新規 `Agree/OracleDb.cs`）

`DBConn.GetOpenDBConn()` を置き換える。`Env` の復号済み値をそのまま使う。

```csharp
using Oracle.ManagedDataAccess.Client;
using AgentlabUtilityLibrary;

internal static class OracleDb
{
    /// <summary>
    /// OPEN_DB（ini）への未接続の OracleConnection を返す。
    /// 旧 AgentlabUtilityLibrary.DBConn.GetOpenDBConn() の置き換え。
    /// </summary>
    public static OracleConnection CreateOpenConnection()
    {
        var builder = new OracleConnectionStringBuilder
        {
            UserID     = Env.OPEN_USER,
            Password   = Env.OPEN_PWD,
            DataSource = Env.OPEN_DB,   // TNSエイリアス名
        };
        return new OracleConnection(builder.ConnectionString);
    }
}
```

置換: `Form1.cs:33` / `TmpAgree.cs:81` / `TmpStaff.cs:21`

> **`Provider=` は OracleConnection の接続文字列では不正なキーワード**。必ず落とすこと。
> ini の `PROVIDER` キーは以降どこからも参照されなくなる。

### Phase 3 — マスタ辞書の自前化（新規 `Agree/MasterDict.cs`）

アプリが `Dict` から使うのは **`DeptDict[code].ShortName` と `StaffDict[code].Name` の 2 つだけ**。
参照する表（`M_DEPT` / `M_USR`）は `TmpStaff.cs:35` や `Form1.Plan.cs:29` で既にアプリ自身が直接引いているものと同一。

```csharp
internal static class MasterDict
{
    private static Dictionary<string, string> _dept;   // CODE -> S_NAME
    private static Dictionary<string, string> _staff;  // CODE -> NAME

    public static Dictionary<string, string> Dept
        => _dept ?? (_dept = Load("select CODE, S_NAME from M_DEPT" + Env.DB_LINK));

    public static Dictionary<string, string> Staff
        => _staff ?? (_staff = Load("select CODE, NAME from M_USR" + Env.DB_LINK));

    private static Dictionary<string, string> Load(string sql) { /* 接続→Read→Trim して詰める */ }
}
```

置換箇所（7 行）:

| ファイル | 行 | 旧 | 新 |
|---|---|---|---|
| `Form1.cs` | 37, 41 | `Dict.DeptDict[key].ShortName` | `MasterDict.Dept[key]` |
| `Form1.cs` | 235, 237 | `Dict.DeptDict` | `MasterDict.Dept` |
| `Form1.cs` | 297, 299 | `Dict.StaffDict[...].Name` | `MasterDict.Staff[...]` |
| `Form1.Plan.cs` | 106, 109 | 同上 | 同上 |
| `TmpStaff.cs` | 145, 147 | 同上 | 同上 |

**⚠️ オフラインモードの挙動を壊さないこと。**
現状 `Form1.cs:35-49` は「`Dict.DeptDict` の初回アクセスで例外 → `Program.OfflineMode = true`」という設計になっている。
自前実装でも**接続失敗を握り潰さず、同じ位置で例外を投げる**必要がある。`Load()` 内で `try/catch` して空辞書を返す実装にしてはいけない。

**副作用（意図した改善）**: 現 `Dict` は初回アクセス時に 7 種のマスタを一括で読むが、自前化により 2 本の SELECT だけになる。
起動が速くなるが、挙動としては等価。

### Phase 4 — OleDb → Oracle の型置換

| 旧 | 新 |
|---|---|
| `using System.Data.OleDb;` | `using Oracle.ManagedDataAccess.Client;` |
| `OleDbConnection` | `OracleConnection` |
| `OleDbCommand` | `OracleCommand` |
| `OleDbDataReader` | `OracleDataReader` |
| `OleDbDataAdapter` | `OracleDataAdapter` |
| `OleDbTransaction` | `OracleTransaction` |

**落とし穴: `OracleCommand` には `(sql, connection, transaction)` の 3 引数コンストラクタが存在しない。**
該当箇所は代入に分解する。

```diff
-using (OleDbCommand cmd = new OleDbCommand(sql, oraConn, tx))
+using (OracleCommand cmd = new OracleCommand(sql, oraConn))
 {
+    cmd.Transaction = tx;
```

対象: `Form1.ImportExport.cs:197, 236` / `OracleIntegrationTests.cs:230, 236, 242`

**各ファイルの既存エンコーディングを必ず維持する**（`Form1.cs` / `Form1.ImportExport.cs` / `Form1.Plan.cs` / `TmpAgree.cs` / `TmpStaff.cs` は **UTF-8 BOM 付き**、`AgreeSql.cs` / `ExcelControl.cs` / `Logger.cs` / テスト 2 ファイルは BOM 無し）。

### Phase 5 — `tnsnames.ora` の配置

`OPEN_DB` は TNS エイリアス名のため、**`C:\Shinseikai\EyeAgree\tnsnames.ora` の配置が必須**。
`Program.cs:27` が `Directory.SetCurrentDirectory(Application.StartupPath)` を実行しており、
ODP.NET マネージドは exe と同じフォルダの `tnsnames.ora` を解決できる（EyeData で実測確認済み）。

- **Oracle クライアントを撤去する前に** `%ORACLE_HOME%\network\admin\tnsnames.ora` を
  `C:\Shinseikai\EyeAgree` へコピーしておくこと。撤去後は入手できなくなる
- 接続文字列も `Setting` も変更不要でエイリアス名のまま運用を継続できる

> 代替案（採らない）: `OPEN_DB` を完全記述子 `(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=…)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=…)))`
> に書き換えれば `tnsnames.ora` は不要になるが、暗号化 ini の再生成（`docs/enc_gen.py`）と全端末への再配布が必要になり、
> 変更点が増える。ファイル 1 個の配置で済むなら上の方が安全。

### Phase 6 — テストの移行

`Agree.Tests/OracleIntegrationTests.cs` を Oracle 化する。

- 既定接続文字列を `User Id=TEST_USER;Password=TEST_PWD;Data Source=localhost:1521/FREEPDB1` に変更
  （EZ-Connect なのでローカルは `tnsnames.ora` 不要）
- 環境変数 `AGREE_TEST_ORACLE` による上書きはそのまま維持
- `EnsureDatabaseAvailable()` の「`OraOLEDB` プロバイダ未登録」判定（59-67 行）は意味を失うため整理する
- **x86 縛りは技術的に不要になる**（マネージド DLL は MSIL）が、本体が Excel COM の都合で x86 のままなので
  `x86.runsettings` は据え置きを推奨（変更点を増やさない）
- `Agree.Tests.csproj` 先頭のコメント（OraOLEDB / 32bit の説明）を現状に合わせて更新

**これにより、ローカル開発環境からも 32bit ODAC の登録作業（[`oracle_oledb_32bit_setup.md`](oracle_oledb_32bit_setup.md) の問題A）が丸ごと不要になる。**

### Phase 7 — 検証

コード改修よりこちらが実質の工数になる。

1. **オフラインモード** — DB 停止状態で起動し、警告ダイアログが出て画面確認モードに落ちること（Phase 3 の最重要確認）
2. 起動 → 診療科コンボが埋まる（`MasterDict.Dept`）
3. 患者ID入力 → Enter → 患者名表示（`M_PATIENT`）＋同意書一覧（`OracleDataAdapter` / `DB_LINK` 越しの join）
4. 同意書の登録 / コピー作成（`ExecuteScalar` の `max(AGREE_ID)`）/ 削除
5. テンプレート適用（`AGREE_TEMPLATE` / `applyTemplate`）
6. 担当者マスタ画面（`TmpStaff` — `Trim(NAME)` の式列を含む）
7. **CSV エクスポート／インポート**（トランザクション + `ResyncSequence` のシーケンス操作）
8. Excel 出力とバーコード生成
9. **日本語のラウンドトリップ**（リスク1）— 既存データの読み取り結果比較と、書き込み→読み戻しの一致確認
10. `dotnet test Agree.Tests\Agree.Tests.csproj -c Debug`

> **7 は移行前後の差分比較として使うのが確実。** 移行前に全テーブルを CSV エクスポートしておき、
> 移行後に同じ操作を行って **ファイル同士を diff する**。これでリスク2（型マッピング差）と
> リスク1（文字コード）を全列まとめて機械的に検出できる。

### Phase 8 — 配布

- `C:\Shinseikai\EyeAgree` へ `EyeAgree.exe` と `Oracle.ManagedDataAccess.dll`、`tnsnames.ora` を配置
- 端末の Oracle クライアント撤去は **`tnsnames.ora` のコピー後**に行う
- ini の `PROVIDER` 行は参照されなくなるため削除してよい（残しても害は無い）
- 本アプリは EyeData と別フォルダのため、**EyeData 側の入れ替えとは独立に実施できる**

---

## 4. リスクと対策

1. **文字コード（最重要）**
   本番DBは SJIS 系。マネージド・ドライバは `NLS_LANG` に非依存で自前に変換するため、
   これまで OLE DB プロバイダが暗黙に変換していた文字（半角カナ・㈱等の機種依存文字・波ダッシュ）で挙動が変わる可能性がある。
   → Phase 7-9 のラウンドトリップ試験と、Phase 7 の CSV 差分比較を必ず実施する。
   なお CSV 入出力は `Encoding.Default`（＝CP932）のままで変更しない。

2. **`reader[i].ToString()` の型マッピング差**
   本アプリは値を全て `reader[i].ToString()` / `Cells[i].Value.ToString()` で受けており、
   **プロバイダが返す .NET 型がそのまま文字列表現に出る**。特に注意:
   - `SAVE_DATE`（`NUMBER(8)`）— `Form1.Plan.cs:118` が `.ToString().Length == 8` で日付判定している
   - `ExecuteScalar` の `nvl(max(ID),0)` / `seq.nextval` — `Convert.ToInt64` で受けているため型差には強い
   - CSV エクスポートの全列
   → Phase 7 の CSV 差分比較で検出する。

3. **式列の列名の差**
   `Form1.Plan.cs:29` は `Trim(M_DEPT.S_NAME)` や `''` リテラル（2 個）を含み、`TmpStaff.cs:35` も `Trim(NAME)` を含む。
   プロバイダによって自動生成される列名が変わる。
   **本アプリは全て列インデックスでアクセスしている**ため低リスクだが、`DataGridView` の列数・並びが変わっていないか目視確認する。

4. **外部DLL が残ることによる暗黙のクライアント依存**
   A案では `AgentlabUtilityLibrary.dll` の参照自体は残る（`Env` と `Barcode128` のため）。
   同DLLは `System.Data` を参照しているが、**`DBConn` / `Dict` / `DB` 型に一切触れなければそれらは JIT ロードされず、
   OLE DB の COM 生成も起きない**ため、クライアント非搭載端末でも動作するはず。
   → **クライアント未導入の端末（または ODAC を無効化した環境）での起動確認を必ず行う**。

5. **x86 縛り**
   32bit 固定の理由だった「ネイティブ OLE DB プロバイダが 32bit」という制約は外れるが、
   `Interop.Excel` の都合があるため **x86 のまま据え置き**（変更点を増やさない）。

6. **ネットワーク要件**
   端末からDBへ TCP 1521 の直結が引き続き必要。EyeData で疎通実績があるため問題にならない見込み。

7. **`ORA-28040: 一致する認証プロトコルがありません`**
   12c 以降のクライアントは 11G 以上のパスワード・ベリファイアを要求する。
   EyeData で同DB・同系ユーザーへの接続が成功しているため顕在化しない見込みだが、
   接続ユーザーが異なる場合は `dba_users.password_versions` に `11G` があるか確認すること。

---

## 5. 未確認事項

| # | 事項 | 状態 |
|---|---|---|
| 1 | 本番 ini `OPEN_DB` のエイリアス名と `tnsnames.ora` の該当エントリ | **未確認**（Phase 0） |
| 2 | 本番端末の `sqlnet.ora` の特殊設定（ANO 等） | **未確認**（Phase 0） |
| 3 | クライアント非搭載端末で `Env` / `Barcode128` のみの DLL 利用が動作するか | **未確認**（リスク4。実機確認） |
| 4 | 本番DBの `NLS_CHARACTERSET` | **未確認**（リスク1のラウンドトリップ検証と併せて確認） |
| 5 | 既存端末の Oracle クライアントを残すのか撤去するのか | **運用判断待ち**。撤去する場合は先に `tnsnames.ora` を退避 |

---

## 6. 作業量の見積もり

| Phase | 内容 | 規模 |
|---|---|---|
| 1 | DLL 配置・csproj 2 件の参照追加 | 小 |
| 2 | `OracleDb.cs` 新規（約 20 行）＋ 3 箇所置換 | 小 |
| 3 | `MasterDict.cs` 新規（約 40 行）＋ 7 箇所置換。**オフラインモードの挙動維持が要注意** | 中 |
| 4 | 型置換 25 行 ＋ トランザクション付きコンストラクタ 5 箇所の分解 | 小 |
| 5 | `tnsnames.ora` の配置（コード変更なし） | 小 |
| 6 | テスト 1 ファイルの移行 | 小 |
| 7 | **検証（本作業の実質的な工数はここ）** | 大 |
| 8 | 配布 | 小 |

---

## 7. 実施記録（2026-09-12）

Phase 1〜4, 6 のコード改修を実施し、開発機でローカル Oracle への疎通と機能テストに成功した。

### 実施したこと

| Phase | 内容 |
|---|---|
| 1 | `Oracle.ManagedDataAccess.dll`（4.122.19.1, MSIL）をリポジトリ直下に配置。`Agree.csproj` / `Agree.Tests.csproj` が同一ファイルを `HintPath` 参照 |
| 2 | `Agree/OracleDb.cs` を新規作成し、`DBConn.GetOpenDBConn()` の3箇所を置換 |
| 3 | `Agree/MasterDict.cs` を新規作成し、`Dict.DeptDict` / `Dict.StaffDict` の7箇所を置換 |
| 4 | 5ファイルの OleDb 型を Oracle 型へ置換（25行）。`OracleCommand` の3引数コンストラクタ欠如に伴う分解を2箇所 |
| 6 | `OracleIntegrationTests.cs` をマネージド・ドライバへ移行。`Agree.Tests.csproj` の参照追加とコメント更新 |
| — | `CLAUDE.md` / `docs/CHANGELOG.md` を更新。`local_oracle_setup.md` / `oracle_oledb_32bit_setup.md` に旧構成である旨を追記 |

各ファイルの既存エンコーディング（UTF-8 BOM の有無）と改行コードは維持した。

### 検証結果（開発機）

- **ビルド**: `dotnet build Agree.slnx` → 成功（0 警告 / 0 エラー）
- **テスト**: `dotnet test` → **18 合格 / 0 失敗 / 0 スキップ**。
  結合テストがスキップされず全て実行された ＝ ローカル Oracle へマネージド・ドライバで実接続できている。
  日本語・アポストロフィのラウンドトリップ、トランザクション、シーケンスも合格
- **新規経路の実接続確認**: 結合テストは独自の接続文字列を使うため `OracleDb` / `MasterDict` を通らない。
  そこで同一コードを別途コンパイルして実行し、以下を確認した:
  - `Env.OPEN_USER` / `OPEN_PWD` / `OPEN_DB`（復号済み）→ `OracleConnectionStringBuilder` → 接続成功
  - `Select CODE, S_NAME from M_DEPT order by CODE` → `[1] => [眼科]`
  - `Select CODE, NAME from M_USR order by CODE` → `[101] => [テスト医師]`
  - コードが `1` / `101`（ゼロ埋めなし）で返り、`short.Parse(...).ToString()` との突き合わせと一致すること
- **生成された `EyeAgree.exe.config`**: bindingRedirect は生成されず（参照バージョンが1つのため）。想定どおり

### 判明したこと

- `OracleConnectionStringBuilder` の `UserID` プロパティは C# では正常に動作する。
  PowerShell から設定すると `IDictionary` インデクサ経由になり「`'USERID'`は無効な接続文字列属性です」になるが、
  これは PowerShell 側の事情であってコードの問題ではない（検証時の注意点）
- ini の `PROVIDER` キーはどこからも参照されなくなった

### 残作業

1. **Phase 0 の事前確認** — 本番 ini `OPEN_DB` のエイリアス名と `tnsnames.ora` の該当エントリ、`sqlnet.ora` の特殊設定
2. **リスク4の実機確認** — Oracle クライアント非搭載の端末で、`Env` / `Barcode128` のみの外部DLL利用が動作すること
3. **Phase 7 の機能検証** — 未実施。優先度が高い順に:
   1. オフラインモード（DB停止状態で起動できること）
   2. 日本語のラウンドトリップ（リスク1）— 本番DBは SJIS 系
   3. 移行前後の CSV エクスポート差分比較（リスク1・2をまとめて検出できる）
   4. 一覧・登録・コピー作成・削除・テンプレート適用・担当者マスタ
   5. CSV インポート（トランザクション + `ResyncSequence`）
   6. Excel 出力とバーコード
4. **Phase 8 の配布** — `C:\Shinseikai\EyeAgree` へ `Oracle.ManagedDataAccess.dll` と `tnsnames.ora` を配置
