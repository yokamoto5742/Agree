using System;
using System.Drawing;
using System.IO;
using AgentlabUtilityLibrary;
using NUnit.Framework;

namespace Agree.Tests
{
    /// <summary>
    /// Agree/Infrastructure（旧 AgentlabUtilityLibrary.dll）の DB 非依存ロジックの単体テスト。
    /// 対象ソースは csproj で &lt;Compile Link&gt; リンクして本体と同一実装を検証する。
    /// 期待値は DLL 時代のテストの値をそのまま使い、取り込み前後で挙動が変わっていないことを確認する。
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class UtilityTests
    {
        // 描画結果のピクセルから求めたハッシュ(リファクタリング前後で描画が変わらないことの確認用)
        private static string BarcodeHash(Barcode128.CODE code, string bar)
        {
            using (Bitmap bitmap = new Bitmap(200, 40))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Gray);
                new Barcode128().Draw(code, bar, graphics, 10f, 5f, 30f, 1.5f);
                long hash = 17;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        hash = hash * 31 + bitmap.GetPixel(x, y).ToArgb();
                    }
                }
                return hash.ToString();
            }
        }

        [Test]
        public void Decrypt_KnownValue()
        {
            Assert.That(Enc.Decrypt("Q?uR"), Is.EqualTo("macs"));
        }

        [Test]
        public void Decrypt_UnmappedCharactersPassThrough()
        {
            Assert.That(Enc.Decrypt("あいう"), Is.EqualTo("あいう"));
        }

        [Test]
        public void Barcode128_CodeC()
        {
            Assert.That(BarcodeHash(Barcode128.CODE.C, "123456"), Is.EqualTo("-3218992874777929554"));
        }

        [Test]
        public void Barcode128_CodeB()
        {
            Assert.That(BarcodeHash(Barcode128.CODE.B, "0123"), Is.EqualTo("6557099246411588869"));
        }

        [Test]
        public void Barcode128_NonDigit_DrawsNothing()
        {
            using (Bitmap bitmap = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Assert.That(new Barcode128().Draw(Barcode128.CODE.C, "12a", graphics, 0f, 0f, 1f, 1f), Is.False);
            }
        }

        /// <summary>
        /// Env は初回アクセス時に1回だけ INI を読むため、Env に触れるテストはこの1つだけにする
        /// （他のテストから先に触れると、出力フォルダの実環境の INI が読み込まれて期待値が変わる）。
        /// カレントディレクトリにテスト用の INI を置き、c:\macs\utility の実環境の INI より優先させる。
        /// </summary>
        [Test]
        public void Env_EhrKeys_FallBackToOpenKeys()
        {
            string dir = Path.Combine(Path.GetTempPath(), "AgreeUtilityTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            string previous = Directory.GetCurrentDirectory();
            try
            {
                File.WriteAllLines(Path.Combine(dir, "AgentlabUtilityLibrary.ini"), new[]
                {
                    "[DB Config Start]",
                    "OPEN_DB=Q?uR",
                    "OPEN_USER=uR",
                    "OPEN_PWD=R?",
                    "PROVIDER=TestProvider",
                    "EHR_USER=Q?",
                    "[DB Config End]",
                });
                Directory.SetCurrentDirectory(dir);

                Assert.That(Env.EHR_DB, Is.EqualTo(Env.OPEN_DB), "EHR_DB が無ければ OPEN_DB");
                Assert.That(Env.EHR_USER, Is.EqualTo(Enc.Decrypt("Q?")), "EHR_USER があればその値(復号済み)");
                Assert.That(Env.EHR_PWD, Is.EqualTo(Env.OPEN_PWD), "EHR_PWD が無ければ OPEN_PWD");
                Assert.That(Env.EHR_PROVIDER, Is.EqualTo("TestProvider"), "EHR_PROVIDER が無ければ PROVIDER");
                Assert.That(DBConn.GetEhrDBConn().ConnectionString, Is.EqualTo(
                    "Provider=TestProvider;Data Source=" + Env.OPEN_DB +
                    ";User ID=" + Enc.Decrypt("Q?") + ";Password=" + Env.OPEN_PWD));
            }
            finally
            {
                Directory.SetCurrentDirectory(previous);
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
