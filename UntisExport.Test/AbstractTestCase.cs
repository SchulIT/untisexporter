using System.IO;
using System.Reflection;
using System.Text;

namespace UntisExport.Test
{
    public abstract class AbstractTestCase
    {
        protected string LoadFile(string file, string encoding = "iso-8859-1")
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{GetType().Namespace}.{file}";
            var inputEncoding = Encoding.GetEncoding(encoding);

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
    }
}
