using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Exams.Html;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UntisExport.Test.Exams.Html
{
    [TestClass]
    public class ExamExporterTest : AbstractHtmlTestCase
    {
        [TestMethod]
        public async Task TestNormalData()
        {
            var exporter = new ExamExporter();
            var settings = new ExamExportSettings();

            var html = GetExamsHtml();
            var result = await exporter.ParseHtmlAsync(settings, html);

            Assert.AreEqual(3, result.Exams.Count);

            var examWithoutDescription = result.Exams[0];
            Assert.AreEqual(DateTime.Parse("2020-03-03"), examWithoutDescription.Date);
            Assert.AreEqual(1, examWithoutDescription.LessonStart);
            Assert.AreEqual(6, examWithoutDescription.LessonEnd);
            CollectionAssert.AreEqual(new string[] { "Q2" }, examWithoutDescription.Grades.ToArray());
            CollectionAssert.AreEqual(new string[] { "D-LK1" }, examWithoutDescription.Courses.ToArray());
            CollectionAssert.AreEqual(new string[] { "ABC", "ABC", "CDE", "CDE", "DEF", "DEF" }, examWithoutDescription.Teachers.ToArray());
            CollectionAssert.AreEqual(new string[] { "A002", "A002", "A002", "A002", "A002", "A002" }, examWithoutDescription.Rooms.ToArray());
            Assert.IsNull(examWithoutDescription.Description);
            Assert.AreEqual("A20-LK1", examWithoutDescription.Name);

            var examWithDescription = result.Exams[1];
            Assert.AreEqual(DateTime.Parse("2020-03-03"), examWithDescription.Date);
            Assert.AreEqual(1, examWithDescription.LessonStart);
            Assert.AreEqual(6, examWithDescription.LessonEnd);
            CollectionAssert.AreEqual(new string[] { "Q2" }, examWithDescription.Grades.ToArray());
            CollectionAssert.AreEqual(new string[] { "E-LK2" }, examWithDescription.Courses.ToArray());
            CollectionAssert.AreEqual(new string[] { "CDE", "CDE", "ABC", "ABC", "DEF", "DEF" }, examWithDescription.Teachers.ToArray());
            CollectionAssert.AreEqual(new string[] { "A001", "A001", "A001", "A001", "A001", "A001" }, examWithDescription.Rooms.ToArray());
            Assert.AreEqual("Lorem ipsum dolor.", examWithDescription.Description);
            Assert.AreEqual("A20-LK1", examWithoutDescription.Name);

            var examWithEmptyValuesAndMultipleCourses = result.Exams[2];
            Assert.AreEqual(DateTime.Parse("2020-03-04"), examWithEmptyValuesAndMultipleCourses.Date);
            Assert.AreEqual(1, examWithEmptyValuesAndMultipleCourses.LessonStart);
            Assert.AreEqual(2, examWithEmptyValuesAndMultipleCourses.LessonEnd);
            CollectionAssert.AreEqual(new string[] { "EF" }, examWithEmptyValuesAndMultipleCourses.Grades.ToArray());
            CollectionAssert.AreEqual(new string[] { "EK-GK1", "IF-GK1", "IF-GK2" }, examWithEmptyValuesAndMultipleCourses.Courses.ToArray());
            CollectionAssert.AreEqual(Array.Empty<string>(), examWithEmptyValuesAndMultipleCourses.Teachers.ToArray());
            CollectionAssert.AreEqual(Array.Empty<string>(), examWithEmptyValuesAndMultipleCourses.Rooms.ToArray());
            Assert.IsNull(examWithEmptyValuesAndMultipleCourses.Description);
            Assert.IsNull(examWithEmptyValuesAndMultipleCourses.Name);
        }

        private string GetExamsHtml()
        {
            return LoadFile("test_exams.htm");
        }
    }
}
