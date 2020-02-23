using FileHelpers;
using System.Linq;

namespace SchulIT.UntisExport.Common.Gpu
{
    internal class SeparatedValuesConverter : ConverterBase
    {
        private readonly char delimiter;
        private readonly bool trim;

        public SeparatedValuesConverter() : this('~') { }

        public SeparatedValuesConverter(char delimiter)
        {
            this.delimiter = delimiter;
            this.trim = false;
        }

        public SeparatedValuesConverter(char delimiter, bool trim) :
            this(delimiter)
        {
            this.trim = true;
        }

        public override object StringToField(string from)
        {
            if (string.IsNullOrEmpty(from))
            {
                return null;
            }

            var entries = from.Split(delimiter).AsEnumerable();

            if (trim)
            {
                entries = entries.Select(x => x.Trim());
            }

            return entries.ToList();
        }
    }
}
