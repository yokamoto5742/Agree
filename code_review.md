# コードレビュー: 眼科同意書システム (EyeAgree)

- 対象: `master` @ `5e8ce9b`（v1.0.3.0）の本体ソース一式（`Agree/*.cs`、Designer 生成コードを除く）
- 観点: **可読性・メンテナンス性**、KISS 原則によるシンプル化
- 行番号は各ファイルの現時点の行

---

## 総評

`Logger` / `AgreeSql` / `ExcelControl` の切り出しと COM 解放の扱いは丁寧で、コメントも「なぜ」を説明できています。
一方でフォーム側（`Form1` / `TmpAgree` / `TmpStaff`）には逆コンパイル由来と思われるコードが多く残っています
（`text2`・`num`・`TextBox textBox = diag;` などの機械的な名前、同じ処理のコピー、数値インデックス参照）。
読みにくさの大半は次の **3つのパターン** から来ています。

1. **DB 接続の Open/Close を手書きで繰り返している**（19箇所。`finally` が無い箇所もある）
2. **同じ処理を列ごとにコピーしている**（`applyTemplate`、`regPlan`、`showPlan`、ボタン状態の切替）
3. **意味をインデックスや文字列に埋め込んでいる**（`Cells[16]`、`"8, 2"`、戻り値 `0/1/-1`）

どれも小さなヘルパーを1つ用意すれば解消でき、合計で数百行減らせる見込みです。
なお、レビュー中に**実害のある不具合を4件**見つけたので、先に挙げます。

### 優先度サマリ

| # | 優先度 | 内容 | 場所 |
|---|---|---|---|
| 1-1 | 🔴 高 | 同意書の性別が常に「男」で出力される | `Form1.Plan.cs:316` |
| 1-2 | 🔴 高 | DB 例外が起きると接続が開いたまま残り、以後の DB 操作がすべて失敗する | `Form1.Plan.cs` ほか |
| 1-3 | 🟠 中 | 「保存しますか？」で「はい」を選んで保存に失敗しても入力内容が消える | `Form1.cs:338-360` |
| 1-4 | 🟠 中 | 同じ分類に同名のテンプレートがあると、別のテンプレートが適用される | `TmpAgree.cs:576` |
| 2-1 | 🟠 中 | DB アクセスのヘルパー化 | 全フォーム |
| 2-2 | 🟠 中 | INI 読込の3重実装を1つにする | `Form1.cs:62,103` / `ExcelControl.cs:211` |
| 2-3〜2-8 | 🟡 低〜中 | コピーされた処理の集約 | 各所 |
| 3-x | 🟡 低 | 命名・マジックナンバー | 各所 |
| 4-x | 🟢 低 | 使われていないコード・ファイルの削除 | 各所 |
| 5-x | 🟢 低 | ドキュメントとビルド設定の食い違い | `docs/`、`Agree.csproj` |

---

## 1. 先に直すべき不具合

### 1-1. 🔴 性別が常に「男」になる — `Form1.Plan.cs:316`

```csharp
if (pt_sex.Equals("2"))   // pt_sex は TextBox。TextBox と文字列の比較なので常に false
```

`pt_sex` は `TextBox`（`Form1.Designer.cs:106`）なので、この条件は必ず false になり、B4 には常に「男」が出力されます。
しかも `pt_sex.Text` には `showList` / `readPatCsv` の時点で既に「女」または「男」が入っています。

```diff
-		if (pt_sex.Equals("2"))
-		{
-			dictionary["4, 2"] = "女";        // B4: 性別（女）
-		}
-		else
-		{
-			dictionary["4, 2"] = "男";        // B4: 性別（男）
-		}
+		dictionary["4, 2"] = pt_sex.Text;   // B4: 性別（showList で「女」/「男」に変換済み）
```

> 医療文書の記載誤りになるため、リファクタリングとは切り離して単独で修正・リリースすることを推奨します。

### 1-2. 🔴 例外のあと接続が開いたまま残る — `Form1.Plan.cs:25,204,267,289,370` / `Form1.cs:368` / `TmpStaff.cs:80,112`

