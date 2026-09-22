# 本番DBのテーブル定義を取得する手順（DumpSchema）

本番DBの接続情報は暗号化された `AgentlabUtilityLibrary.ini` の中にあり、sqlplus や
PowerShell からは接続文字列を組み立てられない。そのため、アプリ本体と同じ
`Env` / `Enc` / `DBConn` を使う調査用ツール `tools/DumpSchema.cs` で
データディクショナリ（`ALL_TAB_COLUMNS`）を読み出す。

- **実行するSQLは SELECT のみ**（`ALL_TAB_COLUMNS` / `SELECT USER FROM DUAL` /
  `NLS_DATABASE_PARAMETERS`、および代替取得時の `select * from 表 where 1 = 0`）。
  テーブルの中のデータは読まない・書き換えない（`where 1 = 0` なので行は返らない）。
- 取得できるのは **接続ユーザに参照権限のある表だけ**。DBA権限は不要。
- 本番環境で実行する前に、実施の可否と時間帯を管理者に確認すること。

---

## 1. 開発機で exe をビルドする

本番PCにVisual Studioは無い前提で、**開発機でビルドしてexeだけを持ち込む**。
PlatformTargetは本体と同じ **x86 固定**（本番のOLE DBプロバイダが32bitのため）。

```cmd
cd /d C:\Users\yokam\source\repos\Agree

"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\Roslyn\csc.exe" ^
  -nologo -platform:x86 -langversion:latest -r:System.Data.dll ^
  -out:DumpSchema.exe ^
  tools\DumpSchema.cs ^
  Agree\Infrastructure\Env.cs Agree\Infrastructure\Enc.cs ^
  Agree\Infrastructure\CharMap.cs Agree\Infrastructure\DBConn.cs
```

- Visual Studio のバージョンによりパスが異なる。`Roslyn\csc.exe` を探して読み替える。
- `warning CS8012`（System.Data のプロセッサ不一致）は出るが無害。
- 生成物は `DumpSchema.exe` 1ファイルのみ。外部DLLは不要。

## 2. 本番PCへ持ち込んで実行する

`DumpSchema.exe` を本番PCの任意の場所（例: `C:\temp`）へコピーする。
設定ファイルは**コピーしない**。本番PCにあるものをそのまま使う。

`Env` は ini を次の順で探す。**`AgentlabUtilityLibrary.ini` があるディレクトリを
カレントディレクトリにして実行すること**（exeの置き場所ではなくカレントで決まる）。

```cmd
cd /d C:\Shinseikai\EyeAgree
C:\temp\DumpSchema.exe OPEN "AGREE%"     > C:\temp\schema_open.txt
C:\temp\DumpSchema.exe EHR  "M!_%"  "%"  > C:\temp\schema_ehr.txt
```

`EHR` で第3引数（オーナー）を省略すると**接続ユーザのスキーマしか見ない**。
電子カルテのマスタは別スキーマにあることが多いので `"%"` を付ける（4章）。

出力の1行目に **Provider / DataSource / DBリンク / オーナー / キャラクタセット** が出るので、
意図した接続先（本番）を見ていることを必ず確認する。パスワードは出力しない。
キャラクタセット（`NLS_CHARACTERSET`）は `VARCHAR2` の桁数（バイト数）が全角で何文字分に
なるかの判断に使う（`JA16SJIS` なら 2 バイト/字、`AL32UTF8` なら 3 バイト/字）。

- リダイレクトしたファイルの文字コードは **Shift_JIS(CP932)**。UTF-8で保存したい場合は
  `powershell -c "C:\temp\DumpSchema.exe OPEN '%' | Out-File -Encoding UTF8 C:\temp\schema.txt"`。
- 画面で見る場合、日本語が化けるなら `chcp 932` を確認する。

## 3. 引数リファレンス

```
DumpSchema.exe [OPEN|EHR] [表名のLIKE または 表名,表名,…] [オーナーのLIKE] [@DBリンク名]
```

