using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Tuitions;
using SchulIT.UntisExport.Tuitions.Gpu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UntisExport.Test.Tuitions.Gpu
{
    [TestClass]
    public class TuitionsExporterTest : AbstractTestCase
    {
        [TestMethod]
        public async Task TestNormalDataAsync()
        {
            var exporter = new TuitionExporter();
            var settings = new TuitionExportSettings();

            var gpu = LoadFile("GPU002.TXT");
            var tuitions = await exporter.ParseGpuAsync(gpu, settings);

            RunAssertions(tuitions);
        }

        [TestMethod]
        public async Task TestNormalDataWithSemiColonAsync()
        {
            var exporter = new TuitionExporter();
            var settings = new TuitionExportSettings
            {
                Delimiter = ";"
            };

            var gpu = LoadFile("GPU002-semicolon.TXT");
            var tuitions = await exporter.ParseGpuAsync(gpu, settings);

            RunAssertions(tuitions);
        }

        public void RunAssertions(IEnumerable<Tuition> tuitions)
        {
            Assert.AreEqual(1057, tuitions.Count());

            var first = tuitions.First();
            Assert.AreEqual(1418, first.Id);
            Assert.AreEqual("12", first.Grade);
            Assert.AreEqual("CoeJM", first.Teacher);
            Assert.AreEqual("D  G4", first.Subject);
            //Assert.AreEqual("MSS 2", first.Room);
        }
    }
}
