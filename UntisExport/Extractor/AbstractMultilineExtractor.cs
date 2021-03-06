using System.Collections.Generic;

namespace SchulIT.UntisExport.Extractor
{
    public abstract class AbstractMultilineExtractor<T> : AbstractMultilineExtractor<T,T>
        where T : class?, new()
    {

    }

    public abstract class AbstractMultilineExtractor<T, V>
        where V : class?, new()
    {
        protected abstract IEnumerable<T> BuildItems(V dto);

        protected abstract void ParseLine(string line, V dto);

        protected abstract bool IsStartSequence(string line, out V dto);

        protected abstract bool IsEndSequence(string line);

        public List<T> ParseContent(string[] content)
        {
            var result = new List<T>();
            V dto = null;
            var firstItemFound = false;
            var lastItemFound = false;

            foreach(var line in content)
            {
                var startSequenceData = IsStartSequence(line, out var newDto);
                var nonStartSequenceData = IsEndSequence(line);

                if (startSequenceData && firstItemFound == false)
                {
                    dto = newDto;
                    firstItemFound = true;
                    continue;
                }

                if(firstItemFound && nonStartSequenceData)
                {
                    lastItemFound = true;
                }

                if (startSequenceData || nonStartSequenceData)
                {
                    if (dto != null)
                    {
                        // Builder!
                        result.AddRange(BuildItems(dto));
                        dto = newDto;
                    }

                    if (startSequenceData)
                    {
                        continue;
                    }
                    else if (firstItemFound) // only break if we found the first item section
                    {
                        break;
                    }
                }

                ParseLine(line, dto);
            }

            if(lastItemFound == false && dto != null)
            {
                result.AddRange(BuildItems(dto));
            }

            return result;
        }
    }
}
