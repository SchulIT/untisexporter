using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Supervisions.Gpu
{
    public interface ISupervisionExporter
    {
        Task<IEnumerable<Supervision>> ParseGpuAsync(string gpu, SupervisionExportSettings settings);
    }
}
