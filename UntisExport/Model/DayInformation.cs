using System;
using System.Collections.Generic;

namespace SchulIT.UntisExport.Model
{
    public class DayInformation
    {
        public DateTime Date { get; set; }

        public List<DayText> Texts { get; } = new List<DayText>();

        public List<int> FreeLessons { get; } = new List<int>();

        public string Note { get; set; }

        public DayType Type { get; set; } = DayType.Normal;

        public string Reason { get; set; }
    }

    public enum DayType
    {
        Normal,
        Unterrichtsfrei,
        Feiertag
    }
}
