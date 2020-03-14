using System.Collections.Generic;

namespace SchulIT.UntisExport.Timetable
{
    public class TimetableExportResult
    {

        /// <summary>
        /// Objective for which the timetable is (either a grade (05A, EF, ...) or a subject (Bereit, ...))
        /// </summary>
        public string Objective { get; private set; }

        public string Period { get; private set; }

        public IReadOnlyList<Lesson> Lessons { get; private set; }

        public TimetableExportResult(string objective, string period, IReadOnlyList<Lesson> lessons)
        {
            Objective = objective;
            Period = period;
            Lessons = lessons;
        }
    }
}
