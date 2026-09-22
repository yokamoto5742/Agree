-- =====================================================================
-- 眼科同意書システム テスト用スキーマ作成スクリプト
-- 実行: sqlplus TEST_USER/TEST_PWD@localhost:1521/FREEPDB1 @test_db_schema.sql
--
-- 前提: 設定ファイルの DB_LINK を「空」にすること。
--       本アプリは M_xxx マスタを "テーブル名 + DB_LINK" で参照するため、
--       DB_LINK が空ならローカル(TEST_USER)の同名テーブルを直接参照する。
--
-- 型・桁数の出どころ:
--   AGREE / AGREE_TEMPLATE / AGREE_STAFF … 本番DBの実測値（docs/schema_open.txt、git管理外。
--     DumpSchema で ALL_TAB_COLUMNS を取得したもの。列順も本番に合わせてある）。
--   M_xxx マスタ … 本番DBの実測値（docs/schema_ehr.txt、git管理外。DBリンク @inno.world
--     越しに ALL_TAB_COLUMNS を取得。本番のオーナーは MEDB）。ただし全列ではなく、
--     アプリ・本スクリプト群が使う列と NOT NULL 列だけを本番の列順で再現している。
--
-- 実測して分かったこと・注意:
--   * 同意書側(OPEN)のキャラクタセットは JA16SJISTILDE（全角 2byte/字）。
--     VARCHAR2 はすべて BYTE セマンティクス（CHAR_LENGTH と DATA_LENGTH が一致）。
--     本スクリプトも BYTE で宣言し、本番の物理的な上限を再現する。
--     ※ ローカルのテストDBが AL32UTF8 だと全角は 3byte/字になり、本番(2byte/字)より
--       少ない文字数で ORA-12899 になる。
--   * 電子カルテ側(EHR)のキャラクタセットは AL32UTF8。マスタの文字列列はすべて
--     NVARCHAR2（桁数は文字数）。
--   * マスタには NOT NULL の REG_USR / REG_DATE / REG_TIME がある（登録者・登録日時）。
--     マスタへ INSERT するときは必ず値を指定すること。
--   * 画面側の TextBox.MaxLength は「文字数」なので、全角入力では本番列を超えうる
--     （例: Form1 の diag は MaxLength=200 だが DIAG は 100 BYTE）。ORA-12899 を
--     再現できるよう、桁数は本番どおりに保つこと。
--   * 本アプリが参照しない列（AGREE.RESERVE1-4 / AGREE.SAVE_STAFF /
--     AGREE_TEMPLATE.RESERVER1-3）も本番に存在するため、そのまま再現している。
--     ※ AGREE 側は RESERVE、AGREE_TEMPLATE 側は RESERVER と綴りが違う（本番のまま）。
--   * DumpSchema は列定義しか取らないため、主キー・索引・DEFAULT・チェック制約は
--     未実測。以下の PRIMARY KEY と DEFAULT はテスト用の付加であり本番の再現ではない。
-- =====================================================================

-- ---------------------------------------------------------------------
-- 1. アプリ専用テーブル（本アプリが INSERT/UPDATE する）
--    列・型・NULL 制約・列順は本番実測（docs/schema_open.txt）。
-- ---------------------------------------------------------------------

