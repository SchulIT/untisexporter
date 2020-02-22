using System.Threading.Tasks;

namespace SchulIT.UntisExport
{
    public interface ISubstitutionExporter
    {
        Task<SubstitutionExportResult> ParseHtmlAsync(SubstitutionExportSettings settings, string html);
    }
}