`Form1` と `TmpStaff` では `oraConn.Open()` と `oraConn.Close()` の間に `try/finally` がありません。

再現例: 患者ID欄に `12a` と入力して Enter を押す
→ `showList` の `where P_ID = 12a` が Oracle エラーになる
→ `Close()` が実行されず、接続が開いたまま残る
→ 以降の `oraConn.Open()` はすべて `InvalidOperationException`（接続は既に開いています）で失敗する
→ アプリを再起動するまで登録・印刷・一覧表示ができない。

対策は §2-1 の DB ヘルパーでまとめて解消できます。あわせて、SQL に直接連結している数値ID
（`pt_id` / `dr_id` / `Agree_id`）は、入口で `int.TryParse` による検証を入れるべきです（§2-1 参照）。

### 1-3. 🟠 保存に失敗しても入力内容が消える — `Form1.cs:338-360`

```csharp
case DialogResult.Yes:
    regPlan();      // 入力チェックで -1 を返しても…
    clearPlan();    // …そのまま消去される
```

```diff
 			case DialogResult.Yes:
-				regPlan();
+				if (regPlan() == -1)
+				{
+					return;
+				}
 				clearPlan();
 				break;
```

`TmpAgree.tmpPlanTree_NodeMouseClick`（`TmpAgree.cs:406-428`）にも同じ流れがあり、`regPlanTemplate()` が入力チェックで中断しても画面の切替が続行されます。

### 1-4. 🟠 同名テンプレートの取り違え — `TmpAgree.cs:576-596`

`getTempIdFromNode` は「ノードの表示名と親ID」でテンプレートIDを探しています。
子テンプレートの登録時には名前の重複チェックが無いため、同じ分類に同名のテンプレートが2つあると、常に1つ目が適用されます。
一方で `TreeNode.Name` には既に `TEMP_ID` が入っています（`initTree` の `Nodes.Add(TEMP_ID, TEMP_NAME)`）。

```diff
 	private int getTempIdFromNode(TreeNode tnode)
 	{
-		int result = -1;
-		if (tnode.Level == 0)
-		{
-			result = int.Parse(parentTable[tnode.Text].ToString());
-		}
-		else
-		{
-			... tmpNodeList を名前で線形探索 ...
-		}
-		return result;
+		return int.Parse(tnode.Name);
 	}
```

これで `TmpNode` 構造体、`tmpNodeList`、`nodeTable` が不要になり、`initTree` で重複している `TmpNode` の生成（`TmpAgree.cs:155-164` と `177-189`）も消せます。
また、`parentTable` はキーを `Trim()` して登録しているのに `tnode.Text` はトリムせずに引いているため、名前の末尾に空白があると `NullReferenceException` になるという潜在バグも同時に解消します。

---

## 2. シンプル化（効果の大きい順）

### 2-1. DB アクセスを小さなヘルパーにまとめる

**現状**: `Form1` / `TmpAgree` / `TmpStaff` がそれぞれ `oraConn`・`oraCmd`・`oraReader` をフィールドに持ち、Open → Command → Close を19箇所で手書きしています。
`TmpAgree` には正しい `try/finally` 版がありますが、同じ定型文が7回コピーされています（例: `TmpAgree.cs:126-136`）。

**提案**: 接続を受け取る静的ヘルパーを1つ用意します。
`OleDbConnection.Close()` は既に閉じた接続に対して呼んでも何も起きないため、`if (State != Closed)` の確認も不要です。

```csharp
internal static class Db
{
	public static int Execute(OleDbConnection con, string sql)
	{
		con.Open();
		try
		{
			using (var cmd = new OleDbCommand(sql, con)) return cmd.ExecuteNonQuery();
		}
		finally { con.Close(); }
	}

	public static object Scalar(OleDbConnection con, string sql) { /* 同形 */ }

	public static void Read(OleDbConnection con, string sql, Action<OleDbDataReader> onRow)
	{
		con.Open();
		try
		{
			using (var cmd = new OleDbCommand(sql, con))
			using (var r = cmd.ExecuteReader())
				while (r.Read()) onRow(r);
		}
		finally { con.Close(); }
	}
}
```

