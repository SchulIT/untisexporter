using FileHelpers;
using SchulIT.UntisExport.Common.Gpu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Supervisions.Gpu
{
    public class SupervisionExporter : ISupervisionExporter
    {
        public Task<IEnumerable<Supervision>> ParseGpuAsync(string gpu, SupervisionExportSettings settings)
        {
            return Task.Run(() =>
            {
                var engine = new DelimitedFileEngine<GpuSupervision>();
                engine.Options.Delimiter = settings.Delimiter;
                var result = engine.ReadString(gpu) as GpuSupervision[];

                var supervisions = result.Select(x =>
                {
                    return new Supervision
                    {
                        Location = x.Location,
                        Teacher = x.Teacher,
                        WeekDay = x.WeekDay,
                        Lesson = x.Lesson,
                        Weeks = x.Weeks
                    };
                });

                return supervisions;
            });
        }

        [DelimitedRecord(",")]
        internal class GpuSupervision
        {
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Location { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Teacher { get; set; }

            public int WeekDay { get; set; }

            public int Lesson { get; set; }

            public int Duration { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            [FieldConverter(typeof(SeparatedIntValuesConverter))]
            public List<int> Weeks { get; set; }
        }
    }
}
