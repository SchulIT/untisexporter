using System.IO;
using System.Reflection;
using System.Text;

namespace UntisExport.Test
{
    public static class HtmlTestCases
    {
        private static string LoadFile(string file)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"UntisExport.Test.{file}";
            var inputEncoding = Encoding.GetEncoding("iso-8859-1");

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, inputEncoding))
                {
                    var html = reader.ReadToEnd();
                    var bytes = inputEncoding.GetBytes(html);
                    var utf8bytes = Encoding.Convert(inputEncoding, Encoding.UTF8, bytes);

                    return Encoding.UTF8.GetString(utf8bytes);
                }
            }
        }

        public static string GetNormalHtmlText()
        {
            return LoadFile("test.htm");
        }

        public static string GetNormalHtmlTextWithAbsences()
        {
            return LoadFile("test_absence.htm");
        }

        public static string GetHtmlWithEmptyData()
        {
            return LoadFile("test_empty.htm");
        }

        public static string GetHtmlWithInvalidDate()
        {
            return LoadFile("test_invaliddate.htm");
        }

        public static string GetExamsHtml()
        {
            return LoadFile("text_exams.htm");
        }
    }
}
