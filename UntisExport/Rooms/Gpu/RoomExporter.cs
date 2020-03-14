using FileHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Rooms.Gpu
{
    public class RoomExporter : IRoomExporter
    {
        public Task<IEnumerable<Room>> ParseGpuAsync(string gpu, RoomExportSettings settings)
        {
            return Task.Run(() =>
            {
                var engine = new DelimitedFileEngine<GpuRooom>();
                engine.Options.Delimiter = settings.Delimiter;
                var result = engine.ReadString(gpu) as GpuRooom[];

                var exams = result.Select(x =>
                {
                    return new Room
                    {
                        Name = x.Name,
                        LongName = x.LongName,
                        Capacity = x.Capacity
                    };
                });

                return exams;
            });
        }

        [DelimitedRecord(",")]
        private class GpuRooom
        {
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Name { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string LongName { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string AlternativeRoom { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Indicator { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Free { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string DislozIndicator { get; set; }

            [FieldValueDiscarded]
            public int? Weight { get; set; }

            public int? Capacity { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Department { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Floor1 { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Floor2 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string ExtraText { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Description { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string ForegroundColor { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string BackgroundColor { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Statistics1 { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string Statistics2 { get; set; }

            [FieldValueDiscarded]
            [FieldQuoted('"', QuoteMode.OptionalForRead)]
            public string UnknownField { get; set; }
        }
    }
}
