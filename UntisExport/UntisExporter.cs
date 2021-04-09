using SchulIT.UntisExport.Extractor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SchulIT.UntisExport
{
    public static class UntisExporter
    {
        public static async Task<UntisExportResult> ParseFileAsync(string path)
        {
            var result = new UntisExportResult { File = path };
            var content = File.ReadAllLines(path);

            var tasks = new List<Task>();

            tasks.Add(Task.Run(() =>
            {
                // Parse settings
                var settingsExtractor = new SettingsExtractor();
                result.Settings = settingsExtractor.ParseContent(content).FirstOrDefault();
            }));

            tasks.Add(Task.Run(() =>
            {
                // Parse holidays
                var holidaysExtractor = new HolidaysExtractor();
                result.Holidays.AddRange(holidaysExtractor.ParseContent(content));
            }));

            tasks.Add(Task.Run(() =>
            {
                // Parse periods
                var periodExtractor = new PeriodExtractor();
                var periods = periodExtractor.ParseContent(content);
                for (int i = 0; i < periods.Count; i++)
                {
                    periods[i].Number = i + 1;
                }
                result.Periods.AddRange(periods);
            }));

            tasks.Add(Task.Run(() =>
            {
                // Parse subjects
                var subjectExtractor = new SubjectExtractor();
                result.Subjects.AddRange(subjectExtractor.ParseContent(content));
            }));

            tasks.Add(Task.Run(() =>
            {
                // Parse rooms
                var roomExtractor = new PeriodAwareRoomExtractor();
                result.Rooms.AddRange(roomExtractor.ParseContent(content));
            }));

            tasks.Add(Task.Run(() =>
            {
                // parse teachers
                var teacherExtractor = new PeriodAwareTeacherExtractor();
                result.Teachers.AddRange(teacherExtractor.ParseContent(content));
            }));

            tasks.Add(Task.Run(() =>
            {
                // parse tuitions
                var tuitionExtractor = new PeriodAwareTuitionExtractor();
                result.Tuitions.AddRange(tuitionExtractor.ParseContent(content));
            }));

            tasks.Add(Task.Run(() =>
            {
                // parse absences
                var absenceExtractor = new AbsenceExtractor();
                result.Absences.AddRange(absenceExtractor.ParseContent(content));
            }));

            tasks.Add(Task.Run(() =>
            {
                // parse day texts
                var dayTextsExtractor = new DayInformationExtractor();
                result.Days.AddRange(dayTextsExtractor.ParseContent(content));
            }));

            tasks.Add(Task.Run(() =>
            {
                // parse exams
                var examsExtractor = new ExamExtractor();
                result.Exams.AddRange(examsExtractor.ParseContent(content));
            }));

            tasks.Add(Task.Run(() =>
            {
                // parse supervisions
                var supervisionsExtractor = new SupervisionExtractor();
                result.SupervisionFloors.AddRange(supervisionsExtractor.ParseContent(content));
            }));

            tasks.Add(Task.Run(() =>
            {
                // parse events
                var eventExtractor = new EventExtractor();
                result.Events.AddRange(eventExtractor.ParseContent(content));
            }));


            await Task.WhenAll(tasks).ContinueWith((task) =>
            {
                // parse substitutions
                var substitutionExtractor = new SubstitutionExtractor(result.Tuitions, result.Periods, new Utilities.SubstitutionTypeResolver(result.SupervisionFloors.Select(x => x.Name), result.Absences));
                result.Substitutions.AddRange(substitutionExtractor.ParseContent(content));
            }).ConfigureAwait(false);

            return result;
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
            var substitutionExtractor = new SubstitutionExtractor(tuitions, periods, new Utilities.SubstitutionTypeResolver(floors.Select(x => x.Name), result.Absences));
            result.Substitutions.AddRange(substitutionExtractor.ParseContent(content));

            // parse events
            var eventExtractor = new EventExtractor();
            result.Events.AddRange(eventExtractor.ParseContent(content));

            return result;
        }
    }
}
