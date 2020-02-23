using System;
using System.Collections.Generic;

namespace SchulIT.UntisExport.Exams
{
    public class Exam
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public int LessonStart { get; set; }

        public int LessonEnd { get; set; }

        public ICollection<string> Grades { get; set; }

        public ICollection<string> Courses { get; set; }

        public ICollection<string> Teachers { get; set; }

        public ICollection<string> Rooms { get; set; }

        public string Description { get; set; }
    }
}
