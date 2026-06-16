# 同意書システム ユニットテスト戦略

眼科同意書管理アプリ（C# / .NET Framework 4.8 / WinForms / x86）のユニットテスト戦略。
本書は **戦略とテスト設計** に焦点を当てる。実テストコードは含まない。

---

## 0. スコープと前提（最初に確定すべき2つの軸）

### 0.1 対象範囲

| 区分 | 対象 |
|---|---|
| **テスト対象（本アプリ）** | `Program.cs` / `Form1.cs` / `ExcelControl.cs` / `FindAgree.cs` / `TmpAgree.cs` / `TmpStaff.cs` |
| **結合点（境界として扱う）** | `AgentlabUtilityLibrary` のうちアプリが直接使う `DBConn`・`Env`・`Dict` |
| **本アプリ視点でスコープ外** | `DataCheck.IsValidDate/IsValidId`、`Enc.Encrypt/Decrypt`、`StrConv`、`DateConvert` 等。grep で確認した通り Agree アプリから**呼び出されていない**。util ライブラリ自体のテストは別タスク（本アプリの優先度表では P3 扱い） |

> 前提として、本アプリは外部DLL `AgentlabUtilityLibrary.dll` と Excel（COM）が無いとビルド・実行できない。
> この前提はテスト基盤の構築方針（§4）に直接影響する。

### 0.2 「業務優先度」と「テスト容易性」は別軸として扱う

本戦略の最重要原則。両者を混同すると一覧が自己矛盾する。

- **業務優先度（P0〜P3）** … タスクが定義した基準（データ整合性・セキュリティ・同意書の正しさ・外部連携・エラーハンドリング）で決まる。
- **テスト容易性** … 現状コードでそのまま検証できるか。本アプリの P0 ロジックの大半は **そのままでは検証不能**（後述の seam 欠如のため）。

→ テスト容易性が低いことを理由に優先度を下げない。
例：`regPlan()` の SQL 構築は P0 のまま据え置き、「現状テスト不能 → 純粋メソッドへ抽出が前提」と**注記**する。
避けるべき失敗は「テストしやすい＝重要」と取り違えること（§0.1 の未使用ユーティリティが典型的な罠）。

### 0.3 カバレッジ目標の成立条件（重要な但し書き）

タスク指定の目標値はそのまま採用する：

| 優先度 | ライン/ブランチ カバレッジ目標 |
|---|---|
| P0 | 100% |
| P1 | 90% 以上 |
| P2 | 80% 以上 |

**ただしこの数値は「抽出後に生成される検証可能なユニット」に対して成立する。**
現状の `regPlan()`（約130行・UI/DB/日時に直結）や `MakeEyeAgree()`（COM直結）に対しては、
ブランチカバレッジを取ること自体が不可能。目標達成は §4 の seam 導入を前提条件とする。

---

## 1. コード構造の分析結果

### 1.1 アーキテクチャ概要

レイヤ分離が無い。UI・業務ロジック・データアクセス・帳票生成がすべて Form クラス内に同居する。

```
Program.Main ──> Form1（メイン画面 / 業務ロジックの中心）
                   ├─ FindAgree（検索）          ── OleDb 直結
                   ├─ TmpAgree（同意書テンプレート）── OleDb 直結
                   ├─ TmpStaff（担当者）          ── OleDb 直結
                   └─ ExcelControl（帳票生成）     ── Excel COM 直結
静的依存: DBConn(OleDb接続) / Env(設定ini) / Dict(マスタ辞書, DB読込)
ファイル依存: Pat.csv（患者連携CSV）
```

### 1.2 依存関係と結合度