| 引数 | 既定値 | 説明 |
|---|---|---|
| 第1 | `OPEN` | `OPEN`=同意書側（`AGREE` 等）、`EHR`=電子カルテのマスタ（`M_xxx`）。iniの `OPEN_*` / `EHR_*` に対応 |
| 第2 | `%` | テーブル名のLIKE。大文字に変換される。`_` を1文字ワイルドカードにしたくない場合は `M!_%` のように `!` でエスケープ。**`%` を含めずに書くと「表名の直接指定」**（カンマ区切り可）になり、ディクショナリが0件のとき代替取得に進む（4.2） |
| 第3 | 接続ユーザ | オーナー（スキーマ）のLIKE。別スキーマを見るなら `KARTE%` のように指定、全件なら `%` |
| 第4 | `EHR` のとき iniの `DB_LINK` | データディクショナリを引くDBリンク（`@LINKNAME`）。明示指定すると上書きできる |

よく使う組み合わせ:

```cmd
DumpSchema.exe OPEN "AGREE%"      rem 同意書側の3表（AGREE / AGREE_TEMPLATE / AGREE_STAFF）
DumpSchema.exe OPEN "%"           rem 接続ユーザが見える表すべて
DumpSchema.exe EHR  "M!_%" "%"    rem 電子カルテのマスタを全オーナー横断で
DumpSchema.exe EHR  "M_DEPT,M_PATIENT,M_USR" "%"   rem 表名を直接指定（代替取得あり）
```

## 4. 電子カルテ側（`M_xxx`）を取るときの注意

本アプリは電子カルテのマスタを `テーブル名 + DB_LINK` の形で参照する（`Agree/Ehr.cs`）。

- iniに `EHR_*` が設定されていれば、`EHR` は電子カルテDBへ直接接続する。
- 設定が無い場合、`EHR` は `OPEN` と同じ接続になり、マスタは `DB_LINK` 経由で参照される。
  このときツールは `ALL_TAB_COLUMNS@リンク名` を引く（第4引数の既定値）。
  出力ヘッダの「DBリンク」が `(なし)` なのに `M_xxx` が出てこない場合は、
  本番のiniで `DB_LINK` が設定されているかを確認する。

### 4.1 実際に0件になった例（解決済み）

> **結果**：下記 1（第3引数に `%`）で取得できた。電子カルテのマスタはオーナー `MEDB` にあり、
> キャラクタセットは `AL32UTF8`（出力は `docs/schema_ehr.txt`）。以下は経緯として残す。

```
--- EHR : Provider=MSDAORA.1 / DataSource=INNO_OPEN / DBリンク=@inno.world / オーナー LIKE OPEN ---

該当する表がありません（表名・オーナーの指定、または参照権限を確認してください）。
```

接続もDBリンクも通っている（`select USER from DUAL@inno.world` が成功して `OPEN` を返した）。
落ちているのは**オーナー条件**で、`ALL_TAB_COLUMNS@inno.world` を `OWNER like 'OPEN'` で
絞ったため 0 件になった。電子カルテのマスタは別スキーマにあり、`OPEN` からはシノニム経由で
見えている、という状態だと考えられる。次の順に切り分ける。

1. **オーナーを外して取り直す**（第3引数に `%`）。これで解決する見込みが高い。

   ```cmd
   C:\temp\DumpSchema.exe EHR "M!_%" "%" > C:\temp\schema_ehr.txt
   ```

2. 1 でも 0 件なら、**リンク先ユーザにデータディクショナリの行が見えていない**（表への
   `SELECT` 権限がシノニム＋直接 GRANT ではなくロール経由、など）。表名を直接指定して
   代替取得に進ませる（4.2）。

   ```cmd
   C:\temp\DumpSchema.exe EHR "M_DEPT,M_PATIENT,M_USR" "%" > C:\temp\schema_ehr.txt
   ```

3. リンク名そのものを疑う場合は第4引数で明示する（`ORA-02019` が出るなら名前違い）。

   ```cmd
   C:\temp\DumpSchema.exe EHR "M!_%" "%" "@inno.world"
   ```

### 4.2 ディクショナリが見えないときの代替取得

