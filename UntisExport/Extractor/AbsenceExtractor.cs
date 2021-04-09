using SchulIT.UntisExport.Model;
using Sprache;
using System;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public class AbsenceExtractor : AbstractSinglelineExtractor<Absence>
    {
        public static readonly Parser<Absence> Absence =
            from identifier in Parse.String("0A")
            from any in Parse.CharExcept(',').Many()
            from comma in Parse.Char(',')
            from number in Parse.Number
            from comma2 in Parse.Char(',')
            from type in Parse.Chars('L', 'K', 'R')
            from comma3 in Parse.Char(',')
            from objective in CsvParser.QuotedCell
            from comma4 in Parse.Char(',')
            from startDate in Parsers.DateTime
            from comma5 in Parse.Char(',')
            from endDate in Parsers.DateTime
            from comma6 in Parse.Char(',')
            from reason in CsvParser.Cell.Optional()
            from comma7 in Parse.Char(',')
            from text in CsvParser.Cell.Optional()
            from comma9 in Parse.Char(',')
            from lessonStart in Parse.Number
            from comma10 in Parse.Char(',')
            from lessonEnd in Parse.Number
            from comma11 in Parse.Char(',')
            from importantPart in Parse.String("0,0,0,").Optional()
            from rest in Parse.AnyChar.Many()
            select new Absence
            {
                Number = int.Parse(number),
                IsInternal = !importantPart.IsDefined,
                Type = GetType(type),
                Objective = objective,
                Start = startDate,
                End = endDate,
                LessonStart = int.Parse(lessonStart),
                LessonEnd = int.Parse(lessonEnd),
                Reason = GetStringOrNull(reason),
                Text = GetStringOrNull(text)
            };


        public static AbsenceType GetType(char type)
        {
            switch(type)
            {
                case 'L':
                    return AbsenceType.Teacher;

                case 'R':
                    return AbsenceType.Room;

                case 'K':
                    return AbsenceType.Grade;
            }

            throw new Exception($"Unknown absence type found: {type}.");
        }

        protected override Absence ParseLine(string line)
        {
            var parsed = Absence.TryParse(line);
            if (parsed.WasSuccessful)
            {
                return parsed.Value;
            }

            return null;
        }

        private static string GetStringOrNull(IOption<string> option)
        {
            if(option.IsDefined)
            {
                var value = option.Get();

                if(string.IsNullOrEmpty(value))
                {
                    return null;
                }

                return value;
            }

            return null;
        }
    }
}
