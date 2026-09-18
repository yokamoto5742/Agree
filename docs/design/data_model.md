# データモデル（ER 図・テーブル定義）

> **注意：本書はソースコードの SQL 文から推定したもの。** 本番の DDL は入手できていない。
> - 型・桁数・NULL 制約は「不明」とし、コード上の扱い（数値として連結しているか、文字列リテラルにしているか）だけを書く。
> - 主キー・参照関係は、SQL の WHERE・JOIN 条件とシーケンスの使い方からの推定。DB 上の外部キー制約があるかはわからない。
> - テスト用の推定 DDL は [../test_db_schema.sql](../test_db_schema.sql) にある（型はそちらでも推測）。

## 1. テーブルの分類

| 区分 | テーブル | 所有 | 参照方法 | アプリの操作 |
| --- | --- | --- | --- | --- |
| トランザクション | `AGREE` | アプリ | スキーマ直下 | 登録・更新・論理削除 |
| アプリ用マスタ | `AGREE_TEMPLATE` | アプリ | スキーマ直下 | 登録・更新・論理削除・並び替え |
| アプリ用マスタ | `AGREE_STAFF` | アプリ | スキーマ直下 | 登録・更新・物理削除 |
| 外部マスタ | `M_PATIENT` | 電子カルテ | `テーブル名 + Env.DB_LINK` | 参照のみ |
| 外部マスタ | `M_DEPT` | 電子カルテ | `テーブル名 + Env.DB_LINK` | 参照のみ |
| 外部マスタ | `M_USR` | 電子カルテ | `テーブル名 + Env.DB_LINK` | 参照のみ |

シーケンス：`AGREE_SEQ`、`AGREE_TEMPLATE_SEQ`、`AGREE_STAFF_SEQ`（INSERT 時の `nextval` と、CSV インポート後の再同期で使う）。

## 2. ER 図

```mermaid
erDiagram
    M_PATIENT ||--o{ AGREE : "P_ID = PATIENT_ID"
    M_DEPT ||--o{ AGREE : "CODE = DEPT"
    M_USR |o--o{ AGREE : "CODE = DR"
    M_USR ||--o{ AGREE_STAFF : "CODE = STAFF"
    AGREE_TEMPLATE |o--o{ AGREE_TEMPLATE : "TEMP_ID = TEMP_PARENT"

    AGREE {
        num AGREE_ID PK
        num PATIENT_ID "論理FK M_PATIENT"
        num SAVE_DATE "yyyyMMdd"
        num DEPT "論理FK M_DEPT"
        num DR "論理FK M_USR"
        str STAFF
        str EYE
        str DIAG
        str ANES
        str OPE
        str EXPLANATION
        str ITEM1
        str ITEM2
        str ITEM3
        str ITEM4
        str SHEET_NAME
        num DR_OK "0/1"
        num DELETE_FLAG "0/1"
        num SAVE_TIME "HHmmss"
    }
    AGREE_TEMPLATE {
        num TEMP_ID PK
        num TEMP_LEVEL "0=分類 1=テンプレート"
        num TEMP_PARENT "分類のTEMP_ID"
        str TEMP_NAME
        str EYE
        str DIAG
        str ANES
        str OPE
        str EXPLANATION
        str ITEM1
        str ITEM2
        str ITEM3
        str ITEM4
        str SHEET_NAME
        num DELETE_FLAG "0/1"
        num DISP_ORDER
    }
    AGREE_STAFF {
        num ID PK
        num STAFF "論理FK M_USR"
        str CONT
    }
    M_PATIENT {
        num P_ID PK
        str P_NAME
        str P_KANA
        str P_SEX "2=女"
    }
    M_DEPT {
        num CODE PK
        str S_NAME
    }
    M_USR {
        num CODE PK
        str NAME
    }
```

型の `num` / `str` は、コードが値を数値として連結しているか、文字列リテラル（`AgreeSql.SqlValue`）にしているかを表す。DB 上の型ではない。

**関係についての補足**

