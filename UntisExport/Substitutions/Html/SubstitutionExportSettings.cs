using System.Collections.Generic;

namespace SchulIT.UntisExport.Substitutions.Html
{
    public class SubstitutionExportSettings
    {
        /// <summary>
        /// Untis does not produce valid HTML as it opens <p>-tags without closing them.
        /// In order to workaround this, you should set this flag to true (which is default behaviour).
        /// </summary>
        public bool FixBrokenPTags { get; set; } = true;

        /// <summary>
        /// Specifies the date format which is used to parse the date (defaults to the German date format dd.mm.yyyy)
        /// </summary>
        public string DateTimeFormat { get; set; } = "d.M.yyyy";

        /// <summary>
        /// List of values which are concidered empty.
        /// </summary>
        public List<string> EmptyValues { get; } = new List<string> { "---", "???" };

        /// <summary>
        /// Flag whether to include absent study groups or teachers (which are encoded like "(05A)").
        /// If set to true, the values are included in the result entity, otherwise the value is set to null or empty list.
        /// </summary>
        public bool IncludeAbsentValues { get; set; } = false;

        /// <summary>
        /// Specifies the column headers for the data (defaults to the German column names).
        /// </summary>
        public SubstitutionColumnSettings ColumnSettings { get; } = new SubstitutionColumnSettings();

        /// <summary>
        /// Specifies the settings for parsing absences (if desired).
        /// </summary>
        public AbsenceSettings AbsenceSettings { get; } = new AbsenceSettings();
    }
}