呼び出し側は次のようになります。

```csharp
// delPlan: 5行 → 1行
Db.Execute(oraConn, "update AGREE set DELETE_FLAG = 1 where AGREE_ID = " + agreeId);
```

- `oraCmd` / `oraReader` フィールドは削除できます（共有の可変状態がなくなる）。
- 1-2 の接続リーク問題は、この置き換えだけで全箇所解消します。
- 数値ID を連結している箇所は `int.TryParse` で検証してから `int` として渡すようにします。
  将来的には OleDb パラメータ（`?`）に移行すれば `AgreeSql.SqlValue` 自体が不要になりますが、現状の `SqlValue` 方針のままでも、数値ID の検証を入れれば十分です。
- 補足: `TmpStaff.initList`（`TmpStaff.cs:34-37`）の `Open()`/`Close()` は、`DataAdapter.Fill` が自分で接続を開閉するため意味がありません。

### 2-2. INI 読込を1つにまとめる — `Form1.cs:62-98`, `103-148` / `ExcelControl.cs:211-272`

`EyeAgreeSettings.ini` をほぼ同じループで3回読み込んでいます。しかも実装ごとに挙動が微妙に違います。

| 実装 | キーの判定 | 捕捉する例外 |
|---|---|---|
| `applySettingButtonVisibility` | `line.Contains("SHOW_SETTING_BUTTON")` | `IOException` のみ |
| `applyWindowPosition` | 完全一致 | `IOException` のみ |
| `loadBarcodeSettings` | 完全一致 | `Exception` |

`UnauthorizedAccessException` はフォームのコンストラクタ内で発生するため、`Application.Run` より前の段階であり、`ThreadException` では捕捉されず起動失敗になります。

```csharp
internal static class AppSettings
{
	// key=value を1回だけ読む。ファイルが無い・読めない場合は空の辞書を返す。
	private static readonly Dictionary<string, string> values = load();

	public static int GetInt(string key, int defaultValue) { ... }
	public static float GetFloat(string key, float defaultValue) { ... }
	public static string Get(string key, string defaultValue) { ... }
}
```

```csharp
Location = new Point(AppSettings.GetInt("WINDOW_X", 0), AppSettings.GetInt("WINDOW_Y", 0));
settingButton.Visible = AppSettings.Get("SHOW_SETTING_BUTTON", "0") == "1";
```

これで約130行が約30行になり、挙動も統一されます。

### 2-3. `applyTemplate` の9回コピーを1行ずつにする — `Form1.Plan.cs:364-460`

```csharp
private static void appendText(Control box, string value)
{
	box.Text = box.Text.Length > 0 ? box.Text + " " + value : value;
}
```

```csharp
appendText(eye, r["EYE"].ToString());
appendText(diag, r["DIAG"].ToString());
appendText(anes, r["ANES"].ToString());
// ... ITEM4 まで
sheetName.Text = r["SHEET_NAME"].ToString();
```

約95行が約20行になります（`TextBox` と `ComboBox` はどちらも `Control.Text` を持つので、1つのメソッドで扱えます）。
あわせて `select *` は、必要な列名を明示した形にします。

### 2-4. `regPlan` の1行 SQL を分解する — `Form1.Plan.cs:262-275`

266行目は1行が約1,500文字あり、INSERT と UPDATE が三項演算子で連結されています。
列を追加するときは、2つの文字列の両方に、正しい位置で追記しなければなりません。

```csharp
var cols = new (string Name, string Value)[]
{
	("SAVE_DATE",   save_date.Value.ToString("yyyyMMdd")),
	("DEPT",        deptCode.ToString()),
	("DR",          drId.ToString()),
	("STAFF",       AgreeSql.SqlValue(staff.Text)),
	// ... SHEET_NAME / DR_OK / SAVE_TIME
};
string sql = Agree_id.Text.Length == 0
	? "insert into AGREE (AGREE_ID, PATIENT_ID, DELETE_FLAG, " + string.Join(", ", cols.Select(c => c.Name)) + ")"
	  + " values (AGREE_SEQ.nextval, " + ptId + ", 0, " + string.Join(", ", cols.Select(c => c.Value)) + ")"
	: "update AGREE set " + string.Join(", ", cols.Select(c => c.Name + " = " + c.Value)) + " where AGREE_ID = " + agreeId;
```

