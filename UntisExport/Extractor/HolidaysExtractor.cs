using SchulIT.UntisExport.Model;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public class HolidaysExtractor : AbstractSinglelineExtractor<Holiday>
    {
        public static readonly Parser<char> Comma = Parse.Char(',');

        public static readonly Parser<Holiday> Holiday =
            from identifier in Parse.String("FE")
            from weekAfter in Parse.Number
            from any in Parse.CharExcept(',').Many()
            from comma in Comma
            from name in CsvParser.QuotedCell
            from comma2 in Comma
            from longName in CsvParser.QuotedCell
            from comma3 in Comma
            from start in Parsers.DateTime
            from comma4 in Comma
            from end in Parsers.DateTime
            from comma5 in Comma
            from type in Parse.CharExcept(',').Many().Optional().Token()
            from comma6 in Comma
            from followingWeek in Parse.Number
            from comma7 in Comma
            from rest in Parse.Number
            select new Holiday
            {
                Name = name,
                LongName = longName,
                Start = start,
                End = end,
                Type = GetType(type),
                ContinueWeekNumbering = followingWeek == "0",
                WeekAfterHolidays = int.Parse(weekAfter)
            };

        protected override Holiday ParseLine(string line)
        {
            var parsed = Holiday.TryParse(line);
            if(parsed.WasSuccessful)
            {
                return parsed.Value;
            }

            return null;
        }

        private static HolidayType GetType(IOption<IEnumerable<char>> type)
        {
            if(!type.IsDefined)
            {
                return HolidayType.Ferien;
            }

            var typeString = new string(type.Get().ToArray());

            switch(typeString)
            {
                case "F":
                    return HolidayType.Feiertag;
            }

            return HolidayType.Ferien;
        }
    }
}
