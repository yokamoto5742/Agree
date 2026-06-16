-- =====================================================================
-- 眼科同意書システム テスト用スキーマ作成スクリプト
-- 実行: sqlplus TEST_USER/TEST_PWD@localhost:1521/FREEPDB1 @test_db_schema.sql
--
-- 前提: 設定ファイルの DB_LINK を「空」にすること。
--       本アプリは M_xxx マスタを "テーブル名 + DB_LINK" で参照するため、
--       DB_LINK が空ならローカル(TEST_USER)の同名テーブルを直接参照する。
--
-- 注意: データ型・桁数はソースに定義が無いため「推測」。本番DDLが入手でき
--       たら差し替えること。日本語格納のため VARCHAR2 は CHAR 長で指定。
--       自由記述欄(DIAG/OPE/EXPLANATION/ITEM1-4)は長文の可能性があり 2000CHAR
--       で確保。不足する場合は CLOB へ変更する。
-- =====================================================================

-- ---------------------------------------------------------------------
-- 1. アプリ専用テーブル（本アプリが INSERT/UPDATE する。INSERT 文から全列確定）
-- ---------------------------------------------------------------------

-- 同意書本体（Form1.regPlan / delPlan / showList, FindAgree.showList）
CREATE TABLE AGREE (
    AGREE_ID     NUMBER          NOT NULL,        -- AGREE_SEQ.nextval
    PATIENT_ID   NUMBER,                          -- M_PATIENT.P_ID に対応
    SAVE_DATE    NUMBER(8),                       -- 作成日 yyyymmdd
    DEPT         NUMBER,                          -- 診療科 → M_DEPT.CODE
    DR           NUMBER,                          -- 医師   → M_USR.CODE
    STAFF        VARCHAR2(100 CHAR),              -- 担当者(文字列で格納)
    EYE          VARCHAR2(50 CHAR),               -- 左右眼
    DIAG         VARCHAR2(2000 CHAR),             -- 病名
    ANES         VARCHAR2(2000 CHAR),             -- 麻酔
    OPE          VARCHAR2(2000 CHAR),             -- 術式
    EXPLANATION  VARCHAR2(2000 CHAR),             -- 説明
    ITEM1        VARCHAR2(2000 CHAR),
    ITEM2        VARCHAR2(2000 CHAR),
    ITEM3        VARCHAR2(2000 CHAR),
    ITEM4        VARCHAR2(2000 CHAR),
    SHEET_NAME   VARCHAR2(200 CHAR),              -- 帳票シート名
    DR_OK        NUMBER(1)       DEFAULT 0,       -- 医師完了 1/0
    DELETE_FLAG  NUMBER(1)       DEFAULT 0,       -- 論理削除 1/0
    SAVE_TIME    NUMBER(6),                       -- 保存時刻 HHmmss
    CONSTRAINT PK_AGREE PRIMARY KEY (AGREE_ID)
);

-- 同意書テンプレート（TmpAgree。親子ツリー構造: TEMP_PARENT で親を参照）
CREATE TABLE AGREE_TEMPLATE (
    TEMP_ID      NUMBER          NOT NULL,        -- AGREE_TEMPLATE_SEQ.nextval
    TEMP_LEVEL   NUMBER,                          -- 階層レベル(0=ルート,1=葉)
    TEMP_PARENT  NUMBER,                          -- 親 TEMP_ID
    TEMP_NAME    VARCHAR2(200 CHAR),
    EYE          VARCHAR2(50 CHAR),
    DIAG         VARCHAR2(2000 CHAR),
    ANES         VARCHAR2(2000 CHAR),
    OPE          VARCHAR2(2000 CHAR),
    EXPLANATION  VARCHAR2(2000 CHAR),
    ITEM1        VARCHAR2(2000 CHAR),
    ITEM2        VARCHAR2(2000 CHAR),
    ITEM3        VARCHAR2(2000 CHAR),
    ITEM4        VARCHAR2(2000 CHAR),
    SHEET_NAME   VARCHAR2(200 CHAR),
    DELETE_FLAG  NUMBER(1)       DEFAULT 0,
    DISP_ORDER   NUMBER,                          -- 表示順
    CONSTRAINT PK_AGREE_TEMPLATE PRIMARY KEY (TEMP_ID)
);

-- 担当医ごとの定型説明文（TmpStaff。STAFF は数値の医師コード → M_USR.CODE）
CREATE TABLE AGREE_STAFF (
    ID           NUMBER          NOT NULL,        -- AGREE_STAFF_SEQ.nextval
    STAFF        NUMBER,                          -- 医師コード → M_USR.CODE
    CONT         VARCHAR2(2000 CHAR),             -- 説明文
    CONSTRAINT PK_AGREE_STAFF PRIMARY KEY (ID)
);

-- シーケンス（INSERT 文で xxx_SEQ.nextval を使用）
CREATE SEQUENCE AGREE_SEQ          START WITH 1 INCREMENT BY 1 NOCACHE;
CREATE SEQUENCE AGREE_TEMPLATE_SEQ START WITH 1 INCREMENT BY 1 NOCACHE;
CREATE SEQUENCE AGREE_STAFF_SEQ    START WITH 1 INCREMENT BY 1 NOCACHE;


