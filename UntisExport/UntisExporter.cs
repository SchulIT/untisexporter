using HtmlAgilityPack;
using SchulIT.UntisExport.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SchulIT.UntisExport
{
    public class UntisExporter : IUntisExporter
    {
        private const string DateSelectorClass = "mon_title";
        private const string InfoTableSelectorClass = "info";
        private const string InfoEntrySelectorClass = "info";
        private const string SubstitutionsTableSelectorClass = "mon_list";
        private const string SubstitutionEntryClass = "list";

        public Task<ExportResult> ParseHtmlAsync(ExportSettings settings, string html)
        {
            return Task.Run(() =>
            {
                // Step 1: Fix broken <p> tags if wanted
                if (settings.FixBrokenPTags)
                {
                    html = FixHtml(html);
                }

                // Step 2: Parse HTML
                var document = new HtmlDocument();
                document.LoadHtml(html);

                var dateNode = document.DocumentNode.SelectSingleNode($"//div[@class='{DateSelectorClass}']");
                var date = ParseDate(dateNode.InnerText, settings.DateTimeFormat);

                // Step 3: Retrieve InfoTexts
                var infoTexts = GetInfotexts(document, date);
                
                // Step 4: Parse absences
                var absences = GetAbsences(infoTexts, settings.AbsenceSettings);

                // Step 5: Retrieve Substitutions
                var substitutions = GetSubstitutions(document, date, settings);

                return new ExportResult(date, substitutions.AsReadOnly(), infoTexts.AsReadOnly(), absences.AsReadOnly());
            });
        }

        private DateTime ParseDate(string dateString, string dateFormat)
        {
            var parts = dateString.Split(' ');
            var provider = CultureInfo.InvariantCulture;

            foreach(var part in parts)
            {
                DateTime dateTime;

                if(DateTime.TryParseExact(part, dateFormat, provider, DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }
            }

            throw new ParseException("Cannot parse date from HTML");
        }

        private List<Infotext> GetInfotexts(HtmlDocument document, DateTime dateTime)
        {
            var infoTexts = new List<Infotext>();

            var node = document.DocumentNode.SelectSingleNode($"//table[@class='{InfoTableSelectorClass}']");

            if(node == null)
            {
                return infoTexts;
            }

            var infoNodes = node.SelectNodes($"./tr[@class='{InfoEntrySelectorClass}']");

            for(var i = 1; i < infoNodes.Count; i++) // The first item is the table header (which is not specified as thead, of course...)
            {
                var text = ClearString(infoNodes[i].InnerText);
                    
                var infoText = new Infotext
                {
                    Date = dateTime,
                    Text = text
                };

                infoTexts.Add(infoText);
            }

            return infoTexts;
        }

        private List<Absence> GetAbsences(List<Infotext> infotexts, AbsenceSettings absenceSettings)
        {
            if (absenceSettings.ParseAbsences == false)
            {
                return new List<Absence>();
            }

            var absences = new List<Absence>();
            var removeIdx = new List<int>();

            for (int idx = 0; idx < infotexts.Count; idx++)
            {
                var infotext = infotexts[idx];

                if (infotext.Text.StartsWith(absenceSettings.TeacherIdentifier))
                {
                    absences.AddRange(ParseAbsence(infotext.Text.Substring(absenceSettings.TeacherIdentifier.Length), Absence.ObjectiveType.Teacher));
                    removeIdx.Add(idx);
                }
                else if (infotext.Text.StartsWith(absenceSettings.StudyGroupIdentifier))
                {
                    absences.AddRange(ParseAbsence(infotext.Text.Substring(absenceSettings.StudyGroupIdentifier.Length), Absence.ObjectiveType.StudyGroup));
                    removeIdx.Add(idx);
                }
            }

            removeIdx.Sort();
            removeIdx.Reverse();

            foreach(int idx in removeIdx)
            {
                infotexts.RemoveAt(idx);
            }

            return absences;
        }

        private List<Absence> ParseAbsence(string absenceText, Absence.ObjectiveType type)
        {
            var absences = new List<Absence>();

            foreach(var part in absenceText.Split(','))
            {
                var absence = new Absence
                {
                    Type = type
                };

                var posLeftBracket = part.IndexOf('(');
                var posRightBracked = part.IndexOf(')');

                if (posLeftBracket >= 0 && posRightBracked >= 0)
                {
                    var bracket = part.Substring(posLeftBracket + 1, posRightBracked - posLeftBracket - 1).Trim();
                    var lessons = bracket.Split('-').Select(x => int.Parse(x.Trim())).ToArray();

                    absence.Objective = part.Substring(0, posLeftBracket).Trim();

                    if (lessons.Length > 0)
                    {
                        absence.LessonStart = lessons[0];
                    }

                    if (lessons.Length > 1)
                    {
                        absence.LessonEnd = lessons[1];
                    }
                }
                else
                {
                    absence.Objective = part.Trim();
                }

                absences.Add(absence);
            }

            return absences;
        }

        private List<Substitution> GetSubstitutions(HtmlDocument document, DateTime dateTime, ExportSettings settings)
        {
            var substitutions = new List<Substitution>();

            var tableNode = document.DocumentNode.SelectSingleNode($"//table[@class='{SubstitutionsTableSelectorClass}']");

            if(tableNode == null)
            {
                return substitutions;
            }

            var substitutionNodes = tableNode.SelectNodes($"./tr[starts-with(@class, '{SubstitutionEntryClass}')]");

            if(substitutionNodes.Count < 2) // We either have an empty list or just column headers
            {
                return substitutions;
            }

            // Step 1: determine column order
            var order = GetColumnOrder(substitutionNodes.First(), settings.ColumnSettings);
            
            // Step 2: Parse substitution
            for(var i = 1; i < substitutionNodes.Count; i++) 
            {
                var substitutionNode = substitutionNodes[i];
                var cells = substitutionNode.SelectNodes("./td").Select(x => x.InnerText).ToArray();

                var substitution = new Substitution();
                substitution.Id = ParseCell(cells, order.IdColumn, x => ParseIntegerColumn(x), default);
                substitution.Date = dateTime;

                var isSupervision = false;
                var lessons = ParseCell(cells, order.LessonColumn, x => ParseLessonColumn(x, out isSupervision), default);

                substitution.LessonStart = lessons.Item1;
                substitution.LessonEnd = lessons.Item2;
                substitution.IsSupervision = isSupervision;

                substitution.Type = ParseCell(cells, order.TypeColumn, x => ParseStringColumn(x, settings), default);
                substitution.Subject = ParseCell(cells, order.SubjectColumn, x => ParseStringColumn(x, settings), default);
                substitution.ReplacementSubject = ParseCell(cells, order.ReplacementSubjectColumn, x => ParseStringColumn(x, settings), default);
                substitution.Room = ParseCell(cells, order.RoomColumn, x => ParseStringColumn(x, settings), default);
                substitution.ReplacementRoom = ParseCell(cells, order.ReplacementRoomColumn, x => ParseStringColumn(x, settings), default);
                substitution.Remark = ParseCell(cells, order.RemarkColumn, x => ParseStringColumn(x, settings), default);

                substitution.Teachers = ParseCell(cells, order.TeachersColumn, x => ParseMultiValueStringColumn(x, settings), default);
                substitution.ReplacementTeachers = ParseCell(cells, order.ReplacementTeachersColumn, x => ParseMultiValueStringColumn(x, settings), default);
                substitution.Grade = ParseCell(cells, order.GradesColumn, x => ParseMultiValueStringColumn(x, settings), default);
                substitution.ReplacementGrades = ParseCell(cells, order.ReplacementGradesColumn, x => ParseMultiValueStringColumn(x, settings), default);

                substitutions.Add(substitution);
            }

            return substitutions;
        }

        private T ParseCell<T>(string[] cells, int? columnIndex, Func<string, T> parseFunc, T defaultValue)
        {
            if(columnIndex.HasValue == false)
            {
                return defaultValue;
            }

            var index = columnIndex.Value;

            if(index < 0 || index > cells.Length - 1)
            {
                return defaultValue;
            }

            var value = cells[index];
            return parseFunc(value);
        }

        private string[] ParseMultiValueStringColumn(string value, ExportSettings settings)
        {
            value = value.Trim();

            if(value.StartsWith("(") && value.EndsWith(")"))
            {
                if (settings.IncludeAbsentValues)
                {
                    value = value.Substring(1, value.Length - 2);
                }
                else
                {
                    return new string[0];
                }
            }

            return value.Split(',')
                .Select(x => x.Trim())
                .Where(x => !settings.EmptyValues.Contains(x))
                .Select(x => ClearString(x))
                .Where(x => x != null)
                .OrderBy(x => x).ToArray();
        }

        private string ParseStringColumn(string value, ExportSettings settings)
        {
            value = value.Trim();

            if (value.StartsWith("(") && value.EndsWith(")"))
            {
                if (settings.IncludeAbsentValues)
                {
                    value = value.Substring(1, value.Length - 2);
                }
                else
                {
                    return null;
                }
            }

            if (settings.EmptyValues.Contains(value))
            {
                return null;
            }

            return ClearString(value);
        }

        private int ParseIntegerColumn(string value)
        {
            return int.Parse(value.Trim());
        }

        private Tuple<int,int> ParseLessonColumn(string value, out bool isSupervision)
        {
            value = value.Trim();

            isSupervision = false;

            // Case 1: single lesson
            int lesson;
            if(int.TryParse(value, out lesson))
            {
                return new Tuple<int, int>(lesson, lesson);
            }

            // Case 2: compound lessons (X-Y)
            if(value.Split('-').Count() == 2)
            {
                var lessons = value.Split('-').Select(x => int.Parse(x.Trim())).ToArray();
                return new Tuple<int, int>(lessons[0], lessons[1]);
            }

            // Case 3: supervisions (X/Y)
            if(value.Split('/').Count() == 2)
            {
                var lessons = value.Split('/').Select(x => int.Parse(x.Trim())).ToArray();
                isSupervision = true;
                return new Tuple<int, int>(lessons[0], lessons[1]);
            }

            throw new ParseException($"Cannot parse lesson: '{value}'");
        }

        private ColumnOrder GetColumnOrder(HtmlNode headerNode, ColumnSettings settings)
        {
            var columns = headerNode.SelectNodes("./th");
            var texts = columns.Select(x => x.InnerText).Select((x, i) => new { Key = i, Value = x }).ToDictionary(x => x.Value, x => x.Key);

            return new ColumnOrder
            {
                IdColumn = GetColumnIndexOrNull(texts, settings.IdColumn),
                DateColumn = GetColumnIndexOrNull(texts, settings.DateColumn),
                LessonColumn = GetColumnIndexOrNull(texts, settings.LessonColumn),
                GradesColumn = GetColumnIndexOrNull(texts, settings.GradesColumn),
                ReplacementGradesColumn = GetColumnIndexOrNull(texts, settings.ReplacementGradesColumn),
                TeachersColumn = GetColumnIndexOrNull(texts, settings.TeachersColumn),
                ReplacementTeachersColumn = GetColumnIndexOrNull(texts, settings.ReplacementTeachersColumn),
                SubjectColumn = GetColumnIndexOrNull(texts, settings.SubjectColumn),
                ReplacementSubjectColumn = GetColumnIndexOrNull(texts, settings.ReplacementSubjectColumn),
                RoomColumn = GetColumnIndexOrNull(texts, settings.RoomColumn),
                ReplacementRoomColumn = GetColumnIndexOrNull(texts, settings.ReplacementRoomColumn),
                TypeColumn = GetColumnIndexOrNull(texts, settings.TypeColumn),
                RemarkColumn = GetColumnIndexOrNull(texts, settings.RemarkColumn)
            };
        }

        private int? GetColumnIndexOrNull(Dictionary<string, int> dictionary, string column)
        {
            if(column == null)
            {
                return null;
            }

            if(!dictionary.ContainsKey(column))
            {
                return null;
            }

            return dictionary[column];
        }


        private List<string> SplitColumnValue(string column)
        {
            return column.Split(',').Select(x => x.Trim()).OrderBy(x => x).ToList();
        }

        private string FixHtml(string htmlInput)
        {
            // First idea: simply remove all opening <p> tags as they are never closed.
            return htmlInput.Replace("<p>", "");
        }

        private string ClearString(string input)
        {
            input = input.Replace("\r\n", "")
                    .Replace("\n", "")
                    .Replace("\r", "")
                    .Replace("&nbsp;", " ")
                    .Trim();

            if(string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            return input;
        }

        private class ColumnOrder
        {
            public int? IdColumn { get; set; }
            public int? DateColumn { get; set; }
            public int? LessonColumn { get; set; }
            public int? GradesColumn { get; set; }
            public int? ReplacementGradesColumn { get; set; }
            public int? TeachersColumn { get; set; }
            public int? ReplacementTeachersColumn { get; set; }
            public int? SubjectColumn { get; set; }
            public int? ReplacementSubjectColumn { get; set; }
            public int? RoomColumn { get; set; }
            public int? ReplacementRoomColumn { get; set; }
            public int? TypeColumn { get; set; }
            public int? RemarkColumn { get; set; }
        }
    }
}
