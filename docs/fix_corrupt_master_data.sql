-- =====================================================================
-- 文字化けマスタ修復スクリプト（既存の破損レコードの再投入）
--
-- 背景:
--   医師名・患者名の文字化けは「アプリのコード不具合」ではなく、
--   DBに格納された時点で文字が壊れている（保存時の文字化け）ことが原因。
--   docs/test_db_schema.sql が投入する初期マスタ
--     M_PATIENT P_ID=1 'テスト患者' / M_USR・M_DR CODE=101 'テスト医師'
--   が、過去にクライアントの NLS_LANG を誤った状態で投入されたため、
--   DB内のバイト列が UTF-8 置換文字(EF BF BD)を含む復元不能な状態になっている。
--   （AL32UTF8 のDBに対し、UTF-8をShift-JISと誤認した二重エンコード）
--
--   一方 docs/test_db_seed.sql で投入したレコード
--     患者2 '同意 花子' / 患者3 '承諾 太郎' / 医師102 '眼科 二郎' / M_DEPT 眼科
--   は正しい UTF-8 で格納されており、アプリでも正常表示される。
--   ＝ 読み取り経路は正常（OleDb 結合テストで日本語の往復一致を確認済み）。
--
-- 実行（このファイルは UTF-8 保存。必ず NLS_LANG=.AL32UTF8 で実行すること）:
--   set NLS_LANG=.AL32UTF8
--   sqlplus TEST_USER/TEST_PWD@localhost:1521/FREEPDB1 @docs\fix_corrupt_master_data.sql
--
-- 検証(falsifiable):
--   修復後、患者1/医師101 も患者2-3/医師102 と同様に正しく表示されること。
--   SELECT P_ID, P_NAME FROM M_PATIENT WHERE P_ID = 1;  -- → テスト患者
--   SELECT CODE, NAME  FROM M_USR     WHERE CODE = 101; -- → テスト医師
-- =====================================================================

UPDATE M_PATIENT SET P_NAME = 'テスト患者', P_KANA = 'テストカンジャ' WHERE P_ID = 1;

UPDATE M_USR SET NAME = 'テスト医師', KANA = 'テストイシ' WHERE CODE = 101;

UPDATE M_DR SET NAME = 'テスト医師' WHERE CODE = 101;

COMMIT;

-- 確認用（実行後に手動で目視）
SELECT P_ID, P_NAME, P_KANA FROM M_PATIENT WHERE P_ID = 1;
SELECT CODE, NAME, KANA  FROM M_USR     WHERE CODE = 101;
SELECT CODE, NAME        FROM M_DR      WHERE CODE = 101;
