using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Timetable.Html;
using System.Linq;
using System.Threading.Tasks;

namespace UntisExport.Test.Timetable.Html
{
    [TestClass]
    public class TimetableExporterTest : AbstractTestCase
    {
        [TestMethod]
        public async Task TestBereitschaftAsync()
        {
            var html = LoadFile("test_Bereit.htm", "utf-8");
            var exporter = new TimetableExporter();

            var result = await exporter.ParseHtmlAsync(html, new TimetableExportSettings { Type = TimetableType.Subject });

            Assert.AreEqual("6. Periode", result.Period);
            Assert.AreEqual("Berei", result.Objective);

            var lessons = result.Lessons;
            Assert.AreEqual(10, lessons.Count);

            var lessonMondayA = lessons.FirstOrDefault(x => x.Day == 1 && x.LessonStart == 1 && x.Weeks.Contains("A"));
            Assert.IsNotNull(lessonMondayA);
            Assert.AreEqual(2, lessonMondayA.LessonEnd);
            Assert.IsNull(lessonMondayA.Grade);
            CollectionAssert.AreEqual(new string[] { "A" }, lessonMondayA.Weeks);
            Assert.AreEqual("BCDE", lessonMondayA.Teacher);

            var lessonFridayB = lessons.FirstOrDefault(x => x.Day == 5 && x.LessonStart == 1 && x.Weeks.Contains("B"));
            Assert.IsNotNull(lessonFridayB);
            Assert.AreEqual(2, lessonFridayB.LessonEnd);
            Assert.IsNull(lessonFridayB.Grade);
            CollectionAssert.AreEqual(new string[] { "B" }, lessonFridayB.Weeks);
            Assert.AreEqual("IJKL", lessonFridayB.Teacher);
        }

        [TestMethod]
        public async Task TestGradeAsync()
        {
            var html = LoadFile("test_05A.htm", "utf-8");
            var exporter = new TimetableExporter();

            var result = await exporter.ParseHtmlAsync(html, new TimetableExportSettings { Type = TimetableType.Grade });

            Assert.AreEqual("6. Periode", result.Period);
            Assert.AreEqual("05A", result.Objective);

            var lessons = result.Lessons;
            Assert.AreEqual(46, lessons.Count);

            var lessonMondayAB34 = lessons.FirstOrDefault(x => x.Day == 1 && x.LessonStart == 3);
            Assert.IsNotNull(lessonMondayAB34);
            Assert.AreEqual("E", lessonMondayAB34.Subject);
            Assert.AreEqual("CDEF", lessonMondayAB34.Teacher);
            Assert.AreEqual("EW003", lessonMondayAB34.Room);
            Assert.AreEqual(4, lessonMondayAB34.LessonEnd);
            Assert.AreEqual("05A", lessonMondayAB34.Grade);
            CollectionAssert.AreEqual(new string[] { "A", "B" }, lessonMondayAB34.Weeks.OrderBy(x => x).ToArray());

            var lessonMondayAB7 = lessons.FirstOrDefault(x => x.Day == 1 && x.LessonStart == 7);
            Assert.IsNotNull(lessonMondayAB7);
            Assert.AreEqual("LEZ", lessonMondayAB7.Subject);
            Assert.AreEqual("CDEF", lessonMondayAB7.Teacher);
            Assert.AreEqual("EW003", lessonMondayAB7.Room);
            Assert.AreEqual(7, lessonMondayAB7.LessonEnd);
            Assert.AreEqual("05A", lessonMondayAB7.Grade);
            CollectionAssert.AreEqual(new string[] { "A", "B" }, lessonMondayAB7.Weeks.OrderBy(x => x).ToArray());

            var lessonThursday78 = lessons.FirstOrDefault(x => x.Day == 4 && x.LessonStart == 7);
            Assert.IsNotNull(lessonThursday78);
            Assert.AreEqual("PK", lessonThursday78.Subject);
            Assert.AreEqual("BCDE", lessonThursday78.Teacher);
            Assert.AreEqual("EW003", lessonThursday78.Room);
            Assert.AreEqual(8, lessonThursday78.LessonEnd);
            Assert.AreEqual("05A", lessonThursday78.Grade);
            CollectionAssert.AreEqual(new string[] { "A", "B" }, lessonThursday78.Weeks.OrderBy(x => x).ToArray());

            var lessonTuesdayA34 = lessons.FirstOrDefault(x => x.Day == 2 && x.LessonStart == 3 && x.Weeks.Contains("A"));
            Assert.IsNotNull(lessonTuesdayA34);
            Assert.AreEqual("E", lessonTuesdayA34.Subject);
            Assert.AreEqual("CDEF", lessonTuesdayA34.Teacher);
            Assert.AreEqual("EW003", lessonTuesdayA34.Room);
            Assert.AreEqual(4, lessonTuesdayA34.LessonEnd);
            Assert.AreEqual("05A", lessonTuesdayA34.Grade);
            CollectionAssert.AreEqual(new string[] { "A" }, lessonTuesdayA34.Weeks);

            var lessonFridayA8 = lessons.FirstOrDefault(x => x.Day == 5 && x.LessonStart == 8);
            Assert.IsNotNull(lessonFridayA8);
            Assert.AreEqual(8, lessonFridayA8.LessonEnd);
        }
    }
}