- 同意書側のテーブルと電子カルテのマスタは SQL では JOIN せず、`Ehr.JoinName` で名称列を C# 側で付ける（別の DB に置けるようにするため）。結果は以下の結合と同じ。
- `AGREE` → `M_DEPT` は一覧取得で **INNER JOIN** 相当。`M_DEPT` に無い診療科コードの同意書は、一覧に出ない。診療科名は起動時に読んだ一覧から付ける。
- `AGREE` → `M_USR` は **LEFT JOIN** 相当。職員マスタに無い入力者（Pat.csv 由来のコードなど）でも一覧に出る（氏名は空）。
- `AGREE_STAFF` → `M_USR` は `TmpStaff` の一覧で **INNER JOIN** 相当。`M_USR` に無い `STAFF` の行は一覧に出ない。
- `AGREE_TEMPLATE` と `AGREE` の間に参照関係は無い。テンプレートの値は `Form1.applyTemplate` で画面に**コピー（追記）**され、そのあと `AGREE` に保存される。
- `AGREE_STAFF.STAFF` は 1 医師 1 行を想定している（`Form1.getStaffRoom` は `ExecuteScalar` で先頭行だけ使う）。一意制約があるかは不明。

## 3. テーブル定義

各表の列「型（コード上）」は推定。桁数・NULL 制約は、すべて不明。

### 3.1 AGREE（同意書）

| 物理名 | 論理名 | 型（コード上） | キー | 値の出どころ・ルール | 参照箇所 |
| --- | --- | --- | --- | --- | --- |
| `AGREE_ID` | 同意書 ID | 数値 | PK（推定） | `AGREE_SEQ.nextval` | `Form1.regAgree` |
| `PATIENT_ID` | 患者 ID | 数値 | 論理 FK | 画面の患者 ID（整数チェック済み）。UPDATE では変更しない | `Form1.regAgree` / `showList` |
| `SAVE_DATE` | 作成日 | 数値 | | 日付ピッカーの値を `yyyyMMdd` | `Form1.regAgree` |
| `DEPT` | 診療科コード | 数値 | 論理 FK | 1〜20 の範囲だけ受け付ける（`tryParseDeptCode`） | `Form1.regAgree` |
| `DR` | 入力者（医師）コード | 数値 | 論理 FK | 画面の入力者 ID（整数チェック済み） | `Form1.regAgree` |
| `STAFF` | 担当者 | 文字列 | | 自由記述。`AGREE_STAFF.CONT` から自動で入ることがある | `Form1.getStaffRoom` |
| `EYE` | 眼 | 文字列 | | 候補：右 / 左 / 両（自由入力も可） | |
| `DIAG` | 病名 | 文字列 | | 自由記述 | |
| `ANES` | 麻酔の形式 | 文字列 | | 自由記述 | |
| `OPE` | 手術・検査名 | 文字列 | | 自由記述 | |
| `EXPLANATION` | 説明 | 文字列 | | 自由記述 | |
| `ITEM1` | 症状 | 文字列 | | 自由記述 | |
| `ITEM2` | 治療計画 | 文字列 | | 自由記述 | |
| `ITEM3` | 検査内容 | 文字列 | | 画面では非表示。保存・テンプレート差し込みはされるが、**印刷には出力しない** | `Form1.Designer.cs` |
| `ITEM4` | 手術内容 | 文字列 | | 自由記述 | |
| `SHEET_NAME` | 帳票シート名 | 文字列 | | 候補：通常 / 短期滞在 / 注射 / 検査同意書 | `ExcelControl.resolveSheetName` |
| `DR_OK` | 医師完了フラグ | 数値 | | 1/0。新規・コピーでは 1。1 のとき登録後に印刷を促す | `Form1.regAgreeButton_Click` |
| `DELETE_FLAG` | 削除フラグ | 数値 | | INSERT で 0、削除で 1（論理削除）。一覧は 0 だけ | `Form1.delAgree` |
| `SAVE_TIME` | 保存時刻 | 数値 | | 登録時刻 `HHmmss`（INSERT・UPDATE とも更新） | `Form1.regAgree` |

### 3.2 AGREE_TEMPLATE（同意書テンプレート）

分類（`TEMP_LEVEL = 0`）とテンプレート（`TEMP_LEVEL = 1`）の 2 階層ツリー。

