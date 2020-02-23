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
        public Task<IEnumerable<Substitutions.Substitution>> ParseGpuAsync(string gpu, SubstitutionExportSettings settings)
        {
            return Task.Run(() =>
            {
                var engine = new FileHelperEngine(typeof(Substitution));
                var result = engine.ReadString(gpu) as Substitution[];

                var substitutions = result.Select(x =>
                {
                    return new Substitutions.Substitution
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

                return substitutions;
            });
        }

        [DelimitedRecord(",")]
        private class Substitution
        {
            public int Id;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(DateConverter))]
            public DateTime Date;

            public int Lesson;

            public int? AbsenceNumber;

            public int TuitionNumber;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string AbsenceTeacher;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string ReplacementTeacher;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Subject;

            public string StatisticsTagForSubject;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string ReplacementSubject;

            public string StatisticsTagForReplacementSubject;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Room;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string ReplacementRoom;

            public string StatisticsTag;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            
            [FieldConverter(typeof(SeparatedValuesConverter))]
            public List<string> Grades;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string AbsenceReason;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Remark;

            public int Type;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(SeparatedValuesConverter))]
            public List<string> ReplacementGrades;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string SubstitutionType;

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(DateTimeConverter))]
            public DateTime? LastChange;

            public string Footer;
        }
    }
}
