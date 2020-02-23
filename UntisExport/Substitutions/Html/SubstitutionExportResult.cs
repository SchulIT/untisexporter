using System;
using System.Collections.Generic;

namespace SchulIT.UntisExport.Substitutions.Html
{
    public class SubstitutionExportResult
    {
        public DateTime Date { get; private set; }

        public IReadOnlyList<Substitution> Substitutions { get; private set; }

        public IReadOnlyList<Infotext> Infotexts { get; private set; }

        public IReadOnlyList<Absence> Absences { get; private set; }

        public SubstitutionExportResult(DateTime date, IReadOnlyList<Substitution> substitutions, IReadOnlyList<Infotext> infotexts, IReadOnlyList<Absence> absences)
        {
            Date = date;
            Substitutions = substitutions;
            Infotexts = infotexts;
            Absences = absences;
        }
    }
}
