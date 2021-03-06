using Sprache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SchulIT.UntisExport.Extractor
{
    public abstract class PeriodAwareExtractor<T, V>
    {
        protected abstract Parser<IEnumerable<int>> StartSequence { get; }

        protected abstract Parser<char> NonStartSequence { get; }

        public static Parser<IEnumerable<int>> PeriodSection =
            from identifier in Parse.String("PE")
            from space in Parse.WhiteSpace.Many()
            from comma in Parse.Char(',')
            from numbers in Parsers.Integer.Many()
            select numbers;

        protected abstract V BuildPeriod(int periodNumber);

        protected abstract void ParseLine(string line, int[] currentPeriods, Dictionary<int, V> periods);

        protected abstract T BuildItem(Dictionary<int, V> periods);

        public List<T> ParseContent(string[] content)
        {
            var result = new List<T>();

            int[] currentPeriods = Array.Empty<int>();
            var periods = new Dictionary<int, V>();
            var firstItemFound = false;
            foreach (var line in content)
            {

                var startSequenceData = StartSequence.TryParse(line);
                var nonStartSequenceData = NonStartSequence.TryParse(line);

                if (startSequenceData.WasSuccessful && firstItemFound == false)
                {
                    firstItemFound = true;
                    continue;
                }

                if (startSequenceData.WasSuccessful || nonStartSequenceData.WasSuccessful)
                {
                    if (periods.Count > 0)
                    {
                        // Builder!
                        result.Add(BuildItem(periods));
                        periods.Clear();
                        currentPeriods = Array.Empty<int>();
                    }

                    if (startSequenceData.WasSuccessful)
                    {
                        continue;
                    }
                    else if (firstItemFound) // only break if we found the first item section
                    {
                        break;
                    }
                }

                var periodSection = PeriodSection.TryParse(line);
                if (periodSection.WasSuccessful)
                {
                    currentPeriods = periodSection.Value.ToArray();
                    continue;
                }

                ParseLine(line, currentPeriods, periods);
            }

            return result;
        }

        protected void ApplyToPeriods(int[] currentPeriods, Dictionary<int, V> periods, Action<V> action)
        {
            foreach (var periodNumber in currentPeriods)
            {
                var period = GetPeriod(periods, periodNumber);
                action(period);
            }
        }

        protected V GetPeriod(Dictionary<int, V> periods, int periodNumber)
        {
            if (!periods.ContainsKey(periodNumber))
            {
                periods.Add(periodNumber, BuildPeriod(periodNumber));
            }

            return periods[periodNumber];
        }
    }
}
