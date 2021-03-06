using System.Collections.Generic;

namespace SchulIT.UntisExport.Model
{
    public class Teacher
    {
        public List<TeacherPeriod> Periods { get; } = new List<TeacherPeriod>();
    }
}
