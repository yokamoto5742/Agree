# 電子カルテ乗り換え対応計画

電子カルテを別製品へ乗り換えるときに、本アプリ（眼科同意書）で必要な作業と前提をまとめる。

## 1. 前提（2026-09-18 時点）

- 乗り換え先の製品は**未定**。DB の種類（Oracle かどうか）、患者・医師情報の受け渡し方法（`Pat.csv` と同じ形式を出力できるか）も未定。
- 同意書 DB（`AGREE` / `AGREE_TEMPLATE` / `AGREE_STAFF`）は、現在の Oracle から**すべてエクスポートして新しい DB へ移す**見込み。現在の Oracle は使えなくなる。
- 製品が決まるまで、製品に依存する改修（③〜⑤）は行わない。先回りして作っても製品の仕様に合わない可能性が高いため。

## 2. 現状の設計（①②の完了後）

```
Agree.exe
  ├─ Ehr（Agree/Ehr.cs）──▶ 電子カルテ  … DBConn.GetEhrDBConn()（INI の EHR_*）
  │    マスタ M_PATIENT / M_USR / M_DEPT、Pat.csv、性別・診療科のコード体系
  └─ 各画面 ─────────────▶ 同意書 DB    … DBConn.GetOpenDBConn()（INI の OPEN_*）
       AGREE / AGREE_TEMPLATE / AGREE_STAFF
```

- 電子カルテとの接点は `Agree/Ehr.cs` だけにある。同意書側のテーブルとマスタを SQL で JOIN せず、名称列は `Ehr.JoinName` で C# 側で付ける（結果は以前の内部結合・外部結合と同じ）。
- 接続は 2 本。INI に `EHR_*` が無ければ `OPEN_*` / `PROVIDER` と同じ接続先になる（従来と同じ動作）。
- INI のキーの一覧は [configuration.md](design/configuration.md) を参照。

### 完了した作業

| # | 内容 | コミット |
|---|---|---|
| ① | 電子カルテへのアクセスを `Ehr` クラスに集約し、マスタとの JOIN をやめた | Agree `a90e133` |
| ② | DLL に `DBConn.GetEhrDBConn()` と INI の `EHR_DB` / `EHR_USER` / `EHR_PWD` / `EHR_PROVIDER` を追加 | AgentlabUtilityLibrary `e115316` |
| ② | `Ehr` の接続を `GetEhrDBConn()` に切り替え、同梱 DLL を更新 | Agree `1b8046b` |
| － | `AgentlabUtilityLibrary` を外部 DLL から本体同梱のソース（`Agree/Infrastructure/`）に変更。以降の接続まわりの変更は Agree 側だけで完結する | Agree（本コミット） |

## 3. 製品が決まってから行う作業

### ③ 電子カルテ連携（`Ehr`）の差し替え

`Ehr` の各メソッドを、新しい製品に合わせて実装し直す。

| メソッド | 現在の実装 | 差し替えで決めること |
|---|---|---|
| `LoadDepartments` | `M_DEPT` の `CODE`, `S_NAME` | 診療科マスタの場所・列 |
| `FindPatient` | `M_PATIENT` の `P_NAME`, `P_KANA`, `P_SEX` | 患者マスタの場所・列、性別コード |
| `StaffName` / `StaffNames` | `M_USR` の `CODE`, `NAME` | 職員マスタの場所・列 |
| `ReadPatCsv` | `{LEGACY_HOME}\Pat.csv` の固定の列位置（2, 3, 5, 6, 9, 10, 13, 14, 27）、Shift-JIS | 患者・医師の受け渡し方法（CSV / 起動引数 / Web の連携 API など） |
| `TryParseDeptCode` | 診療科コード 1〜20 | 診療科のコード体系 |

- あわせて、`Ehr` を DLL 側へ移すかどうかを判断する。DLL に移せば、次に電子カルテを乗り換えるときに Agree.exe を再ビルドせずに済む。
- マスタを SQL で直接読めない製品（Web の連携 API のみなど）の場合は、`Ehr` の中身を API 呼び出しに置き換える。

### ④ バーコード形式の設定化

同意書に印字する 36 桁バーコードの形式は、電子カルテのスキャン文書取込の仕様に合わせたもので、製品によって違う可能性が高い。

- 現在の形式：患者ID 9 桁 + 文書コード 5 桁 + 診療科 3 桁 + 医師 5 桁 + 作成日 8 桁 + 時刻 6 桁（`ExcelControl`）
- 文書コードは `EyeAgreeSettings.ini` の `DOCUMENT_CODE`（既定 `39911`）
- 新しい製品の取込仕様（桁構成、文書コード、バーコード種別）に合わせて、形式を設定で変えられるようにする。

### ⑤ 同意書側 SQL の書き換え

同意書 DB を新しい DB へ移すため、同意書側の Oracle 専用の書き方を移行先の DB に合わせる。

| 箇所 | Oracle 専用の書き方 |
|---|---|
| `Form1.Agree.cs`（`regAgree`） | `AGREE_SEQ.nextval` |
| `TmpAgree.cs` | `AGREE_TEMPLATE_SEQ.nextval` |
| `TmpStaff.cs` | `AGREE_STAFF_SEQ.nextval` |
| `Form1.ImportExport.cs`（`ResyncSequence`） | `nvl(...)`、`select ... from dual`、シーケンスの再同期 |

- `Trim(...)` などの関数も、移行先の DB で使えるか確認する。
- データの移行には、既存の CSV エクスポート・インポート機能（設定ボタンから表示）が使える可能性がある。インポートのシーケンス再同期も移行先の DB に合わせる必要がある。
- 移行先が Oracle なら、この作業はほぼ不要。

## 4. 製品が決まったら確認すること

1. **DB の種類と参照方法**：Oracle かどうか。外部から DB を直接読めるか（ベンダーが禁止している場合がある）。`@` 形式の DB リンクが使えるか。
2. **患者・医師の受け渡し方法**：CSV ファイル、起動時の引数、Web の連携 API（HL7 / FHIR など）のどれか。CSV なら文字コード・区切り文字・列の並び。
3. **コード体系**：患者IDの桁数（バーコードは 9 桁固定）、医師コード（5 桁）、診療科コード（3 桁、現在は 1〜20）、性別コードの値。
4. **スキャン文書の取込仕様**：バーコードの桁構成・種別、文書コード `39911` がそのまま使えるか。
5. **マスタの参照権限**：職員・診療科のマスタを外部から参照できるか。
6. **オフライン時の動作**：DB に接続できないときの扱いを今と同じにしてよいか。
7. **同意書 DB の移行先**：どの DB に置くか、移行の手順と時期。

## 5. 配布時の注意

- 接続情報は `AgentlabUtilityLibrary.ini` からのみ読む（ini が無い場合の既定値は持たない）。**配置先に ini が必要**。
- `AgentlabUtilityLibrary.dll` は不要になった（`EyeAgree.exe` に取り込み済み）。配布物は exe だけでよい。
- 同意書 DB と電子カルテの接続先を分けた場合、起動時のオフライン判定は電子カルテ側（診療科一覧の読込）だけで行う。同意書 DB に接続できないことは、操作したときのエラーで分かる。
