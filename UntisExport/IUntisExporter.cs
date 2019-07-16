using System.Threading.Tasks;

namespace SchulIT.UntisExport
{
    public interface IUntisExporter
    {
        Task<ExportResult> ParseHtmlAsync(ExportSettings settings, string html);
    }
}
