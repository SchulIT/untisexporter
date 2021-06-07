using SchulIT.UntisExport.Model;
using SchulIT.UntisExport.Utilities;
using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public class SubstitutionExtractor : AbstractMultilineExtractor<Substitution>
    {
        private readonly IReadOnlyCollection<Tuition> tuitions;
        private readonly IReadOnlyCollection<Period> periods;
        private readonly IReadOnlyCollection<SupervisionFloor> floors;
        private readonly IReadOnlyCollection<Absence> absences;
        private readonly SubstitutionTypeResolver typeResolver;

        public SubstitutionExtractor(IReadOnlyCollection<Tuition> tuitions, IReadOnlyCollection<Period> periods, 
            IReadOnlyCollection<SupervisionFloor> floors, IReadOnlyCollection<Absence> absences, SubstitutionTypeResolver typeResolver)
        {
            this.tuitions = tuitions;
            this.periods = periods;
            this.floors = floors;
            this.absences = absences;
            this.typeResolver = typeResolver;
        }

        public static readonly Parser<Substitution> Substitution =
            from identifier in Parse.String("0V")
            from any in Parse.CharExcept(',').Many()
            from comma in Parse.Char(',')
            from number in Parse.Number
            from comma2 in Parse.Char(',')
            from date in Parsers.DateTime
            from comma3 in Parse.Char(',')
            from lesson in Parse.Number
            from comma4 in Parse.Char(',')
            from replacementTeacher in CsvParser.QuotedCell.Optional()
            from comma5 in Parse.Char(',')
            from oldTeacher in CsvParser.QuotedCell.Optional()
            from comma6 in Parse.Char(',')
            from subject in CsvParser.QuotedCell.Optional()
            from comma7 in Parse.Char(',')
            from replacementRoom in CsvParser.QuotedCell.Optional()
            from comma8 in Parse.Char(',')
            from tuitionNumber in Parse.Number.Optional()
            from comma9 in Parse.Char(',')
            from text in CsvParser.QuotedCell.Optional()
            from comma10 in Parse.Char(',')
            from room in CsvParser.QuotedCell.Optional()
            from comma11 in Parse.Char(',')
            from anyNumber in Parse.Number
            from comma12 in Parse.Char(',')
            from anyNumber2 in Parse.Number
            from comma13 in Parse.Char(',')
            from type in CsvParser.Cell.Optional()
            from rest in Parse.AnyChar.Many()
            select new Substitution
            {
                Number = int.Parse(number),
                Date = date,
                Lesson = int.Parse(lesson),
                TuitionNumber = GetTuitionNumber(tuitionNumber),
                Teacher = GetStringOrNull(oldTeacher),
                Rooms = GetStringListOrEmptyList(room),
                ReplacementTeacher = GetStringOrNull(replacementTeacher),
                ReplacementRooms = GetStringListOrEmptyList(replacementRoom),
                ReplacementSubject = GetStringOrNull(subject),
                Text = GetStringOrNull(text),
                RawType = GetStringOrNull(type)
            };
        
        public static readonly Parser<IEnumerable<string>> Grades = 
            from identifier in Parse.String("Vk")
            from any in Parse.CharExcept(',').Many()
            from twoComma in Parse.String(",,")
            from grades in CsvParser.Record
            select grades;

        public static readonly Parser<IEnumerable<string>> Room =
            from identifier in Parse.String("Vr")
            from any in Parse.CharExcept(',').Many()
            from twoComma in Parse.String(",,")
            from rooms in CsvParser.Record
            select rooms;

        public static readonly Parser<IEnumerable<int>> Absence =
            from identifier in Parse.String("Va")
            from any in Parse.CharExcept(',').Many()
            from twoComma in Parse.String(",,")
            from absences in CsvParser.Record
            select absences.Select(x => int.Parse(x));

        public static readonly Parser<char> NonStartSequence =
            from identifier in Parse.Char('0')
            from character in Parse.CharExcept('V')
            select character;

        protected override IEnumerable<Substitution> BuildItems(Substitution dto)
        {
            // resolve old room + subject
            if (dto.TuitionNumber != null)
            {
                var period = periods.Reverse().FirstOrDefault(x => x.Start <= dto.Date);
                if (period != null)
                {
                    var tuitionPeriod = tuitions
                        .SelectMany(x => x.Periods.Where(x => x.PeriodNumber == period.Number && x.TuitionNumber == dto.TuitionNumber))
                        .FirstOrDefault(x => x.Teacher == dto.Teacher);

                    if (tuitionPeriod != null)
                    {
                        var timetableEntry = tuitionPeriod.Timetable.FirstOrDefault(x => x.Day == (int)dto.Date.DayOfWeek && x.Lesson == dto.Lesson);

                        if (timetableEntry != null && !string.IsNullOrEmpty(timetableEntry.Room))
                        {
                            //dto.Grades.Clear();
                            //dto.Grades.AddRange(tuitionPeriod.Grades);
                            dto.Rooms.Clear();
                            dto.Rooms.Add(timetableEntry.Room);
                        }
                        else
                        {
                            //dto.Grades.Clear();
                            //dto.Grades.AddRange(tuitionPeriod.Grades);
                            dto.Rooms.Clear();
                            dto.Rooms.Add(string.IsNullOrEmpty(tuitionPeriod.LabRoom) ? tuitionPeriod.RegularRoom : tuitionPeriod.LabRoom);
                        }
                        dto.Subject = tuitionPeriod.Subject;
                    }
                }
            }
            else if(dto.Rooms.Count == 1)
            {
                // check if substitution is related to a supervision
                var floor = floors.FirstOrDefault(floor => floor.Name == dto.Rooms.First());
                var absence = absences.FirstOrDefault(absence => absence.Type == AbsenceType.Teacher && dto.AbsenceNumbers != null && dto.AbsenceNumbers.Contains(absence.Number));

                if(floor != null && absence != null)
                {
                    var entry = floor.Supervisions.FirstOrDefault(s => s.Day == (int)dto.Date.DayOfWeek && s.Lesson == dto.Lesson && s.Teacher == absence.Objective);

                    if(entry != null)
                    {
                        dto.Teacher = absence.Objective;
                        dto.Type = SubstitutionType.Pausenaufsicht;
                        dto.IsBefore = true;
                        dto.ReplacementRooms.Clear();
                    }
                }
            }

            // resolve type
            if (dto.Type == default)
            {
                dto.Type = typeResolver.ResolveType(dto);
            }

            if(dto.Type == SubstitutionType.Entfall || dto.Type == SubstitutionType.Freisetzung)
            {
                dto.ReplacementRooms.Clear();
                dto.ReplacementSubject = null;
                dto.ReplacementTeacher = null;
            }

            return new Substitution[] { dto };
        }

        protected override bool IsEndSequence(string line)
        {
            return NonStartSequence.TryParse(line).WasSuccessful;
        }

        protected override bool IsStartSequence(string line, out Substitution dto)
        {
            dto = new Substitution();

            var parsed = Substitution.TryParse(line);
            if(parsed.WasSuccessful)
            {
                dto = parsed.Value;
                return true;
            }

            return false;
        }

        protected override void ParseLine(string line, Substitution dto)
        {
            var gradesLine = Grades.TryParse(line);
            if(gradesLine.WasSuccessful)
            {
                dto.Grades.AddRange(gradesLine.Value);
                return;
            }

            var roomLine = Room.TryParse(line);
            if(roomLine.WasSuccessful)
            {
                if (dto.ReplacementRooms.Any())
                {
                    dto.ReplacementRooms.Clear();
                    dto.ReplacementRooms.AddRange(roomLine.Value.ToList());
                }
                /*else if (dto.ReplacementRoom != roomLine.Value)
                {
                    dto.ReplacementRoom = $"ERR: {dto.ReplacementRoom} OR {roomLine.Value}"; 
                }*/
                return;
            }

            var absenceLine = Absence.TryParse(line);
            if(absenceLine.WasSuccessful)
            {
                dto.AbsenceNumbers.AddRange(absenceLine.Value);
                return;
            }
        }

        private static string GetStringOrNull(IOption<string> option)
        {
            if(option.IsDefined)
            {
                return option.Get();
            }

            return null;
        }
        
        private static List<string> GetStringListOrEmptyList(IOption<string> option)
        {
            if(option.IsDefined)
            {
                return new List<string> { option.Get() };
            }

            return new List<string>();
        }

        private static int? GetTuitionNumber(IOption<string> tuitionNumber)
        {
            if(!tuitionNumber.IsDefined || tuitionNumber.Get() == "0")
            {
                return null;
            }

            return int.Parse(tuitionNumber.Get());
        }
    }
}
