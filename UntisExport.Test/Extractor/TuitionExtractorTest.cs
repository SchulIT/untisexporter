using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Extractor;
using Sprache;
using System;
using System.Linq;

namespace UntisExport.Test.Extractor
{
    [TestClass]
    public class TuitionExtractorTest
    {
        [TestMethod]
        public void TestStartSequence()
        {
            var input = "0pU";
            var output = PeriodAwareTuitionExtractor.TuitionPeriod.TryParse(input);

            Assert.IsTrue(output.WasSuccessful);
        }

        [TestMethod]
        public void TestTuitionNumber()
        {
            var input = "0U ,1418,UW,UW,,,,,,20200801,20201231,,,Bc,,,,1418,402251226";
            var output = PeriodAwareTuitionExtractor.Info.Parse(input);

            Assert.AreEqual(1418, output.Number);
            Assert.AreEqual(new DateTime(2020, 8, 1), output.StartDate);
            Assert.AreEqual(new DateTime(2020, 12, 31), output.EndDate);
        }

        [TestMethod]
        public void TestSingleSubject()
        {
            var input = "Uf ,,\"LEZ\"";
            var output = PeriodAwareTuitionExtractor.Subjects.Parse(input);

            CollectionAssert.AreEqual(new string[] { "LEZ" }, output.ToArray());
        }

        [TestMethod]
        public void TestMultipleSubject()
        {
            var input = "Uf ,,\"LEZ\",,D";
            var output = PeriodAwareTuitionExtractor.Subjects.Parse(input);

            CollectionAssert.AreEqual(new string[] { "LEZ", "D" }, output.ToArray());
        }

        [TestMethod]
        public void TestParseNumberWithFixedLengthOneDigit()
        {
            var input = "1 ";
            var output = PeriodAwareTuitionExtractor.TwoDigitNumber.Parse(input);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void TestParseNumberWithFixedLengthTwoDigits()
        {
            var input = "12";
            var output = PeriodAwareTuitionExtractor.TwoDigitNumber.Parse(input);

            Assert.AreEqual(12, output);
        }

        [TestMethod]
        public void TestParseNumberWithFixedLengthEmpty()
        {
            var input = "  ";
            var output = PeriodAwareTuitionExtractor.OptionalTwoDigitNumber.Parse(input);

            Assert.IsNull(output);
        }

        [TestMethod]
        public void TestParseTuitionDataMatrix()
        {
            var input = "Uz1 1 2 304  0    0   n,,,,,0,,\"SW-ZK1_Q2\"";
            var output = PeriodAwareTuitionExtractor.TuitionData.Parse(input);

            Assert.AreEqual(1, output.GradeStartIndex);
            Assert.AreEqual(2, output.GradeEndIndex);
            Assert.AreEqual(30, output.SubjectIndex);
            Assert.AreEqual(1, output.TeacherIndex);
            Assert.AreEqual(4, output.RoomIndex);
            Assert.AreEqual("SW-ZK1_Q2", output.StudentGroup);
        }

        [TestMethod]
        public void TestParseTuitionDataMatrixMultipleGradeIndices()
        {
            var input = "Uz1     304  0    0   n,,,,,0,,,3,4,5";
            var output = PeriodAwareTuitionExtractor.TuitionData.Parse(input);

            Assert.IsNull(output.GradeStartIndex);
            Assert.IsNull(output.GradeEndIndex);
            Assert.AreEqual(30, output.SubjectIndex);
            Assert.AreEqual(1, output.TeacherIndex);
            Assert.AreEqual(4, output.RoomIndex);
            Assert.IsNull(output.StudentGroup);
            CollectionAssert.AreEqual(new int[] { 3, 4, 5 }, output.GradeIndices);
        }

        [TestMethod]
        public void TestTimetable()
        {
            var input = "UZ00 ,1/5,\"A\",\"A310\",\"A311\",\"A103\",\"A008\",\"T103\",\"A312\",\"A209\",\"A010\",\"A304\",\"N109\"";
            var output = PeriodAwareTuitionExtractor.Timetable.Parse(input);

            Assert.AreEqual(1, output.Day);
            Assert.AreEqual(5, output.Lesson);
            Assert.AreEqual("A", output.Week);
            Assert.AreEqual(10, output.Rooms.Count);
        }

        [TestMethod]
        public void TestTimetableWithoutWeeks()
        {
            var input = "UZ00 ,1/5";
            var output = PeriodAwareTuitionExtractor.Timetable.Parse(input);

            Assert.AreEqual(1, output.Day);
            Assert.AreEqual(5, output.Lesson);
            Assert.IsNull(output.Week);
            Assert.AreEqual(0, output.Rooms.Count);
        }
    }
}
