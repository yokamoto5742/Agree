using System;
using System.Data.OleDb;
using Agree;
using NUnit.Framework;

namespace Agree.Tests
{
    /// <summary>
    /// ローカル Oracle Free (OraOLEDB.Oracle / localhost:1521/FREEPDB1) に対する
    /// SQL 読み書きの結合テスト。アプリ本体や AgentlabUtilityLibrary.dll には依存しない。
    ///
    /// 前提:
    ///   1. ローカルに Oracle Database Free が起動している。
    ///   2. 32bit 版 ODAC (OraOLEDB.Oracle) がインストール済み（このテストは x86 で走る）。
    ///   3. docs/test_db_schema.sql を TEST_USER スキーマへ投入済み。
    /// いずれかが満たされない場合、各テストは Ignore（失敗ではなくスキップ）になる。
    /// 接続文字列は環境変数 AGREE_TEST_ORACLE で上書き可能。
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    public class OracleIntegrationTests
    {
        private const string DefaultConnectionString =
            "Provider=OraOLEDB.Oracle;Data Source=localhost:1521/FREEPDB1;User ID=TEST_USER;Password=TEST_PWD";

        private static string ConnectionString =>
            Environment.GetEnvironmentVariable("AGREE_TEST_ORACLE") ?? DefaultConnectionString;

        /// <summary>
        /// DB未起動 / スキーマ未投入なら Ignore（スキップ）。
        /// 一方、ビット不一致(BadImageFormatException)やプロバイダ未登録は環境構築ミスなので、
        /// Ignore で握り潰さず Fail させて表面化させる（"DB未起動" と取り違えないため）。
        /// </summary>
        [OneTimeSetUp]
        public void EnsureDatabaseAvailable()
        {
            try
            {
                using (var con = new OleDbConnection(ConnectionString))
                {
                    con.Open();
                    using (var cmd = new OleDbCommand(
                        "SELECT COUNT(*) FROM USER_TABLES " +
                        "WHERE TABLE_NAME IN ('AGREE','AGREE_TEMPLATE','AGREE_STAFF')", con))
                    {
                        var tableCount = Convert.ToInt32(cmd.ExecuteScalar());
                        if (tableCount < 3)
                            Assert.Ignore("テスト用スキーマが未投入です。docs/test_db_schema.sql を投入してください。");
                    }
                }
            }
            catch (BadImageFormatException ex)
            {
                Assert.Fail("テストホストのアーキテクチャ不一致です。x86 で実行してください" +
                            "（x86.runsettings / テストエクスプローラーのプロセスアーキテクチャ=x86）: " + ex.Message);
            }
            catch (Exception ex) when (!(ex is IgnoreException))
            {
                var msg = ex.Message ?? "";
                bool providerProblem =
                    msg.IndexOf("OraOLEDB", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    msg.IndexOf("provider", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    msg.Contains("プロバイダー") || msg.Contains("登録されていません");
                if (providerProblem)
                    Assert.Fail("OraOLEDB.Oracle プロバイダをロードできません。" +
                                "32bit 版 ODAC のインストール / ビット一致を確認してください: " + ex.Message);
                Assert.Ignore("ローカル Oracle に接続できません（DB未起動など）: " + ex.Message);
            }
        }

        [Test]
        public void Connection_Opens()
        {
            using (var con = new OleDbConnection(ConnectionString))
            {
                con.Open();
                Assert.That(con.State, Is.EqualTo(System.Data.ConnectionState.Open));
            }
        }

        [Test]
        public void Agree_InsertSelectUpdate_RoundTrips()
        {
            using (var con = Open())
            using (var tx = con.BeginTransaction())
            {
                long id = NextVal(con, tx, "AGREE_SEQ");

                int inserted = Exec(con, tx,
                    "INSERT INTO AGREE " +
                    "(AGREE_ID, PATIENT_ID, SAVE_DATE, DEPT, DR, STAFF, EYE, DIAG, DR_OK, DELETE_FLAG, SAVE_TIME) " +
                    "VALUES (" + id + ", 1, 20260616, 1, 101, 'テスト担当', '右', 'テスト病名', 0, 0, 101010)");
                Assert.That(inserted, Is.EqualTo(1), "INSERT は 1 行のはず");

                var diag = Scalar(con, tx, "SELECT DIAG FROM AGREE WHERE AGREE_ID = " + id);
                Assert.That(diag, Is.EqualTo("テスト病名"), "INSERT した日本語が読み戻せること");

                int updated = Exec(con, tx, "UPDATE AGREE SET DELETE_FLAG = 1 WHERE AGREE_ID = " + id);
                Assert.That(updated, Is.EqualTo(1), "論理削除 UPDATE は 1 行のはず");

                var flag = Convert.ToInt32(Scalar(con, tx, "SELECT DELETE_FLAG FROM AGREE WHERE AGREE_ID = " + id));
                Assert.That(flag, Is.EqualTo(1), "DELETE_FLAG が 1 に更新されていること");

                tx.Rollback(); // テストを再実行可能に保つ
            }
        }

        /// <summary>
        /// 自由記述欄に ' と日本語が混じっても、AgreeSql.SqlValue で正しくエスケープすれば
        /// そのまま格納・読み戻しできること（regPlan が本来通すべきエスケープ契約の DB 実証）。
        /// </summary>
        [Test]
        public void Agree_StoresApostropheAndJapanese_RoundTrips()
        {
            const string diag = "加齢黄斑変性（O'Brien法）";
            using (var con = Open())
            using (var tx = con.BeginTransaction())
            {
                long id = NextVal(con, tx, "AGREE_SEQ");

                int inserted = Exec(con, tx,
                    "INSERT INTO AGREE " +
                    "(AGREE_ID, PATIENT_ID, SAVE_DATE, DEPT, DR, DIAG, DR_OK, DELETE_FLAG, SAVE_TIME) " +
                    "VALUES (" + id + ", 1, 20260616, 1, 101, " + AgreeSql.SqlValue(diag) + ", 0, 0, 101010)");
                Assert.That(inserted, Is.EqualTo(1), "エスケープ済みリテラルで INSERT が成立すること");

                var read = Scalar(con, tx, "SELECT DIAG FROM AGREE WHERE AGREE_ID = " + id);
                Assert.That(read, Is.EqualTo(diag), "' と日本語が無損失で読み戻せること");

                tx.Rollback();
            }
        }

        /// <summary>
        /// regPlan が INSERT する全列を一度に往復させ、列名・型・本数の不一致を検出する。
        /// </summary>
        [Test]
        public void Agree_FullColumnSet_RoundTrips()
        {
            using (var con = Open())
            using (var tx = con.BeginTransaction())
            {
                long id = NextVal(con, tx, "AGREE_SEQ");

                int inserted = Exec(con, tx,
                    "INSERT INTO AGREE " +
                    "(AGREE_ID, PATIENT_ID, SAVE_DATE, DEPT, DR, STAFF, EYE, DIAG, ANES, OPE, EXPLANATION, " +
                    " ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DR_OK, DELETE_FLAG, SAVE_TIME) " +
                    "VALUES (" + id + ", 1, 20260616, 1, 101, '担当', '右', '病名', '局所', '術式', '説明', " +
                    "'症状', '計画', '検査', '手術内容', '日帰り', 1, 0, 123456)");
                Assert.That(inserted, Is.EqualTo(1));

                using (var cmd = new OleDbCommand(
                    "SELECT STAFF, EYE, DIAG, ANES, OPE, EXPLANATION, ITEM1, ITEM2, ITEM3, ITEM4, SHEET_NAME, DR_OK " +
                    "FROM AGREE WHERE AGREE_ID = " + id, con, tx))
                using (var r = cmd.ExecuteReader())
                {
                    Assert.That(r.Read(), Is.True, "INSERT した 1 行が読めること");
                    Assert.That(r["STAFF"], Is.EqualTo("担当"));
                    Assert.That(r["EYE"], Is.EqualTo("右"));
                    Assert.That(r["DIAG"], Is.EqualTo("病名"));
                    Assert.That(r["ITEM4"], Is.EqualTo("手術内容"));
                    Assert.That(r["SHEET_NAME"], Is.EqualTo("日帰り"));
                    Assert.That(Convert.ToInt32(r["DR_OK"]), Is.EqualTo(1));
                }

                tx.Rollback();
            }
        }

        [Test]
        public void AgreeTemplate_InsertSelectDelete_RoundTrips()
        {
            using (var con = Open())
            using (var tx = con.BeginTransaction())
            {
                long id = NextVal(con, tx, "AGREE_TEMPLATE_SEQ");

                Exec(con, tx,
                    "INSERT INTO AGREE_TEMPLATE " +
                    "(TEMP_ID, TEMP_LEVEL, TEMP_PARENT, TEMP_NAME, DELETE_FLAG, DISP_ORDER) " +
                    "VALUES (" + id + ", 0, NULL, 'テンプレ親', 0, 1)");

                var name = Scalar(con, tx, "SELECT TEMP_NAME FROM AGREE_TEMPLATE WHERE TEMP_ID = " + id);
                Assert.That(name, Is.EqualTo("テンプレ親"));

                int deleted = Exec(con, tx, "UPDATE AGREE_TEMPLATE SET DELETE_FLAG = 1 WHERE TEMP_ID = " + id);
                Assert.That(deleted, Is.EqualTo(1));

                tx.Rollback();
            }
        }

        [Test]
        public void AgreeStaff_InsertSelectUpdate_RoundTrips()
        {
            using (var con = Open())
            using (var tx = con.BeginTransaction())
            {
                long id = NextVal(con, tx, "AGREE_STAFF_SEQ");

                Exec(con, tx,
                    "INSERT INTO AGREE_STAFF (ID, STAFF, CONT) VALUES (" + id + ", 101, '定型説明文')");

                var cont = Scalar(con, tx, "SELECT CONT FROM AGREE_STAFF WHERE ID = " + id);
                Assert.That(cont, Is.EqualTo("定型説明文"));

                int updated = Exec(con, tx,
                    "UPDATE AGREE_STAFF SET CONT = '更新後の説明文' WHERE ID = " + id);
                Assert.That(updated, Is.EqualTo(1));

                var cont2 = Scalar(con, tx, "SELECT CONT FROM AGREE_STAFF WHERE ID = " + id);
                Assert.That(cont2, Is.EqualTo("更新後の説明文"));

                tx.Rollback();
            }
        }

        // ---- helpers ----

        private static OleDbConnection Open()
        {
            var con = new OleDbConnection(ConnectionString);
            con.Open();
            return con;
        }

        private static long NextVal(OleDbConnection con, OleDbTransaction tx, string sequenceName)
        {
            using (var cmd = new OleDbCommand("SELECT " + sequenceName + ".NEXTVAL FROM DUAL", con, tx))
                return Convert.ToInt64(cmd.ExecuteScalar());
        }

        private static int Exec(OleDbConnection con, OleDbTransaction tx, string sql)
        {
            using (var cmd = new OleDbCommand(sql, con, tx))
                return cmd.ExecuteNonQuery();
        }

        private static object Scalar(OleDbConnection con, OleDbTransaction tx, string sql)
        {
            using (var cmd = new OleDbCommand(sql, con, tx))
                return cmd.ExecuteScalar();
        }
    }
}