-- 同意書本体（Form1.regAgree / delAgree / showList）
CREATE TABLE AGREE (
    AGREE_ID     NUMBER               NOT NULL,   -- AGREE_SEQ.nextval
    PATIENT_ID   NUMBER(9,0)          NOT NULL,   -- M_PATIENT.P_ID に対応
    DEPT         NUMBER(3,0)          NOT NULL,   -- 診療科 → M_DEPT.CODE
    DR           NUMBER(5,0)          NOT NULL,   -- 医師   → M_USR.CODE
    STAFF        VARCHAR2(60 BYTE),               -- 担当者(文字列で格納)
    EYE          VARCHAR2(20 BYTE),               -- 左右眼
    DIAG         VARCHAR2(100 BYTE),              -- 病名
    OPE          VARCHAR2(100 BYTE),              -- 術式
    EXPLANATION  VARCHAR2(1200 BYTE),             -- 説明
    ITEM1        VARCHAR2(200 BYTE),              -- 症状
    ITEM2        VARCHAR2(200 BYTE),              -- 治療計画
    ITEM3        VARCHAR2(200 BYTE),              -- 検査内容
    ITEM4        VARCHAR2(500 BYTE),              -- 手術内容
    RESERVE1     VARCHAR2(200 BYTE),              -- 予備列。アプリは未参照
    RESERVE2     VARCHAR2(200 BYTE),              -- 〃
    RESERVE3     VARCHAR2(200 BYTE),              -- 〃
    RESERVE4     VARCHAR2(200 BYTE),              -- 〃
    SHEET_NAME   VARCHAR2(50 BYTE),               -- 帳票シート名
    DR_OK        NUMBER(1,0)     DEFAULT 0,       -- 医師完了 1/0（DEFAULT は未実測）
    DELETE_FLAG  NUMBER(1,0)     DEFAULT 0,       -- 論理削除 1/0（DEFAULT は未実測）
    SAVE_STAFF   NUMBER(5,0),                     -- 保存者。アプリは未参照
    SAVE_DATE    NUMBER(8,0)          NOT NULL,   -- 作成日 yyyymmdd
    SAVE_TIME    NUMBER(6,0)          NOT NULL,   -- 保存時刻 HHmmss
    ANES         VARCHAR2(100 BYTE),              -- 麻酔
    CONSTRAINT PK_AGREE PRIMARY KEY (AGREE_ID)    -- 本番の制約は未実測
);

-- 同意書テンプレート（TmpAgree。親子ツリー構造: TEMP_PARENT で親を参照）
CREATE TABLE AGREE_TEMPLATE (
    TEMP_ID      NUMBER               NOT NULL,   -- AGREE_TEMPLATE_SEQ.nextval
    TEMP_LEVEL   NUMBER(1,0)          NOT NULL,   -- 階層レベル(0=分類,1=テンプレート)
    TEMP_PARENT  NUMBER               NOT NULL,   -- 親 TEMP_ID（分類は 0）
    TEMP_NAME    VARCHAR2(40 BYTE)    NOT NULL,
    EYE          VARCHAR2(20 BYTE),
    DIAG         VARCHAR2(100 BYTE),
    OPE          VARCHAR2(100 BYTE),
    EXPLANATION  VARCHAR2(1200 BYTE),
    ITEM1        VARCHAR2(200 BYTE),
    ITEM2        VARCHAR2(200 BYTE),
    ITEM3        VARCHAR2(200 BYTE),
    ITEM4        VARCHAR2(500 BYTE),
    RESERVER1    VARCHAR2(200 BYTE),              -- 予備列。アプリは未参照（綴りは本番のまま）
    RESERVER2    VARCHAR2(200 BYTE),              -- 〃
    RESERVER3    VARCHAR2(200 BYTE),              -- 〃
    DISP_ORDER   NUMBER(2,0),                     -- 表示順（0〜99 まで）
    SHEET_NAME   VARCHAR2(50 BYTE),
    DELETE_FLAG  NUMBER(1,0)     DEFAULT 0,       -- （DEFAULT は未実測）
    ANES         VARCHAR2(100 BYTE),
    CONSTRAINT PK_AGREE_TEMPLATE PRIMARY KEY (TEMP_ID)  -- 本番の制約は未実測
);

-- 担当医ごとの定型説明文（TmpStaff。STAFF は数値の医師コード → M_USR.CODE）
CREATE TABLE AGREE_STAFF (
    ID           NUMBER               NOT NULL,   -- AGREE_STAFF_SEQ.nextval
    STAFF        NUMBER(5,0)          NOT NULL,   -- 医師コード → M_USR.CODE
    CONT         VARCHAR2(60 BYTE),               -- 説明文
    CONSTRAINT PK_AGREE_STAFF PRIMARY KEY (ID)    -- 本番の制約は未実測
);

-- シーケンス（INSERT 文で xxx_SEQ.nextval を使用。本番の現在値は未実測）
CREATE SEQUENCE AGREE_SEQ          START WITH 1 INCREMENT BY 1 NOCACHE;
CREATE SEQUENCE AGREE_TEMPLATE_SEQ START WITH 1 INCREMENT BY 1 NOCACHE;
CREATE SEQUENCE AGREE_STAFF_SEQ    START WITH 1 INCREMENT BY 1 NOCACHE;


