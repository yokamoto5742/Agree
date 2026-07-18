# コードレビュー: ExcelControl.cs

対象: `ExcelControl.cs`（341行）
観点: 可読性・メンテナンス性・KISS原則
呼び出し元: `Agree/Form1.Plan.cs` の `printAgree()` のみ

全体として、COM解放のnullガードやバーコード値のゼロ埋め理由など、コメントで「なぜ」が説明されている点は良好です。一方で、`MakeEyeAgree` への責務集中、`dynamic` の多用、無意味な変数名がメンテナンス性を下げています。以下、優先度順に指摘します。

---

## 優先度: 高

### 1. `MakeEyeAgree` が長すぎ、責務が多すぎる（94〜172行）

1メソッドで「ブックを開く → 全シートアクティブ化 → セル書込み → バーコード値構築 → バーコード画像挿入 → 保存 → シート選択」の7つを行っており、約80行あります。修正時に影響範囲を追いにくく、このクラス最大のメンテナンス性リスクです。

処理単位で private メソッドに抽出することを推奨します:

```csharp
public void MakeEyeAgree(string sheetName)
{
    string openError = Open(Env.AGENT_HOME + "\\EyeAgree\\EyeAgree.xlsm", "共通情報");
    if (!string.IsNullOrEmpty(openError))
    {
        throw new IOException(openError);
    }
    exApp.ScreenUpdating = false;
    activateAllSheets();          // フォームコントロール脱落回避のワークアラウンド
    setValue(valueList);
    DateTime now = DateTime.Now;
    string barcodeValue = writeCommonSheetAndBuildBarcode(now);
    insertBarcodeToFormSheets(barcodeValue);
    saveWorkbook(now);
    selectTargetSheet(sheetName);
}
```

各ワークアラウンド（シートアクティブ化、先頭ゼロ問題）の長文コメントもメソッド単位に移り、本流の流れが読みやすくなります。

### 2. 例外発生時に `ScreenUpdating` / `EnableEvents` が復元されない

105行目で `ScreenUpdating = false`（`Open` 内で `EnableEvents = false`）にした後、`setValue` や `SaveAs` で例外が出ると復元されずに終わります。Excel は `Visible = true` のまま残る設計なので、ユーザーの手元に「描画が凍結し、イベントも無効な Excel」が残ります。呼び出し側の `ReleaseExcel()` は COM 参照を解放するだけで、この状態は直りません。

```csharp
exApp.ScreenUpdating = false;
try
{
    // シート処理・保存
}
finally
{
    exApp.EnableEvents = true;
    exApp.ScreenUpdating = true;
}
```

### 3. 意味を持たない変数名（134〜145行）

```csharp
Range range = ...;   // 実体は患者ID (B1)
Range range2 = ...;  // 実体は入力者ID (B7)
Range range5 = ...;  // 実体は診療科コード (B5)
string doc1 = ...;
string barcode11 = ...;  // 実際の出力先は B11 ではなく B10
```

`range2` の次が `range5` で欠番があり、`barcode11` は書込み先が B10 なので名前が実態と矛盾しています（コメントで補足する羽目になっている）。セルの意味で命名すれば行コメントの大半も不要になります:

```csharp
Range patientIdCell = (Range)exWorksheet.Cells[1, 2];
Range doctorIdCell  = (Range)exWorksheet.Cells[7, 2];
Range deptCodeCell  = (Range)exWorksheet.Cells[5, 2];
string barcodeValue = patient + docCode + dept + doctor + ymd + hms;
```

`setValue` 内の `num` / `num2` も同様に `row` / `col` へ。

---

## 優先度: 中

### 4. `(dynamic)` キャストの多用

`(_Worksheet)(dynamic)exWorkbook.Sheets[...]`、`((dynamic)range.Value2).ToString()`、`((dynamic)shapes).AddPicture(...)` など計10か所以上で `dynamic` を経由しています。インデクサや `Value2` の戻り値は `object` なので直接キャストでき、`dynamic` はコンパイル時チェックを失わせるだけです:

```csharp
exWorksheet = (_Worksheet)exWorkbook.Sheets[sheetName];
string patient = Convert.ToString(patientIdCell.Value2).PadLeft(9, '0');
Shapes shapes = sheet.Shapes;
Shape picture = shapes.AddPicture(tempPath, MsoTriState.msoFalse, MsoTriState.msoTrue, left, top, 250f, 30f);
```

`shapes` / `picture` を `object` でなく実型（`Shapes` / `Shape`）で持てば、`(int)XlPlacement.xlMove` のような enum→int 変換も不要になります。

### 5. `Missing.Value` の羅列（63行・162行）

C# 4.0 以降は COM のオプション引数を省略できます。KISS の観点で最も効果の大きい即効修正です:

