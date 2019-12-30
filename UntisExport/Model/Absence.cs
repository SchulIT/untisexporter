namespace SchulIT.UntisExport.Model
{
    public class Absence
    {
        
        public ObjectiveType Type { get; set; }

        public string Objective { get; set; }

        public int? LessonStart { get; set; }

        public int? LessonEnd { get; set; }

        public enum ObjectiveType
        {
            StudyGroup,
            Teacher
        }
    }
}
