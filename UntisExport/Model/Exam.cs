using System;

namespace SchulIT.UntisExport.Model
{
    public class Exam
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public int LessonStart { get; set; }

        public int LessonEnd { get; set; }

        public ExamCourse[] Courses { get; set; } = Array.Empty<ExamCourse>();

        public string[] Supervisions { get; set; } = Array.Empty<string>();

        public string[] Rooms { get; set; } = Array.Empty<string>();

        public string[] Students { get; set; } = Array.Empty<string>();
    }

    public class ExamCourse
    {
        public int TuitionNumber { get; set; }

        public string CourseName { get; set; }

        public int TuitionIndex { get; set; }
    }
}
