using System;
using System.Collections.Generic;

namespace SchulIT.UntisExport.Model
{
    public class Supervision
    {
        public string Teacher { get; set; }

        public int Day { get; set; }

        public int Lesson { get; set; }

        public List<int> Weeks { get; } = new List<int>();
    }
}
