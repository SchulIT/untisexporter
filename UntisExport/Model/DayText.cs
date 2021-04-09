using System;

namespace SchulIT.UntisExport.Model
{
    public class DayText
    {
        public string Guid { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Text { get; set; }

        public string[] Grades { get; set; } = Array.Empty<string>();

        public int[] Lessons { get; set; } = Array.Empty<int>();
    }
}
