using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SchulIT.UntisExport
{
    public abstract class AbstractExporter
    {

        protected T ParseCell<T>(string[] cells, int? columnIndex, Func<string, T> parseFunc, T defaultValue)
        {
            if (columnIndex.HasValue == false)
            {
                return defaultValue;
            }

            var index = columnIndex.Value;

            if (index < 0 || index > cells.Length - 1)
            {
                return defaultValue;
            }

            var value = cells[index];
            return parseFunc(value);
        }

        protected int ParseIntegerColumn(string value)
        {
            return int.Parse(value.Trim());
        }

        protected int? GetColumnIndexOrNull(Dictionary<string, int> dictionary, string column)
        {
            if (column == null)
            {
                return null;
            }

            if (!dictionary.ContainsKey(column))
            {
                return null;
            }

            return dictionary[column];
        }


        protected List<string> SplitColumnValue(string column)
        {
            return column.Split(',').Select(x => x.Trim()).OrderBy(x => x).ToList();
        }

        protected string ClearString(string input)
        {
            if(input == null)
            {
                return null;
            }

            input = input.Replace("\r\n", "")
                    .Replace("\n", "")
                    .Replace("\r", "")
                    .Replace("&nbsp;", " ")
                    .Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            return input;
        }

        protected DateTime ParseDate(string dateString, string dateFormat)
        {
            var parts = dateString.Split(' ');
            var provider = CultureInfo.InvariantCulture;

            foreach (var part in parts)
            {
                DateTime dateTime;

                if (DateTime.TryParseExact(part, dateFormat, provider, DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }
            }

            throw new ParseException("Cannot parse date from HTML");
        }
    }
}
