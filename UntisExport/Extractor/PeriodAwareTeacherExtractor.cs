using SchulIT.UntisExport.Model;
using Sprache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    /// <summary>
    /// Extracts teachers from an period aware file.
    /// !!!Note: this won't work on non-period aware files!!!
    /// </summary>
    public class PeriodAwareTeacherExtractor : PeriodAwareExtractor<Teacher, TeacherPeriod>
    {
        protected override Parser<IEnumerable<int>> StartSequence => TeacherPeriod;

        protected override Parser<char> NonStartSequence => NonTeacherStartSequence;

        public static Parser<IEnumerable<int>> TeacherPeriod =
            from identifier in Parse.String("0pL")
            from space in Parse.WhiteSpace.Many()
            from sep in Parse.Char(',')
            from space2 in Parse.WhiteSpace.Optional()
            from numbers in Parsers.Integer.AtLeastOnce()
            select numbers;

        public static Parser<string> QualificationLine =
            from identifier in Parse.String("BF")
            from space in Parse.WhiteSpace.Many()
            from comma in Parse.Char(',')
            from subject in CsvParser.Cell
            from comma2 in Parse.Char(',')
            from any in Parse.AnyChar.Except(Parse.LineEnd).Many()
            select subject;

        public static Parser<TeacherBasicData> TeacherBasic =
            from identifier in Parse.String("00L")
            from statisticsIdentifier in Parse.AnyChar.Except(Parse.WhiteSpace).Optional().Many()
            from space in Parse.WhiteSpace.Many()
            from comma in Parse.Char(',')
            from acronym in Parse.AnyChar.Except(Parse.Char(',')).Many().Text()
            from comma2 in Parse.Char(',')
            from lastname in Parsers.QuotedText
            from any in Parse.AnyChar.Except(Parse.LineEnd).Many().Optional()
            select new TeacherBasicData(acronym, lastname);

        public static Parser<IEnumerable<string>> TeacherExtendedLine =
            from identifier in Parse.String("Le")
            from space in Parse.WhiteSpace.Many()
            from comma in Parse.Char(',')
            from record in CsvParser.Record
            select record;

        public static Parser<char> NonTeacherStartSequence =
            from identifier in Parse.String("0p")
            from nonTeacher in Parse.CharExcept("L")
            select nonTeacher;

        public class TeacherBasicData
        {
            public string Acronym { get; private set; }
            public string Lastname { get; private set; }

            public TeacherBasicData(string acronym, string lastname)
            {
                Acronym = acronym;
                Lastname = lastname;
            }
        }

        public class TeacherExtendedData
        {
            public string Firstname { get; private set; }

            public IEnumerable<string> Subjects { get; private set; }

            public TeacherExtendedData(string firstname, IEnumerable<string> subjects)
            {
                Firstname = firstname;
                Subjects = subjects;
            }
        }

        protected override void ParseLine(string line, int[] currentPeriods, Dictionary<int, TeacherPeriod> periods)
        {
            var basicData = TeacherBasic.TryParse(line);
            if (basicData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.Acronym = basicData.Value.Acronym;
                    period.Lastname = basicData.Value.Lastname;
                });

                return;
            }

            var extendedData = TeacherExtendedLine.TryParse(line);
            if (extendedData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.Firstname = extendedData.Value.ElementAt(4); // Firstname | TODO: Improve
                });

                return;
            }

            var qualificationData = QualificationLine.TryParse(line);
            if (qualificationData.WasSuccessful)
            {
                ApplyToPeriods(currentPeriods, periods, period =>
                {
                    period.Subjects.Add(qualificationData.Value);
                });

                return;
            }
        }

        protected override TeacherPeriod BuildPeriod(int periodNumber)
        {
            return new TeacherPeriod { PeriodNumber = periodNumber };
        }

        protected override Teacher BuildItem(Dictionary<int, TeacherPeriod> periods)
        {
            var teacher = new Teacher();
            teacher.Periods.AddRange(periods.Values.ToList());

            return teacher;
        }
    }
}
