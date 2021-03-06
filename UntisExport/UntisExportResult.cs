using SchulIT.UntisExport.Model;
using System.Collections.Generic;

namespace SchulIT.UntisExport
{
    public class UntisExportResult
    {
        public string File { get; internal set; }

        public Settings Settings { get; internal set; }

        public List<Holiday> Holidays { get; } = new List<Holiday>();

        public List<Period> Periods { get; } = new List<Period>();

        public List<Teacher> Teachers { get; } = new List<Teacher>();

        public List<Subject> Subjects { get; } = new List<Subject>();

        public List<Room> Rooms { get; } = new List<Room>();

        public List<Tuition> Tuitions { get; } = new List<Tuition>();

        public List<Substitution> Substitutions { get; } = new List<Substitution>();

        public List<Absence> Absences { get; } = new List<Absence>();

        public List<DayInformation> Days { get; } = new List<DayInformation>();

        public List<Exam> Exams { get; } = new List<Exam>();

        public List<SupervisionFloor> SupervisionFloors { get; } = new List<SupervisionFloor>();

        public List<Event> Events { get; } = new List<Event>();
    }
}
