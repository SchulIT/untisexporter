using System;

namespace SchulIT.UntisExport.Substitutions
{
    public class Absence
    {

        public DateTime Date { get; set; }

        public ObjectiveType Type { get; set; }

        public string Objective { get; set; }

        public int? LessonStart { get; set; }

        public int? LessonEnd { get; set; }

        public enum ObjectiveType
        {
            StudyGroup,
            Teacher,
            Room
        }
    }
}
