using SchulIT.UntisExport.Model;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public class PeriodAwareRoomExtractor : PeriodAwareExtractor<Room, RoomPeriod>
    {
        protected override Parser<IEnumerable<int>> StartSequence => RoomPeriod;

        protected override Parser<char> NonStartSequence => NonTeacherStartSequence;

        public static Parser<IEnumerable<int>> RoomPeriod =
            from identifier in Parse.String("0pR")
            from space in Parse.WhiteSpace.Many()
            from sep in Parse.Char(',')
            from space2 in Parse.WhiteSpace.Optional()
            from numbers in Parsers.Integer.AtLeastOnce()
            select numbers;

        public static Parser<char> NonTeacherStartSequence =
            from identifier in Parse.String("0p")
            from nonTeacher in Parse.CharExcept("R")
            select nonTeacher;

        public static Parser<RoomBasicData> Room =
            from identifier in Parse.String("00R")
            from statisticsIdentifier in Parse.AnyChar.Except(Parse.WhiteSpace).Optional().Many()
            from space in Parse.WhiteSpace.Many()
            from comma in Parse.Char(',')
            from record in CsvParser.Record
            select new RoomBasicData
            {
                Name = record.ElementAt(0),
                LongName = record.ElementAt(1),
                AlternativeRoom = record.ElementAt(2)
            };

        public static Parser<RoomExtendedData> RoomCapacityWeight =
            from identifier in Parse.String("RA")
            from capacity in Parse.Number.Optional()
            from whitespace in Parse.WhiteSpace.AtLeastOnce()
            from weight in Parse.Number.Optional()
            select new RoomExtendedData
            {
                Capacity = FromOptionalNumber(capacity),
                Weight = FromOptionalNumber(weight)
            };

        public static Parser<string> RoomFloor = 
            from identifier in Parse.String("Ra")
            from whitespace in Parse.WhiteSpace.Optional().Many()
            from comma in Parse.Char(',')
            from floor in Parsers.QuotedText
            select floor;

        private static int? FromOptionalNumber(IOption<string> number)
        {
            if (number.IsDefined)
            {
                return int.Parse(number.Get());
            }

            return null;
        }

        public class RoomBasicData
        {
            public string Name { get; set; }
            public string LongName { get; set; }
            public string AlternativeRoom { get; set; }
        }

        public class RoomExtendedData
        {
            public int? Capacity { get; set; }
            public int? Weight { get; set; }
        }

        protected override Room BuildItem(Dictionary<int, RoomPeriod> periods)
        {
            var room = new Room();
            room.Periods.AddRange(periods.Values.ToList());

            return room;
        }

        protected override RoomPeriod BuildPeriod(int periodNumber)
        {
            return new RoomPeriod { PeriodNumber = periodNumber };
        }

        protected override void ParseLine(string line, int[] currentPeriods, Dictionary<int, RoomPeriod> periods)
        {
            var basicData = Room.TryParse(line);
            if (basicData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.Name = basicData.Value.Name;
                    period.LongName = basicData.Value.LongName;
                    period.AlternativeRoom = basicData.Value.AlternativeRoom;
                });
                return;
            }

            var extendedData = RoomCapacityWeight.TryParse(line);
            if(extendedData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.Capacity = extendedData.Value.Capacity;
                });
                return;
            }

            var floorData = RoomFloor.TryParse(line);
            if(floorData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.Floors.Add(floorData.Value);
                });
                return;
            }
        }
    }
}
