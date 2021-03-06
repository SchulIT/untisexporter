using SchulIT.UntisExport.Extractor;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SchulIT.UntisExport
{
    public static class UntisExporter
    {
        public static Task<UntisExportResult> ParseFileAsync(string path)
        {
            return Task.Run(() => ParseFile(path));
        }

        public static UntisExportResult ParseFile(string path)
        {
            var result = new UntisExportResult { File = path };
            var content = File.ReadAllLines(path);

            // Parse settings
            var settingsExtractor = new SettingsExtractor();
            result.Settings = settingsExtractor.ParseContent(content).FirstOrDefault();

            // Parse holidays
            var holidaysExtractor = new HolidaysExtractor();
            result.Holidays.AddRange(holidaysExtractor.ParseContent(content));

            // Parse periods
            var periodExtractor = new PeriodExtractor();
            var periods = periodExtractor.ParseContent(content);
            for(int i = 0; i < periods.Count; i++)
            {
                periods[i].Number = i+1;
            }
            result.Periods.AddRange(periods);

            // Parse subjects
            var subjectExtractor = new SubjectExtractor();
            result.Subjects.AddRange(subjectExtractor.ParseContent(content));

            // Parse rooms
            var roomExtractor = new PeriodAwareRoomExtractor();
            result.Rooms.AddRange(roomExtractor.ParseContent(content));

            // parse teachers
            var teacherExtractor = new PeriodAwareTeacherExtractor();
            result.Teachers.AddRange(teacherExtractor.ParseContent(content));

            // parse tuitions
            var tuitionExtractor = new PeriodAwareTuitionExtractor();
            var tuitions = tuitionExtractor.ParseContent(content);
            result.Tuitions.AddRange(tuitions);

            // parse absences
            var absenceExtractor = new AbsenceExtractor();
            result.Absences.AddRange(absenceExtractor.ParseContent(content));

            // parse day texts
            var dayTextsExtractor = new DayInformationExtractor();
            result.Days.AddRange(dayTextsExtractor.ParseContent(content));

            // parse exams
            var examsExtractor = new ExamExtractor();
            result.Exams.AddRange(examsExtractor.ParseContent(content));

            // parse supervisions
            var supervisionsExtractor = new SupervisionExtractor();
            var floors = supervisionsExtractor.ParseContent(content);
            result.SupervisionFloors.AddRange(floors);

            // parse substitutions
            var substitutionExtractor = new SubstitutionExtractor(tuitions, periods, new Utilities.SubstitutionTypeResolver(floors.Select(x => x.Name)));
            result.Substitutions.AddRange(substitutionExtractor.ParseContent(content));

            // parse events
            var eventExtractor = new EventExtractor();
            result.Events.AddRange(eventExtractor.ParseContent(content));

            return result;
        }
    }
}
