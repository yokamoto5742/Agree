-- =====================================================================
-- 眼科同意書システム 手動動作確認用テストデータ投入スクリプト（標準セット）
-- 実行: set NLS_LANG=.AL32UTF8 のうえ（このファイルは UTF-8 保存）
--   sqlplus TEST_USER/TEST_PWD@localhost:1521/FREEPDB1 @test_db_seed.sql
--
-- 前提: docs/test_db_schema.sql でテーブル/シーケンスが作成済みであること。
-- 内容: 患者3名・医師2名・同意書(AGREE)5件・テンプレート(親2/子2)・スタッフ定型文2件。
--
-- マスタ(M_PATIENT/M_USR/M_DR)は NOT EXISTS ガードで重複投入を防止。
-- AGREE / AGREE_TEMPLATE / AGREE_STAFF はシーケンス採番のため、再実行すると
-- 行が増える点に注意（初期投入は1回のみ実行する想定）。
-- =====================================================================

-- ---------------------------------------------------------------------
-- マスタ追加（既存: M_DEPT 眼科=1 / M_USR=101 / M_PATIENT=1）
-- ---------------------------------------------------------------------
-- 患者2・3を追加
INSERT INTO M_PATIENT (P_ID, P_NAME, P_KANA, P_SEX)
SELECT 2, '同意 花子', 'ドウイ ハナコ', 2 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM M_PATIENT WHERE P_ID = 2);

INSERT INTO M_PATIENT (P_ID, P_NAME, P_KANA, P_SEX)
SELECT 3, '承諾 太郎', 'ショウダク タロウ', 1 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM M_PATIENT WHERE P_ID = 3);

-- 医師102を追加（M_USR と M_DR の両方。Dict.StaffDict は M_USR から構築される）
INSERT INTO M_USR (CODE, NAME, KANA, SYOZOKU, SHIKAKU, DEPT, DR)
SELECT 102, '眼科 二郎', 'ガンカ ジロウ', 1, 1, 1, 102 FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM M_USR WHERE CODE = 102);

INSERT INTO M_DR (CODE, NAME, CATEGORY, VAL_4)
SELECT 102, '眼科 二郎', 0, NULL FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM M_DR WHERE CODE = 102);

-- ---------------------------------------------------------------------
-- 同意書 AGREE（5件）— 作成日/医師/眼/シート/完了フラグを変えて検索・一覧確認用
-- ---------------------------------------------------------------------
INSERT INTO AGREE
 (AGREE_ID, PATIENT_ID, SAVE_DATE, DEPT, DR, STAFF, EYE, DIAG, ANES, OPE,
  EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DR_OK, DELETE_FLAG, SAVE_TIME)
VALUES
 (AGREE_SEQ.nextval, 1, 20260601, 1, 101, '外来看護師A', '右', '白内障', '点眼麻酔',
  '水晶体再建術（眼内レンズ挿入術）',
  '加齢に伴い水晶体が混濁する病気です。手術で濁った水晶体を取り除き眼内レンズを挿入します。',
  '視力低下・かすみ・まぶしさ', '日帰り手術にて水晶体再建術を行います',
  '視力検査・眼圧検査・角膜内皮細胞検査', '局所麻酔下で約15分の手術です',
  '日帰り', 1, 0, 101500);

INSERT INTO AGREE
 (AGREE_ID, PATIENT_ID, SAVE_DATE, DEPT, DR, STAFF, EYE, DIAG, ANES, OPE,
  EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DR_OK, DELETE_FLAG, SAVE_TIME)
VALUES
 (AGREE_SEQ.nextval, 1, 20260515, 1, 102, '', '左', '緑内障', '球後麻酔',
  '線維柱帯切除術（トラベクレクトミー）',
  '眼圧を下げるための手術です。房水の流出路を新たに作成します。',
  '眼圧上昇・視野欠損', '入院のうえ手術を行います', '視野検査・眼圧日内変動検査', '',
  '入院', 0, 0, 93000);

INSERT INTO AGREE
 (AGREE_ID, PATIENT_ID, SAVE_DATE, DEPT, DR, STAFF, EYE, DIAG, ANES, OPE,
  EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DR_OK, DELETE_FLAG, SAVE_TIME)
VALUES
 (AGREE_SEQ.nextval, 2, 20260610, 1, 101, '病棟看護師B', '両', '糖尿病網膜症', 'テノン嚢下麻酔',
  '硝子体手術',
  '網膜の出血や増殖膜を取り除く手術です。',
  '飛蚊症・視力低下', '入院のうえ両眼を順次手術します', '蛍光眼底造影・OCT検査', '硝子体切除および網膜光凝固',
  '入院', 1, 0, 140000);

