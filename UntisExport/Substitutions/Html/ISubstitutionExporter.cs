using System.Threading.Tasks;

namespace SchulIT.UntisExport.Substitutions.Html
{
    public interface ISubstitutionExporter
    {
        Task<SubstitutionExportResult> ParseHtmlAsync(string html, SubstitutionExportSettings settings);
    }
}
