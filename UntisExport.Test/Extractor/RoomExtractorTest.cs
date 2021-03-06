using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Extractor;
using Sprache;

namespace UntisExport.Test.Extractor
{
    [TestClass]
    public class RoomExtractorTest
    {
        [TestMethod]
        public void TestRoomData()
        {
            var input = "00R   ,\"R001\",\"Oberstufenraum, klein\",\"ALT\",,,,,,,,,,,0,0,,,,,0";
            var output = PeriodAwareRoomExtractor.Room.Parse(input);

            Assert.AreEqual("R001", output.Name);
            Assert.AreEqual("Oberstufenraum, klein", output.LongName);
            Assert.AreEqual("ALT", output.AlternativeRoom);
        }

        [TestMethod]
        public void TestRoomCapacity()
        {
            var input = "RA16   2";
            var output = PeriodAwareRoomExtractor.RoomCapacityWeight.Parse(input);

            Assert.IsNotNull(output.Capacity);
            Assert.AreEqual(16, output.Capacity.Value);
            Assert.AreEqual(2, output.Weight);
        }

        [TestMethod]
        public void TestEmptyRoomCapacity()
        {
            var input = "RA   2";
            var output = PeriodAwareRoomExtractor.RoomCapacityWeight.Parse(input);

            Assert.IsNull(output.Capacity);
            Assert.AreEqual(2, output.Weight);
        }

        [TestMethod]
        public void TestRoomFloor()
        {
            var input = "Ra ,\"Alt\"";
            var output = PeriodAwareRoomExtractor.RoomFloor.Parse(input);

            Assert.AreEqual("Alt", output);
        }
    }
}
