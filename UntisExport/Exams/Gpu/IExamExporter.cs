using SchulIT.UntisExport.Tuitions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Exams.Gpu
{
    public interface IExamExporter
    {
        Task<IEnumerable<Exam>> ParseGpuAsync(string gpu, ExamExportSettings settings, IEnumerable<Tuition> tuitions = null);
    }
}