-- ---------------------------------------------------------------------
-- 2. 電子カルテ側マスタ（本来は DB_LINK 経由。DB_LINK を空にしローカルで代替）
--    起動時 Dict.InitDict() が下記 7 表を順次 SELECT する。1 つでも欠けると
--    例外 → オフラインモードに落ちる。列は SELECT で参照される分のみ定義。
--    (TM01RC は HolidayDict 専用で本アプリ未使用のため不要)
-- ---------------------------------------------------------------------

-- 患者マスタ（Form1.showList: P_NAME,P_KANA,P_SEX を P_ID で取得）
CREATE TABLE M_PATIENT (
    P_ID    NUMBER          NOT NULL,
    P_NAME  VARCHAR2(100 CHAR),
    P_KANA  VARCHAR2(100 CHAR),
    P_SEX   NUMBER(1),                            -- "2"=女, それ以外=男
    CONSTRAINT PK_M_PATIENT PRIMARY KEY (P_ID)
);

-- 診療科マスタ（Dict 481行: CODE,NAME,S_NAME / アプリ: S_NAME, CODE 参照）
CREATE TABLE M_DEPT (
    CODE    NUMBER          NOT NULL,
    NAME    VARCHAR2(100 CHAR),
    S_NAME  VARCHAR2(50 CHAR),                    -- 略称
    CONSTRAINT PK_M_DEPT PRIMARY KEY (CODE)
);

-- 職員マスタ（Dict 504行: CODE,NAME,KANA,SYOZOKU,SHIKAKU,DEPT,DR / アプリ: NAME,CODE）
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

-- 医師マスタ（Dict 489行: t1=M_DR の CODE,NAME,CATEGORY,VAL_4 を参照）
-- 注意: 489行の "where SYOZOKU = 1" は無修飾。SYOZOKU は M_USR(t2) 側の列
--       (504行で M_USR から単独SELECTされ確定)。M_DR に SYOZOKU を持たせると
--       ORA-00918(列が一義的でない)で InitDict が落ち、オフラインに転落するため
--       ここには SYOZOKU を置かない。
CREATE TABLE M_DR (
    CODE      NUMBER         NOT NULL,
    NAME      VARCHAR2(100 CHAR),
    CATEGORY  NUMBER,
    VAL_4     VARCHAR2(100 CHAR),
    CONSTRAINT PK_M_DR PRIMARY KEY (CODE)
);

-- 所属マスタ（Dict 537行: CODE,NAME,S_NAME,CATEGORY）
CREATE TABLE M_SYOZOKU (
    CODE      NUMBER         NOT NULL,
    NAME      VARCHAR2(100 CHAR),
    S_NAME    VARCHAR2(50 CHAR),
    CATEGORY  NUMBER,
    CONSTRAINT PK_M_SYOZOKU PRIMARY KEY (CODE)
);

-- 資格マスタ（Dict 545行: CODE,NAME,S_NAME,CATEGORY）
CREATE TABLE M_SHIKAKU (
    CODE      NUMBER         NOT NULL,
    NAME      VARCHAR2(100 CHAR),
    S_NAME    VARCHAR2(50 CHAR),
    CATEGORY  NUMBER,
    CONSTRAINT PK_M_SHIKAKU PRIMARY KEY (CODE)
);

-- 診療区分マスタ（Dict 466行: CODE,NAME）
CREATE TABLE M_SHINKU (
    CODE    NUMBER          NOT NULL,
    NAME    VARCHAR2(100 CHAR),
    CONSTRAINT PK_M_SHINKU PRIMARY KEY (CODE)
);

-- 施行マスタ（Dict 473行: CODE,NAME,S_NAME）
CREATE TABLE M_SEKOU (
    CODE    NUMBER          NOT NULL,
    NAME    VARCHAR2(100 CHAR),
    S_NAME  VARCHAR2(50 CHAR),
    CONSTRAINT PK_M_SEKOU PRIMARY KEY (CODE)
);


-- ---------------------------------------------------------------------
-- 3. 起動と最小動作に必要な初期データ（例）
--    InitDict() は各表を SELECT するだけなので空でも起動はするが、
--    診療科コンボや一覧結合(INNER JOIN)のため最低限のマスタを投入しておく。
-- ---------------------------------------------------------------------
INSERT INTO M_DEPT (CODE, NAME, S_NAME) VALUES (1, '眼科', '眼科');
INSERT INTO M_USR  (CODE, NAME, KANA, SYOZOKU, SHIKAKU, DEPT, DR)
       VALUES (101, 'テスト医師', 'テストイシ', 1, 1, 1, 101);
INSERT INTO M_DR   (CODE, NAME, CATEGORY, VAL_4) VALUES (101, 'テスト医師', 0, NULL);
INSERT INTO M_PATIENT (P_ID, P_NAME, P_KANA, P_SEX) VALUES (1, 'テスト患者', 'テストカンジャ', 1);

COMMIT;
