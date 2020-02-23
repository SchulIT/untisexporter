using FileHelpers;
using System;
using System.Globalization;

namespace SchulIT.UntisExport.Common.Gpu
{
    internal abstract class DateConverterBase : ConverterBase
    {
        public override object StringToField(string from)
        {
            var provider = CultureInfo.InvariantCulture;
            DateTime dateTime;

            if(DateTime.TryParseExact(from, GetDateFormatString(), provider, DateTimeStyles.None, out dateTime))
            {
                return dateTime;
            }

            throw new ParseException($"Cannot parse DateTime from '{from}'.");
        }

        protected abstract string GetDateFormatString();
    }
}
