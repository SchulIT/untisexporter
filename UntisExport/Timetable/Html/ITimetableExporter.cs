using System.Threading.Tasks;

namespace SchulIT.UntisExport.Timetable.Html
{
    public interface ITimetableExporter
    {
        Task<TimetableExportResult> ParseHtmlAsync(string html, TimetableExportSettings settings);
    }
}