列の定義が1箇所になり、INSERT と UPDATE で列がずれることがなくなります。

戻り値の `0 / 1 / -1` も意味が読み取れません。`bool`（成功したかどうか）を返し、印刷を促すかどうかは呼び出し側で `staff1_ok` を見て判断する方が素直です。

### 2-5. 一覧グリッドを列名で扱う — `Form1.Plan.cs:28, 49-77, 105-142`

- SELECT にダミー列（`''` が2つ）や未使用の列（`医師完了`）があり、`Columns[0]`〜`Columns[20]` と `Cells[16]` などを数値で参照しています。
  列を1つ追加するだけで、画面表示と `showPlan` の両方が静かに壊れます。
- SQL では `Trim(M_DEPT.S_NAME) as DEPT_NAME` のように別名を付け、未使用の列は削除します。

```csharp
foreach (DataGridViewColumn c in AgreeList.Columns) c.Visible = false;
showColumn("SAVE_DATE", "作成日", 80, format: "0000/00/00");
showColumn("DEPT_NAME", "診療科", 80);
showColumn("DR_NAME",   "医師",   80);
showColumn("EYE",       "眼",     40, center: true);
showColumn("OPE",       "手術",  150, center: true);
```

```csharp
// showPlan
DataGridViewRow row = AgreeList.Rows[rowIndex];
string cell(string name) => row.Cells[name].Value.ToString().Trim();
diag.Text = cell("DIAG");
```

`AgreeList.Rows[rowIndex].Cells[n].Value.ToString().Trim()` が約30回繰り返されている部分が、それぞれ1行で読めるようになります。

### 2-6. `TmpAgree` のボタン状態切替を1つのメソッドにする — `TmpAgree.cs:204-214, 432-451, 454-500, 502-518`

`applyTmpButton` / `newTmpButton` / `editTmpButton` / `regTmpButton` / `delTmpButton` の `Enabled` を6つのメソッドでそれぞれ手書きしています。
`temp_parent.Text = ""; BackColor = LightGray; Enabled = false;` の3行も6回繰り返されています。

```csharp
private void setButtons(bool apply, bool create, bool edit, bool register, bool delete, bool addParent) { ... }
private void lockParentCombo() { temp_parent.Text = ""; temp_parent.BackColor = Color.LightGray; temp_parent.Enabled = false; }
```

どの画面状態でどのボタンが押せるのかを、1行で読めるようになります。
さらに、1-4 を直したあとは `Hashtable` 3つを `Dictionary<string, int>`（分類名→ID）と `Dictionary<int, string>`（ID→分類名）の2つに減らせます。

### 2-7. `printAgree` と `ExcelControl` の受け渡しを明示的にする — `Form1.Plan.cs:312-341` / `ExcelControl.cs:75-109`

- キーが `"行, 列"` 形式の文字列で、`ExcelControl.setValue` 側で `Split` と `int.Parse` によって復元しています。列はすべて2（B列）なので、`Dictionary<int, string>`（行→値）で十分です。
- `MakeEyeAgree` は `values["8, 2"]` という文字列キーで作成日を取り出し、患者IDはセルから読み戻しています（`getCellText(1, 2)`）。
  自分で渡した値を別の経路から取り直しているため、依存関係が見えにくくなっています。`MakeEyeAgree(sheetName, values, patientId, saveDate)` のように引数で渡す方が単純です。
- `B9`（作成時刻）は `printAgree:331` で設定したあと、`MakeEyeAgree:91` で上書きされています。`printAgree` 側の設定は削除できます。
- `ExcelControl` に `IDisposable` を実装すれば、`try/finally { ReleaseExcel(); }` を `using` 1行にでき、`Open` も `private` にできます。

