using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Extractor;
using Sprache;
using System;
using System.Linq;

namespace UntisExport.Test.Extractor
{
    [TestClass]
    public class ExamExtractorTest
    {
        [TestMethod]
        public void TestStartSequence()
        {
            var input = "0k3  5            ,42,\"Klausurname\",20210221,,,1,\"0~0~0\",\"0~0~0\",\"0~0~0\"";
            var output = ExamExtractor.ExamStartSequence.Parse(input);

            Assert.AreEqual(42, output.Number);
            Assert.AreEqual(3, output.LessonStart);
            Assert.AreEqual(5, output.LessonEnd);
            Assert.AreEqual("Klausurname", output.Name);
            Assert.AreEqual(new DateTime(2021, 2, 21), output.Date);
        }

        [TestMethod]

        public void TestCoursesLine()
        {
            var input = "kk1               ,\"1234~L-GK1~1\",\"4567~L-GK2~1\"";
            var output = ExamExtractor.Courses.Parse(input);

            Assert.AreEqual(2, output.Count());

            var first = output.First();
            Assert.AreEqual(1234, first.TuitionNumber);
            Assert.AreEqual("L-GK1", first.CourseName);
            Assert.AreEqual(1, first.TuitionIndex);
        }

        [TestMethod]
        public void TestStudentsLine()
        {
            var input = "kk2               ,\"EFDummy\",\"Q1Dummy\"";
            var output = ExamExtractor.Students.Parse(input);

            CollectionAssert.AreEqual(new string[] { "EFDummy", "Q1Dummy" }, output.ToArray());
        }

        [TestMethod]
        public void TestSupervisionLine()
        {
            var input = "kk3               ,\"TEST\",\"LEHRER\"";
            var output = ExamExtractor.Supervisions.Parse(input);

            CollectionAssert.AreEqual(new string[] { "TEST", "LEHRER" }, output.ToArray());
        }

        [TestMethod]
        public void TestRoomLine()
        {
            var input = "kk4               ,\"R001\",\"R001\"";
            var output = ExamExtractor.Rooms.Parse(input);

            CollectionAssert.AreEqual(new string[] { "R001", "R001" }, output.ToArray());
        }
    }
}
