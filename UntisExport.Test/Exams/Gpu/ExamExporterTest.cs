using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Exams;
using SchulIT.UntisExport.Exams.Gpu;
using SchulIT.UntisExport.Tuitions;
using SchulIT.UntisExport.Tuitions.Gpu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UntisExport.Test.Exams.Gpu
{
    [TestClass]
    public class ExamExporterTest : AbstractTestCase
    {

        public Task<IEnumerable<Tuition>> ParseTuitionsAsync()
        {
            var exporter = new TuitionExporter();
            var settings = new TuitionExportSettings();
            var gpu = LoadFile("GPU002.TXT");

            return exporter.ParseGpuAsync(gpu, settings);
        }

        [TestMethod]
        public async Task TestNormalDataWithoutTuitionsAsync()
        {
            var exporter = new ExamExporter();
            var settings = new ExamExportSettings();
            var gpu = LoadFile("GPU017.TXT");

            var result = await exporter.ParseGpuAsync(gpu, settings);
            RunAssertions(result.Exams, false);
        }

        [TestMethod]
        public async Task TestNormalDataWithoutTuitionsWithSemicolonAsync()
        {
            var exporter = new ExamExporter();
            var settings = new ExamExportSettings
            {
                Delimiter = ";"
            };
            var gpu = LoadFile("GPU017-semicolon.TXT");

            var result = await exporter.ParseGpuAsync(gpu, settings);
            RunAssertions(result.Exams, false);
        }

        private void RunAssertions(IEnumerable<Exam> exams, bool withTuitions)
        {
            Assert.IsNotNull(exams);

            var first = exams.FirstOrDefault();
            Assert.IsNotNull(first);

            Assert.AreEqual(DateTime.Parse("2009-09-29"), first.Date);
            Assert.AreEqual(3, first.LessonStart);
            Assert.AreEqual(4, first.LessonEnd);
            CollectionAssert.AreEqual(new string[] { "E  L2", "E  L4", "F  L1", "M  L1", "B  L2", "B  L4" }, first.Courses.ToArray());
            CollectionAssert.AreEqual(new string[] { "BecSa", "BecSa~CurMa" }, first.Supervisions.ToArray());
            CollectionAssert.AreEqual(new string[] { "R 5b", "R 5b" }, first.Rooms.ToArray());

            if (withTuitions)
            {
                CollectionAssert.AreEqual(new string[] { "11" }, first.Grades.ToArray());
            }
            else
            {
                Assert.AreEqual(Array.Empty<string>(), first.Grades);
            }
        }

        /// <summary>
        /// The second line has courses, that do not match their tuition numbers.
        /// Those grades should not be resolved.
        /// </summary>
        /// <param name="exams"></param>
        private void RunAssertionForNotMatchedTuitions(IEnumerable<Exam> exams)
        {
            var second = exams.Skip(1).FirstOrDefault();
            Assert.IsNotNull(second);

            CollectionAssert.AreEqual(Array.Empty<string>(), second.Grades.ToArray());
        }

        [TestMethod]
        public async Task TestNormalDataWithTuitionsAsync()
        {
            var exporter = new ExamExporter();
            var settings = new ExamExportSettings();
            var gpu = LoadFile("GPU017.TXT");

            var result = await exporter.ParseGpuAsync(gpu, settings, await ParseTuitionsAsync());
            RunAssertions(result.Exams, true);
            RunAssertionForNotMatchedTuitions(result.Exams);
        }
    }
}
