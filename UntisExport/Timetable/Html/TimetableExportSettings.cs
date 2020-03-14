namespace SchulIT.UntisExport.Timetable.Html
{
    public class TimetableExportSettings
    {
        public int FirstLesson { get; set; } = 1;

        public TimetableType Type { get; set; } = TimetableType.Grade;

        public bool UseWeeks { get; set; } = true;
    }
}
