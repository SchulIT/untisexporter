using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public sealed class Parsers
    {
        public static Parser<string> AnyLine =
            from any in Parse.AnyChar.Except(Parse.LineEnd).Many().Token().Text()
            from newline in Parse.LineEnd
            select any;


        public static Parser<string> QuotedText =
            (

                from open in Parse.Char('"').Optional()
                from content in Parse.CharExcept('"').Many().Text()
                from close in Parse.Char('"').Optional()
                select content
            ).Token();


        public static Parser<int> Integer =
            from number in Parse.Number
            from sep in Parse.Char('-')
            select int.Parse(number);

        public static Parser<int> FixedLengthInteger(int min, int max)
        {
            return from digits in Parse.Digit.Repeat(min, max)
                   from space in Parse.WhiteSpace.Repeat(max - digits.Count())
                   select int.Parse(string.Join("", digits));
        }

        public static Parser<int?> OptionalFixedLengthInteger(int min, int max)
        {
            return from digits in Parse.Digit.Repeat(min, max)
                   from space in Parse.WhiteSpace.Repeat(max - digits.Count())
                   select FromDigits(digits);
        }

        public static Parser<DateTime> DateTime =
            from year in Parse.Digit.Repeat(4).Text()
            from month in Parse.Digit.Repeat(2).Text()
            from day in Parse.Digit.Repeat(2).Text()
            select new DateTime(
                int.Parse(year), 
                int.Parse(month), 
                int.Parse(day)
            );

        public static Parser<int> HexaDecimal =
            from character in Parse.Chars('A', 'B', 'C', 'D', 'E', 'F').Or(Parse.Digit)
            select int.Parse(character.ToString(), System.Globalization.NumberStyles.HexNumber);

        private static int? FromDigits(IEnumerable<char> digits)
        {
            if(digits.Any())
            {
                return int.Parse(string.Join("", digits));
            }

            return null;
        }
    }
}
