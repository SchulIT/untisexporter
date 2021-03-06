using System;
using System.Collections.Generic;
using System.Text;

namespace SchulIT.UntisExport.Model
{
    public class TeacherPeriod
    {
        public int PeriodNumber { get; set; }

        public string Status { get; set; }

        public string Acronym { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public List<string> Subjects { get; set; } = new List<string>();
    }
}
