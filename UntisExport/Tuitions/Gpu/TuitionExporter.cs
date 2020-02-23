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
                        //Room = x.Room,
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

            //[FieldQuoted('"', QuoteMode.OptionalForRead)]
            //public string Room { get; set; }
        }
    }
}
