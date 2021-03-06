using System.Collections.Generic;

namespace SchulIT.UntisExport.Model
{
    public class Tuition
    {
        public List<TuitionPeriod> Periods { get; } = new List<TuitionPeriod>();
    }
}
