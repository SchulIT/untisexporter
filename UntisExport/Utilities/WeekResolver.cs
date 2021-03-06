using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SchulIT.UntisExport.Utilities
{
    public static class WeekResolver
    {
        public static List<Week> SchoolWeekToCalendarWeek(UntisExportResult result)
        {
            var weeks = new List<Week>();
            var weekNames = Enumerable.Range(65, result.Settings.Periodicity).Select(x => (char)x).ToArray();

            var current = new DateTime(result.Settings.Start.Ticks);
            var last = result.Settings.End;

            var currentWeekIdx = result.Settings.StartWeek - 1;
            var schoolWeek = 1;
            var tuitionWeek = 1;

            var holidays = result.Holidays.Where(x => x.Type == Model.HolidayType.Ferien).ToList();
            var weekShift = 0;

            do
            {
                // check if holidays
                var holiday = holidays.FirstOrDefault(x => x.Start <= current && current <= x.End);

                if (holiday != null)
                {
                    // current day seems to be a holiday
                    if (holiday.ContinueWeekNumbering == false)
                    {
                        weekShift = holiday.WeekAfterHolidays;
                    }
                }
                else
                {
                    if(weekShift > 0)
                    {
                        currentWeekIdx = (currentWeekIdx + weekShift + 1) % result.Settings.Periodicity;
                        weekShift = 0;
                    }

                    weeks.Add(new Week
                    {
                        FirstDay = new DateTime(current.Ticks),
                        SchoolYearWeek = schoolWeek,
                        TuitionWeek = tuitionWeek,
                        CalendarWeek = CultureInfo.CurrentUICulture.Calendar.GetWeekOfYear(current, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday),
                        WeekName = weekNames[currentWeekIdx]
                    });

                    tuitionWeek++;
                    currentWeekIdx = (currentWeekIdx + 1) % result.Settings.Periodicity;
                }

                schoolWeek++;
                current = current.AddDays(7); // add a week
            } while (current < last);

            return weeks;
        }
    }

    public class Week
    {
        public DateTime FirstDay { get; internal set; }

        public int SchoolYearWeek { get; internal set; }

        public int TuitionWeek { get; internal set; }

        public int CalendarWeek { get; internal set; }

        public char WeekName { get; internal set; }
    }
}
