using SchulIT.UntisExport.Model;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public class DayInformationExtractor : AbstractMultilineExtractor<DayInformation, DayInformation>
    {
        public static readonly Parser<DayInformation> DayTextStartSequence =
            from identifier in Parse.String("0K")
            from any in Parse.CharExcept(',').Many()
            from comma in Parse.Char(',')
            from date in Parsers.DateTime
            from additionalInfo in AdditionalInformation.Optional()
            select CreateDay(date, additionalInfo);

        public static readonly Parser<DayInformation> AdditionalInformation =
            from comma in Parse.Char(',')
            from any in Parse.CharExcept(',').Many()
            from comma2 in Parse.Char(',')
            from reason in CsvParser.QuotedCell
            from comma3 in Parse.Char(',')
            from any2 in Parse.CharExcept(',').Many()
            from comma4 in Parse.Char(',')
            from note in CsvParser.QuotedCell
            from type in (
                from any in (
                    from comma in Parse.Char(',')
                    from any in Parse.CharExcept(',').Many()
                    select any
                    ).Repeat(2)
                from comma in Parse.Char(',')
                from rawType in Parse.AnyChar.Many().Token()
                select rawType).Optional()
            select new DayInformation
            {
                Reason = reason,
                Note = note,
                Type = FromType(type)
            };


        public static readonly Parser<string> Text =
            from identifier in Parse.String("Kt")
            from any in Parse.CharExcept('"').Many()
            from startQuote in Parse.Char('"')
            from text in Parse.AnyChar.Many().Text()
            select text.Substring(0, text.Length - 1);  // Dirty hack - last " is also parsed and " inside the text are not escaped by Untis 🥴

        public static readonly Parser<string> Guid =
            from identifier in Parse.String("Kg")
            from startBracket in Parse.Char('{')
            from guid in Parse.CharExcept('}').Many()
            from endBracket in Parse.Char('}')
            from any in Parse.AnyChar.Many()
            select new string(guid.ToArray());

        public static readonly Parser<IEnumerable<string>> Grades =
            from identifier in Parse.String("KnK")
            from grades in Parsers.QuotedText
            select grades.Split(",").Select(x => x.Trim());

        public static readonly Parser<int> Lesson =
            from comma in Parse.Char(',')
            from number in Parse.Number
            select int.Parse(number);

        public static readonly Parser<IEnumerable<int>> Lessons =
            from identifier in Parse.String("Ks")
            from space in Parse.WhiteSpace.Many()
            from comma in Parse.Char(',')
            from any in Parse.CharExcept(',').Many()
            from numbers in Lesson.Many()
            select numbers;

        public static readonly Parser<char> NonDayTextStartSequence =
            from identifier in Parse.String("0")
            from character in Parse.CharExcept('K')
            select character;

        protected override IEnumerable<DayInformation> BuildItems(DayInformation dto)
        {
            return new DayInformation[] { dto };
        }

        protected override bool IsEndSequence(string line)
        {
            return NonDayTextStartSequence.TryParse(line).WasSuccessful;
        }

        protected override bool IsStartSequence(string line, out DayInformation dto)
        {
            var day = DayTextStartSequence.TryParse(line);
            dto = new DayInformation();
            if(day.WasSuccessful)
            {
                dto = day.Value;
            }

            return day.WasSuccessful;
        }

        protected override void ParseLine(string line, DayInformation dto)
        {
            var textLine = Text.TryParse(line);
            if(textLine.WasSuccessful)
            {
                var dayText = new DayText
                {
                    Date = dto.Date,
                    Text = new string(textLine.Value.ToArray())
                };
                dto.Texts.Add(dayText);
                return;
            }

            var guidLine = Guid.TryParse(line);
            if(guidLine.WasSuccessful)
            {
                ApplyToLastDayText(dto, dayText => dayText.Guid = guidLine.Value);
                return;
            }

            var gradesLine = Grades.TryParse(line);
            if (gradesLine.WasSuccessful)
            {
                ApplyToLastDayText(dto, dayText => dayText.Grades = gradesLine.Value.ToArray());
                return;
            }

            var lessonsLine = Lessons.TryParse(line);
            if (lessonsLine.WasSuccessful)
            {
                dto.FreeLessons.AddRange(lessonsLine.Value);
                return;
            }
        }

        private void ApplyToLastDayText(DayInformation dto, Action<DayText> action)
        {
            if (dto.Texts.Count == 0)
            {
                return;
            }

            var last = dto.Texts.Last();
            if (last == null)
            {
                return;
            }

            action(last);
        }

        private static DayInformation CreateDay(DateTime dateTime, IOption<DayInformation> additionalInfo) {
            var day = new DayInformation();

            if(additionalInfo.IsDefined)
            {
                day = additionalInfo.Get();
            }

            day.Date = dateTime;

            return day;
        }

        private static DayType FromType(IOption<IEnumerable<char>> type)
        {
            if(!type.IsDefined)
            {
                return DayType.Normal;
            }

            var typeString = new string(type.Get().ToArray());

            switch(typeString)
            {
                case "U":
                    return DayType.Unterrichtsfrei;

                case "F":
                    return DayType.Feiertag;
            }

            return DayType.Normal;
        }
    }
}
