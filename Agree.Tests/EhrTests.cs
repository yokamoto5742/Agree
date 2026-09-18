using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Agree;
using NUnit.Framework;

namespace Agree.Tests
{
    /// <summary>
    /// Ehr のうち DB を使わない部分（名称列の付与・Pat.csv 読み込み・診療科コード判定）の単体テスト。
    /// 名称列の付与は、以前 SQL の内部結合／外部結合で行っていた処理の置き換えなので、結合と同じ結果になることを固定する。
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class EhrTests
    {
        private string tempPath;

        [SetUp]
        public void SetUp()
        {
            tempPath = Path.Combine(Path.GetTempPath(), "AgreeEhrTests_" + Guid.NewGuid().ToString("N") + ".csv");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }

        // Oracle の NUMBER 列は OleDb では decimal で返るため、テスト表も decimal のキー列にする。
        private static DataTable AgreeTable(params object[] depts)
        {
            var table = new DataTable();
            table.Columns.Add("AGREE_ID", typeof(decimal));
            table.Columns.Add("DEPT", typeof(decimal));
            table.Columns.Add("STAFF", typeof(string));
            for (int i = 0; i < depts.Length; i++)
                table.Rows.Add(i + 1, depts[i], "担当" + i);
            table.AcceptChanges();
            return table;
        }

        [Test]
        public void JoinName_InsertsNameColumnRightAfterKey()
        {
            var table = AgreeTable(1m);
            Ehr.JoinName(table, "DEPT", "DEPT_NAME", new Dictionary<string, string> { ["1"] = "眼科" }, dropUnmatched: true);

            Assert.That(table.Columns.Cast<DataColumn>().Select(c => c.ColumnName),
                Is.EqualTo(new[] { "AGREE_ID", "DEPT", "DEPT_NAME", "STAFF" }));
            Assert.That(table.Rows[0]["DEPT_NAME"], Is.EqualTo("眼科"));
        }

        [Test]
        public void JoinName_DropUnmatched_RemovesRowsLikeInnerJoin()
        {
            var table = AgreeTable(1m, 99m, DBNull.Value, 1m);
            Ehr.JoinName(table, "DEPT", "DEPT_NAME", new Dictionary<string, string> { ["1"] = "眼科" }, dropUnmatched: true);

            // 並び順は保ったまま、マスタに無い行・キーが NULL の行が消えること。
            Assert.That(table.Rows.Cast<DataRow>().Select(r => r["AGREE_ID"]), Is.EqualTo(new object[] { 1m, 4m }));
        }

        [Test]
        public void JoinName_KeepUnmatched_LeavesNameNullLikeOuterJoin()
        {
            var table = AgreeTable(1m, 99m);
            Ehr.JoinName(table, "DEPT", "DEPT_NAME", new Dictionary<string, string> { ["1"] = "眼科" }, dropUnmatched: false);

            Assert.That(table.Rows.Count, Is.EqualTo(2));
            Assert.That(table.Rows[1]["DEPT_NAME"], Is.EqualTo(DBNull.Value));
            // showAgree は Value.ToString() で読むため、NULL は空文字として表示される。
            Assert.That(table.Rows[1]["DEPT_NAME"].ToString(), Is.EqualTo(""));
        }

        [TestCase("1", true, 1)]
        [TestCase("20", true, 20)]
        [TestCase("0", false, 0)]
        [TestCase("21", false, 21)]
        [TestCase("眼科", false, 0)]
        [TestCase(null, false, 0)]
        public void TryParseDeptCode_AcceptsOnly1To20(string text, bool expected, int expectedCode)
        {
            Assert.That(Ehr.TryParseDeptCode(text, out short code), Is.EqualTo(expected));
            Assert.That(code, Is.EqualTo(expectedCode));
        }

        private PatCsvFields ReadPatCsv(string content)
        {
            File.WriteAllText(tempPath, content, Encoding.Default);
            return new Ehr(null, "", tempPath).ReadPatCsv();
        }

        private static string PatCsvLine(string ptId, string sex)
        {
            var fields = Enumerable.Range(0, 28).Select(i => "f" + i).ToArray();
            fields[2] = ptId;
            fields[3] = "同意 花子";
            fields[5] = "ドウイ ハナコ";
            fields[6] = sex;
            fields[9] = "101";
            fields[10] = "眼科 一郎";
            fields[13] = "1";
            fields[14] = "x";
            fields[27] = "1";
            return string.Join(",", fields) + "\r\n2行目は読まない\r\n";
        }

        [Test]
        public void ReadPatCsv_MapsFieldsByPosition()
        {
            var csv = ReadPatCsv(PatCsvLine("123", "2"));

            Assert.That(csv.PtId, Is.EqualTo(123));
            Assert.That(csv.PtName, Is.EqualTo("同意 花子"));
            Assert.That(csv.PtKana, Is.EqualTo("ドウイ ハナコ"));
            Assert.That(csv.PtSex, Is.EqualTo("女"));
            Assert.That(csv.DrId, Is.EqualTo("101"));
            Assert.That(csv.DrName, Is.EqualTo("眼科 一郎"));
            Assert.That(csv.DeptCode, Is.EqualTo("1"));
            Assert.That(csv.Field14, Is.EqualTo("x"));
            Assert.That(csv.Field27, Is.EqualTo("1"));
        }

        [TestCase("1", "男")]
        [TestCase("2", "女")]
        [TestCase("9", "")]
        [TestCase("", "")]
        public void ReadPatCsv_ConvertsSex(string sex, string expected)
        {
            Assert.That(ReadPatCsv(PatCsvLine("1", sex)).PtSex, Is.EqualTo(expected));
        }

        [Test]
        public void ReadPatCsv_NonNumericPatientId_IsNull()
        {
            Assert.That(ReadPatCsv(PatCsvLine("ABC", "1")).PtId, Is.Null);
        }

        [Test]
        public void ReadPatCsv_ShortLine_MissingFieldsAreNull()
        {
            var csv = ReadPatCsv("a,b,5,氏名\r\n");

            Assert.That(csv.PtId, Is.EqualTo(5));
            Assert.That(csv.PtName, Is.EqualTo("氏名"));
            Assert.That(csv.PtKana, Is.Null);
            Assert.That(csv.Field27, Is.Null);
        }

        [Test]
        public void ReadPatCsv_MissingOrEmptyFile_ReturnsNull()
        {
            Assert.That(new Ehr(null, "", tempPath).ReadPatCsv(), Is.Null, "ファイルが無い");
            Assert.That(ReadPatCsv(""), Is.Null, "空ファイル");
        }
    }
}
