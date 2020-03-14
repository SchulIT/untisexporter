using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Rooms;
using SchulIT.UntisExport.Rooms.Gpu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UntisExport.Test.Rooms.Gpu
{
    [TestClass]
    public class RoomExporterTest : AbstractTestCase
    {
        [TestMethod]
        public async Task TestCommaAsync()
        {
            var csv = LoadFile("GPU005.TXT", "utf-8");
            var exporter = new RoomExporter();
            var settings = new RoomExportSettings { Delimiter = "," };

            var rooms = await exporter.ParseGpuAsync(csv, settings);
            RunTests(rooms);
        }

        [TestMethod]
        public async Task TestSemicolonAsync()
        {
            var csv = LoadFile("GPU005-semicolon.TXT", "utf-8");
            var exporter = new RoomExporter();
            var settings = new RoomExportSettings { Delimiter = ";" };

            var rooms = await exporter.ParseGpuAsync(csv, settings);
            RunTests(rooms);
        }

        private void RunTests(IEnumerable<Room> rooms)
        {
            Assert.AreEqual(121, rooms.Count());

            var rSIa = rooms.FirstOrDefault(x => x.Name == "R SIa");
            Assert.IsNotNull(rSIa);
            Assert.AreEqual("Einstiegsraum 5", rSIa.LongName);
            Assert.AreEqual(10, rSIa.Capacity);

            var r5 = rooms.FirstOrDefault(x => x.Name == "R 5");
            Assert.IsNotNull(r5);
            Assert.AreEqual(string.Empty, r5.LongName);
            Assert.IsNull(r5.Capacity);
        }
    }
}
