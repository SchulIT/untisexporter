using System;
using System.Collections.Generic;

namespace SchulIT.UntisExport.Model
{
    public class Event
    {
        public int Number { get; set; }

        public List<string> Teachers { get; } = new List<string>();

        public List<string> Rooms { get; } = new List<string>();

        public List<string> Grades { get; } = new List<string>();

        public DateTime StartDate { get; set; }

        public int StartLesson { get; set; }

        public DateTime EndDate { get; set; }

        public int EndLesson { get; set; }

        public string Text { get; set; }

    }
}
