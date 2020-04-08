namespace SchulIT.UntisExport.Substitutions.Html
{
    public class AbsenceSettings
    {
        /// <summary>
        /// Flag which enables parsing absences from infotexts
        /// </summary>
        public bool ParseAbsences { get; set; } = true;

        /// <summary>
        /// Prefix for absent teachers
        /// </summary>
        public string TeacherIdentifier { get; set; } = "Abwesende Lehrer";

        /// <summary>
        /// Prefix for absent study groups
        /// </summary>
        public string StudyGroupIdentifier { get; set; } = "Abwesende Klassen";

        /// <summary>
        /// Prefix for blocked (absent) rooms
        /// </summary>
        public string RoomIdentifier { get; set; } = "Blockierte Räume";
    }
}
