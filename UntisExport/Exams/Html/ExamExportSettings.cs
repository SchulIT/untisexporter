namespace SchulIT.UntisExport.Exams.Html
{
    public class ExamExportSettings
    {
        /// <summary>
        /// Specifies the date format which is used to parse the date (defaults to the German date format dd.mm.yyyy)
        /// </summary>
        public string DateTimeFormat { get; set; } = "d.M.yyyy";

        /// <summary>
        /// Specifies the column headers for the data (defaults to the German column names).
        /// </summary>
        public ExamColumnSettings ColumnSettings { get; set; } = new ExamColumnSettings();
    }
}
