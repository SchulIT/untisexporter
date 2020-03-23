using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport.Substitutions.Gpu;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UntisExport.Test.Substitutions.Gpo
{
    [TestClass]
    public class SubstitionExporterTest : AbstractTestCase
    {
        [TestMethod]
        public async Task TestEmptyData()
        {
            var exporter = new SubstitutionExporter();
            var settings = new SubstitutionExportSettings();

            var gpu = LoadFile("GPU014-empty.TXT", "utf-8");
            var result = await exporter.ParseGpuAsync(gpu, settings);
            var substitutions = result.Substitutions;

            Assert.AreEqual(0, substitutions.Count());
        }

        [TestMethod]
        public async Task TestNormalData()
        {
            var exporter = new SubstitutionExporter();
            var settings = new SubstitutionExportSettings();

            var gpu = LoadFile("GPU014.TXT", "utf-8");
            var result = await exporter.ParseGpuAsync(gpu, settings);
            var substitutions = result.Substitutions;

            Assert.AreEqual(3, substitutions.Count());

            var first = substitutions.FirstOrDefault(x => x.Id == 1);
            Assert.IsNotNull(first);
            Assert.AreEqual(DateTime.Parse("2009-08-24"), first.Date);
            Assert.AreEqual(1, first.LessonStart);
            Assert.AreEqual(1, first.LessonEnd);
            CollectionAssert.AreEqual(new string[] { "BecSa" }, first.Teachers.ToArray());
            CollectionAssert.AreEqual(new string[] { "BecHe" }, first.ReplacementTeachers.ToArray());
            Assert.AreEqual("BIO", first.Subject);
            Assert.AreEqual("M", first.ReplacementSubject);
            Assert.AreEqual("R B3", first.Room);
            Assert.AreEqual("R B4", first.ReplacementRoom);
            CollectionAssert.AreEqual(new string[] { "10c", "10a" }, first.Grades.ToArray());
            CollectionAssert.AreEqual(new string[] { "10c", "10a", "10b" }, first.ReplacementGrades.ToArray());
            Assert.IsNull(first.Remark);
            Assert.AreEqual("Vertretung", first.Type);

            var fourth = substitutions.FirstOrDefault(x => x.Id == 4);
            Assert.IsNotNull(fourth);
            CollectionAssert.AreEqual(new string[] { "8e" }, fourth.Grades.ToArray());
            CollectionAssert.AreEqual(Array.Empty<string>(), fourth.ReplacementGrades.ToArray());
            Assert.AreEqual("Lorem ipsum.", fourth.Remark);

            var fifth = substitutions.FirstOrDefault(x => x.Id == 5);
            Assert.IsNotNull(fifth);
            Assert.AreEqual("Entfall", fifth.Type);
        }
    }
}