### 2-8. その他の小さな重複

| 場所 | 現状 | 提案 |
|---|---|---|
| `Form1.cs:324-336` | `if (regAgreeButton.Enabled)` で分岐しているが、`Enabled` を変更するコードはどこにも無い（常に true） | `new TmpAgree(this, applyButtonVisible: true).Show();` |
| `Form1.cs:235-237` | `short.Parse(patCont[13])` を4回実行している | ローカル変数に1回だけ代入する |
| `Form1.cs:235` と `Form1.cs:281` | 診療科コードの範囲が `>0 && <20` と `1..20` で食い違っている | `tryParseDeptCode` を1つ作り、両方で使う |
| `Form1.Plan.cs:24,78,79,91` | `clearPlan()` を2回呼び、`printAgreeButton.Enabled = false` も重複している（`clearPlan` の中で設定済み） | 1回にする |
| `Form1.cs:199-221` | `int.Parse(patCont[2])` がコンストラクタから呼ばれ、Pat.csv が不正だと起動できない | `int.TryParse` で読み飛ばす |

---

## 3. 可読性・命名

### 3-1. 逆コンパイル由来の機械的な名前を直す

| 場所 | 現状 | 提案 |
|---|---|---|
| `Form1.Plan.cs:262-265` | `text`, `text2`, `text3` | `sql`, `drOk`, `saveDate` |
| `Form1.Plan.cs:155` | `num` | `rowIndex` |
| `Form1.Plan.cs:377,386,...` | `ComboBox comboBox = eye;` など8箇所 | 2-3 のヘルパーで消える |
| `Form1.cs:333-338` | `tmpAgree` / `tmpAgree2` | 2-8 で消える |
| `Program.cs:37` | `Process[] array = processesByName;` | `processesByName` をそのまま `foreach` する |
| `TmpStaff.cs:148` | `foreach (DataGridViewRow item in (IEnumerable)staffGridView.Rows)` | キャストは不要 |
| `ExcelControl.cs:236,244,252` | `float f;` を分けて宣言している | `out float f` |

### 3-2. 業務の用語とコード上の名前を揃える

- **Plan と Agree が混在している**: `regPlan` / `showPlan` / `clearPlan` / `delPlanButton_Click` / `TmpPlan_Load` / `tmpPlanTree_NodeMouseClick` は、いずれも「同意書」を扱っています（別アプリ「計画書」から流用した名残と思われます）。
  `regAgreeButton.Click` が `regPlanButton_Click` に結びついているなど、検索しても見つけにくい状態です。
- **`staff1_ok`** の実体は `DR_OK`（医師完了フラグ）です → `doctorConfirmed` のような名前にする。
- **`dr_id` / `dr_name`** は画面上「入力者」と表示されています。どちらの用語を正とするか決め、コメントかフィールド名で統一する。
- **`readPatCsv` / `readPatCsv2`**: 2つ目は「Pat.csv の医師情報を画面に反映する」処理です → `applyDoctorFromPatCsv`。
- **命名規則が3種類混在している**（`showList` / `ExportTableToCsv` / `pt_id`・`Agree_id`）。一括リネームは差分が大きくなるため、新規コードと上記の改修箇所から、既存の多数派であるメソッド lowerCamel に揃えるのが現実的です。

### 3-3. エラー処理の方針を揃える

- `printAgree` は `Logger.Error` を呼んでから日本語メッセージを出していますが、
  `showPlan`（`Form1.Plan.cs:146-150`）と `exportButton_Click` / `importButton_Click`（`Form1.ImportExport.cs:39,75`）はログを残さず `ex.Message` をそのまま表示しています。
  → `catch` では必ず `Logger.Error(論理名, ex)` を呼ぶ、という方針に統一する。
- `MergeCsvToTable` の `throw new Exception(...)`（`Form1.ImportExport.cs:163`）は `InvalidDataException` にする。

### 3-4. 名前空間

