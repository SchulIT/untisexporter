using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Extractor;
using Sprache;
using System;

namespace UntisExport.Test.Extractor
{
    [TestClass]
    public class SettingsExtractorTest
    {
        [TestMethod]
        public void TestParseSettings()
        {
            var input = "AA02 20200826 20210702 115 8  2  0  A0 0  0   0     2         DE";
            var output = SettingsExtractor.Settings.Parse(input);

            Assert.AreEqual(new DateTime(2020, 8, 26), output.Start);
            Assert.AreEqual(new DateTime(2021, 7, 2), output.End);
            Assert.AreEqual(11, output.Periodicity);
            Assert.AreEqual(5, output.NumberOfDays);
            Assert.AreEqual(8, output.NumberOfLessonsPerDay);
            Assert.AreEqual(10, output.StartWeek);
            Assert.AreEqual(0, output.NumberOfFirstLesson);
            Assert.AreEqual(DayOfWeek.Tuesday, output.FirstSchoolDayOfWeek);
        }

        [TestMethod]
        public void TestParseSettings2()
        {
            var input = "AA02 20200826 20210702 115 8  2  0  A1 0  0   0     2         DE";
            var output = SettingsExtractor.Settings.Parse(input);

            Assert.AreEqual(new DateTime(2020, 8, 26), output.Start);
            Assert.AreEqual(new DateTime(2021, 7, 2), output.End);
            Assert.AreEqual(11, output.Periodicity);
            Assert.AreEqual(5, output.NumberOfDays);
            Assert.AreEqual(8, output.NumberOfLessonsPerDay);
            Assert.AreEqual(10, output.StartWeek);
            Assert.AreEqual(1, output.NumberOfFirstLesson);
            Assert.AreEqual(DayOfWeek.Tuesday, output.FirstSchoolDayOfWeek);
        }
    }
}
