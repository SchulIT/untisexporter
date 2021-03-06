using SchulIT.UntisExport.Model;
using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public class SupervisionExtractor : AbstractMultilineExtractor<SupervisionFloor, SupervisionFloor>
    {
        public static readonly Parser<SupervisionFloor> StartSequence =
            from identifier in Parse.String("00H")
            from space in Parse.WhiteSpace.Many()
            from comma in Parse.Char(',')
            from name in CsvParser.QuotedCell
            from comma2 in Parse.Char(',')
            from longName in CsvParser.QuotedCell
            from rest in Parse.AnyChar.Many()
            select new SupervisionFloor
            {
                Name = name,
                LongName = longName
            };

        public static readonly Parser<char> NonStartSequence =
            from identifier in Parse.String("00")
            from character in Parse.CharExcept('H')
            select character;

        public static readonly Parser<ParsedSupervisionLine> Part =
            from teacher in CsvParser.QuotedCell.Optional()
            from comma2 in Parse.Char(',')
            from day in Parse.Digit
            from slash in Parse.Char('/')
            from lesson in Parse.Number
            from comma3 in Parse.Char(',')
            from supervisions in CsvParser.QuotedCell
            select new ParsedSupervisionLine
            {
                Day = int.Parse(day.ToString()),
                Teacher = GetStringOrNull(teacher),
                Lesson = int.Parse(lesson),
                UnparsedSupervisions = supervisions
            };

        public static readonly Parser<IEnumerable<ParsedSupervisionLine>> Supervision =
            from identifier in Parse.String("GZ")
            from space in Parse.WhiteSpace.Many()
            from comma in Parse.Char(',')
            from parts in Part.Many()
            select parts;

        protected override IEnumerable<SupervisionFloor> BuildItems(SupervisionFloor dto)
        {
            return new SupervisionFloor[] { dto };
        }

        protected override bool IsEndSequence(string line)
        {
            return NonStartSequence.TryParse(line).WasSuccessful;
        }

        protected override bool IsStartSequence(string line, out SupervisionFloor dto)
        {
            dto = new SupervisionFloor();
            var startLine = StartSequence.TryParse(line);
            if (startLine.WasSuccessful)
            {
                dto = startLine.Value;
                return true;
            }

            return false;
        }

        protected override void ParseLine(string line, SupervisionFloor dto)
        {
            var supervisionLine = Supervision.TryParse(line);
            if(supervisionLine.WasSuccessful)
            {
                foreach (var data in supervisionLine.Value)
                {
                    if (!string.IsNullOrEmpty(data.Teacher))
                    {
                        var supervision = new Supervision
                        {
                            Day = data.Day,
                            Lesson = data.Lesson,
                            Teacher = data.Teacher
                        };
                        dto.Supervisions.Add(supervision);
                    }
                    else
                    {
                        var unparsedData = data.UnparsedSupervisions.Substring(data.UnparsedSupervisions.LastIndexOf('~') + 1);

                        foreach (var section in unparsedData.Split('*'))
                        {
                            var parts = section.Split(';');

                            if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
                            {
                                continue;
                            }

                            var supervision = new Supervision
                            {
                                Day = data.Day,
                                Lesson = data.Lesson,
                                Teacher = parts[0]
                            };

                            foreach (var weeks in parts[1].Split('/'))
                            {
                                var weekParts = weeks.Split('-').Select(x => int.Parse(x));

                                if (weekParts.Count() == 1)
                                {
                                    supervision.Weeks.Add(weekParts.ElementAt(0));
                                }
                                else if (weekParts.Count() == 2)
                                {
                                    for (int week = weekParts.ElementAt(0); week <= weekParts.ElementAt(1); week++)
                                    {
                                        supervision.Weeks.Add(week);
                                    }
                                }
                            }

                            dto.Supervisions.Add(supervision);
                        }
                    }
                }
            }
        }

        public class ParsedSupervisionLine
        {
            public string Teacher { get; set; }

            public int Day { get; set; }

            public int Lesson { get; set; }

            public string UnparsedSupervisions { get; set; }
        }

        private static string GetStringOrNull(IOption<string> value)
        {
            if(value.IsDefined)
            {
                return value.Get();
            }

            return null;
        }
    }
}