`ExcelControl` だけがグローバル名前空間にあり、`Agree.Logger` と完全修飾で呼んでいます（`ExcelControl.cs:283,304`）。
`namespace Agree;` を付ければ、他のファイルと揃います。

---

## 4. 削除候補（使われていないコード・ファイル）

| 対象 | 理由 |
|---|---|
| `Form1.cs:397-415` `label5_Click` / `agreePlanListLabel_Click` / `panel1_Paint` / `explanation_TextChanged` | 中身が空のイベントハンドラ。Designer 側の `+=` の行（`Form1.Designer.cs:304,388,457,622`）と一緒に削除する |
| `Properties/Resources.cs`、`Properties/Settings.cs` | `Agree.csproj` にビルド対象として登録されていない。`*.Designer.cs` と重複する逆コンパイルの残骸 |
| 各ファイルの未使用の `using` | `System.ComponentModel`、`Microsoft.VisualBasic.FileIO`（`Form1.ImportExport.cs` 以外）、`System.Collections.Generic` など |
| `Form1.Plan.cs:28` の `''` 列2つと `医師完了` 列 | どこからも参照されていない（2-5 参照） |
| `Agree.csproj` の `<LangVersion>` 3重定義 | 共通の `PropertyGroup` にある1つだけ残す |

---

## 5. ドキュメント・ビルド設定の食い違い

- **`docs/dataflow.md` が現状と一致していない**
  - 行番号が古い（`Form1.cs:1029` などとあるが、`Form1.cs` は partial に分割され、現在は416行）。
  - バーコード値の出力先が「`[11,2]`」と書かれているが、現在は B10（`ExcelControl.cs:97`）。本文の「27〜35行へ割当て」も、現在の12〜20行と一致しない。
  - → 行番号はすぐに古くなるため、**メソッド名での参照に置き換える**ことを推奨します。
- **CLAUDE.md の「32bit(x86)専用ビルド」と一致していない**: `Agree.csproj` に `Debug|AnyCPU` / `Release|AnyCPU` の構成が残っており、`Agree.slnx` にも `Any CPU` プラットフォームがあります。
  64bit 環境で AnyCPU 構成を選ぶと、32bit の Oracle OLE DB プロバイダを読み込めません。使っていないのであれば、AnyCPU 構成を削除すると誤ったビルドを防げます。

---

## 6. 良い点（このまま維持する）

- `ExcelControl` の COM 解放（null ガード付きの `ReleaseExcel`、`Workbooks` / `Sheets` / `Range` の個別解放、`finally` での `ScreenUpdating` 復元）。
- `Logger` の「患者情報を記録しない」方針と、ロガー自身が例外を投げない設計。
- `AgreeSql` をソースリンクしてテストする構成（依存ゼロの L1 テスト）。
- 「なぜそうしているか」を書いたコメント（例: `activateAllSheets` のフォームコントロール脱落対策、`buildBarcodeValue` の先頭ゼロ落ち対策）。

---

## 推奨する進め方

テストが無いため、**不具合修正 → 外部から見た挙動を変えない整理** の順に、小さなコミットに分けて進めます。

```
1. 1-1 性別の修正（単独でリリース）       → 検証: 女性患者で印刷し、B4 が「女」になる
2. 1-3 / 1-4 の修正                     → 検証: 保存失敗時に入力が残る／同名テンプレートで正しい方が適用される
3. 2-1 Db ヘルパー導入 + 数値ID検証       → 検証: 患者IDに「12a」を入力したあとも一覧表示・登録ができる
                                           （OracleIntegrationTests が引き続き green）
4. 4 の削除 + 3-1 の機械的リネーム         → 検証: dotnet build Agree.slnx が警告を増やさずに通る
5. 2-2〜2-7 を1項目ずつ                  → 検証: docs/test_strategy.md §5 の手動スモークテスト
6. 5 のドキュメント・構成の整理
```

`AppSettings` の `load()` や `tryParseDeptCode` のような純粋な関数は、`AgreeSql.cs` と同じ方式でテストプロジェクトにソースリンクすれば、L1 の単体テストを追加できます。
