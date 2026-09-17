# CRUD マトリクス

画面操作ごとに、どのテーブル・ファイルを作成（C）・参照（R）・更新（U）・削除（D）するかをまとめる。
マスタは `テーブル名 + Env.DB_LINK` で参照する。

## 1. テーブル × 機能

凡例：C = INSERT、R = SELECT、U = UPDATE、D = DELETE（物理削除）、U(論) = 論理削除（`DELETE_FLAG = 1` への UPDATE）、`-` = 操作なし

| 画面 | 機能・イベント | AGREE | AGREE_TEMPLATE | AGREE_STAFF | M_PATIENT | M_DEPT | M_USR | 実装 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Form1 | 起動（診療科コンボの作成・接続確認） | - | - | - | - | R | - | `Form1` コンストラクタ |
| Form1 | 起動時の Pat.csv 読込 → 一覧表示 | R | - | - | - | R | R | `readPatCsv` → `showList` |
| Form1 | 患者 ID を入力して Enter（一覧表示） | R | - | - | R ※1 | R | R | `showList` |
| Form1 | 一覧の行を選択（内容表示） | - | - | - | - | - | - | `showAgree`（取得済みの一覧データを使う） |
| Form1 | 新規作成 | C/U ※2 | - | R | - | - | - | `newAgreeButton_Click` → `applyDoctorFromPatCsv` → `getStaffRoom` |
| Form1 | 入力者 ID を入力して確定 | - | - | R | - | - | R | `dr_id_Leave` → `Db.StaffName` / `getStaffRoom` |
| Form1 | 登録 | C または U | - | - | - | R | R | `regAgree`（ID が無ければ C、あれば U）→ `showList` |
| Form1 | 削除 | U(論) | - | - | - | R | R | `delAgree` → `showList` |
| Form1 | コピーして作成（右クリック） | C, R | - | - | - | R | R | `copyAsNew`（`regAgree` → `max(AGREE_ID)` → `showList`） |
| Form1 | 印刷 | - | - | - | - | - | - | `printAgree`（画面の値だけを使う。DB は読まない） |
| Form1 | CSV エクスポート | R | R | R | - | - | - | `ExportTableToCsv`（`select *`） |
| Form1 | CSV インポート | R, C/U | R, C/U | R, C/U | - | R | R | `ImportTable` → `MergeRow`、`ResyncSequence`、`showList` |
| TmpAgree | 画面を開く（分類一覧・ツリー） | - | R | - | - | - | - | `TmpAgree_Load` → `loadParents` / `initTree` |
| TmpAgree | ノードを選択 | - | R | - | - | - | - | `showTemplate` |
| TmpAgree | 分類・テンプレートの登録 | - | C または U, R | - | - | - | - | `regAgreeTemplate` → `loadParents` / `initTree` |
| TmpAgree | 削除 | - | R, U(論) | - | - | - | - | `delAgreeTemplate`（分類は子の件数を確認） |
| TmpAgree | 上へ・下へ（並び替え） | - | U, R | - | - | - | - | `moveNode`（兄弟全件の `DISP_ORDER` を更新） |
| TmpAgree | 適用（Form1 へ差し込み） | - | R | - | - | - | - | `Form1.applyTemplate` |
| TmpStaff | 画面を開く（一覧） | - | - | R | - | - | R | `initList` |
| TmpStaff | 入力者 ID を入力して確定 | - | - | - | - | - | R | `staff_id_Leave` → `Db.StaffName` |
| TmpStaff | 登録 | - | - | C または U, R | - | - | R | `saveButton_Click` → `initList` |
| TmpStaff | 削除 | - | - | D, R | - | - | R | `deleteButton_Click` → `initList` |

※1 `M_PATIENT` は、入力した患者 ID が Pat.csv の患者 ID と違うときだけ参照する。
※2 入力中の同意書があり、「保存しますか？」で「はい」を選んだときだけ `regAgree` を実行する。

**オフラインモード**のときは、`Form1` の一覧・登録・削除・コピー・CSV 入出力・テンプレート差し込みと、`TmpAgree` / `TmpStaff` の DB 操作を行わない。印刷ボタンは一覧の行を選んだときに有効になるため、オフラインでは実質使えない。

## 2. DDL・シーケンス操作

| 機能 | 対象 | 操作 | 実装 |
| --- | --- | --- | --- |
| 同意書・テンプレート・担当者の新規登録 | `AGREE_SEQ` / `AGREE_TEMPLATE_SEQ` / `AGREE_STAFF_SEQ` | `nextval` | `regAgree` / `regAgreeTemplate` / `saveButton_Click` |
| CSV インポート後の再同期 | 同上 | `nextval` の取得と `ALTER SEQUENCE ... INCREMENT BY`（進めるだけで、戻さない） | `ResyncSequence` |

`ALTER SEQUENCE` を実行するため、CSV インポートには、接続ユーザーにシーケンスの変更権限が必要。

## 3. ファイル × 機能

| 機能 | Pat.csv | EyeAgreeSettings.ini | AgentlabUtilityLibrary.ini | EyeAgree.xlsm（テンプレート） | 出力 xlsm（%TEMP%） | バーコード PNG（%TEMP%） | バックアップ CSV | ログ |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 起動 | R | R | R | - | - | - | - | C（古いファイルは D） |
| 新規作成・コピーして作成 | R | - | - | - | - | - | - | - |
| 印刷 | - | R | R（`AGENT_HOME`） | R | C | C → D | - | 異常時 C |
| CSV エクスポート | - | - | - | - | - | - | C（上書き） | 異常時 C |
| CSV インポート | - | - | - | - | - | - | R | 異常時 C |

## 4. 気づいた点（仕様の確認候補）

- 同意書の削除は論理削除だが、`AGREE_STAFF` の削除は物理削除で、方針が揃っていない。
- CSV インポートは `DELETE_FLAG` を含めて上書きする。CSV に無い行は削除されない（マージのみ）。
- `copyAsNew` は登録のあと `max(AGREE_ID)` で新しい ID を取り直す。同じ患者を同時に登録すると、別の行を開く可能性がある。
