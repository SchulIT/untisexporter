using SchulIT.UntisExport.Model;
using Sprache;
using System;

namespace SchulIT.UntisExport.Extractor
{
    public class SettingsExtractor : AbstractSinglelineExtractor<Settings>
    {
        public static readonly Parser<Settings> Settings =
            from identifier in Parse.String("AA02")
            from space in Parse.WhiteSpace
            from start in Parsers.DateTime
            from space2 in Parse.WhiteSpace
            from end in Parsers.DateTime
            from space3 in Parse.WhiteSpace
            from periodicity in Parsers.FixedLengthInteger(1, 2)
            from numberOfDaysPerWeek in Parsers.FixedLengthInteger(1, 2)
            from numberOfLessonsPerDay in Parsers.FixedLengthInteger(1, 2)
            from any in Parse.AnyChar.Repeat(7)
            from startWeek in Parsers.HexaDecimal
            from firstLesson in Parse.Digit
            from any2 in Parse.AnyChar.Repeat(14)
            from firstDayInWeek in Parse.Digit
            from rest in Parse.AnyChar.Many()
            select new Settings
            {
                Start = start,
                End = end,
                Periodicity = periodicity,
                NumberOfDays = numberOfDaysPerWeek,
                NumberOfLessonsPerDay = numberOfLessonsPerDay,
                StartWeek = startWeek,
                FirstSchoolDayOfWeek = (DayOfWeek)(int.Parse(firstDayInWeek.ToString()) % 7)
            };

        protected override Settings ParseLine(string line)
        {
            var parsed = Settings.TryParse(line);
            if(parsed.WasSuccessful)
            {
                return parsed.Value;
            }

            return null;
        }
    }
}
