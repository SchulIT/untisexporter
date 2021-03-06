using System;
using System.Collections.Generic;

namespace SchulIT.UntisExport.Model
{
    public class SupervisionFloor
    {
        public string Name { get; set; }

        public string LongName { get; set; }

        public List<Supervision> Supervisions { get; } = new List<Supervision>();
    }
}
