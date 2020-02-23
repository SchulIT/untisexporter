using System.Threading.Tasks;

namespace SchulIT.UntisExport.Exams.Html
{
    public interface IExamExporter
    {
        Task<ExamExportResult> ParseHtmlAsync(ExamExportSettings settings, string html);
    }
}
