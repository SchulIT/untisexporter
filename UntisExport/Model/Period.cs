using System;

namespace SchulIT.UntisExport.Model
{
    public class Period
    {
        public int Number { get; internal set; }

        public string Name { get; set; }

        public string LongName { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Parent { get; set; }
    }
}
