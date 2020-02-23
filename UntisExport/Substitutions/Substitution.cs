using System;
using System.Collections.Generic;

namespace SchulIT.UntisExport.Substitutions
{
    public class Substitution
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int LessonStart { get; set; }

        public int LessonEnd { get; set; }

        public string Type { get; set; }

        public string Subject { get; set; }

        public string ReplacementSubject { get; set; }

        public string Room { get; set; }

        public string ReplacementRoom { get; set; }

        public ICollection<string> Teachers { get; set; }

        public ICollection<string> ReplacementTeachers { get; set; }

        public ICollection<string> Grades { get; set; }

        public ICollection<string> ReplacementGrades { get; set; }

        public string Remark { get; set; }

        public bool IsSupervision { get; set; } = false;
    }
}