| 結合先 | 形態 | 影響（テスト観点） |
|---|---|---|
| WinForms コントロール | ロジックが `pt_id.Text` 等を直接 read/write | ロジックを UI から切り離せない＝純粋関数として呼べない |
| OleDb（Oracle） | 各 Form が `OleDbConnection` を直接生成・SQL文字列を直書き | DB が無いと実行不可。SQLビルドと実行が同一メソッド内 |
| Excel COM | `ExcelControl` が Interop を直接操作 | Excel インストール必須。検証は実 Excel 起動を伴う |
| `Pat.csv` | `StreamReader` でローカルCSVを固定インデックス読込 | ファイル存在・列数に依存 |
| 静的クラス `Env`/`Dict`/`DBConn` | グローバル状態（static フィールド + 遅延初期化） | テスト間で状態が共有され、モック差し替え不可 |

**結論：現状コードにはユニットテスト用の継ぎ目（seam）がほとんど無い。**
これが本アプリ最大の構造的課題であり、テスト戦略の出発点である。

### 1.3 複雑度ホットスポット（優先的に分解・検証すべき箇所）

| メソッド | 複雑度の要因 |
|---|---|
| `Form1.regPlan()` | 入力検証 + 新規/更新の分岐 + 約130行のSQL文字列連結 + 日時生成 + DB実行 |
| `Form1.showList()` | SQL構築 + DataGridView 20列の表示制御 + 分岐 |
| `Form1.printAgree()` | 13セルの値マッピング + CSV読込 + COM呼び出し |
| `ExcelControl.MakeEyeAgree()` | セル書込 + 患者番号合成（PadLeft連結）+ 日時 + 保存パス生成 + COM解放 |
| `Form1.applyTemplate()` / `TmpAgree.regPlanTemplate()` | 9項目の「空なら代入/非空なら連結」分岐 + SQL構築 |
| `TmpAgree.getTempIdFromNode()` / `initTree()` | 親子ツリー構築・名前/親IDによるID逆引き |
| `Dict.InitDict()` | 7テーブルを順次読込しグローバル辞書へ展開（巨大な副作用の塊） |

### 1.4 エラーハンドリングの実装状況

- 例外処理は薄く、握りつぶし（bare `catch {}`）が散見される。
  - `Program.Main`：プロセス Kill を `catch {}` で無視。
  - `Form1` コンストラクタ：DB接続失敗を捕捉し `OfflineMode` へ移行（数少ない明示的なフォールバック）。
- 未検証の `int.Parse` / `short.Parse` / 固定インデックス参照が多く、入力次第で未捕捉例外になる。
- `OfflineMode` による早期 return が各メソッドに点在し、**分岐網羅の主要対象**になる。

### 1.5 テスト中に表面化させるべき既知リスク（※指摘のみ。本戦略では修正しない）

プロジェクト規約（外科的変更・未依頼コードは触らない）に従い、以下は「テストで露見させる対象」として記録する。

1. **SQLインジェクション / データ破損（P0・セキュリティ）**
   `regPlan` / `delPlan` / `applyTemplate` / `TmpAgree.regPlanTemplate` / `TmpStaff.saveButton_Click` / `FindAgree.showList` は、テキスト入力を文字列連結で SQL に埋め込む。
   `説明` や氏名にアポストロフィ（`'`）が含まれると SQL 破損、悪意ある入力で注入が成立しうる。患者の同意書という性質上、整合性・機密性の両面で最重要。
2. **`printAgree()` の性別判定バグ（P0・同意書の正しさ）**
   `if (pt_sex.Equals("2"))` は `TextBox` オブジェクトと文字列 `"2"` を比較しており**常に false** → 出力は常に「男」。同意書帳票に誤った性別が印字される。
3. **CSV 解析の境界欠陥（P0・データ整合性）**
   `readPatCsv` 系は `for (i=0; i<50; i++) patCont[i] = line.Split(',')[i]` と固定50列を前提。列数不足の CSV で `IndexOutOfRangeException`。`int.Parse(patCont[2])` 等は非数値で `FormatException`。
4. **COM 解放漏れの可能性（P1・リソース）**
   `ExcelControl.Open()` が例外時、`exWorkbook`/`exApp` が未解放のまま残り Excel プロセスが残留しうる。

---

## 2. 優先度別テスト対象一覧

