using System;
using System.Collections.Generic;
using System.Text;

namespace SchulIT.UntisExport.Model
{
    public class Settings
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int Periodicity { get; set; }

        public int StartWeek { get; set; }

        public int NumberOfDays { get; set; }

        public int NumberOfLessonsPerDay { get; set; }

        public DayOfWeek FirstSchoolDayOfWeek { get; set; }

        public int NumberOfFirstLesson { get; set; }
    }
}
