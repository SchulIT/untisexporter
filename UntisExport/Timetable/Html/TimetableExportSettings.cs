namespace SchulIT.UntisExport.Timetable.Html
{
    public class TimetableExportSettings
    {
        /// <summary>
        /// Specifies the first lesson in the timetable (usually 1; but Untis also supports lesson 0)
        /// </summary>
        public int FirstLesson { get; set; } = 1;

        /// <summary>
        /// Type of the timetable (currently only Grade and Subject are supported)
        /// </summary>
        public TimetableType Type { get; set; } = TimetableType.Grade;

        /// <summary>
        /// Flag whether this timetable is week-dependent
        /// </summary>
        public bool UseWeeks { get; set; } = true;
    }
}
