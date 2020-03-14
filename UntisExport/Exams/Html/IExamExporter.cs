using System.Threading.Tasks;

namespace SchulIT.UntisExport.Exams.Html
{
    public interface IExamExporter
    {
        Task<ExamExportResult> ParseHtmlAsync(string html, ExamExportSettings settings);
    }
}
