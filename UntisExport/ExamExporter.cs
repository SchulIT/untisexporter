using HtmlAgilityPack;
using SchulIT.UntisExport.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchulIT.UntisExport
{
    public class ExamExporter : AbstractExporter, IExamExporter
    {
        private const string TableSelector = "//table[@rules='all']";

        public Task<ExamExportResult> ParseHtmlAsync(ExamExportSettings settings, string html)
        {
            return Task.Run(() =>
            {
                // Step 1: Parse HTML
                var document = new HtmlDocument();
                document.LoadHtml(html);

                // Step 2: Parse Exams
                var exams = GetExams(document, settings);

                return new ExamExportResult(exams.AsReadOnly());
            });
        }

        private List<Exam> GetExams(HtmlDocument document, ExamExportSettings settings)
        {
            var exams = new List<Exam>();
            var table = document.DocumentNode.SelectSingleNode(TableSelector);

            if(table == null)
            {
                return exams;
            }

            var nodes = table.SelectNodes("./tr");
            var order = GetColumnOrder(nodes.First(), settings.ColumnSettings);

            for(var i = 1; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var cells = node.SelectNodes("./td").Select(x => x.InnerText).ToArray();

                var exam = new Exam
                {
                    Name = ParseCell(cells, order.NameColumn, x => ClearString(x), null),
                    Date = ParseCell(cells, order.DateColumn, x => ParseDate(ClearString(x), settings.DateTimeFormat), default),
                    LessonStart = ParseCell(cells, order.LessonStartColumn, x => ParseIntegerColumn(ClearString(x)), default),
                    LessonEnd = ParseCell(cells, order.LessonEndColumn, x => ParseIntegerColumn(ClearString(x)), default),
                    Grades = ParseCell(cells, order.GradesColumn, x => ParseMultiValueStringColumn(x, settings.ColumnSettings.GradesSeparator), default),
                    Courses = ParseCell(cells, order.CoursesColumn, x => ParseMultiValueStringColumn(x, settings.ColumnSettings.CoursesSeparator), default),
                    Teachers = ParseCell(cells, order.TeachersColumn, x => ParseMultiValueStringColumn(x, settings.ColumnSettings.TeachersSeparator), default),
                    Rooms = ParseCell(cells, order.RoomsColumn, x => ParseMultiValueStringColumn(x, settings.ColumnSettings.RoomsSeparator), default),
                    Description = ParseCell(cells, order.DescriptionColumn, x => ClearString(x), default)
                };

                exams.Add(exam);
            }

            return exams;
        }

        private string[] ParseMultiValueStringColumn(string value, char separator)
        {
            if(value == null)
            {
                return Array.Empty<string>();
            }
            
            return value.Split(separator)
                .Select(x => ClearString(x))
                .Where(x => x != null)
                .ToArray();
        }

        private ColumnOrder GetColumnOrder(HtmlNode headerNode, ExamColumnSettings settings)
        {
            var columns = headerNode.SelectNodes("./td");
            var texts = columns.Select(x => x.InnerText).Select((x, i) => new { Key = i, Value = ClearString(x) }).ToDictionary(x => x.Value, x => x.Key);

            return new ColumnOrder
            {
                NameColumn = GetColumnIndexOrNull(texts, settings.NameColumn),
                DateColumn = GetColumnIndexOrNull(texts, settings.DateColumn),
                LessonStartColumn = GetColumnIndexOrNull(texts, settings.LessonStartColumn),
                LessonEndColumn = GetColumnIndexOrNull(texts, settings.LessonEndColumn),
                GradesColumn = GetColumnIndexOrNull(texts, settings.GradesColumn),
                CoursesColumn = GetColumnIndexOrNull(texts, settings.CoursesColumn),
                TeachersColumn = GetColumnIndexOrNull(texts, settings.TeachersColumn),
                RoomsColumn = GetColumnIndexOrNull(texts, settings.RoomsColumn),
                DescriptionColumn = GetColumnIndexOrNull(texts, settings.RemarkColumn)
            };
        }

        private class ColumnOrder
        {
            public int? NameColumn { get; set; }
            public int? DateColumn { get; set; }
            public int? LessonStartColumn { get; set; }
            public int? LessonEndColumn { get; set; }
            public int? GradesColumn { get; set; }
            public int? CoursesColumn { get; set; }
            public int? TeachersColumn { get; set; }
            public int? RoomsColumn { get; set; }
            public int? DescriptionColumn { get; set; }
        }
    }
}
