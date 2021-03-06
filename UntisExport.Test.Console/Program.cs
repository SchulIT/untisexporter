using SchulIT.UntisExport;
using SchulIT.UntisExport.Utilities;
using System.Linq;

namespace UntisExport.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Untis Export CLI test tool. Please insert path to GPN file:");
            var file = System.Console.ReadLine();
            var result = UntisExporter.ParseFile(file);

            foreach (var room in result.Rooms)
            {
                foreach (var period in room.Periods)
                {
                    System.Console.WriteLine($"{string.Join('-', period.PeriodNumber)} - {period.Name} - {period.LongName}, {period.AlternativeRoom} - {period.Capacity} - {string.Join(", ", period.Floors)}");
                }
            }

            foreach (var period in result.Periods)
            {
                System.Console.WriteLine($"{period.Name} - {period.LongName} - {period.Start.ToShortDateString()} - {period.End.ToShortDateString()}");
            }

            foreach (var subject in result.Subjects)
            {
                System.Console.WriteLine($"{subject.Abbreviation}: {subject.Name}");
            }

            foreach (var teacher in result.Teachers)
            {
                foreach(var period in teacher.Periods)
                {
                    System.Console.WriteLine($"{period.PeriodNumber} - {period.Acronym} - {period.Lastname}, {period.Firstname} - {string.Join(", ", period.Subjects)}");
                }
            }

            foreach(var tuition in result.Tuitions)
            {
                foreach(var period in tuition.Periods)
                {
                    System.Console.WriteLine($"{period.PeriodNumber} - {period.TuitionNumber} - {string.Join(',', period.Grades)} - {period.Teacher} - {period.Subject}");
                }
            }

            foreach(var @event in result.Events)
            {
                System.Console.WriteLine($"Event: {@event.Text}");
            }

            var weeks = WeekResolver.SchoolWeekToCalendarWeek(result);

            foreach(var week in weeks)
            {
                System.Console.WriteLine($"{week.SchoolYearWeek} ({week.TuitionWeek}) - KW{week.CalendarWeek} - {week.WeekName} - {week.FirstDay.ToShortDateString()}");
            }

            return;
        }
    }
}
