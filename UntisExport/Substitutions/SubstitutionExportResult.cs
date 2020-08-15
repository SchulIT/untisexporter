using System.Collections.Generic;

namespace SchulIT.UntisExport.Substitutions
{
    public class SubstitutionExportResult
    {
        public IReadOnlyList<Substitution> Substitutions { get; private set; }

        public IReadOnlyList<Infotext> Infotexts { get; private set; }

        public IReadOnlyList<Absence> Absences { get; private set; }

        public IReadOnlyList<FreeLessonsTimespan> FreeLessons { get; private set; }


        public SubstitutionExportResult(IReadOnlyList<Substitution> substitutions)
            : this(substitutions, new List<Infotext>(), new List<Absence>())
        {
        }

        public SubstitutionExportResult(IReadOnlyList<Substitution> substitutions, IReadOnlyList<Infotext> infotexts, IReadOnlyList<Absence> absences)
            : this(substitutions, infotexts, absences, new List<FreeLessonsTimespan>())
        {
        }

        public SubstitutionExportResult(IReadOnlyList<Substitution> substitutions, IReadOnlyList<Infotext> infotexts, IReadOnlyList<Absence> absences, IReadOnlyList<FreeLessonsTimespan> freeLessons)
        {
            Substitutions = substitutions;
            Infotexts = infotexts;
            Absences = absences;
            FreeLessons = freeLessons;
        }
    }
}
