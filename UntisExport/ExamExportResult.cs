using SchulIT.UntisExport.Model;
using System.Collections.Generic;

namespace SchulIT.UntisExport
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
