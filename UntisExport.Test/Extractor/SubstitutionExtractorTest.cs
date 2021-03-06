using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Extractor;
using Sprache;
using System;
using System.Collections.Generic;
using System.Text;

namespace UntisExport.Test.Extractor
{
    [TestClass]
    public class SubstitutionExtractorTest
    {
        [TestMethod]
        public void TestStartLine()
        {
            var input = "0V ,42,20200812,1,\"NEU\",\"ALT\",\"IF\",\"R001\",1337,\"Text\",,0,0,RGg,,0,,442391834,0,0,,,,0,0,,0.00000,0.00000,0.00000";
            var output = SubstitutionExtractor.Substitution.Parse(input);

            Assert.AreEqual(42, output.Number);
            Assert.AreEqual(1337, output.TuitionNumber);
            Assert.AreEqual(new DateTime(2020, 8, 12), output.Date);
            Assert.AreEqual(1, output.Lesson);
            Assert.AreEqual("ALT", output.Teacher);
            Assert.AreEqual("NEU", output.ReplacementTeacher);
            Assert.AreEqual("IF", output.ReplacementSubject);
            Assert.AreEqual("Text", output.Text);
        }

        [TestMethod]
        public void TestStartLineWithoutText()
        {
            var input = "0V ,42,20200812,3,,\"ALT\",,\"R001\",1337,,,0,0,ELg,,0,,440481151,0,0,,,,0,0,,0.00000,0.00000,0.00000";
            var output = SubstitutionExtractor.Substitution.Parse(input);

            Assert.AreEqual(42, output.Number);
            Assert.AreEqual(1337, output.TuitionNumber);
            Assert.AreEqual(new DateTime(2020, 8, 12), output.Date);
            Assert.AreEqual(3, output.Lesson);
            Assert.AreEqual("ALT", output.Teacher);
            Assert.IsNull(output.ReplacementTeacher);
            Assert.IsNull(output.ReplacementSubject);
            Assert.IsNull(output.Text);
        }
    }
}
