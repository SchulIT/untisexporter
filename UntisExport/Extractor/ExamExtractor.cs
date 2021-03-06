using SchulIT.UntisExport.Model;
using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public class ExamExtractor : AbstractMultilineExtractor<Exam, Exam>
    {
        public static readonly Parser<Exam> ExamStartSequence =
            from identifier in Parse.String("0k")
            from lessonStart in Parse.Number
            from space in Parse.WhiteSpace.Many()
            from lessonEnd in Parse.Number
            from space2 in Parse.WhiteSpace.Many()
            from comma in Parse.Char(',')
            from number in Parse.Number
            from comma2 in Parse.Char(',')
            from name in CsvParser.QuotedCell
            from comma3 in Parse.Char(',')
            from date in Parsers.DateTime
            from comma4 in Parse.Char(',')
            from any in Parse.AnyChar.Many()
            select new Exam
            {
                Number = int.Parse(number),
                Name = name,
                LessonStart = int.Parse(lessonStart),
                LessonEnd = int.Parse(lessonEnd),
                Date = date
            };

        public static readonly Parser<char> NonExamStartSequence =
            from identifier in Parse.String("0")
            from character in Parse.CharExcept('k')
            select character;

        public static readonly Parser<ExamCourse> Course =
            from quoteStart in Parse.Char('"')
            from number in Parse.Number
            from sep in Parse.Char('~')
            from name in Parse.CharExcept('~').Many().Text()
            from sep2 in Parse.Char('~')
            from index in Parse.Number
            from quoteEnd in Parse.Char('"')
            select new ExamCourse
            {
                TuitionNumber = int.Parse(number),
                CourseName = name,
                TuitionIndex = int.Parse(index)
            };

        public static readonly Parser<IEnumerable<ExamCourse>> Courses =
            from identifier in Parse.String("kk1")
            from space in Parse.CharExcept(',').Many()
            from comma in Parse.Char(',')
            from courses in Course.DelimitedBy(Parse.Char(',')).Token()
            select courses;

        public static readonly Parser<IEnumerable<string>> Students =
            from identifier in Parse.String("kk2")
            from space in Parse.CharExcept(',').Many()
            from comma in Parse.Char(',')
            from records in CsvParser.Record
            select records;

        public static readonly Parser<IEnumerable<string>> Supervisions =
            from identifier in Parse.String("kk3")
            from space in Parse.CharExcept(',').Many()
            from comma in Parse.Char(',')
            from records in CsvParser.Record
            select records;

        public static readonly Parser<IEnumerable<string>> Rooms =
            from identifier in Parse.String("kk4")
            from space in Parse.CharExcept(',').Many()
            from comma in Parse.Char(',')
            from records in CsvParser.Record
            select records;

        protected override IEnumerable<Exam> BuildItems(Exam dto)
        {
            return new Exam[] { dto };
        }

        protected override bool IsEndSequence(string line)
        {
            return NonExamStartSequence.TryParse(line).WasSuccessful;
        }

        protected override bool IsStartSequence(string line, out Exam dto)
        {
            dto = new Exam();
            var examLine = ExamStartSequence.TryParse(line);
            if (examLine.WasSuccessful)
            {
                dto = examLine.Value;
                return true;
            }

            return false;
        }

        protected override void ParseLine(string line, Exam dto)
        {
            var coursesLine = Courses.TryParse(line);
            if(coursesLine.WasSuccessful)
            {
                dto.Courses = coursesLine.Value.ToArray();
                return;
            }

            var studentLine = Students.TryParse(line);
            if(studentLine.WasSuccessful)
            {
                dto.Students = studentLine.Value.ToArray();
                return;
            }

            var supervisionLine = Supervisions.TryParse(line);
            if (supervisionLine.WasSuccessful)
            {
                dto.Supervisions = supervisionLine.Value.ToArray();
                return;
            }

            var roomLine = Rooms.TryParse(line);
            if (roomLine.WasSuccessful)
            {
                dto.Rooms = roomLine.Value.ToArray();
                return;
            }
        }
    }
}
