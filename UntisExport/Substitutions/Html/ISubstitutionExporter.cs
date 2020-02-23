using System.Threading.Tasks;

namespace SchulIT.UntisExport.Substitutions.Html
{
    public interface ISubstitutionExporter
    {
        Task<SubstitutionExportResult> ParseHtmlAsync(SubstitutionExportSettings settings, string html);
    }
}
