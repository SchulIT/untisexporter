using System.Collections.Generic;

namespace SchulIT.UntisExport.Model
{
    public class RoomPeriod
    {
        public int PeriodNumber { get; set; }

        public string Name { get; set; }

        public string LongName { get; set; }

        public int? Capacity { get; set; }

        public string AlternativeRoom { get; set; }

        public List<string> Floors { get; } = new List<string>();
    }
}
