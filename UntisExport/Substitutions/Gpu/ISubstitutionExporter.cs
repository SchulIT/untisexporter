using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Substitutions.Gpu
{
    public interface ISubstitutionExporter
    {
        Task<IEnumerable<Substitution>> ParseGpuAsync(string gpu, SubstitutionExportSettings settings);
    }
}