列の意味：
- **優先度** … 業務優先度（§0.2）。
- **テスト容易性** … `直接可`＝現状そのまま検証可 / `要抽出`＝純粋メソッド/インタフェース抽出が前提 / `要seam`＝DB/COM/ファイルの差し替え口が前提。

### 2.1 最優先（P0）— 必須テスト

| クラス.メソッド | 理由（業務観点） | テスト容易性 | 前処理 |
|---|---|---|---|
| `Form1.regPlan()` の SQL構築部 | 同意書の登録/更新。データ整合性 + SQLインジェクション | 要抽出 | SQL生成を入力DTO→文字列の純粋メソッドに分離 |
| `Form1.delPlan()` の SQL構築部 | 論理削除（`DELETE_FLAG=1`）。誤削除・未削除が患者影響 | 要抽出 | 同上 |
| `Form1.applyTemplate()` の項目マージ分岐 | 「空なら代入/非空なら連結」の9項目ロジック。記載内容の正しさ | 要seam | DataReader をインタフェース化 |
| `TmpAgree.regPlanTemplate()` / `delPlanTemplate()` のSQL構築 | テンプレ登録/削除。注入・整合性 | 要抽出 | SQL生成の分離 |
| `TmpStaff.saveButton_Click()` のSQL構築 | 担当者 INSERT/UPDATE 分岐 + 入力検証 | 要抽出 | 検証ロジックとSQL生成の分離 |
| `FindAgree.showList()` の検索SQL構築 | 日付範囲 + 完了状態フィルタの組合せ | 要抽出 | 条件→WHERE句生成の分離 |
| `ExcelControl.MakeEyeAgree()` の値合成部 | 患者番号合成（`PadLeft(9/3/5)` + 連結）。同意書の識別子の正しさ | 要抽出 | 文字列合成を純粋関数化（COMと分離） |
| `ExcelControl.setValue()` のキー解析 | `"row,col"` → セル座標の解析。書込先の正しさ | 要seam | Worksheet をインタフェース化 |
| `Form1.printAgree()` の値マッピング | 帳票への13項目割付（性別判定バグを含む） | 要抽出 | マッピングを Dictionary 生成関数に分離 |
| `Form1.readPatCsv()` / `readPatCsv2()` / `printAgree` のCSV解析 | 患者連携データの取込。境界/例外（§1.5-3） | 要抽出 | CSV1行→患者DTO のパーサに分離 |
| `Program.Main()` の多重起動制御 | 既存プロセスの終了制御（外部連携/副作用） | 要seam | プロセス列挙/Kill をインタフェース化 |
| 各メソッドの `OfflineMode` ガード | オフライン時に登録/削除/DB読込を抑止する安全機構 | 要抽出/要seam | ガード判定を検証可能な形に |

### 2.2 高優先度（P1）— 推奨テスト

| クラス.メソッド | 理由 | テスト容易性 |
|---|---|---|
| `Form1.showPlan(int)` / `showPlan(string,string)` | グリッド行→入力欄への反映、日付整形（8桁→`yyyy/MM/dd`）、医師ID辞書照合 | 要seam |
| `Form1.dept_Leave()` | 診療科の形式・範囲(1〜20)バリデーション | 要抽出 |
| `Form1.staff1_id_Leave()` / `getStaffRoom()` | 医師ID存在チェック、担当者上書き確認分岐 | 要seam |
| `TmpAgree.initTree()` / `getTempIdFromNode()` | 親子ツリー構築とID逆引き（名前+親IDの一致探索） | 要seam |
| `Form1` コンストラクタ | DB接続失敗 → `OfflineMode` フォールバック、診療科コンボ初期化 | 要seam |
| `Form1.clearPlan()` / `initShow()` | 画面初期化の網羅（全欄クリア） | 直接可（Form実体化が必要） |
| `ExcelControl.Open()` | ファイル非存在時のメッセージ返却、例外時の戻り値 | 要seam |

### 2.3 中優先度（P2）— 可能であれば実施

