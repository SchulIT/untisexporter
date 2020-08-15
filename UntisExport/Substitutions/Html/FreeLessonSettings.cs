namespace SchulIT.UntisExport.Substitutions.Html
{
    public class FreeLessonSettings
    {
        public bool ParseFreeLessons { get; set; } = false;

        public bool RemoveInfotext { get; set; } = true;

        public string FreeLessonIdentifier { get; set; } = "Unterrichtsfrei";

        public string LessonIdentifier { get; set; } = "Std.";
    }
}
