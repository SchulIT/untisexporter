using System;

namespace SchulIT.UntisExport.Model
{
    public class Holiday
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public HolidayType Type { get; set; }

        public string Name { get; set; }

        public string LongName { get; set; }

        public int WeekAfterHolidays { get; set; }

        public bool ContinueWeekNumbering { get; set; } = false;
    }

    public enum HolidayType
    {
        Ferien,
        Feiertag
    }
}