第2引数を LIKE ではなく**表名の直接指定**（`%` を含めない。カンマ区切り可）にすると、
`ALL_TAB_COLUMNS` が 0 件だったときだけ
`select * from 表@リンク where 1 = 0` の結果セットから `OleDbDataReader.GetSchemaTable()`
で列定義を取り直す。アプリ自体が `M_DEPT@inno.world` / `M_PATIENT@inno.world` /
`M_USR@inno.world` を読めている以上、表への `SELECT` は通るのでこの経路なら取れる。

- 行は返らない（`where 1 = 0`）。患者データは読まない。
- 取れるのは**列名・OLE DB 上の型・桁数・NULL 可否**まで。
  **`VARCHAR2` の BYTE/CHAR の別、桁数無指定の `NUMBER`、Oracle の型名そのもの**は分からない。
  ディクショナリから取れたときはそちらが優先（自動でそうなる）。
- 表ごとに失敗しても止まらず、`取得できません: <エラー>` を出して次の表へ進む。

出力例:

```
ALL_TAB_COLUMNS が 0 件のため、select * from 表 where 1 = 0 から取り直します。

[M_USR@inno.world] ※結果セットから取得。BYTE/CHAR の別と桁数無指定の NUMBER は分からない
  CODE             Numeric(5,0)         NOT NULL
  NAME             VarChar(100)
```

## 5. 出力例

```
--- OPEN : Provider=OraOLEDB.Oracle / DataSource=localhost:1521/FREEPDB1 / DBリンク=(なし) / オーナー LIKE TEST_USER / キャラクタセット=AL32UTF8 ---

[TEST_USER.AGREE]
  AGREE_ID         NUMBER               NOT NULL
  PATIENT_ID       NUMBER
  STAFF            VARCHAR2(100 CHAR / 400 BYTE)
  DIAG             VARCHAR2(2000 CHAR / 4000 BYTE)
  DR_OK            NUMBER(1,0)
```

`VARCHAR2` は **文字長 / バイト長の両方**を表示する（`USER_TAB_COLUMNS.DATA_LENGTH`
だけを見るとバイト長なので、`100 CHAR` を 400 と誤読しやすい）。

## 6. トラブルシュート

| 症状 | 原因と対処 |
|---|---|
| `BadImageFormatException` | 64bitでビルドした。`-platform:x86` で再ビルドする |
| `プロバイダーが見つかりません` / `Provider cannot be found` | 32bitのOLE DBプロバイダが未登録。本番は `MSDAORA.1`（Oracle Client 32bit）、ローカル検証機は `OraOLEDB.Oracle`（`docs/oracle_oledb_32bit_setup.md`） |
| 接続先が想定と違う（1行目のDataSourceが違う） | iniを拾えていない。カレントディレクトリを ini のある場所にする（2章） |
| `ORA-01017`（ユーザー名/パスワード無効） | iniの暗号化値が環境と合っていない。その環境のiniを使う |
| `ORA-00942` / 表が0件 | 接続ユーザに参照権限がない。第3引数でオーナーを指定する、または権限を確認する |
| `ORA-02019`（接続記述子が見つからない） | DBリンク名が違う。第4引数で `@正しいリンク名` を指定する |
| `該当する表がありません` と出る | 表名LIKEの指定ミス（`_` は1文字ワイルドカードなので `M!_%` とエスケープする）、またはオーナー指定。`EHR` では第3引数に `%` を付ける（4.1） |

## 7. 取得後

- 出力には患者データは含まれないが、テーブル定義は院内システムの情報なので
  取り扱い（保管場所・共有範囲）に注意する。`docs/schema_*.txt` は `.gitignore` 済み
  （コミットされない）。
- 本番の定義が判明したら `docs/test_db_schema.sql` の「推測」コメントを実測値に更新し、
  `docs/design/data_model.md` と差分がないか確認する。
  → 同意書側（`AGREE` / `AGREE_TEMPLATE` / `AGREE_STAFF`）は `docs/schema_open.txt`、
  電子カルテ側（`M_xxx`）は `docs/schema_ehr.txt` を元に反映済み。
- 持ち込んだ `DumpSchema.exe` は作業後に本番PCから削除する。
