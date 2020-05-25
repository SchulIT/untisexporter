using FileHelpers;
using SchulIT.UntisExport.Common.Gpu;
using SchulIT.UntisExport.Tuitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Exams.Gpu
{
    public class ExamExporter : IExamExporter
    {
        public Task<ExamExportResult> ParseGpuAsync(string gpu, ExamExportSettings settings, IEnumerable<Tuition> tuitions = null)
        {
            return Task.Run(() =>
            {
                var engine = new DelimitedFileEngine<GpuExam>();
                engine.Options.Delimiter = settings.Delimiter;
                var result = engine.ReadString(gpu) as GpuExam[];

                var exams = result.Select(exam =>
                {
                    var examGrades = tuitions?
                        .Where(tuition => exam.TuitionIds.Contains(tuition.Id) && exam.Courses.Contains(tuition.Subject))
                        .Select(tuition => tuition.Grade)
                        .Distinct()
                        .ToArray();

                    return new Exam
                    {
                        Id = exam.Id,
                        Name = exam.Name,
                        Grades = examGrades ?? Array.Empty<string>(),
                        LessonStart = exam.LessonStart,
                        LessonEnd = exam.LessonEnd,
                        Remark = exam.Remark,
                        Date = exam.Date,
                        Courses = exam.Courses,
                        Supervisions = exam.Supervisions,
                        Rooms = exam.Rooms
                    };
                });

                return new ExamExportResult(exams.ToList().AsReadOnly());
            });
        }

        [DelimitedRecord(",")]
        internal class GpuExam
        {
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldTrim(TrimMode.Both)]
            public string Name { get; set; }

            public int? Id { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldTrim(TrimMode.Both)]
            public string Remark { get; set; }

            [FieldConverter(typeof(DateConverter))]
            public DateTime Date { get; set; }

            public int LessonStart { get; set; }

            public int LessonEnd { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(SeparatedValuesConverter))]
            [FieldTrim(TrimMode.Both)]
            public List<string> Courses { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(SeparatedIntValuesConverter), '~', true)]
            [FieldTrim(TrimMode.Both)]
            public List<int> TuitionIds { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(SeparatedValuesConverter))]
            [FieldTrim(TrimMode.Both)]
            public List<string> Students { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(SeparatedValuesConverter), '-', true)]
            [FieldTrim(TrimMode.Both)]
            public List<string> Supervisions { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(SeparatedValuesConverter), '-', true)]
            [FieldTrim(TrimMode.Both)]
            public List<string> Rooms { get; set; }
        }
    }
}
