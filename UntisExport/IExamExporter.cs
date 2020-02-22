using System.Threading.Tasks;

namespace SchulIT.UntisExport
{
    public interface IExamExporter
    {
        Task<ExamExportResult> ParseHtmlAsync(ExamExportSettings settings, string html);
    }
}