| 対象 | 理由 | テスト容易性 |
|---|---|---|
| `TmpStaff.showStaff()` / `clearStaff()` | グリッド→入力欄反映、クリア範囲のフラグ分岐 | 要seam |
| `FindAgree.fromToCheck_CheckedChanged()` / `dateFrom_ValueChanged()` | 日付範囲指定の有効化/連動 | 要seam |
| `Dict.Staff.LastName` / `FirstName` 等の派生プロパティ | 純粋な文字列分割ロジック（※本アプリからはほぼ未使用、util側テスト寄り） | 直接可 |

### 2.4 低優先度（P3）— テスト不要またはオプション

§3 の「テスト不要部分」を参照。

---

## 3. テストケース設計方針

各 P0/P1 対象に対し **正常系・異常系・境界値** を設計する。
以下は代表例を「入力 → 期待結果」で示す（実コードは書かない）。

### 3.1 SQL構築（`regPlan` 抽出後の純粋メソッド想定）

| 種別 | 入力 | 期待結果 |
|---|---|---|
| 正常（新規） | `Agree_id` 空、患者/医師/診療科すべて有効 | `INSERT INTO AGREE ...` を生成、`SAVE_DATE` が `yyyyMMdd` |
| 正常（更新） | `Agree_id` 値あり | `UPDATE AGREE SET ... WHERE AGREE_ID=...` を生成 |
| 異常（注入/破損） | `説明` に `O'Brien` 等アポストロフィ | 文字列がエスケープ/パラメータ化され SQL が破綻しない（現状実装は破綻＝失敗を露見させる） |
| 異常（未入力） | 患者番号空 / 医師空 / 診療科空 | 各々の検証で `-1` 返却（登録に進まない） |
| 境界 | `staff1_ok` チェック有/無 | `DR_OK` が `1`/`0`、戻り値が `0`/`1` |

### 3.2 CSV 解析（`readPatCsv` 抽出後のパーサ想定）

| 種別 | 入力 | 期待結果 |
|---|---|---|
| 正常 | 50列以上・`patCont[6]="1"` | 患者DTO生成、性別「男」 |
| 正常 | `patCont[6]="2"` | 性別「女」 |
| 境界 | ちょうど50列 | 例外なく解析 |
| 異常 | 49列以下 | 列数不足を検出（現状は `IndexOutOfRange`＝失敗を露見） |
| 異常 | `patCont[2]` が非数値 | 数値変換失敗を検出（現状は `FormatException`） |
| 異常 | ファイル非存在 | 早期 return（画面はクリア状態のまま） |

### 3.3 患者番号合成（`MakeEyeAgree` 抽出後の純粋関数想定）

| 種別 | 入力 | 期待結果 |
|---|---|---|
| 正常 | ID=`123`, dept=`4`, ... | `PadLeft` 後の連結が桁数仕様（9/3/5桁）通り |
| 境界 | 規定桁ちょうど / 0埋め不要 | 切り詰めず保持 |
| 異常 | 規定桁超過の値 | 仕様（切詰め有無）に沿って一貫した結果 |

### 3.4 性別マッピング（`printAgree` 抽出後）

| 種別 | 入力 | 期待結果 |
|---|---|---|
| 正常 | 性別コード `"2"` | 「女」 |
| 正常 | 性別コード `"1"` | 「男」 |
| 回帰 | 現状の `pt_sex.Equals("2")` | **常に「男」になるバグを検出**（テストが失敗することで露見） |

### 3.5 検索条件 WHERE 構築（`FindAgree.showList` 抽出後）

| 種別 | 入力 | 期待結果 |
|---|---|---|
| 正常 | 範囲指定オフ | `SAVE_DATE >= from AND <= from`（from=to） |
| 正常 | 範囲指定オン | from〜to の範囲 |
| 境界 | `dr_ok2`（完成のみ） | `AND DR_OK = 1` 付与 |
| 境界 | `dr_ok3`（未完成のみ） | `AND DR_OK = 0` 付与 |
| 境界 | `dr_ok1`（すべて） | DR_OK 条件なし |

