using SchulIT.UntisExport.Model;
using Sprache;
using System.Collections.Generic;

namespace SchulIT.UntisExport.Extractor
{
    public class EventExtractor : AbstractMultilineExtractor<Event>
    {

        public static readonly Parser<Event> StartSequence =
            from identifier in Parse.String("0W")
            from any in Parse.CharExcept(',').Many()
            from comma in Parse.Char(',')
            from number in Parse.Number
            from comma2 in Parse.Char(',')
            from anyCell in Parse.CharExcept(',').Many()
            from comma3 in Parse.Char(',')
            from anyCell2 in Parse.CharExcept(',').Many()
            from comma4 in Parse.Char(',')
            from startDate in Parsers.DateTime
            from comma5 in Parse.Char(',')
            from endDate in Parsers.DateTime
            from comma6 in Parse.Char(',')
            from type in CsvParser.QuotedCell.Optional()
            from comma7 in Parse.Char(',')
            from text in CsvParser.QuotedCell.Optional().Token()
            from comma8 in Parse.Char(',')
            from startLesson in Parse.Number
            from comma9 in Parse.Char(',')
            from endLesson in Parse.Number
            from comma10 in Parse.Char(',')
            from rest in Parse.AnyChar.Many()
            select new Event
            {
                Number = int.Parse(number),
                StartDate = startDate,
                StartLesson = int.Parse(startLesson),
                EndDate = endDate,
                EndLesson = int.Parse(endLesson),
                Text = text.Get()
            };

        public static readonly Parser<IEnumerable<string>> Teachers =
            from identifier in Parse.String("Wl")
            from any in Parse.CharExcept(',').Many()
            from twoComma in Parse.String(",,")
            from teachers in CsvParser.Record
            select teachers;

        public static readonly Parser<IEnumerable<string>> Rooms =
            from identifier in Parse.String("Wr")
            from any in Parse.CharExcept(',').Many()
            from twoComma in Parse.String(",,")
            from rooms in CsvParser.Record
            select rooms;

        public static readonly Parser<IEnumerable<string>> Grades =
            from identifier in Parse.String("Wk")
            from any in Parse.CharExcept(',').Many()
            from twoComma in Parse.String(",,")
            from grades in CsvParser.Record
            select grades;

        public static readonly Parser<char> NonStartSequence =
            from identifier in Parse.Char('0')
            from character in Parse.CharExcept('W')
            select character;

        protected override IEnumerable<Event> BuildItems(Event dto)
        {
            return new Event[] { dto };
        }

        protected override bool IsEndSequence(string line)
        {
            return NonStartSequence.TryParse(line).WasSuccessful;
        }

        protected override bool IsStartSequence(string line, out Event dto)
        {
            dto = new Event();
            var parsed = StartSequence.TryParse(line);

            if(parsed.WasSuccessful)
            {
                dto = parsed.Value;
                return true;
            }

            return false;
        }

        protected override void ParseLine(string line, Event dto)
        {
            var teachersLine = Teachers.TryParse(line);
            if (teachersLine.WasSuccessful)
            {
                dto.Teachers.AddRange(teachersLine.Value);
                return;
            }

            var gradesLine = Grades.TryParse(line);
            if (gradesLine.WasSuccessful)
            {
                dto.Grades.AddRange(gradesLine.Value);
                return;
            }

            var roomsLine = Rooms.TryParse(line);
            if (gradesLine.WasSuccessful)
            {
                dto.Rooms.AddRange(roomsLine.Value);
                return;
            }
        }
    }
}
