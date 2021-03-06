﻿using SchulIT.UntisExport.Model;
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
        private readonly SubstitutionTypeResolver typeResolver;

        public SubstitutionExtractor(IReadOnlyCollection<Tuition> tuitions, IReadOnlyCollection<Period> periods, SubstitutionTypeResolver typeResolver)
        {
            this.tuitions = tuitions;
            this.periods = periods;
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
            from room in CsvParser.QuotedCell
            from comma8 in Parse.Char(',')
            from tuitionNumber in Parse.Number.Optional()
            from comma9 in Parse.Char(',')
            from text in CsvParser.QuotedCell.Optional()
            from comma10 in Parse.Char(',')
            from anyCell in CsvParser.QuotedCell.Optional()
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
                ReplacementTeacher = GetStringOrNull(replacementTeacher),
                ReplacementRooms = new List<string>() { room },
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

        public static readonly Parser<char> NonStartSequence =
            from identifier in Parse.Char('0')
            from character in Parse.CharExcept('V')
            select character;

        protected override IEnumerable<Substitution> BuildItems(Substitution dto)
        {
            // resolve old room + subject
            if(dto.TuitionNumber != null)
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
                            dto.Rooms = new List<string> { timetableEntry.Room };
                        }
                        else
                        {
                            dto.Rooms = new List<string> { string.IsNullOrEmpty(tuitionPeriod.LabRoom) ? tuitionPeriod.RegularRoom : tuitionPeriod.LabRoom };
                        }
                        dto.Subject = tuitionPeriod.Subject;
                    }
                }
            }

            // resolve type
            dto.Type = typeResolver.ResolveType(dto);

            if(dto.Type == SubstitutionType.Pausenaufsicht)
            {
                dto.IsBefore = true;
            }

            if(dto.Type == SubstitutionType.Entfall)
            {
                dto.Rooms = dto.ReplacementRooms;
                dto.ReplacementRooms = null;
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
                    dto.ReplacementRooms = roomLine.Value.ToList();
                }
                /*else if (dto.ReplacementRoom != roomLine.Value)
                {
                    dto.ReplacementRoom = $"ERR: {dto.ReplacementRoom} OR {roomLine.Value}"; 
                }*/
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

        private static int? GetTuitionNumber(IOption<string> tuitionNumber)
        {
            if(!tuitionNumber.IsDefined)
            {
                return null;
            }

            return int.Parse(tuitionNumber.Get());
        }
    }
}