### 3.6 OfflineMode ガード（各 P0 メソッド共通）

| 種別 | 入力 | 期待結果 |
|---|---|---|
| 異常系（オフライン） | `Program.OfflineMode = true` | `regPlan`/`delPlan`/`getStaffRoom`/`applyTemplate`/`showList` 等が DB アクセスせず早期 return（登録/削除されない） |
| 正常系（オンライン） | `false` | 通常経路を実行 |

---

## 4. テスト基盤・前処理（戦略の前提）

カバレッジ目標（§0.3）を成立させるための最小限の足場。**「テストのための分離」に限定**し、全面リファクタは行わない。

1. **テストフレームワーク**：xUnit もしくは NUnit（.NET Framework 4.8 / x86 ターゲットでテストプロジェクトを構成）。
2. **純粋ロジックの抽出**：SQL構築・値合成・CSV解析・値マッピングを、UI/DB/COM に依存しない `static` または純粋メソッドへ切り出す（入力は DTO、出力は文字列/DTO）。→ P0 の大半がこれで `直接可` になる。
3. **境界の seam 化**：
   - DB：`OleDbDataReader` 相当をインタフェース化、または Repository を 1 枚挟む（SQL文字列の検証はインタフェースなしでも可能）。
   - Excel：`ExcelControl` のセル書込先を最小インタフェースで抽象化（合成ロジックと COM を分離）。
   - ファイル/プロセス：CSV 読込・プロセス列挙/Kill を差し替え可能にする。
4. **静的状態（`Env`/`Dict`/`DBConn`/`Program.OfflineMode`）**：テスト前後で初期化・復元できる仕組みを用意（グローバル状態の漏れ防止）。
5. **段階導入**：まず §3 の純粋ロジックを抽出して P0 を確保 → 次に seam で P1 を確保、という順で進める。

> これらは「テスト可能化のための分離」に範囲を限定する。既存スタイルの改変や未依頼の機能変更は行わない。

---

## 5. テストが不要な部分（理由付き）

| 対象 | 理由 |
|---|---|
| `InitializeComponent()`（全 Form）/ `*.Designer` 相当 | デザイナ自動生成のUIレイアウト定義。ロジックを含まず、変更はデザイナ経由。ユニットテスト対象外 |
| `ExcelControl` の単純 setter（`FileName` / `SheetName` / `ValueList`） | フィールド代入のみ。副作用・分岐なし |
| `ExcelControl.ReleaseExcel()` / `RunVBA()` / `Save()` | COM への薄いラッパ（`Marshal.ReleaseComObject` / `Run` の委譲）。ロジックが無く、検証は実 Excel が必要。手動/結合テスト側で扱う |
| `printDocument1_PrintPage()` | 中身が空のイベントハンドラ |
| `closeButton_Click()` / `TmpStaff_Load()` 等の薄いハンドラ | `Dispose()` 呼び出しのみ等、分岐なし |
| ラベル文字列・定数・コンボ初期項目（`"日帰り"` 等） | 静的データ。振る舞いを持たない |
| `Dict` のネストクラスの自動実装 getter/setter・コンストラクタ代入 | 単純な値保持。`LastName`/`FirstName` 等の派生プロパティのみ P2 候補（§2.3） |
| util_project の未使用関数（`DataCheck` / `Enc` / `StrConv` / `DateConvert`） | 本アプリから呼び出されていない（grep 確認済み）。本アプリ視点では対象外。util ライブラリ単体テストとして別管理 |

---

## まとめ

- 本アプリの業務ロジックは Form/COM/DB/CSV に密結合しており、**現状のままでは P0 ロジックを単体検証できない**。
- したがって戦略の核心は「優先度（P0〜P3）はそのまま維持しつつ、検証可能化のために最小限の抽出・seam 化を前処理として行う」こと。
- 指定カバレッジ目標は、その前処理で生まれる**純粋ユニットに対して**達成を目指す。
- 既知の不具合（SQL注入・性別判定・CSV境界）は修正せず、**テストで露見させる対象**として設計に組み込む。
