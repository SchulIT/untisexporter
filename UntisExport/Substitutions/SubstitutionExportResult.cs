using System.Collections.Generic;

namespace SchulIT.UntisExport.Substitutions
{
    public class SubstitutionExportResult
    {
        public IReadOnlyList<Substitution> Substitutions { get; private set; }

        public IReadOnlyList<Infotext> Infotexts { get; private set; }

        public IReadOnlyList<Absence> Absences { get; private set; }

        public SubstitutionExportResult(IReadOnlyList<Substitution> substitutions)
        {
            Substitutions = substitutions;
            Infotexts = new List<Infotext>();
            Absences = new List<Absence>();
        }

        public SubstitutionExportResult(IReadOnlyList<Substitution> substitutions, IReadOnlyList<Infotext> infotexts, IReadOnlyList<Absence> absences)
        {
            Substitutions = substitutions;
            Infotexts = infotexts;
            Absences = absences;
        }
    }
}
