using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Tuitions.Gpu
{
    public class TuitionExporter : ITuitionExporter
    {
        public Task<IEnumerable<Tuition>> ParseGpuAsync(string gpu, TuitionExportSettings settings)
        {
            return Task.Run(() =>
            {
                var engine = new DelimitedFileEngine<GpuTuition>();
                engine.Options.Delimiter = settings.Delimiter;
                var result = engine.ReadString(gpu) as GpuTuition[];

                var exams = result.Select(x =>
                {
                    return new Tuition
                    {
                        Id = x.Id,
                        Grade = x.Grade,
                        Room = x.Room,
                        Subject = x.Subject,
                        Teacher = x.Teacher
                    };
                });

                return exams;
            });
        }

        [DelimitedRecord(",")]
        private class GpuTuition
        {
            public int Id { get; set; }

            [FieldValueDiscarded]
            public int? HoursPerWeek { get; set; }

            [FieldValueDiscarded]
            public int? HoursPerWeekCourse { get; set; }

            [FieldValueDiscarded]
            public int? HoursPerWeekTeacher { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Grade { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Teacher { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Subject { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Room { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Statistics1Tuition { get; set; }

            [FieldValueDiscarded]
            public int? NumberOfLessons { get; set; }

            [FieldValueDiscarded]
            public int? WeekValue { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Group { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string LineText1 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string LineValue { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string From { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Until { get; set; }

            [FieldValueDiscarded]
            public int? YearValue { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Text { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string DivideNumber { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string RootRoom { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Description { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string ForegroundColor { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string BackgroundColor { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Identifier { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string UpcomingGrades { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string UpcomingTeacher { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string GradeCollisionIdentifier { get; set; }

            [FieldValueDiscarded]
            public int? DoubleLessonMin { get; set; }

            [FieldValueDiscarded]
            public int? DoubleLessonMax { get; set; }

            [FieldValueDiscarded]
            public int? BlockSize { get; set; }

            [FieldValueDiscarded]
            public int? LessonsInRoom { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Priority { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Statistics1Teacher { get; set; }

            [FieldValueDiscarded]
            public int? NumberOfMaleStudents { get; set; }

            [FieldValueDiscarded]
            public int? NumberOfFemaleStudents { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string ValueOrFactor { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string SecondBlock { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string ThirdBlock { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string LineText2 { get; set; }

            [FieldValueDiscarded]
            public int? EigenValue { get; set; }

            [FieldValueDiscarded]
            public double? EigenValueFraction { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string StudentGroup { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string WeekHours { get; set; }

            [FieldValueDiscarded]
            public int? YearHours { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string LineTuitionGroup { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Unknown1 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Unknown2 { get; set; }
        }
    }
}
