using SchulIT.UntisExport.Model;
using Sprache;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public class SubjectExtractor : AbstractSinglelineExtractor<Subject>
    {
        public static readonly Parser<Subject> Subject =
            from identifier in Parse.String("00F")
            from any in Parse.CharExcept(',').Many()
            from comma in Parse.Char(',')
            from abbreviation in CsvParser.QuotedCell
            from comma2 in Parse.Char(',')
            from name in CsvParser.QuotedCell
            from rest in Parse.AnyChar.Many()
            select new Subject
            {
                Abbreviation = abbreviation,
                Name = name
            };

        protected override Subject ParseLine(string line)
        {
            var parsed = Subject.TryParse(line);
            if (parsed.WasSuccessful)
            {
                return parsed.Value;
            }

            return null;
        }
    }
}
