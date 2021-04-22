using System;
using System.Collections.Generic;

namespace SchulIT.UntisExport.Model
{
    public class TuitionPeriod
    {
        public int PeriodNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int TuitionNumber { get; set; }

        public int TuitionIndex { get; set; }

        public string Subject { get; set; }

        public string Teacher { get; set; }

        public string[] Grades { get; set; } = Array.Empty<string>();

        public string LabRoom { get; set; }

        public string RegularRoom { get; set; }

        public string StudentGroup { get; set; }

        public string[] TuitionGroups { get; set; }

        public List<TimetableEntry> Timetable { get; } = new List<TimetableEntry>();
    }
}