```csharp
// Before
exWorkbook = exApp.Workbooks.Open(fileName, Missing.Value, Missing.Value, /* ×13 */ ...);
exWorkbook.SaveAs(filename, XlFileFormat.xlOpenXMLWorkbookMacroEnabled, Missing.Value, /* ×9 */ ...);

// After
exWorkbook = exApp.Workbooks.Open(fileName);
exWorkbook.SaveAs(filename, XlFileFormat.xlOpenXMLWorkbookMacroEnabled,
    AccessMode: XlSaveAsAccessMode.xlExclusive);
```

なお `exApp.Workbooks.Open(...)` は `Workbooks` コレクションの COM 参照を解放せずに使っています（いわゆる 2ドット問題）。CLAUDE.md の「COMオブジェクトは確実に解放」方針に合わせるなら、`Workbooks` を一度変数に受けて `ReleaseComObject` してください。同様に 163行目で `exWorksheet` を差し替える際、旧参照（共通情報シート）が未解放のまま上書きされています。

### 6. フィールド宣言がメソッドの間に散在（176〜184行）

`barcodeLineWidth` などバーコード設定4フィールドが `MakeEyeAgree` と `loadBarcodeSettings` の間に置かれています。クラス先頭の他フィールド（14〜20行）と場所が分かれており、状態の全体像を把握しにくくなっています。宣言をクラス先頭にまとめてください。

### 7. `"行, 列"` 文字列キーの辞書は契約として脆弱

`Dictionary<string, string>` のキー `"1, 2"` を `int.Parse(key.Split(','))` で分解しています。空白入りキーが動くのは `int.Parse` が空白を許容するためで、書式の契約が呼び出し側（`Form1.Plan.cs`）とのコメント頼みになっています。また `key.Split(',')` を2回呼んでいます。

最小修正なら `Split` を1回にまとめる、より堅くするならタプルキーが型で契約を表現できます:

```csharp
// 最小修正
string[] rc = key.Split(',');
exWorksheet.Cells[int.Parse(rc[0]), int.Parse(rc[1])] = valueToCell[key];

// 推奨（呼び出し側も含めた変更になるため任意）
public Dictionary<(int Row, int Col), string> ValueList { ... }
```

---

## 優先度: 低

### 8. set専用プロパティ `ValueList`（22〜28行）

書込み専用プロパティは不自然で、`excelControl.ValueList = dict; excelControl.MakeEyeAgree(...)` と2段階の呼び出し規約を強います。使用箇所は1か所だけなので、引数で渡す方がシンプルです: `MakeEyeAgree(string sheetName, Dictionary<string, string> values)`。

### 9. `Open` のエラー通知がエラー文字列戻り値（51〜72行）

`Open` は失敗をエラー文字列で返し、唯一の呼び出し元 `MakeEyeAgree` がそれを `IOException` に詰め直しています。2つのエラー通知方式が混在しており、`Open` が直接例外を投げれば `result` 変数・詰め直し・「空文字＝成功」の暗黙契約がすべて消えます。ただし public メソッドのシグネチャ変更になるため、着手する場合は呼び出し規約の確認を。

### 10. CLSID 直書きによる Excel 起動（60行）

```csharp
exApp = (Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("00024500-...")));
```

interop 参照があるため `exApp = new Application();` で同じことができ、マジック GUID が消えます。（逆コンパイル由来のコードと思われます。動作は同一なので優先度低。）

### 11. ループ内の冗長なローカル変数（154行）

```csharp
string barcodeText = barcode11;
insertBarcode(formSheet, barcodeText);
```

`insertBarcode(formSheet, barcode11);` で十分です。

### 12. `loadBarcodeSettings` の catch が `IOException` のみ（245行）

コメントは「読めない場合は既定値を維持」ですが、`UnauthorizedAccessException` は `IOException` の派生ではないため素通りしてアプリ側に伝播します。意図どおりにするなら `catch (Exception)` に広げるか、コメントを実態に合わせてください。また INI のセクションヘッダ `[BARCODE_SETTINGS]` を無視して全行を走査しているため、将来別セクションに同名キーが増えると誤読します（現状はキーが一意なので実害なし。認識だけ）。

### 13. ファイル配置

`ExcelControl.cs` だけリポジトリ直下にあり、他のソースは `Agree/` 配下です。`Agree/` へ移動すると構成が揃います（csproj の参照更新が必要）。

---

## 対応サマリー

| # | 指摘 | 種別 | 修正コスト |
|---|------|------|-----------|
| 1 | MakeEyeAgree の分割 | メンテナンス性 | 中 |
| 2 | ScreenUpdating/EnableEvents の finally 復元 | 堅牢性 | 小 |
| 3 | 変数名の改善 | 可読性 | 小 |
| 4 | dynamic の除去 | 可読性・型安全 | 小 |
| 5 | Missing.Value 省略・COM参照の解放漏れ | KISS・方針準拠 | 小 |
| 6 | フィールド宣言の集約 | 可読性 | 小 |
| 7 | セルキー書式の整理 | 堅牢性 | 小〜中 |
| 8〜13 | プロパティ設計・Open契約・CLSID ほか | KISS | 任意 |

まず 2（バグに近い）と 3〜6（機械的で安全）を適用し、その後 1 の分割に着手する順序を推奨します。
