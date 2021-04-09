using SchulIT.UntisExport.Model;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public class PeriodAwareTuitionExtractor : PeriodAwareExtractor<Tuition, PeriodAwareTuitionExtractor.ParsedTuitionData>
    {
        protected override Parser<IEnumerable<int>> StartSequence => TuitionPeriod;

        protected override Parser<char> NonStartSequence => NonTuitionStartSequence;

        public static Parser<IEnumerable<int>> TuitionPeriod =
            from identifier in Parse.String("0pU")
            select Array.Empty<int>();

        public static Parser<char> NonTuitionStartSequence =
            from identifier in Parse.String("0p")
            from nonTeacher in Parse.CharExcept("U")
            select nonTeacher;

        public static Parser<TuitionInfo> Info =
            from identifier in Parse.String("0U")
            from any in Parse.CharExcept(',').Many()
            from comma in Parse.Char(',')
            from number in Parse.Number
            from comma2 in Parse.Char(',')
            from any2 in Parse.CharExcept(',').Many().Optional()
            from comma3 in Parse.Char(',')
            from any3 in Parse.CharExcept(',').Many().Optional()
            from comma4 in Parse.Char(',')
            from any4 in Parse.CharExcept(',').Many().Optional()
            from comma5 in Parse.Char(',')
            from any5 in Parse.CharExcept(',').Many().Optional()
            from comma6 in Parse.Char(',')
            from any6 in Parse.CharExcept(',').Many().Optional()
            from comma7 in Parse.Char(',')
            from any7 in Parse.CharExcept(',').Many().Optional()
            from comma8 in Parse.Char(',')
            from any8 in Parse.CharExcept(',').Many().Optional()
            from comma9 in Parse.Char(',')
            from start in Parsers.DateTime.Optional()
            from comma10 in Parse.Char(',')
            from end in Parsers.DateTime.Optional()
            from rest in Parse.AnyChar.Many()
            select new TuitionInfo
            {
                Number = int.Parse(number),
                StartDate = GetDateTimeOrNull(start),
                EndDate = GetDateTimeOrNull(end)
            };

        public static Parser<string> QuotedCellWithTwoCommaSep =
            from comma in Parse.Char(',').Repeat(2)
            from content in Parsers.QuotedText.Except(Parse.Char(','))
            select content;

        public static Parser<IEnumerable<string>> Subjects =
            from identifier in Parse.String("Uf")
            from space in Parse.WhiteSpace.AtLeastOnce()
            from subjects in QuotedCellWithTwoCommaSep.AtLeastOnce()
            select subjects;

        public static Parser<IEnumerable<string>> Grades =
            from identifier in Parse.String("Uk")
            from space in Parse.WhiteSpace.AtLeastOnce()
            from grades in QuotedCellWithTwoCommaSep.AtLeastOnce()
            select grades;

        public static Parser<IEnumerable<string>> Teachers =
            from identifier in Parse.String("Ul")
            from space in Parse.WhiteSpace.AtLeastOnce()
            from teachers in QuotedCellWithTwoCommaSep.AtLeastOnce()
            select teachers;

        public static Parser<IEnumerable<string>> LabRooms =
            from identifier in Parse.String("Ur")
            from space in Parse.WhiteSpace.AtLeastOnce()
            from rooms in QuotedCellWithTwoCommaSep.AtLeastOnce()
            select rooms;

        public static Parser<IEnumerable<string>> RegularRooms =
            from identifier in Parse.String("Us")
            from space in Parse.WhiteSpace.AtLeastOnce()
            from rooms in QuotedCellWithTwoCommaSep.AtLeastOnce()
            select rooms;

        public static Parser<int> TwoDigitNumber = Parsers.FixedLengthInteger(1, 2);

        public static Parser<int?> OptionalTwoDigitNumber = Parsers.OptionalFixedLengthInteger(0, 2);

        public static Parser<ParsedTuitionDataMatrixData> TuitionData =
            from identifier in Parse.String("Uz")
            from teacherIndex in TwoDigitNumber
            from gradeStartIndex in OptionalTwoDigitNumber
            from gradeEndIndex in OptionalTwoDigitNumber
            from subjectIndex in TwoDigitNumber
            from roomIndex in TwoDigitNumber
            from any in Parse.AnyChar.Except(Parse.Char('n'))
            from record in CsvParser.Record
            select new ParsedTuitionDataMatrixData
            {
                TeacherIndex = teacherIndex,
                GradeStartIndex = gradeStartIndex,
                GradeEndIndex = gradeEndIndex,
                SubjectIndex = subjectIndex,
                RoomIndex = roomIndex,
                StudentGroup = (string.IsNullOrEmpty(record.ElementAtOrDefault(7)) ? null : record.ElementAtOrDefault(7)),
                GradeIndices = record.Skip(7).Where(x => int.TryParse(x, out int result)).Select(x => int.Parse(x)).ToArray()
            };

        public static Parser<ParsedTimetableData> Timetable =
            from identifier in Parse.String("UZ00")
            from space in Parse.WhiteSpace.Many()
            from comma in Parse.Char(',')
            from day in Parse.Number
            from slash in Parse.Char('/')
            from lesson in Parse.Number
            from comma2 in Parse.Char(',')
            from record in CsvParser.Record
            select new ParsedTimetableData
            {
                Day = int.Parse(day),
                Lesson = int.Parse(lesson),
                Week = record.ElementAt(0)?.Split("~~").FirstOrDefault(),
                Rooms = record.Skip(1).ToList()
            };

        /// <summary>
        /// Builds the tuition object based on the parsed input
        /// </summary>
        /// <param name="periods"></param>
        /// <returns></returns>
        protected override Tuition BuildItem(Dictionary<int, ParsedTuitionData> periods)
        {
            var tuition = new Tuition();

            foreach(var period in periods)
            {
                var numberOfGroupedTuitions = period.Value.TuitionDataMatrix.Count;

                for(int idx = 0; idx < numberOfGroupedTuitions; idx++)
                {
                    var matrix = period.Value.TuitionDataMatrix[idx];
                    var tuitionPeriod = new TuitionPeriod
                    {
                        TuitionNumber = period.Value.TuitionNumber,
                        TuitionIndex = (idx + 1),
                        PeriodNumber = period.Value.PeriodNumber,
                        StartDate = period.Value.StartDate,
                        EndDate = period.Value.EndDate
                    };

                    // Get data straight 🥴
                    tuitionPeriod.StudentGroup = matrix.StudentGroup;
                    tuitionPeriod.Subject = period.Value.Subjects.ElementAtOrDefault(matrix.SubjectIndex - 1);
                    tuitionPeriod.Teacher = period.Value.Teachers.ElementAtOrDefault(matrix.TeacherIndex - 1);

                    var labRoom = period.Value.LabRooms.ElementAtOrDefault(matrix.RoomIndex - 1);
                    var regularRoom = period.Value.RegularRooms.ElementAtOrDefault(matrix.RoomIndex - 1);

                    if (labRoom != regularRoom)
                    {
                        tuitionPeriod.LabRoom = labRoom;
                    }
                    else
                    {
                        tuitionPeriod.RegularRoom = regularRoom;
                    }

                    if (matrix.GradeStartIndex != null && matrix.GradeEndIndex != null)
                    {
                        tuitionPeriod.Grades = period.Value.Grades.Skip(matrix.GradeStartIndex.Value - 1).Take(matrix.GradeEndIndex.Value - matrix.GradeStartIndex.Value + 1).ToArray();
                    }
                    if (matrix.GradeIndices.Count() > 0)
                    {
                        tuitionPeriod.Grades = period.Value.Grades.Where((x, i) => matrix.GradeIndices.Contains(i + 1)).ToArray();
                    }

                    // Timetable
                    foreach(var timetableData in period.Value.Timetable)
                    {
                        var timetable = new TimetableEntry
                        {
                            Day = timetableData.Day,
                            Lesson = timetableData.Lesson,
                            Week = timetableData.Week,
                            Room = timetableData.Rooms.ElementAtOrDefault(matrix.RoomIndex - 1)
                        };

                        tuitionPeriod.Timetable.Add(timetable);
                    }

                    tuition.Periods.Add(tuitionPeriod);
                }
            }

            return tuition;
        }

        protected override ParsedTuitionData BuildPeriod(int periodNumber)
        {
            return new ParsedTuitionData { PeriodNumber = periodNumber };
        }

        protected override void ParseLine(string line, int[] currentPeriods, Dictionary<int, ParsedTuitionData> periods)
        {
            var tuitionInfo = Info.TryParse(line);
            if(tuitionInfo.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.TuitionNumber = tuitionInfo.Value.Number;
                    period.StartDate = tuitionInfo.Value.StartDate;
                    period.EndDate = tuitionInfo.Value.EndDate;
                });
                return;
            }

            var subjectsData = Subjects.TryParse(line);
            if(subjectsData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.Subjects.AddRange(subjectsData.Value);
                });
                return;
            }

            var gradesData = Grades.TryParse(line);
            if(gradesData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.Grades.AddRange(gradesData.Value);
                });
                return;
            }

            var teachersData = Teachers.TryParse(line);
            if(teachersData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.Teachers.AddRange(teachersData.Value);
                });
                return;
            }

            var roomsData = LabRooms.TryParse(line);
            if (roomsData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.LabRooms.AddRange(roomsData.Value);
                });
                return;
            }

            var regularRoomsData = RegularRooms.TryParse(line);
            if (regularRoomsData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.RegularRooms.AddRange(regularRoomsData.Value);
                });
                return;
            }

            var tuitionData = TuitionData.TryParse(line);
            if(tuitionData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.TuitionDataMatrix.Add(tuitionData.Value);
                });
                return;
            }

            var timetableData = Timetable.TryParse(line);
            if(timetableData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.Timetable.Add(timetableData.Value);
                });
                return;
            }
        }

        public class ParsedTuitionData
        {
            public int PeriodNumber { get; set; }

            public int TuitionNumber { get; set; }

            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }

            public List<string> Subjects { get; } = new List<string>();
            
            public List<string> Grades { get; } = new List<string>();
            
            public List<string> Teachers { get; } = new List<string>();

            public List<string> LabRooms { get; } = new List<string>();

            public List<string> RegularRooms { get; } = new List<string>();
            
            public List<ParsedTuitionDataMatrixData> TuitionDataMatrix { get; } = new List<ParsedTuitionDataMatrixData>();
            
            public List<ParsedTimetableData> Timetable { get; } = new List<ParsedTimetableData>();
        }

        public class ParsedTimetableData
        {
            public int Day { get; set; }

            public int Lesson { get; set; }

            public string Week { get; set; }

            public List<string> Rooms { get; set; }
        }

        public class ParsedTuitionDataMatrixData
        {
            public int TeacherIndex { get; set; }

            public int? GradeStartIndex { get; set; }

            public int? GradeEndIndex { get; set; }

            public int SubjectIndex { get; set; }

            public int RoomIndex { get; set; }

            public string StudentGroup { get; set; }

            public int[] GradeIndices { get; set; }
        }

        public class TuitionInfo
        {
            public int Number { get; set; }

            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }
        }

        private static DateTime? GetDateTimeOrNull(IOption<DateTime> value)
        {
            if(value.IsDefined)
            {
                return value.Get();
            }

            return null;
        }
    }
}
