using System;
using System.Collections.Generic;

namespace SchulIT.UntisExport.Model
{
    public class Substitution
    {
        public int Number { get; set; }

        public int? TuitionNumber { get; set; }

        public DateTime Date { get; set; }

        public int Lesson { get; set; }

        public bool IsBefore { get; set; } = false;

        public string Subject { get; set; }

        public string ReplacementSubject { get; set; }

        public string Teacher { get; set; }

        public string ReplacementTeacher { get; set; }

        public List<string> Rooms { get; internal set; } = new List<string>();

        public List<string> ReplacementRooms { get; internal set; } = new List<string>();

        public List<string> Grades { get; } = new List<string>();

        public SubstitutionType Type { get; set; }

        public string RawType { get; set; }

        public string Text { get; set; }

        public List<int> AbsenceNumbers { get; } = new List<int>();
    }
}
