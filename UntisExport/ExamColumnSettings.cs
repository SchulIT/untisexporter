namespace SchulIT.UntisExport
{
    public class ExamColumnSettings
    {
        public string DateColumn { get; set; } = "Datum";

        public string LessonStartColumn { get; set; } = "Von";

        public string LessonEndColumn { get; set; } = "Bis";

        public string GradesColumn { get; set; } = "Klassen";

        public char GradesSeparator { get; set; } = ',';

        public string CoursesColumn { get; set; } = "Kurs";

        public char CoursesSeparator { get; set; } = ',';

        public string TeachersColumn { get; set; } = "Lehrer";

        public char TeachersSeparator { get; set; } = '-';

        public string RoomsColumn { get; set; } = "Räume";

        public char RoomsSeparator { get; set; } = '-';

        public string NameColumn { get; set; } = "Name";

        public string RemarkColumn { get; set; } = "Text";
    }
}
