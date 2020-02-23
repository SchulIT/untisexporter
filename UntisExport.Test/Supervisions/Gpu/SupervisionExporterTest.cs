using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Supervisions.Gpu;
using System.Linq;
using System.Threading.Tasks;

namespace UntisExport.Test.Supervisions.Gpu
{
    [TestClass]
    public class SupervisionExporterTest : AbstractTestCase
    {
        [TestMethod]
        public async Task TestNormalData()
        {
            var exporter = new SupervisionExporter();
            var settings = new SupervisionExportSettings();

            var gpu = LoadFile("GPU009.TXT");
            var supervisions = await exporter.ParseGpuAsync(gpu, settings);

            Assert.AreEqual(20, supervisions.Count());

            var mondayBeforeFirstLesson = supervisions.FirstOrDefault(x => x.WeekDay == 1 && x.Lesson == 1 && x.Location == "Alt");
            Assert.IsNotNull(mondayBeforeFirstLesson);
            Assert.AreEqual("AnnKo", mondayBeforeFirstLesson.Teacher);
            Assert.IsNull(mondayBeforeFirstLesson.Weeks);

            var wednesdayBeforeFifthLessonEveryWeek = supervisions.FirstOrDefault(x => x.WeekDay == 3 && x.Lesson == 5 && x.Teacher == "AnnKo");
            Assert.IsNotNull(wednesdayBeforeFifthLessonEveryWeek);
            Assert.IsNotNull(wednesdayBeforeFifthLessonEveryWeek.Weeks);
            CollectionAssert.AreEqual(Enumerable.Range(1, 47).ToArray(), wednesdayBeforeFifthLessonEveryWeek.Weeks.ToArray()); ;
        }

        [TestMethod]
        public async Task TestNormalDataWithSemicolon()
        {
            var exporter = new SupervisionExporter();
            var settings = new SupervisionExportSettings
            {
                Delimiter = ";"
            };

            var gpu = LoadFile("GPU009-semicolon.TXT");
            var supervisions = await exporter.ParseGpuAsync(gpu, settings);

            Assert.AreEqual(20, supervisions.Count());
        }
    }
}
