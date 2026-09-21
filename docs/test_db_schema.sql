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
--   M_xxx マスタ … 未実測（電子カルテ側は取得できていない。
--     docs/dump_schema_production.md の4章参照）。従来どおり「推測」。
--
-- 実測して分かったこと・注意:
--   * VARCHAR2 はすべて BYTE セマンティクス（CHAR_LENGTH と DATA_LENGTH が一致）。
--     本スクリプトも BYTE で宣言し、本番の物理的な上限を再現する。全角で何文字
--     入るかはDBキャラクタセット依存（JA16SJIS なら 2byte/字、AL32UTF8 なら 3byte/字）。
--     本番の NLS_CHARACTERSET は未確認。
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
--    ※ 本番の定義は未取得のため、以下はすべて「推測」。
--    アプリが参照するのは M_PATIENT / M_DEPT / M_USR の 3 表のみ。
--    起動時に M_DEPT を SELECT し、失敗するとオフラインモードに落ちる。
--    M_DR / M_SYOZOKU / M_SHIKAKU / M_SHINKU / M_SEKOU は旧実装（Dict.InitDict）の
--    名残で現在のアプリは参照しないが、test_db_seed.sql / fix_corrupt_master_data.sql
--    が M_DR を使うため残している。
-- ---------------------------------------------------------------------

-- 患者マスタ（Form1.showList: P_NAME,P_KANA,P_SEX を P_ID で取得）
CREATE TABLE M_PATIENT (
    P_ID    NUMBER          NOT NULL,
    P_NAME  VARCHAR2(100 CHAR),
    P_KANA  VARCHAR2(100 CHAR),
    P_SEX   NUMBER(1),                            -- "2"=女, それ以外=男
    CONSTRAINT PK_M_PATIENT PRIMARY KEY (P_ID)
);

-- 診療科マスタ（アプリ: CODE, S_NAME 参照。Form1 コンストラクタ / showList）
CREATE TABLE M_DEPT (
    CODE    NUMBER          NOT NULL,
    NAME    VARCHAR2(100 CHAR),
    S_NAME  VARCHAR2(50 CHAR),                    -- 略称
    CONSTRAINT PK_M_DEPT PRIMARY KEY (CODE)
);

-- 職員マスタ（アプリ: CODE, NAME 参照。Ehr.StaffName / showList / TmpStaff。他の列は旧実装の名残）
CREATE TABLE M_USR (
    CODE     NUMBER         NOT NULL,
    NAME     VARCHAR2(100 CHAR),
    KANA     VARCHAR2(100 CHAR),
    SYOZOKU  NUMBER,                              -- 所属コード → M_SYOZOKU.CODE
    SHIKAKU  NUMBER,                              -- 資格コード → M_SHIKAKU.CODE
    DEPT     NUMBER,                              -- 診療科     → M_DEPT.CODE
    DR       NUMBER,                              -- 医師コード → M_DR.CODE
    CONSTRAINT PK_M_USR PRIMARY KEY (CODE)
);

-- 医師マスタ（現在のアプリは未参照。test_db_seed.sql / fix_corrupt_master_data.sql が使用）
CREATE TABLE M_DR (
    CODE      NUMBER         NOT NULL,
    NAME      VARCHAR2(100 CHAR),
    CATEGORY  NUMBER,
    VAL_4     VARCHAR2(100 CHAR),
    CONSTRAINT PK_M_DR PRIMARY KEY (CODE)
);

-- 所属マスタ（現在のアプリは未参照）
CREATE TABLE M_SYOZOKU (
    CODE      NUMBER         NOT NULL,
    NAME      VARCHAR2(100 CHAR),
    S_NAME    VARCHAR2(50 CHAR),
    CATEGORY  NUMBER,
    CONSTRAINT PK_M_SYOZOKU PRIMARY KEY (CODE)
);

-- 資格マスタ（現在のアプリは未参照）
CREATE TABLE M_SHIKAKU (
    CODE      NUMBER         NOT NULL,
    NAME      VARCHAR2(100 CHAR),
    S_NAME    VARCHAR2(50 CHAR),
    CATEGORY  NUMBER,
    CONSTRAINT PK_M_SHIKAKU PRIMARY KEY (CODE)
);

-- 診療区分マスタ（現在のアプリは未参照）
CREATE TABLE M_SHINKU (
    CODE    NUMBER          NOT NULL,
    NAME    VARCHAR2(100 CHAR),
    CONSTRAINT PK_M_SHINKU PRIMARY KEY (CODE)
);

-- 施行マスタ（現在のアプリは未参照）
CREATE TABLE M_SEKOU (
    CODE    NUMBER          NOT NULL,
    NAME    VARCHAR2(100 CHAR),
    S_NAME  VARCHAR2(50 CHAR),
    CONSTRAINT PK_M_SEKOU PRIMARY KEY (CODE)
);


-- ---------------------------------------------------------------------
-- 3. 起動と最小動作に必要な初期データ（例）
--    マスタが空でも起動はするが、
--    診療科コンボや一覧結合(INNER JOIN)のため最低限のマスタを投入しておく。
-- ---------------------------------------------------------------------
INSERT INTO M_DEPT (CODE, NAME, S_NAME) VALUES (1, '眼科', '眼科');
INSERT INTO M_USR  (CODE, NAME, KANA, SYOZOKU, SHIKAKU, DEPT, DR)
       VALUES (101, 'テスト医師', 'テストイシ', 1, 1, 1, 101);
INSERT INTO M_DR   (CODE, NAME, CATEGORY, VAL_4) VALUES (101, 'テスト医師', 0, NULL);
INSERT INTO M_PATIENT (P_ID, P_NAME, P_KANA, P_SEX) VALUES (1, 'テスト患者', 'テストカンジャ', 1);

COMMIT;
