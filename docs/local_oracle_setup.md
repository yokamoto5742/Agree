# ローカル Oracle Free デバッグ環境 セットアップ手順

ローカル端末で Agree アプリをデバッグ実行（F5）した際、本番DBではなく
ローカルの Oracle Database Free に対して SQL の読み書きを行うための構成。
本書は、本リポジトリに加えた変更と、各端末で必要な作業をまとめる。

---

## 0. 前提（各端末で必要）

- Oracle Database Free が起動済み（CDB `localhost:1521`、PDB `FREEPDB1`）。
- **32bit 版 ODAC（`OraOLEDB.Oracle`）** がインストール済み。
  本アプリは x86 ビルドのため、64bit 版だけでは動かない。
  確認: `C:\Windows\SysWOW64\rundll32.exe "C:\Program Files\Common Files\System\Ole DB\oledb32.dll",OpenDSLFile <udlのパス>`
  で「Oracle Provider for OLE DB」が一覧に出ること。
- テスト用ユーザー `TEST_USER` / パスワード `TEST_PWD`（`FREEPDB1` 内）。

---

## 1. テスト用スキーマの投入

```cmd
sqlplus TEST_USER/TEST_PWD@localhost:1521/FREEPDB1 @docs\test_db_schema.sql
```

`AGREE` / `AGREE_TEMPLATE` / `AGREE_STAFF` と、起動時 `Dict.InitDict()` が読む
7つのマスタ（`M_DEPT` 等）＋最小初期データが作成される。マスタが読めないと
アプリは起動時に**オフラインモード**へ落ち、登録・削除ができなくなる。

> `DB_LINK` を空にしているため、本アプリは `M_xxx` を「テーブル名 + DB_LINK」=
> ローカル同名表として直接参照する（`docs/test_db_schema.sql` 冒頭の注記参照）。

---

## 2. 接続設定（ini）

接続先・プロバイダは暗号化 ini（`Env` が読む）で切り替える。本番値は触らない。

- **プロバイダは ini の `PROVIDER` キーで切替**。未指定なら従来どおり `MSDAORA.1`
  （＝本番構成を壊さない）。ローカルでは `OraOLEDB.Oracle` を指定する。
- ローカル用テンプレート: リポジトリ直下 `AgentlabUtilityLibrary.local.ini`
  - `OPEN_DB` = `localhost:1521/FREEPDB1`（暗号化済み）
  - `OPEN_USER` = `TEST_USER` / `OPEN_PWD` = `TEST_PWD`（暗号化済み）
  - `DB_LINK` = 空
  - `PROVIDER` = `OraOLEDB.Oracle`（**平文**。秘密ではないため復号しない）

`Env` の読み込み順は「①カレントディレクトリ → ②`C:\macs\utility\AgentlabUtilityLibrary.ini`」。
リビルドで `bin` が消えても効くよう、**②に配置するのが確実**:

```cmd
copy AgentlabUtilityLibrary.local.ini C:\macs\utility\AgentlabUtilityLibrary.ini
```

（暗号値を作り直す場合は `python docs\enc_gen.py`。`Enc` の置換表と同一実装。）

---

## 3. AgentlabUtilityLibrary.dll の再ビルドと差し替え

`DBConn` / `DB` / `Env` は外部DLL `AgentlabUtilityLibrary.dll`（リポジトリ直下を
`Agree.csproj` が HintPath 参照）に含まれる。プロバイダ切替を反映するには再ビルドが必要。

加えた変更（`util_project/`）:
- `Env.cs`: `PROVIDER` プロパティを追加（既定 `MSDAORA.1`・平文・ini から読む）。
- `DBConn.cs` / `DB.cs`: 接続文字列の `Provider=` を `Env.PROVIDER` 経由に変更（4箇所）。
- `AgentlabUtilityLibrary.csproj`: 欠落参照 `InnoUketsukeLib` を除去し、`net48` 化、
  非文字列 resx 用に `GenerateResourceUsePreserializedResources` ＋ `System.Resources.Extensions` を追加。
- `LoginPrompt.cs`: `InnoUketsukeLib` 依存の2行（`AppInit` / `M_USR.g_Usr1`）を無効化
  （Agree アプリはこのログイン経路を使わない）。

再ビルド手順:

```cmd
cd util_project
dotnet build AgentlabUtilityLibrary.csproj -c Debug
copy bin\Debug\net48\AgentlabUtilityLibrary.dll        ..\AgentlabUtilityLibrary.dll
copy bin\Debug\net48\System.Resources.Extensions.dll   ..\System.Resources.Extensions.dll
```

> 元の本番DLL（2018年製）は `AgentlabUtilityLibrary.dll.orig` に退避済み。
> `System.Resources.Extensions.dll` は再ビルドDLLの実行時依存。Agree の出力先
> （`bin\x86\Debug`）にも配置すること（`Agree.csproj` は自動コピーしない）。

### ⚠️ 本番への混入防止

- 再ビルドDLLは**逆コンパイル再構成物**（元は net40、本書では net48・login 無効化）。
  本番ブランチへコミット／配布しないこと。本番へ戻す場合:
  ```cmd
  copy AgentlabUtilityLibrary.dll.orig AgentlabUtilityLibrary.dll
  ```
- プロバイダは ini 既定で `MSDAORA.1` のため、`PROVIDER` 行を削れば本番構成に戻る。

---

## 4. 実アプリのデバッグ実行（F5）

1. 上記 1〜3 を完了。
2. Visual Studio で `Agree.csproj` を開き、構成 `Debug` / プラットフォーム `x86` で F5。
3. 起動時に診療科コンボが埋まり、オフライン警告が出なければローカルDBに接続成功。
   登録・削除・検索がローカルの `AGREE` 表に反映される。

---

## 5. 結合テスト（アプリ非依存・DLL 再ビルド不要）

`Agree.Tests`（`net48` / **x86**）が `OraOLEDB.Oracle` で直接ローカルDBへ接続し、
`AGREE` / `AGREE_TEMPLATE` / `AGREE_STAFF` の INSERT→SELECT→UPDATE を検証する
（各テストはトランザクションをロールバックするため繰り返し実行可能）。

```cmd
dotnet test Agree.Tests\Agree.Tests.csproj -c Debug
```

- ローカルOracle未起動／スキーマ未投入のときは **失敗ではなく Ignore（スキップ）**。
- 接続先は既定 `localhost:1521/FREEPDB1`。環境変数 `AGREE_TEST_ORACLE` で上書き可能。
- x86 で実行されること（32bit `OraOLEDB` をロードするため）。Visual Studio の
  テストエクスプローラーで実行する場合もプロセスアーキテクチャを x86 にする。

---

## 既知の注意（本対応では未修正）

- `Form1.regPlan` 等は入力を文字列連結で SQL 化するため、氏名や説明に
  アポストロフィ `'` が含まれると SQL が破損する。テストで露見し得るが、
  プロジェクト規約（未依頼の変更はしない）に従い本対応の範囲外とする。