-- ---------------------------------------------------------------------
-- 2. 電子カルテ側マスタ（本来は DB_LINK 経由。DB_LINK を空にしローカルで代替）
--    型・NULL 制約・列順は本番実測（docs/schema_ehr.txt、本番のオーナーは MEDB）。
--    再現しているのは、アプリ・本スクリプト群が使う列と NOT NULL 列だけ。
--    アプリが参照するのは M_PATIENT / M_DEPT / M_USR の 3 表のみ。
--    起動時に M_DEPT を SELECT し、失敗するとオフラインモードに落ちる。
--    M_DR は現在のアプリは参照しないが、test_db_seed.sql / fix_corrupt_master_data.sql
--    が使うため残している。
--    PRIMARY KEY は未実測（テスト用の付加）。
-- ---------------------------------------------------------------------

-- 患者マスタ（Ehr.FindPatient: P_NAME,P_KANA,P_SEX を P_ID で取得）
CREATE TABLE M_PATIENT (
    P_ID      NUMBER(9,0)          NOT NULL,
    P_KANA    NVARCHAR2(40),
    P_NAME    NVARCHAR2(40),
    P_SEX     NUMBER(1,0),                        -- "2"=女, それ以外=男
    REG_USR   NUMBER(5,0)          NOT NULL,      -- 登録者。アプリは未参照
    REG_DATE  NUMBER(8,0)          NOT NULL,      -- 登録日 yyyymmdd。アプリは未参照
    REG_TIME  NUMBER(6,0)          NOT NULL,      -- 登録時刻 HHmmss。アプリは未参照
    CONSTRAINT PK_M_PATIENT PRIMARY KEY (P_ID)
);

-- 診療科マスタ（Ehr.LoadDepartments: CODE, S_NAME 参照）
CREATE TABLE M_DEPT (
    CODE      NUMBER(5,0)          NOT NULL,
    NAME      NVARCHAR2(50),
    S_NAME    NVARCHAR2(50),                      -- 略称
    REG_USR   NUMBER(5,0)          NOT NULL,
    REG_DATE  NUMBER(8,0)          NOT NULL,
    REG_TIME  NUMBER(6,0)          NOT NULL,
    CONSTRAINT PK_M_DEPT PRIMARY KEY (CODE)
);

-- 職員マスタ（Ehr.StaffName / StaffNames: CODE, NAME 参照。KANA は fix_corrupt_master_data.sql が使用）
CREATE TABLE M_USR (
    CODE      NUMBER(5,0)          NOT NULL,
    KANA      NVARCHAR2(40),
    NAME      NVARCHAR2(40),
    REG_USR   NUMBER(5,0)          NOT NULL,
    REG_DATE  NUMBER(8,0)          NOT NULL,
    REG_TIME  NUMBER(6,0)          NOT NULL,
    CONSTRAINT PK_M_USR PRIMARY KEY (CODE)
);

-- 医師マスタ（現在のアプリは未参照。test_db_seed.sql / fix_corrupt_master_data.sql が使用）
CREATE TABLE M_DR (
    CODE      NUMBER(3,0)          NOT NULL,      -- 3 桁（M_USR.CODE は 5 桁）
    NAME      NVARCHAR2(50),
    REG_USR   NUMBER(5,0)          NOT NULL,
    REG_DATE  NUMBER(8,0)          NOT NULL,
    REG_TIME  NUMBER(6,0)          NOT NULL,
    CONSTRAINT PK_M_DR PRIMARY KEY (CODE)
);


-- ---------------------------------------------------------------------
-- 3. 起動と最小動作に必要な初期データ（例）
--    マスタが空でも起動はするが、
--    診療科コンボや一覧結合(INNER JOIN)のため最低限のマスタを投入しておく。
--    REG_USR / REG_DATE / REG_TIME はテスト用の固定値。
-- ---------------------------------------------------------------------
INSERT INTO M_DEPT (CODE, NAME, S_NAME, REG_USR, REG_DATE, REG_TIME)
       VALUES (1, '眼科', '眼科', 0, 20260101, 0);
INSERT INTO M_USR  (CODE, KANA, NAME, REG_USR, REG_DATE, REG_TIME)
       VALUES (101, 'テストイシ', 'テスト医師', 0, 20260101, 0);
INSERT INTO M_DR   (CODE, NAME, REG_USR, REG_DATE, REG_TIME)
       VALUES (101, 'テスト医師', 0, 20260101, 0);
INSERT INTO M_PATIENT (P_ID, P_KANA, P_NAME, P_SEX, REG_USR, REG_DATE, REG_TIME)
       VALUES (1, 'テストカンジャ', 'テスト患者', 1, 0, 20260101, 0);

COMMIT;

