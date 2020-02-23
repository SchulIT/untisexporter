using System.Collections.Generic;

namespace SchulIT.UntisExport.Exams
{
    public class ExamExportResult
    {
        public IReadOnlyList<Exam> Exams { get; private set; }

        public ExamExportResult(IReadOnlyList<Exam> exams)
        {
            Exams = exams;
        }
    }
}
