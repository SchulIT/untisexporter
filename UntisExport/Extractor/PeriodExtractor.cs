using SchulIT.UntisExport.Model;
using Sprache;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public class PeriodExtractor : AbstractSinglelineExtractor<Period>
    {
        public static readonly Parser<Period> Period =
            from idetifier in Parse.String("0P")
            from any in Parse.CharExcept(',').Many()
            from comma in Parse.Char(',')
            from name in CsvParser.QuotedCell
            from comma2 in Parse.Char(',')
            from longname in CsvParser.QuotedCell
            from comma3 in Parse.Char(',')
            from start in Parsers.DateTime
            from comma4 in Parse.Char(',')
            from end in Parsers.DateTime
            from comma5 in Parse.Char(',')
            from parent in CsvParser.QuotedCell.Optional()
            from rest in Parse.AnyChar.Many()
            select new Period
            {
                Start = start,
                End = end,
                Name = name,
                LongName = longname,
                Parent = GetValueOrNull(parent)
            };

        protected override Period ParseLine(string line)
        {
            var parsed = Period.TryParse(line);
            if (parsed.WasSuccessful)
            {
                return parsed.Value;
            }

            return null;
        }

        private static string GetValueOrNull(IOption<string> value)
        {
            if(value.IsDefined)
            {
                return value.Get();
            }

            return null;
        }
    }
}
