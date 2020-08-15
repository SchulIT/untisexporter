using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Substitutions.Html
{
    public class SubstitutionExporter : AbstractExporter, ISubstitutionExporter
    {
        private const string DateSelectorClass = "mon_title";
        private const string InfoTableSelectorClass = "info";
        private const string InfoEntrySelectorClass = "info";
        private const string SubstitutionsTableSelectorClass = "mon_list";
        private const string SubstitutionEntryClass = "list";

        public Task<SubstitutionExportResult> ParseHtmlAsync(string html, SubstitutionExportSettings settings)
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

                var freeLessons = GetFreeLessons(infoTexts, settings.FreeLessonSettings);

                // Step 5: Retrieve Substitutions
                var substitutions = GetSubstitutions(document, date, settings);

                return new SubstitutionExportResult(substitutions.AsReadOnly(), infoTexts.AsReadOnly(), absences.AsReadOnly(), freeLessons.AsReadOnly());
            });
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

        private List<FreeLessonsTimespan> GetFreeLessons(List<Infotext> infotexts, FreeLessonSettings settings)
        {
            if(settings.ParseFreeLessons == false)
            {
                return new List<FreeLessonsTimespan>();
            }

            var freeLessons = new List<FreeLessonsTimespan>();
            var removeIdx = new List<int>();

            for (int idx = 0; idx < infotexts.Count; idx++)
            {
                var infotext = infotexts[idx];

                if(infotext.Text.StartsWith(settings.FreeLessonIdentifier))
                {
                    var content = infotext.Text.Substring(settings.FreeLessonIdentifier.Length).Trim();

                    if (content.EndsWith(settings.LessonIdentifier))
                    {
                        content = content.Substring(0, content.Length - settings.LessonIdentifier.Length).Trim();

                    }

                    foreach(var part in content.Split(','))
                    {
                        try
                        {
                            var parts = part.Split('-').Select(x => int.Parse(x)).ToArray();
                            var timespan = new FreeLessonsTimespan
                            {
                                Date = infotext.Date
                            };

                            if (parts.Length == 1)
                            {
                                timespan.Start = timespan.End = parts[0];
                            }
                            else if (parts.Length == 2)
                            {
                                timespan.Start = parts[0];
                                timespan.End = parts[1];
                            }

                            freeLessons.Add(timespan);
                        }
                        catch (InvalidCastException e) { }
                    }

                    removeIdx.Add(idx);
                }
            }

            removeIdx.Sort();
            removeIdx.Reverse();

            foreach (int idx in removeIdx)
            {
                infotexts.RemoveAt(idx);
            }

            return freeLessons;
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
                    absences.AddRange(ParseAbsence(infotext.Text.Substring(absenceSettings.TeacherIdentifier.Length), Absence.ObjectiveType.Teacher, infotext.Date));
                    removeIdx.Add(idx);
                }
                else if (infotext.Text.StartsWith(absenceSettings.StudyGroupIdentifier))
                {
                    absences.AddRange(ParseAbsence(infotext.Text.Substring(absenceSettings.StudyGroupIdentifier.Length), Absence.ObjectiveType.StudyGroup, infotext.Date));
                    removeIdx.Add(idx);
                } else if(infotext.Text.StartsWith(absenceSettings.RoomIdentifier))
                {
                    absences.AddRange(ParseAbsence(infotext.Text.Substring(absenceSettings.RoomIdentifier.Length), Absence.ObjectiveType.Room, infotext.Date));
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

        private List<Absence> ParseAbsence(string absenceText, Absence.ObjectiveType type, DateTime date)
        {
            var absences = new List<Absence>();

            foreach(var part in absenceText.Split(','))
            {
                var absence = new Absence
                {
                    Date = date,
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

        private List<Substitution> GetSubstitutions(HtmlDocument document, DateTime dateTime, SubstitutionExportSettings settings)
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
                substitution.Grades = ParseCell(cells, order.GradesColumn, x => ParseMultiValueStringColumn(x, settings), default);
                substitution.ReplacementGrades = ParseCell(cells, order.ReplacementGradesColumn, x => ParseMultiValueStringColumn(x, settings), default);

                substitutions.Add(substitution);
            }

            return substitutions;
        }

        private string[] ParseMultiValueStringColumn(string value, SubstitutionExportSettings settings)
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

        private string ParseStringColumn(string value, SubstitutionExportSettings settings)
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

        private ColumnOrder GetColumnOrder(HtmlNode headerNode, SubstitutionColumnSettings settings)
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

        

        private string FixHtml(string htmlInput)
        {
            // First idea: simply remove all opening <p> tags as they are never closed.
            return htmlInput.Replace("<p>", "");
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
