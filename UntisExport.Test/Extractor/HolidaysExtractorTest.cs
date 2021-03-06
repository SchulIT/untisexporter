using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Extractor;
using SchulIT.UntisExport.Model;
using Sprache;
using System;
using System.Collections.Generic;
using System.Text;

namespace UntisExport.Test.Extractor
{
    [TestClass]
    public class HolidaysExtractorTest
    {

        [TestMethod]
        public void TestLine()
        {
            var input = "FE1  ,\"Herbst\",\"Herbstferien\",20201012,20201024,,10,0";
            var output = HolidaysExtractor.Holiday.Parse(input);

            Assert.AreEqual("Herbst", output.Name);
            Assert.AreEqual("Herbstferien", output.LongName);
            Assert.AreEqual(new DateTime(2020, 10, 12), output.Start);
            Assert.AreEqual(new DateTime(2020, 10, 24), output.End);
            Assert.AreEqual(HolidayType.Ferien, output.Type);
            Assert.AreEqual(1, output.WeekAfterHolidays);
            Assert.IsFalse(output.ContinueWeekNumbering);
        }

        [TestMethod]
        public void TestLineWithFollowingWeek()
        {
            var input = "FE0  ,\"3.10.\",\"Tag der deutschen Einheit\",20201003,20201003,F,0,0";
            var output = HolidaysExtractor.Holiday.Parse(input);

            Assert.AreEqual(0, output.WeekAfterHolidays);
            Assert.IsTrue(output.ContinueWeekNumbering);
            Assert.AreEqual(HolidayType.Feiertag, output.Type);
        }
    }
}
