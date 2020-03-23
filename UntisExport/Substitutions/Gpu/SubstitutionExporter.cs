using FileHelpers;
using SchulIT.UntisExport.Common.Gpu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Substitutions.Gpu
{
    public class SubstitutionExporter : ISubstitutionExporter
    {
        public Task<SubstitutionExportResult> ParseGpuAsync(string gpu, SubstitutionExportSettings settings)
        {
            return Task.Run(() =>
            {
                var engine = new DelimitedFileEngine<GpuSubstitution>();
                engine.Options.Delimiter = settings.Delimiter;
                var result = engine.ReadString(gpu) as GpuSubstitution[];

                var substitutions = result.Select(x =>
                {
                    return new Substitution
                    {
                        Id = x.Id,
                        Date = x.Date,
                        LessonStart = x.Lesson,
                        LessonEnd = x.Lesson,
                        Type = settings.TypeConverter?.ConvertType(x.Type, x.SubstitutionType),
                        Subject = x.Subject,
                        ReplacementSubject = x.ReplacementSubject,
                        Room = x.Room,
                        ReplacementRoom = x.ReplacementRoom,
                        Teachers = !string.IsNullOrEmpty(x.AbsenceTeacher) ? new string[1] { x.AbsenceTeacher } : Array.Empty<string>(),
                        ReplacementTeachers = !string.IsNullOrEmpty(x.ReplacementTeacher) ? new string[1] { x.ReplacementTeacher } : Array.Empty<string>(),
                        Grades = x.Grades ?? new List<string>(),
                        ReplacementGrades = x.ReplacementGrades ?? new List<string>(),
                        Remark = string.IsNullOrWhiteSpace(x.Remark) ? null : x.Remark
                    };
                });

                return new SubstitutionExportResult(substitutions.ToList().AsReadOnly());
            });
        }

        [DelimitedRecord(",")]
        private class GpuSubstitution
        {
            public int Id { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(DateConverter))]
            public DateTime Date { get; set; }

            public int Lesson { get; set; }

            public int? AbsenceNumber { get; set; }

            public int TuitionNumber { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string AbsenceTeacher { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string ReplacementTeacher { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Subject { get; set; }

            public string StatisticsTagForSubject { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string ReplacementSubject { get; set; }

            public string StatisticsTagForReplacementSubject { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Room { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string ReplacementRoom { get; set; }

            public string StatisticsTag { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            
            [FieldConverter(typeof(SeparatedValuesConverter))]
            public List<string> Grades { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string AbsenceReason { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Remark { get; set; }

            public int Type { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(SeparatedValuesConverter))]
            public List<string> ReplacementGrades { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string SubstitutionType { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(DateTimeConverter))]
            public DateTime? LastChange { get; set; }

            public string Footer { get; set; }
        }
    }
}
