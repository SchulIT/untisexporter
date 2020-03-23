using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Substitutions.Gpu
{
    public interface ISubstitutionExporter
    {
        Task<SubstitutionExportResult> ParseGpuAsync(string gpu, SubstitutionExportSettings settings);
    }
}
