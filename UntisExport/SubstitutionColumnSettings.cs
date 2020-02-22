namespace SchulIT.UntisExport
{
    public class SubstitutionColumnSettings
    {
        public string IdColumn { get; set; } = "Vtr-Nr.";
        public string DateColumn { get; set; } = "Datum";
        public string LessonColumn { get; set; } = "Stunde";
        public string GradesColumn { get; set; } = "(Klasse(n))";
        public string ReplacementGradesColumn { get; set; } = "Klasse(n)";
        public string TeachersColumn { get; set; } = "(Lehrer)";
        public string ReplacementTeachersColumn { get; set; } = "Vertreter";
        public string SubjectColumn { get; set; } = "(Fach)";
        public string ReplacementSubjectColumn { get; set; } = "Fach";
        public string RoomColumn { get; set; } = "(Raum)";
        public string ReplacementRoomColumn { get; set; } = "Raum";
        public string TypeColumn { get; set; } = "Art";
        public string RemarkColumn { get; set; } = "Vertretungs-Text";
    }
}
