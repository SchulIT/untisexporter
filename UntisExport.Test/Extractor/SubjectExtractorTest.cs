using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Extractor;
using Sprache;

namespace UntisExport.Test.Extractor
{
    [TestClass]
    public class SubjectExtractorTest
    {
        [TestMethod]
        public void TestSubjectLine()
        {
            var input = "00F   ,\"KR\",\"Kath.Religion\",,,,,,,,,,,,0,434930751,,,,,0";
            var output = SubjectExtractor.Subject.Parse(input);

            Assert.AreEqual("KR", output.Abbreviation);
            Assert.AreEqual("Kath.Religion", output.Name);
        }

    }
}
