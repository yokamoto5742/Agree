using Agree;
using NUnit.Framework;

namespace Agree.Tests
{
    /// <summary>
    /// AppSettings.Parse（EyeAgreeSettings.ini の key=value 解析）の単体テスト。
    /// 対象ソース Agree/AppSettings.cs は csproj で &lt;Compile Link&gt; リンクして本体と同一実装を検証する。
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class AppSettingsTests
    {
        [Test]
        public void Parse_KeyValue_TrimsKeyAndValue()
        {
            var values = AppSettings.Parse(new[] { " WINDOW_X = 100 " });
            Assert.That(values["WINDOW_X"], Is.EqualTo("100"));
        }

        [Test]
        public void Parse_IgnoresBlankCommentSectionAndMalformedLines()
        {
            var values = AppSettings.Parse(new[] { "", "   ", "; SHOW_SETTING_BUTTON=1", "[UI_SETTINGS]", "A=B=C", "NO_EQUALS" });
            Assert.That(values, Is.Empty);
        }

        [Test]
        public void Parse_DuplicateKey_LastLineWins()
        {
            var values = AppSettings.Parse(new[] { "DOCUMENT_CODE=11111", "DOCUMENT_CODE=39911" });
            Assert.That(values["DOCUMENT_CODE"], Is.EqualTo("39911"));
        }
    }
}
