namespace SchulIT.UntisExport.Timetable
{
    public class Lesson
    {
        public string[] Weeks { get; set; }

        public int LessonStart { get; set; }

        public int LessonEnd { get; set; }

        public int Day { get; set; }

        public string Subject { get; set; }

        public string Teacher { get; set; }

        public string Room { get; set; }

        public string Grade { get; set; }
    }
}
