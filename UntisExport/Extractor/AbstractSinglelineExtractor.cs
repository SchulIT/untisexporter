using System.Collections.Generic;

namespace SchulIT.UntisExport.Extractor
{
    public abstract class AbstractSinglelineExtractor<T>
    {
        protected abstract T ParseLine(string line);

        public List<T> ParseContent(string[] content)
        {
            var result = new List<T>();
            var firstItemParsed = false;

            foreach(var line in content)
            { 

                var parsed = ParseLine(line);

                if (parsed != null)
                {
                    firstItemParsed = true;
                    result.Add(parsed);
                }
                else if (firstItemParsed)   // optimization: do not parse any more lines after first failure
                {
                    break;
                }
            }

            return result;
        }
    }
}
