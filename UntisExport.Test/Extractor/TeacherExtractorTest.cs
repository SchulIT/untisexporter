using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Extractor;
using Sprache;
using System.Linq;

namespace UntisExport.Test.Extractor
{
    [TestClass]
    public class TeacherExtractorTest
    {
        [TestMethod]
        public void TestPeriod()
        {
            var input = "0pL ,1-";
            var output = PeriodAwareTeacherExtractor.TeacherPeriod.Parse(input);

            CollectionAssert.AreEqual(new int[] { 1 }, output.ToArray());
        }

        [TestMethod]
        public void TestPeriodSection()
        {
            var input = "PE ,1-2-";
            var output = PeriodAwareTeacherExtractor.PeriodSection.Parse(input);

            CollectionAssert.AreEqual(new int[] { 1, 2 }, output.ToArray());
        }

        [TestMethod]
        public void TestQualificationLine()
        {
            var input = "BF     ,\"LEZ\",0";
            var output = PeriodAwareTeacherExtractor.QualificationLine.Parse(input);

            Assert.AreEqual("LEZ", output);
        }

        [TestMethod]
        public void TestQualificationLineWithoutQuotes()
        {
            var input = "BF     ,LEZ,0";
            var output = PeriodAwareTeacherExtractor.QualificationLine.Parse(input);

            Assert.AreEqual("LEZ", output);
        }

        [TestMethod]
        public void TestTeacherBasic()
        {
            var input = "00L   ,SchGr,\"Schlierenzauer\",,,,,,,,,n,,,78,402881535";
            var output = PeriodAwareTeacherExtractor.TeacherBasic.Parse(input);

            Assert.AreEqual("SchGr", output.Acronym);
            Assert.AreEqual("Schlierenzauer", output.Lastname);
        }

        [TestMethod]
        public void TestTeacherExtendedLine()
        {
            var input = "Le ,,,\"123456\",\"Status\",Robert,,,,,0,0,,,,,,,,,TZ";
            var output = PeriodAwareTeacherExtractor.TeacherExtendedLine.Parse(input);

            var expected = new string[] { "", "", "123456", "Status", "Robert" };

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(output.ElementAt(i), expected[i], $"Assert elements at index {i} are same.");
            }
        }

        [TestMethod]
        public void TestNonTeacherStartSequenceWithTeacherBeginning()
        {
            var input = "0pL,...";
            var output = PeriodAwareTeacherExtractor.NonTeacherStartSequence.TryParse(input);

            Assert.IsFalse(output.WasSuccessful);
        }

        [TestMethod]
        public void TestNonTeacherStartSequenceWithAnyLine()
        {
            var input = "AnyLine";
            var output = PeriodAwareTeacherExtractor.NonTeacherStartSequence.TryParse(input);

            Assert.IsFalse(output.WasSuccessful);
        }

        [TestMethod]
        public void TestNonTeacherStartSequenceWithoutTeacherBeginning()
        {
            var input = "0pR,...";
            var output = PeriodAwareTeacherExtractor.NonTeacherStartSequence.TryParse(input);

            Assert.IsTrue(output.WasSuccessful);
        }
    }
}
