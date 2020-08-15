using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulIT.UntisExport;
using SchulIT.UntisExport.Substitutions;
using SchulIT.UntisExport.Substitutions.Html;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UntisExport.Test.Substitutions.Html
{
    [TestClass]
    public class SubstitutionExporterTest : AbstractTestCase
    {
        [TestMethod]
        public async Task TestEmptyData()
        {
            var exporter = new SubstitutionExporter();
            var settings = new SubstitutionExportSettings();

            var html = GetHtmlWithEmptyData();
            var result = await exporter.ParseHtmlAsync(html, settings);

            Assert.AreEqual(0, result.Infotexts.Count);
            Assert.AreEqual(0, result.Substitutions.Count);
            
        }

        [TestMethod]
        public async Task TestNormalData()
        {
            var exporter = new SubstitutionExporter();
            var settings = new SubstitutionExportSettings();
            settings.AbsenceSettings.ParseAbsences = false;

            var html = GetNormalHtmlText();
            var result = await exporter.ParseHtmlAsync(html, settings);

            Assert.AreEqual(4, result.Infotexts.Count);
            Assert.AreEqual(DateTime.Parse("2019-06-10"), result.Infotexts[0].Date);
            Assert.AreEqual("Unterrichtsfrei 1,3-5 Std.", result.Infotexts[0].Text);
            Assert.AreEqual(DateTime.Parse("2019-06-10"), result.Infotexts[1].Date);
            Assert.AreEqual("Infotext 1", result.Infotexts[1].Text);
            Assert.AreEqual(DateTime.Parse("2019-06-10"), result.Infotexts[2].Date);
            Assert.AreEqual("Infotext 2", result.Infotexts[2].Text);
            Assert.AreEqual(DateTime.Parse("2019-06-10"), result.Infotexts[3].Date);
            Assert.AreEqual("Infotext mit HTML", result.Infotexts[3].Text);

            Assert.AreEqual(8, result.Substitutions.Count);

            var supervision = result.Substitutions.FirstOrDefault(x => x.Id == 1);
            Assert.IsNotNull(supervision);
            Assert.IsTrue(supervision.IsSupervision);
            Assert.AreEqual(0, supervision.LessonStart);
            Assert.AreEqual(1, supervision.LessonEnd);
            Assert.AreEqual(1, supervision.Teachers.Count);
            Assert.IsTrue(supervision.Teachers.Contains("XXXX"));
            Assert.AreEqual(1, supervision.ReplacementTeachers.Count);
            Assert.IsTrue(supervision.ReplacementTeachers.Contains("XXXX"));

            var substitution = result.Substitutions.FirstOrDefault(x => x.Id == 2);
            Assert.IsNotNull(substitution);
            Assert.AreEqual(1, substitution.LessonStart);
            Assert.AreEqual(2, substitution.LessonEnd);
            Assert.AreEqual("GE-GK1", substitution.Subject);
            Assert.AreEqual("GE-GK1", substitution.ReplacementSubject);
            Assert.AreEqual("R001", substitution.Room);
            Assert.AreEqual("R002", substitution.ReplacementRoom);
            Assert.AreEqual("Raum-Vtr.", substitution.Type);
            Assert.AreEqual("Lorem ipsum", substitution.Remark);
            CollectionAssert.AreEqual(new string[] { "EF" }, substitution.Grades.ToArray());
            CollectionAssert.AreEqual(new string[] { "EF" }, substitution.ReplacementGrades.ToArray());
            CollectionAssert.AreEqual(new string[] { "XXXX" }, substitution.Teachers.ToArray());
            CollectionAssert.AreEqual(new string[] { "YYYY" }, substitution.ReplacementTeachers.ToArray());

            var substitutionWithMultipleTeachers = result.Substitutions.FirstOrDefault(x => x.Id == 7);
            Assert.IsNotNull(substitutionWithMultipleTeachers);
            Assert.AreEqual(1, substitutionWithMultipleTeachers.LessonStart);
            Assert.AreEqual(9, substitutionWithMultipleTeachers.LessonEnd);
            Assert.AreEqual(3, substitutionWithMultipleTeachers.Teachers.Count);
            CollectionAssert.AreEqual(new string[] { "XXXX", "YYYY", "ZZZZ" }, substitutionWithMultipleTeachers.Teachers.ToArray());
            Assert.AreEqual(3, substitutionWithMultipleTeachers.ReplacementTeachers.Count);
            CollectionAssert.AreEqual(new string[] { "XXXX", "YYYY", "ZZZZ" }, substitutionWithMultipleTeachers.ReplacementTeachers.ToArray());

            var substitutionWithMultipleStudyGroups = result.Substitutions.FirstOrDefault(x => x.Id == 6);
            Assert.IsNotNull(substitutionWithMultipleStudyGroups);
            Assert.AreEqual(null, substitutionWithMultipleStudyGroups.ReplacementSubject, "ReplacementSubject must be null as its value is '---'");
            Assert.AreEqual(3, substitutionWithMultipleStudyGroups.Grades.Count);
            CollectionAssert.AreEqual(new string[] { "08A", "08B", "08C" }, substitutionWithMultipleStudyGroups.Grades.ToArray());
            Assert.AreEqual(0, substitutionWithMultipleStudyGroups.ReplacementGrades.Count, "Replacement grades must be empty as its value is embraced with ( and )");
            Assert.AreEqual(0, substitutionWithMultipleStudyGroups.ReplacementTeachers.Count, "ReplacementTeachers must be empty as its value is '---'");
        }

        [TestMethod]
        public async Task TestNormalDataWithAbsencesNoAbsenceParsing()
        {
            var exporter = new SubstitutionExporter();
            var settings = new SubstitutionExportSettings();
            settings.AbsenceSettings.ParseAbsences = false;

            var html = GetNormalHtmlTextWithAbsences();
            var result = await exporter.ParseHtmlAsync(html, settings);

            Assert.AreEqual(7, result.Infotexts.Count);
            Assert.AreEqual(DateTime.Parse("2019-06-10"), result.Infotexts[0].Date);
            Assert.AreEqual("Unterrichtsfrei 1-3 Std.", result.Infotexts[0].Text);
            Assert.AreEqual(DateTime.Parse("2019-06-10"), result.Infotexts[1].Date);
            Assert.AreEqual("Infotext 1", result.Infotexts[1].Text);
            Assert.AreEqual(DateTime.Parse("2019-06-10"), result.Infotexts[2].Date);
            Assert.AreEqual("Infotext 2", result.Infotexts[2].Text);
            Assert.AreEqual(DateTime.Parse("2019-06-10"), result.Infotexts[3].Date);
            Assert.AreEqual("Infotext mit HTML", result.Infotexts[3].Text);
            Assert.AreEqual(DateTime.Parse("2019-06-10"), result.Infotexts[4].Date);
            Assert.AreEqual("Abwesende Lehrer AB (1-1), BC, CD (1-5)", result.Infotexts[4].Text);
            Assert.AreEqual("Abwesende Klassen 05A (3-8), 05B (3-3), 05C", result.Infotexts[5].Text);
            Assert.AreEqual("Blockierte Räume R 5b, R 6e (2-5)", result.Infotexts[6].Text);
        }

        [TestMethod]
        public async Task TestNormalDataWithAbsentTeachersAndGradesAndRooms()
        {
            var exporter = new SubstitutionExporter();
            var settings = new SubstitutionExportSettings();
            settings.AbsenceSettings.ParseAbsences = true;

            var html = GetNormalHtmlTextWithAbsences();
            var result = await exporter.ParseHtmlAsync(html, settings);

            Assert.AreEqual(8, result.Absences.Count);

            // Teachers
            var absenceAB = result.Absences.FirstOrDefault(x => x.Objective == "AB");
            Assert.IsNotNull(absenceAB);
            Assert.AreEqual(1, absenceAB.LessonStart);
            Assert.AreEqual(1, absenceAB.LessonEnd);
            Assert.AreEqual(Absence.ObjectiveType.Teacher, absenceAB.Type);
            Assert.AreEqual(DateTime.Parse("2019-06-10"), absenceAB.Date);

            var absenceBC = result.Absences.FirstOrDefault(x => x.Objective == "BC");
            Assert.IsNotNull(absenceBC);
            Assert.IsNull(absenceBC.LessonStart);
            Assert.IsNull(absenceBC.LessonEnd);
            Assert.AreEqual(Absence.ObjectiveType.Teacher, absenceBC.Type);

            var absenceCD = result.Absences.FirstOrDefault(x => x.Objective == "CD");
            Assert.IsNotNull(absenceCD);
            Assert.AreEqual(1, absenceCD.LessonStart);
            Assert.AreEqual(5, absenceCD.LessonEnd);
            Assert.AreEqual(Absence.ObjectiveType.Teacher, absenceCD.Type);

            // Grades
            var absence5A = result.Absences.FirstOrDefault(x => x.Objective == "05A");
            Assert.IsNotNull(absence5A);
            Assert.AreEqual(3, absence5A.LessonStart);
            Assert.AreEqual(8, absence5A.LessonEnd);
            Assert.AreEqual(Absence.ObjectiveType.StudyGroup, absence5A.Type);

            var absence5B = result.Absences.FirstOrDefault(x => x.Objective == "05B");
            Assert.IsNotNull(absence5B);
            Assert.AreEqual(3, absence5B.LessonStart);
            Assert.AreEqual(3, absence5B.LessonEnd);
            Assert.AreEqual(Absence.ObjectiveType.StudyGroup, absence5B.Type);

            var absence5C = result.Absences.FirstOrDefault(x => x.Objective == "05C");
            Assert.IsNotNull(absence5C);
            Assert.IsNull(absence5C.LessonStart);
            Assert.IsNull(absence5C.LessonEnd);
            Assert.AreEqual(Absence.ObjectiveType.StudyGroup, absence5C.Type);

            // Rooms
            var roomR5b = result.Absences.FirstOrDefault(x => x.Objective == "R 5b");
            Assert.IsNotNull(roomR5b);
            Assert.IsNull(roomR5b.LessonStart);
            Assert.IsNull(roomR5b.LessonEnd);
            Assert.AreEqual(Absence.ObjectiveType.Room, roomR5b.Type);

            var roomR6e = result.Absences.FirstOrDefault(x => x.Objective == "R 6e");
            Assert.IsNotNull(roomR6e);
            Assert.AreEqual(2, roomR6e.LessonStart);
            Assert.AreEqual(5, roomR6e.LessonEnd);
            Assert.AreEqual(Absence.ObjectiveType.Room, roomR6e.Type);
        }

        [TestMethod]
        public async Task TestNormalDataWithAbsentValuesIncluded()
        {
            var exporter = new SubstitutionExporter();
            var settings = new SubstitutionExportSettings();
            settings.IncludeAbsentValues = true;
            settings.EmptyValues.Clear();

            var html = GetNormalHtmlText();
            var result = await exporter.ParseHtmlAsync(html, settings);

            var substitutionWithMultipleStudyGroups = result.Substitutions.FirstOrDefault(x => x.Id == 6);
            Assert.IsNotNull(substitutionWithMultipleStudyGroups);
            Assert.AreEqual("---", substitutionWithMultipleStudyGroups.ReplacementSubject);
            CollectionAssert.AreEqual(new string[] { "---" }, substitutionWithMultipleStudyGroups.ReplacementTeachers.ToArray());
            Assert.AreEqual(3, substitutionWithMultipleStudyGroups.Grades.Count);
            CollectionAssert.AreEqual(new string[] { "08A", "08B", "08C" }, substitutionWithMultipleStudyGroups.Grades.ToArray());
            Assert.AreEqual(3, substitutionWithMultipleStudyGroups.ReplacementGrades.Count);
            CollectionAssert.AreEqual(new string[] { "08A", "08B", "08C" }, substitutionWithMultipleStudyGroups.ReplacementGrades.ToArray());
        }

        [TestMethod]
        public async Task TestWithFreeLessons()
        {
            var exporter = new SubstitutionExporter();
            var settings = new SubstitutionExportSettings();
            settings.AbsenceSettings.ParseAbsences = false;
            settings.FreeLessonSettings.ParseFreeLessons = true;

            var html = GetNormalHtmlText();
            var result = await exporter.ParseHtmlAsync(html, settings);

            Assert.AreEqual(2, result.FreeLessons.Count);
            Assert.AreEqual(1, result.FreeLessons[0].Start);
            Assert.AreEqual(1, result.FreeLessons[0].End);
            Assert.AreEqual(3, result.FreeLessons[1].Start);
            Assert.AreEqual(5, result.FreeLessons[1].End);

            Assert.AreEqual(8, result.Substitutions.Count);
        }

        [TestMethod]
        public async Task TestEmptyCells()
        {
            var exporter = new SubstitutionExporter();
            var settings = new SubstitutionExportSettings();

            var html = GetNormalHtmlText();
            var result = await exporter.ParseHtmlAsync(html, settings);

            var substitution = result.Substitutions.FirstOrDefault(x => x.Id == 3);

            Assert.IsNotNull(substitution);

            Assert.IsNull(substitution.Subject);
            Assert.IsNull(substitution.Room);
            Assert.AreEqual(0, substitution.Grades.Count);
            Assert.AreEqual(0, substitution.ReplacementGrades.Count);
            Assert.AreEqual(0, substitution.Teachers.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public async Task TestInvalidDateThrowsParseException()
        {
            var exporter = new SubstitutionExporter();
            var settings = new SubstitutionExportSettings();

            var html = GetHtmlWithInvalidDate();
            var result = await exporter.ParseHtmlAsync(html, settings);
        }

        private string GetNormalHtmlText()
        {
            return LoadFile("test.htm");
        }

        private string GetNormalHtmlTextWithAbsences()
        {
            return LoadFile("test_absence.htm");
        }

        private string GetHtmlWithEmptyData()
        {
            return LoadFile("test_empty.htm");
        }

        private string GetHtmlWithInvalidDate()
        {
            return LoadFile("test_invaliddate.htm");
        }
    }
}
