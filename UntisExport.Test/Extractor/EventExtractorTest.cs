using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Extractor;
using Sprache;
using System;

namespace UntisExport.Test.Extractor
{
    [TestClass]
    public class EventExtractorTest
    {
        [TestMethod]
        public void TestStartLine()
        {
            var input = "0W ,42,,,20201008,20201009,\"type\",\"Lorem ipsum\",1,8,,,,,441041418,0,0";
            var output = EventExtractor.StartSequence.Parse(input);

            Assert.AreEqual(42, output.Number);
            Assert.AreEqual(new DateTime(2020, 10, 8), output.StartDate);
            Assert.AreEqual(new DateTime(2020, 10, 9), output.EndDate);
            Assert.AreEqual(1, output.StartLesson);
            Assert.AreEqual(8, output.EndLesson);
            Assert.AreEqual("Lorem ipsum", output.Text);
        }
    }
}
