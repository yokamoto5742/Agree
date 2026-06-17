# 32bit OraOLEDB 登録 ＆ テストDB整合 トラブルシュート手順

x86 の `Agree.Tests`（および x86 アプリ本体）をローカル Oracle Free に接続する際、
`OraOLEDB.Oracle` プロバイダ未登録 / テストDBのスキーマdrift でつまずいた場合の復旧手順。
セットアップ全体は [`local_oracle_setup.md`](local_oracle_setup.md) を参照。本書はその「32bit プロバイダ」と
「スキーマ不整合」の2点に絞った実戦メモ。

> 症状の見分け方
> - `'OraOLEDB.Oracle' プロバイダーはローカルのコンピューターに登録されていません` → **問題A（登録）**
> - 接続は通るが `ORA-00904: "XXX": invalid identifier` 等 → **問題B（スキーマdrift）**

---

## 問題A: 32bit `OraOLEDB.Oracle` が登録されていない

### 根本原因（ここが落とし穴）
1. 端末に **64bit 版しか登録されていない**ことが多い。x86 は 32bit 版が必須。
2. ODAC Xcopy を展開しても **`install.bat` の regsvr32 は黙って失敗する**。理由は2つ:
   - **`oci.dll` がホーム直下（`bin` ではない）にある**。`OraOLEDB.dll`(bin) は `OCI.dll` を静的依存
     するため、**Oracle ホームが PATH に通っていないとロードできず登録に失敗する**（`/s` なのでエラーも出ない）。
   - regsvr32 は **HKLM 書き込み＝管理者権限**が必要。

### 手順
前提: 32bit ODAC Xcopy を `C:\oracle\odac32` に展開済み（`C:\oracle\odac32\oci.dll` と
`C:\oracle\odac32\bin\OraOLEDB.dll` が存在すること）。

**1) 管理者 PowerShell を開く**（スタート→「powershell」を右クリック→「管理者として実行」→UACで「はい」）。

**2) システム PATH に Oracle ホームを追加 ＋ 32bit 登録**（ヘルパ `C:\oracle\odac32\register_oledb32.ps1` を実行）:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "C:\oracle\odac32\register_oledb32.ps1"
```
ヘルパがやること:
- マシン(システム)PATH に `C:\oracle\odac32` と `C:\oracle\odac32\bin` を追加（重複スキップ）。
- 現プロセス PATH にも反映し、`SysWOW64\regsvr32.exe /s C:\oracle\odac32\bin\OraOLEDB.dll` を実行。
- 結果を `C:\oracle\odac32\register_log.txt` に記録。`regsvr32 exit=0` なら登録成功。

> ヘルパが無い/壊れた場合の最小手順（管理者 PowerShell）:
> ```powershell
> [Environment]::SetEnvironmentVariable('Path', "C:\oracle\odac32;C:\oracle\odac32\bin;" + [Environment]::GetEnvironmentVariable('Path','Machine'), 'Machine')
> $env:PATH = "C:\oracle\odac32;C:\oracle\odac32\bin;" + $env:PATH
> & "$env:WINDIR\SysWOW64\regsvr32.exe" "C:\oracle\odac32\bin\OraOLEDB.dll"
> ```

> ⚠️ ハマりどころ
> - `WindowsPowerShell` は `.ps1` を ANSI で読むため、**スクリプト内に日本語(非ASCII)を書くと文字化けでパースエラー**になる。ヘルパは ASCII のみで書くこと。
> - `Start-Process -Verb RunAs` で UAC を**実際に「はい」**しないと本体は走らない（ログ未生成＝未実行のサイン）。

### 検証（登録できたか）
レジストリのキーパス確認は WOW64 リダイレクトで空振りしやすい。**機能テストが本物の確認**:
```powershell
# 32bit プロセスで接続できるか
$h='C:\oracle\odac32'; $env:PATH="$h;$h\bin;"+$env:PATH
$cs="Provider=OraOLEDB.Oracle;Data Source=localhost:1521/FREEPDB1;User ID=TEST_USER;Password=TEST_PWD"
$c=New-Object System.Data.OleDb.OleDbConnection $cs; $c.Open(); $c.State; $c.Close()
```
↑ を **32bit PowerShell**（`C:\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe`）で実行し
`Open` になれば成功。

> 重要: システム PATH の変更は **新規プロセスにしか効かない**。
> vstest / Visual Studio Test Explorer は **新しいシェルを開く / VS を再起動**してから実行すること。

---

## 問題B: テストDBのスキーマが `test_db_schema.sql` より古い（drift）

### 症状と原因
プロバイダ解決後に `ORA-00904: "SAVE_DATE": invalid identifier` 等で一部テストだけ失敗。
ローカルの `AGREE` テーブルが古い版で作られ、定義に対して列が欠落していた
（実例: `SAVE_DATE, ANES, OPE, EXPLANATION, ITEM1` の5列）。**正はスキーマ定義とテスト**で、ズレているのは DB 側。

### 検出（実テーブルと定義の差分を見る）
```powershell
$cs="Provider=OraOLEDB.Oracle;Data Source=localhost:1521/FREEPDB1;User ID=TEST_USER;Password=TEST_PWD"
$c=New-Object System.Data.OleDb.OleDbConnection $cs; $c.Open()
$cmd=$c.CreateCommand()
$cmd.CommandText="SELECT COLUMN_NAME FROM USER_TAB_COLUMNS WHERE TABLE_NAME='AGREE' ORDER BY COLUMN_ID"
$r=$cmd.ExecuteReader(); while($r.Read()){$r.GetString(0)}; $r.Close(); $c.Close()
```
出力を `docs/test_db_schema.sql` の `CREATE TABLE AGREE (...)` と突き合わせ、欠けている列を特定する。

### 補正（テストもスキーマ定義も変えない。DB を定義に合わせる）
欠けている列だけを追加（TEST_USER 権限で可。OS 管理者権限は不要）:
```sql
ALTER TABLE AGREE ADD (
  SAVE_DATE   NUMBER(8),
  ANES        VARCHAR2(2000 CHAR),
  OPE         VARCHAR2(2000 CHAR),
  EXPLANATION VARCHAR2(2000 CHAR),
  ITEM1       VARCHAR2(2000 CHAR)
);
```
※ 列が大きくズレている/初期データを作り直したい場合は、該当表を `DROP TABLE ... CASCADE CONSTRAINTS`
してから `docs/test_db_schema.sql` を再投入する方が確実。

---

## 仕上げ: テスト実行（新しいシェルで）

```cmd
"C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe" Agree.Tests\bin\Debug\net48\Agree.Tests.dll /Settings:Agree.Tests\x86.runsettings
```
`成功: 4` になれば完了。`x86.runsettings`（テストホストを x86 固定）とテストコードは変更不要。

---

## チェックリスト（次に困ったらここだけ見る）
- [ ] `C:\oracle\odac32\oci.dll` と `...\bin\OraOLEDB.dll` が存在するか
- [ ] **システム PATH** に `C:\oracle\odac32` と `\bin` があるか（無いと登録も実行時ロードも失敗）
- [ ] 管理者 PowerShell で `register_oledb32.ps1` を実行し `regsvr32 exit=0` か
- [ ] 32bit PowerShell で `OleDbConnection.Open()` が通るか
- [ ] vstest は **新しいシェル / VS 再起動後**に実行したか
- [ ] `ORA-00904` が出たら `USER_TAB_COLUMNS` で実列を確認し `test_db_schema.sql` に合わせて `ALTER`
