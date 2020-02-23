using System;
using System.Collections.Generic;

namespace SchulIT.UntisExport.Exams
{
    public class Exam
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public int LessonStart { get; set; }

        public int LessonEnd { get; set; }

        public ICollection<string> Grades { get; set; }

        public ICollection<string> Courses { get; set; }

        public ICollection<string> Invigilators { get; set; }

        public ICollection<string> Rooms { get; set; }

        public string Remark { get; set; }
    }
}
