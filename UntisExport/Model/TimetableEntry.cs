using System;
using System.Collections.Generic;
using System.Text;

namespace SchulIT.UntisExport.Model
{
    public class TimetableEntry
    {
        public string Room { get; set; }

        public string Week { get; set; }

        public int Day { get; set; }

        public int Lesson { get; set; }
    }
}
