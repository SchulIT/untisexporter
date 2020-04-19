using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Timetable.Html
{
    public class TimetableExporter : ITimetableExporter
    {

        /// <summary>
        /// Index of the element which contains the objective (grade or subject)
        /// </summary>
        private const string ObjectiveSelector = @"/html/body/center/font[3]";
        private const string PeriodSelector = @"/html/body/center/font[4]";

        private const string TableSelector = "//table[@rules='all']";
        private const string LessonIndicatorCellSelector = "./tr[position()>1]/td[1]"; // First tr is the header

        private enum CellInformationType
        {
            Weeks,
            Subject,
            Teacher,
            Room
        }

        private readonly Dictionary<TimetableType, List<CellInformationType>> CellInformation = new Dictionary<TimetableType, List<CellInformationType>>
        {
            { TimetableType.Grade, new List<CellInformationType>{ CellInformationType.Weeks, CellInformationType.Subject, CellInformationType.Teacher, CellInformationType.Room } },
            { TimetableType.Subject, new List<CellInformationType> {CellInformationType.Weeks, CellInformationType.Teacher } }
        };

        public Task<TimetableExportResult> ParseHtmlAsync(string html, TimetableExportSettings settings)
        {
            return Task.Run(() =>
            {
                // Step 1: Parse HTML
                var document = new HtmlDocument();
                document.LoadHtml(html);

                // Step 2: Parse objective and period
                var objective = GetObjective(document);
                var period = GetPeriod(document);

                // Step 2: Parse lessons
                var lessons = GetLessons(document, settings);

                if(settings.Type == TimetableType.Grade)
                {
                    // Set grade
                    foreach(var lesson in lessons)
                    {
                        lesson.Grade = objective;
                    }
                }

                return new TimetableExportResult(objective, period, lessons.AsReadOnly());
            });
        }

        private string GetPeriod(HtmlDocument document)
        {
            var node = document.DocumentNode.SelectSingleNode(PeriodSelector);

            if (node == null)
            {
                return null;
            }

            return HtmlEntity.DeEntitize(node.InnerText).Trim();
        }

        private string GetObjective(HtmlDocument document)
        {
            var node = document.DocumentNode.SelectSingleNode(ObjectiveSelector);

            if (node == null)
            {
                return null;
            }

            return HtmlEntity.DeEntitize(node.InnerText).Trim();
        }

        private List<Lesson> GetLessons(HtmlDocument document, TimetableExportSettings settings)
        {
            var lessons = new List<Lesson>();

            var tableNode = document.DocumentNode.SelectSingleNode(TableSelector);

            if(tableNode == null)
            {
                // TODO: Raise error!
                return lessons;
            }

            var numberOfLessons = GetNumberOfLessons(tableNode);
            var lessonStarts = ComputeLessonStarts(tableNode, numberOfLessons);

            // Navigate through the timetable
            var trNodes = tableNode.SelectNodes("./tr");
            var currentLesson = 0;

            foreach(var trNode in trNodes.Skip(1)) // First row contains headers
            {
                var tdNodes = trNode.SelectNodes("./td");

                if (tdNodes == null || tdNodes.Count == 0)
                {
                    // Every second row is empty/has no child
                    continue;
                }

                var currentDay = 0;

                foreach(var tdNode in tdNodes.Skip(1)) // First node is the lesson column
                {
                    // Advance day if necessary
                    if (MustAdvanceDay(lessonStarts, currentLesson, currentDay, out int advanceValue))
                    {
                        currentDay += advanceValue;
                    }

                    var lessonStart = currentLesson;
                    var lessonEnd = currentLesson + lessonStarts[currentDay].Where(x => x == lessonStart).Count() - 1;

                    lessons.AddRange(GetLessonsFromCell(tdNode, currentDay, lessonStart, lessonEnd, settings));

                    currentDay++;
                }

                currentLesson++;
            }

            return lessons;
        }

        private List<Lesson> GetLessonsFromCell(HtmlNode tdNode, int day, int lessonStart, int lessonEnd, TimetableExportSettings settings)
        {
            var lessons = new List<Lesson>();
            var lessonNodes = tdNode.SelectNodes("./table/tr");

            if(lessonNodes == null || lessonNodes.Count == 0)
            {
                return lessons;
            }

            foreach(var lessonNode in lessonNodes)
            {
                var lessonTdNodes = lessonNode.SelectNodes("./td");

                if(lessonTdNodes == null || lessonTdNodes.Count < 2)
                {
                    continue;
                }

                var lesson = new Lesson
                {
                    LessonStart = lessonStart + settings.FirstLesson, // As C# is zero-based, all lessons starts at lesson 0 internally -> shift if necessay
                    LessonEnd = lessonEnd + settings.FirstLesson,
                    Day = day + 1, // As C# is zero-based, Mondays = 0, shift it to 1, ...
                };

                for(int nodeIdx = 0; nodeIdx < lessonTdNodes.Count; nodeIdx++)
                {
                    var value = HtmlEntity.DeEntitize(lessonTdNodes[nodeIdx].InnerText).Trim();
                    var targetProperty = CellInformation[settings.Type][settings.UseWeeks ? nodeIdx : nodeIdx+1];

                    switch(targetProperty)
                    {
                        case CellInformationType.Room:
                            lesson.Room = value;
                            break;

                        case CellInformationType.Subject:
                            lesson.Subject = value;
                            break;

                        case CellInformationType.Teacher:
                            lesson.Teacher = value;
                            break;

                        case CellInformationType.Weeks:
                            lesson.Weeks = value.Split(',');
                            break;
                    }
                }

                lessons.Add(lesson);
            }

            return lessons;
        }

        private bool MustAdvanceDay(int[][] lessonStarts, int lesson, int index, out int advanceValue)
        {
            if (index > lessonStarts.Length)
            {
                throw new ArgumentException($"index must be less than {lessonStarts.Length}");
            }

            if (lesson > lessonStarts[0].Length)
            {
                throw new ArgumentException($"lesson must be less than {lessonStarts[0].Length}");
            }

            advanceValue = 0;

            for (int currentDay = 0; currentDay < index; currentDay++)
            {
                if (lessonStarts[currentDay][lesson] != lesson)
                {
                    advanceValue++;
                }
            }

            return advanceValue > 0;
        }

        private int GetNumberOfLessons(HtmlNode tableNode)
        {
            var tdNodes = tableNode.SelectNodes(LessonIndicatorCellSelector);
            return tdNodes.Count;
        }

        private int[][] ComputeLessonStarts(HtmlNode tableNode, int numberOfLessons)
        {
            var trNodes = tableNode.SelectNodes("./tr");
            var lessonStarts = Enumerable
                .Range(1, 5) // Monday to Friday
                .Select(x => Enumerable.Range(0, numberOfLessons).ToArray()) // Create each lesson
                .ToArray();

            var lesson = 0;

            foreach (var trNode in trNodes.Skip(1)) // First row contains headers
            {
                var tdNodes = trNode.SelectNodes("./td");

                if (tdNodes == null || tdNodes.Count == 0)
                {
                    // Every second row is empty/has no child
                    continue;
                }

                var day = 0;

                foreach(var tdNode in tdNodes.Skip(1)) // First node is the lesson column
                {
                    // Check if the lesson has not started before
                    if (lessonStarts[day][lesson] == lesson)
                    {
                        if (tdNode.Attributes.Contains("rowspan"))
                        {
                            var rowSpan = int.Parse(tdNode.Attributes["rowspan"].Value);
                            var duration = rowSpan / 2; // Untis needs two rows per lesson, for whatever reason?!

                            for (int currentLesson = lesson + 1; currentLesson < lesson + duration; currentLesson++)
                            {
                                lessonStarts[day][currentLesson] = lesson;
                            }
                        }
                    }
                    
                    day++;
                }

                lesson++;
            }

            return lessonStarts;
        }
    }
}
