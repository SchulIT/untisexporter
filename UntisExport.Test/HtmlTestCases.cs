using System.IO;
using System.Reflection;

namespace UntisExport.Test
{
    public static class HtmlTestCases
    {
        private static string LoadFile(string file)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"UntisExport.Test.{file}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static string GetNormalHtmlText()
        {
            return LoadFile("test.htm");
        }

        public static string GetHtmlWithEmptyData()
        {
            return LoadFile("test_empty.htm");
        }

        public static string GetHtmlWithInvalidDate()
        {
            return LoadFile("test_invaliddate.htm");
        }
    }
}