INSERT INTO AGREE
 (AGREE_ID, PATIENT_ID, SAVE_DATE, DEPT, DR, STAFF, EYE, DIAG, ANES, OPE,
  EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DR_OK, DELETE_FLAG, SAVE_TIME)
VALUES
 (AGREE_SEQ.nextval, 2, 20260616, 1, 101, '', '右', '翼状片', '点眼麻酔',
  '翼状片切除術',
  '結膜の組織が角膜に進入する病気で、切除します。',
  '充血・異物感', '日帰りにて切除術を行います', '', '',
  '日帰り', 0, 0, 110000);

INSERT INTO AGREE
 (AGREE_ID, PATIENT_ID, SAVE_DATE, DEPT, DR, STAFF, EYE, DIAG, ANES, OPE,
  EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DR_OK, DELETE_FLAG, SAVE_TIME)
VALUES
 (AGREE_SEQ.nextval, 3, 20260420, 1, 102, '外来看護師C', '左', '裂孔原性網膜剥離', 'テノン嚢下麻酔',
  '強膜内陥術（バックリング手術）',
  '網膜の裂孔をふさぎ、剥離した網膜を復位させる手術です。',
  '光視症・視野欠損', '短期滞在にて手術を行います', '眼底検査・OCT検査', '',
  '短期滞在', 1, 0, 100000);

-- ---------------------------------------------------------------------
-- テンプレート AGREE_TEMPLATE（親2 / 子2）
--   親(TEMP_LEVEL=0)は initTree で TEMP_PARENT を int.Parse するため NULL 不可 → 0 を格納。
--   子(TEMP_LEVEL=1)の TEMP_PARENT には親の TEMP_ID を設定。
--   シーケンス採番を崩さないため PL/SQL で nextval を捕捉して親子を関連付ける。
-- ---------------------------------------------------------------------
DECLARE
  v_cat NUMBER;
  v_gla NUMBER;
BEGIN
  -- 親「白内障」
  v_cat := AGREE_TEMPLATE_SEQ.NEXTVAL;
  INSERT INTO AGREE_TEMPLATE
   (TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME, EYE, DIAG, ANES, OPE,
    EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DELETE_FLAG, DISP_ORDER)
  VALUES
   (v_cat, 0, 0, '白内障', '', '', '', '', '', '', '', '', '', '', 0, 1);

  -- 子「白内障 日帰り標準」
  INSERT INTO AGREE_TEMPLATE
   (TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME, EYE, DIAG, ANES, OPE,
    EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DELETE_FLAG, DISP_ORDER)
  VALUES
   (AGREE_TEMPLATE_SEQ.NEXTVAL, 1, v_cat, '白内障 日帰り標準', '右', '白内障', '点眼麻酔',
    '水晶体再建術（眼内レンズ挿入術）',
    '濁った水晶体を取り除き眼内レンズを挿入する手術です。',
    '視力低下・かすみ', '日帰り手術', '視力・眼圧・角膜内皮検査', '局所麻酔・約15分',
    '日帰り', 0, 1);

  -- 親「緑内障」
  v_gla := AGREE_TEMPLATE_SEQ.NEXTVAL;
  INSERT INTO AGREE_TEMPLATE
   (TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME, EYE, DIAG, ANES, OPE,
    EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DELETE_FLAG, DISP_ORDER)
  VALUES
   (v_gla, 0, 0, '緑内障', '', '', '', '', '', '', '', '', '', '', 0, 2);

  -- 子「緑内障 線維柱帯切除」
  INSERT INTO AGREE_TEMPLATE
   (TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME, EYE, DIAG, ANES, OPE,
    EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DELETE_FLAG, DISP_ORDER)
  VALUES
   (AGREE_TEMPLATE_SEQ.NEXTVAL, 1, v_gla, '緑内障 線維柱帯切除', '左', '緑内障', '球後麻酔',
    '線維柱帯切除術（トラベクレクトミー）',
    '房水の流出路を作成し眼圧を下げる手術です。',
    '眼圧上昇・視野欠損', '入院手術', '視野・眼圧日内変動検査', '',
    '入院', 0, 1);
END;
/

-- ---------------------------------------------------------------------
-- スタッフ定型説明文 AGREE_STAFF（主治医IDごとの担当者欄テンプレ）
--   STAFF は M_USR.CODE を参照（TmpStaff 画面で M_USR と INNER JOIN）。
-- ---------------------------------------------------------------------
INSERT INTO AGREE_STAFF (ID, STAFF, CONT)
VALUES (AGREE_STAFF_SEQ.nextval, 101, '外来看護師A / 視能訓練士D');

INSERT INTO AGREE_STAFF (ID, STAFF, CONT)
VALUES (AGREE_STAFF_SEQ.nextval, 102, '病棟看護師B / 視能訓練士E');

COMMIT;
