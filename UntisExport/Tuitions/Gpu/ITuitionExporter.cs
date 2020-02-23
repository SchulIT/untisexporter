using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Tuitions.Gpu
{
    public interface ITuitionExporter
    {
        Task<IEnumerable<Tuition>> ParseGpuAsync(string gpu, TuitionExportSettings settings);
    }
}
