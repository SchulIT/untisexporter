using System;

namespace SchulIT.UntisExport.Model
{
    public class Absence
    {
        public int Number { get; set; }

        public AbsenceType Type { get; set; }

        public string Objective { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int LessonStart { get; set; }

        public int LessonEnd { get; set; }

        public string Reason { get; set; }

        public string Text { get; set; }
    }

    public enum AbsenceType
    {
        Room,
        Grade,
        Teacher
    }
}
