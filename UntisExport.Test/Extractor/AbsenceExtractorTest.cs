using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Extractor;
using SchulIT.UntisExport.Model;
using Sprache;
using System;

namespace UntisExport.Test.Extractor
{
    [TestClass]
    public class AbsenceExtractorTest
    {
        [TestMethod]
        public void TestParseAbsenceLine()
        {
            var input = "0A ,42,L,\"TEST\",20200212,20200213,\"P\",\"Test\",1,8,0,0,0,,11365468,0,0,0,,0";
            var output = AbsenceExtractor.Absence.Parse(input);

            Assert.AreEqual(42, output.Number);
            Assert.AreEqual(AbsenceType.Teacher, output.Type);
            Assert.AreEqual("TEST", output.Objective);
            Assert.AreEqual(new DateTime(2020, 2, 12), output.Start);
            Assert.AreEqual(new DateTime(2020, 2, 13), output.End);
            Assert.AreEqual("P", output.Reason);
            Assert.AreEqual("Test", output.Text);
            Assert.AreEqual(1, output.LessonStart);
            Assert.AreEqual(8, output.LessonEnd);
        }

        [TestMethod]
        public void TestParseAbsenceLineWithoutText()
        {
            var input = "0A ,42,L,\"TEST\",20200212,20200213,\"P\",,1,8,0,0,0,,11365468,0,0,0,,0";
            var output = AbsenceExtractor.Absence.Parse(input);

            Assert.AreEqual(42, output.Number);
            Assert.AreEqual(AbsenceType.Teacher, output.Type);
            Assert.AreEqual("TEST", output.Objective);
            Assert.AreEqual(new DateTime(2020, 2, 12), output.Start);
            Assert.AreEqual(new DateTime(2020, 2, 13), output.End);
            Assert.AreEqual("P", output.Reason);
            Assert.IsNull(output.Text);
            Assert.AreEqual(1, output.LessonStart);
            Assert.AreEqual(8, output.LessonEnd);
        }

        [TestMethod]
        public void TestParseNonAbsenceLine()
        {
            var input = "0A ,42,L,\"TEST\",20200212,20200213,\"P\",\"Test\",1,8,1,0,0,,11365468,0,0,0,,0";

            Assert.ThrowsException<ParseException>(() => AbsenceExtractor.Absence.Parse(input));
        }
    }
}