| 物理名 | 論理名 | 型（コード上） | キー | 値の出どころ・ルール | 参照箇所 |
| --- | --- | --- | --- | --- | --- |
| `TEMP_ID` | テンプレート ID | 数値 | PK（推定） | `AGREE_TEMPLATE_SEQ.nextval` | `TmpAgree.regAgreeTemplate` |
| `TEMP_LEVEL` | 階層 | 数値 | | 0 = 分類、1 = テンプレート | |
| `TEMP_PARENT` | 親 ID | 数値 | 自己参照 | 分類は 0、テンプレートは分類の `TEMP_ID` | |
| `TEMP_NAME` | 名称 | 文字列 | | 必須（画面でチェック）。分類名は重複不可（画面でチェック） | |
| `EYE`〜`ITEM4` | 各項目 | 文字列 | | `AGREE` の同名列と同じ意味。分類行では NULL | |
| `SHEET_NAME` | 帳票シート名 | 文字列 | | 同上 | |
| `DELETE_FLAG` | 削除フラグ | 数値 | | INSERT で 0、削除で 1。子テンプレートのある分類は削除不可 | `TmpAgree.delAgreeTemplate` |
| `DISP_ORDER` | 表示順 | 数値 | | 並び替えのとき、兄弟ノードに 0 から振り直す。INSERT では設定しない（NULL） | `TmpAgree.moveNode` |

### 3.3 AGREE_STAFF（担当者文例）

| 物理名 | 論理名 | 型（コード上） | キー | 値の出どころ・ルール | 参照箇所 |
| --- | --- | --- | --- | --- | --- |
| `ID` | ID | 数値 | PK（推定） | `AGREE_STAFF_SEQ.nextval` | `TmpStaff.saveButton_Click` |
| `STAFF` | 入力者（医師）コード | 数値 | 論理 FK | `M_USR` に存在するコードだけ入力できる（画面でチェック） | `TmpStaff.staff_id_Leave` |
| `CONT` | 担当者文例 | 文字列 | | 前後の空白を除いて保存。`AGREE.STAFF` の初期値になる | `Form1.getStaffRoom` |

### 3.4 外部マスタ（参照する列だけ）

| テーブル | 列 | 論理名 | 用途 | 参照箇所 |
| --- | --- | --- | --- | --- |
| `M_PATIENT` | `P_ID` | 患者 ID | 検索キー | `Form1.showList` |
| | `P_NAME` / `P_KANA` | 氏名 / カナ | 画面表示・印刷 | 〃 |
| | `P_SEX` | 性別 | `2` → 女、それ以外 → 男 | 〃 |
| `M_DEPT` | `CODE` | 診療科コード | コンボの値・結合キー（`0` はコンボに出さない） | `Form1` コンストラクタ / `showList` |
| | `S_NAME` | 診療科略称 | 表示・印刷（`Trim` して使う） | 〃 |
| `M_USR` | `CODE` | 職員コード | 入力者の存在チェック・結合キー | `Ehr.StaffName` / `showList` / `TmpStaff.initList` |
| | `NAME` | 職員氏名 | 表示・印刷（`Trim` して使う） | 〃 |

> [../test_db_schema.sql](../test_db_schema.sql) には `M_DR`・`M_SYOZOKU`・`M_SHIKAKU`・`M_SHINKU`・`M_SEKOU` もある。
> 現在の Agree のコードはこれらを参照していない（以前の `Dict.InitDict` 用の名残と思われる）。

## 4. コード値

| 項目 | 値 | 意味 |
| --- | --- | --- |
| `AGREE.DELETE_FLAG` / `AGREE_TEMPLATE.DELETE_FLAG` | 0 / 1 | 有効 / 削除済み |
| `AGREE.DR_OK` | 0 / 1 | 未完了 / 医師完了 |
| `AGREE_TEMPLATE.TEMP_LEVEL` | 0 / 1 | 分類 / テンプレート |
| `M_PATIENT.P_SEX`、Pat.csv の性別 | 1 / 2 | 男 / 女（`M_PATIENT` は 2 以外を男、Pat.csv は 1・2 以外を空として扱う） |
| `SHEET_NAME` | 通常・短期滞在・注射・検査同意書（ほかに「入院」「日帰り」は「通常」扱い） | 帳票テンプレートのシート名 |
