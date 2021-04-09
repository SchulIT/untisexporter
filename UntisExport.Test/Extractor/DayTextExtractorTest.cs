using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Extractor;
using SchulIT.UntisExport.Model;
using Sprache;
using System;
using System.Linq;

namespace UntisExport.Test.Extractor
{
    [TestClass]
    public class DayTextExtractorTest
    {

        [TestMethod]
        public void TestStartSequence()
        {
            var input = "0K ,20200219";
            var output = DayInformationExtractor.DayTextStartSequence.Parse(input);

            Assert.AreEqual(new DateTime(2020, 2, 19), output.Date);
        }

        [TestMethod]
        public void TestStartSequenceWithAdditionalInfo()
        {
            var input = "0K ,20200219,,\"F\",,\"Lorem ipsum...\"";
            var output = DayInformationExtractor.DayTextStartSequence.Parse(input);

            Assert.AreEqual(new DateTime(2020, 2, 19), output.Date);
            Assert.AreEqual("F", output.Reason);
            Assert.AreEqual("Lorem ipsum...", output.Note);
        }

        [TestMethod]
        public void TestStartSequenceWithFullInfo()
        {
            var input = "0K ,20200219,,\"F\",,\"Lorem ipsum...\",,,U";
            var output = DayInformationExtractor.DayTextStartSequence.Parse(input);

            Assert.AreEqual(new DateTime(2020, 2, 19), output.Date);
            Assert.AreEqual("F", output.Reason);
            Assert.AreEqual("Lorem ipsum...", output.Note);
            Assert.AreEqual(DayType.Unterrichtsfrei, output.Type);
        }

        [TestMethod]
        public void TestTextLine()
        {
            var input = "Kt             \"Test \"mit Anführungszeichen\"\"";
            var output = DayInformationExtractor.Text.Parse(input);

            Assert.AreEqual("Test \"mit Anführungszeichen\"", output);
        }

        [TestMethod]
        public void TestGuidLine()
        {
            var input = "Kg{D0045896-679F-4C2C-A789-D7503085C5D3}~0";
            var output = DayInformationExtractor.Guid.Parse(input);

            Assert.AreEqual("D0045896-679F-4C2C-A789-D7503085C5D3", output.Guid);
            Assert.AreEqual(0, output.Days);
        }

        [TestMethod]
        public void TestGradeLine()
        {
            var input = "KnK\"08A, 08B\"";
            var output = DayInformationExtractor.Grades.Parse(input);

            CollectionAssert.AreEqual(new string[] { "08A", "08B" }, output.ToArray());
        }

        [TestMethod]
        public void TestLessonsLine()
        {
            var input = "Ks ,,1,2,3";
            var output = DayInformationExtractor.Lessons.Parse(input);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, output.ToArray());
        }
    }
}
