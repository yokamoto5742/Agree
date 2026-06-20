using Agree;
using NUnit.Framework;

namespace Agree.Tests
{
    /// <summary>
    /// AgreeSql の純粋ロジック単体テスト。DB・COM・外部DLL に依存しないため、
    /// ローカル Oracle が無い環境でも常に実行・グリーンになる（CI 向けの高速層）。
    /// 対象ソース Agree/AgreeSql.cs は csproj で &lt;Compile Link&gt; リンクして本体と同一実装を検証する。
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class AgreeSqlTests
    {
        // ---- SqlValue: SQL 文字列リテラル化 ----

        [Test]
        public void SqlValue_Null_ReturnsNullKeyword()
        {
            Assert.That(AgreeSql.SqlValue(null), Is.EqualTo("NULL"));
        }

        [Test]
        public void SqlValue_Empty_ReturnsNullKeyword()
        {
            Assert.That(AgreeSql.SqlValue(""), Is.EqualTo("NULL"));
        }

        [Test]
        public void SqlValue_PlainText_IsQuoted()
        {
            Assert.That(AgreeSql.SqlValue("白内障"), Is.EqualTo("'白内障'"));
        }

        [Test]
        public void SqlValue_SingleApostrophe_IsDoubled()
        {
            // 例: 「加齢黄斑変性（O'Brien法）」のように本文に ' が混じると、
            // エスケープしないと SQL が壊れる/注入余地が生じる。'' へ二重化されること。
            Assert.That(AgreeSql.SqlValue("O'Brien"), Is.EqualTo("'O''Brien'"));
        }

        [Test]
        public void SqlValue_MultipleApostrophes_AllDoubled()
        {
            Assert.That(AgreeSql.SqlValue("a'b'c"), Is.EqualTo("'a''b''c'"));
        }

        [Test]
        public void SqlValue_InjectionLikeText_IsNeutralized()
        {
            // 文字列リテラルを閉じてコメント化する典型パターンが、丸ごとリテラル内に閉じ込められること。
            Assert.That(AgreeSql.SqlValue("'); drop table AGREE --"),
                Is.EqualTo("'''); drop table AGREE --'"));
        }

        // ---- CsvEscape: CSV 1 フィールドのエスケープ ----

        [Test]
        public void CsvEscape_Null_ReturnsEmpty()
        {
            Assert.That(AgreeSql.CsvEscape(null), Is.EqualTo(""));
        }

        [Test]
        public void CsvEscape_PlainText_IsUnchanged()
        {
            Assert.That(AgreeSql.CsvEscape("テスト担当"), Is.EqualTo("テスト担当"));
        }

        [Test]
        public void CsvEscape_ContainsComma_IsQuoted()
        {
            Assert.That(AgreeSql.CsvEscape("右,左"), Is.EqualTo("\"右,左\""));
        }

        [Test]
        public void CsvEscape_ContainsQuote_QuoteIsDoubledAndWrapped()
        {
            Assert.That(AgreeSql.CsvEscape("12\"3"), Is.EqualTo("\"12\"\"3\""));
        }

        [Test]
        public void CsvEscape_ContainsNewline_IsQuoted()
        {
            Assert.That(AgreeSql.CsvEscape("行1\n行2"), Is.EqualTo("\"行1\n行2\""));
        }

        [Test]
        public void CsvEscape_ContainsCarriageReturn_IsQuoted()
        {
            Assert.That(AgreeSql.CsvEscape("行1\r行2"), Is.EqualTo("\"行1\r行2\""));
        }
    }
}
