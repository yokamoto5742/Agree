# 本番DBのテーブル定義を取得する手順（DumpSchema）

本番DBの接続情報は暗号化された `AgentlabUtilityLibrary.ini` の中にあり、sqlplus や
PowerShell からは接続文字列を組み立てられない。そのため、アプリ本体と同じ
`Env` / `Enc` / `DBConn` を使う調査用ツール `tools/DumpSchema.cs` で
データディクショナリ（`ALL_TAB_COLUMNS`）を読み出す。

- **実行するSQLは SELECT のみ**（`ALL_TAB_COLUMNS` と `SELECT USER FROM DUAL`）。
  テーブルの中のデータは読まない・書き換えない。
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

- Visual Studio のバージョンによりパスが異なる。`Roslyn\csc.exe` を探して読み替える
  （`C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe` は古いC#しか通らないので使えない）。
- `warning CS8012`（System.Data のプロセッサ不一致）は出るが無害。
- 生成物は `DumpSchema.exe` 1ファイルのみ。外部DLLは不要。

## 2. 本番PCへ持ち込んで実行する

`DumpSchema.exe` を本番PCの任意の場所（例: `C:\temp`）へコピーする。
設定ファイルは**コピーしない**。本番PCにあるものをそのまま使う。

`Env` は ini を次の順で探す。**`AgentlabUtilityLibrary.ini` があるディレクトリを
カレントディレクトリにして実行すること**（exeの置き場所ではなくカレントで決まる）。

1. `カレントディレクトリ\AgentlabUtilityLibrary.ini`
2. `c:\macs\utility\AgentlabUtilityLibrary.ini`

```cmd
rem 例: 2 のフォールバック先を使う場合はどこから実行してもよい
C:\temp\DumpSchema.exe OPEN "AGREE%" > C:\temp\schema_open.txt

rem 例: アプリと同じフォルダの ini を使う場合はそこへ cd してから実行する
cd /d C:\Shinseikai\Agree
C:\temp\DumpSchema.exe OPEN "AGREE%" > C:\temp\schema_open.txt
C:\temp\DumpSchema.exe EHR  "M!_%"   > C:\temp\schema_ehr.txt
```

出力の1行目に **Provider / DataSource / DBリンク / オーナー** が出るので、
意図した接続先（本番）を見ていることを必ず確認する。パスワードは出力しない。

- リダイレクトしたファイルの文字コードは **Shift_JIS(CP932)**。UTF-8で保存したい場合は
  `powershell -c "C:\temp\DumpSchema.exe OPEN '%' | Out-File -Encoding UTF8 C:\temp\schema.txt"`。
- 画面で見る場合、日本語が化けるなら `chcp 932` を確認する。

## 3. 引数リファレンス

```
DumpSchema.exe [OPEN|EHR] [表名のLIKE] [オーナーのLIKE] [@DBリンク名]
```

| 引数 | 既定値 | 説明 |
|---|---|---|
| 第1 | `OPEN` | `OPEN`=同意書側（`AGREE` 等）、`EHR`=電子カルテのマスタ（`M_xxx`）。iniの `OPEN_*` / `EHR_*` に対応 |
| 第2 | `%` | テーブル名のLIKE。大文字に変換される。`_` を1文字ワイルドカードにしたくない場合は `M!_%` のように `!` でエスケープ |
| 第3 | 接続ユーザ | オーナー（スキーマ）のLIKE。別スキーマを見るなら `KARTE%` のように指定、全件なら `%` |
| 第4 | `EHR` のとき iniの `DB_LINK` | データディクショナリを引くDBリンク（`@LINKNAME`）。明示指定すると上書きできる |

よく使う組み合わせ:

```cmd
DumpSchema.exe OPEN "AGREE%"      rem 同意書側の3表（AGREE / AGREE_TEMPLATE / AGREE_STAFF）
DumpSchema.exe OPEN "%"           rem 接続ユーザが見える表すべて
DumpSchema.exe EHR  "M!_USR"      rem 電子カルテの職員マスタだけ
DumpSchema.exe EHR  "M!_%" "%"    rem 電子カルテのマスタを全オーナー横断で
```

## 4. 電子カルテ側（`M_xxx`）を取るときの注意

本アプリは電子カルテのマスタを `テーブル名 + DB_LINK` の形で参照する（`Agree/Ehr.cs`）。

- iniに `EHR_*` が設定されていれば、`EHR` は電子カルテDBへ直接接続する。
- 設定が無い場合、`EHR` は `OPEN` と同じ接続になり、マスタは `DB_LINK` 経由で参照される。
  このときツールは `ALL_TAB_COLUMNS@リンク名` を引く（第4引数の既定値）。
  出力ヘッダの「DBリンク」が `(なし)` なのに `M_xxx` が出てこない場合は、
  本番のiniで `DB_LINK` が設定されているかを確認する。

## 5. 出力例

```
--- OPEN : Provider=OraOLEDB.Oracle / DataSource=localhost:1521/FREEPDB1 / DBリンク=(なし) / オーナー LIKE TEST_USER ---

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
| `該当する表がありません` と出る | 表名LIKEの指定ミス。`_` は1文字ワイルドカードなので `M!_%` のようにエスケープする |

## 7. 取得後

- 出力には患者データは含まれないが、テーブル定義は院内システムの情報なので
  取り扱い（保管場所・共有範囲）に注意する。
- 本番の定義が判明したら `docs/test_db_schema.sql` の「推測」コメントを実測値に更新し、
  `docs/design/data_model.md` と差分がないか確認する。
- 持ち込んだ `DumpSchema.exe` は作業後に本番PCから削除する。
