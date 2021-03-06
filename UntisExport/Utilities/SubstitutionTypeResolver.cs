using SchulIT.UntisExport.Model;
using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace SchulIT.UntisExport.Utilities
{
    public class SubstitutionTypeResolver
    {
        private readonly IEnumerable<string> floors;

        public static readonly Parser<SubstitutionType> Betreuung =
            from any in Parse.CharExcept('B').Many()
            from E in Parse.Char('B')
            from rest in Parse.AnyChar.Many()
            select SubstitutionType.Betreuung;

        public static readonly Parser<SubstitutionType> Entfall =
            from any in Parse.CharExcept('E').Many()
            from E in Parse.Char('E')
            from rest in Parse.AnyChar.Many()
            select SubstitutionType.Entfall;

        public static readonly Parser<SubstitutionType> Freisetzung =
            from any in Parse.CharExcept('F').Many()
            from E in Parse.Char('F')
            from rest in Parse.AnyChar.Many()
            select SubstitutionType.Entfall;

        public static readonly Parser<SubstitutionType> Sondereinsatz =
            from any in Parse.CharExcept('S').Many()
            from E in Parse.Char('S')
            from rest in Parse.AnyChar.Many()
            select SubstitutionType.Sondereinsatz;

        public static readonly Parser<SubstitutionType> Raumvertretung =
            from any in Parse.CharExcept('r').Many()
            from r in Parse.Char('r')
            from rest in Parse.AnyChar.Many()
            select SubstitutionType.Raumvertretung;

        public static readonly IEnumerable<Parser<SubstitutionType>> Rules = new List<Parser<SubstitutionType>>()
        {
            Raumvertretung,
            Betreuung,
            Entfall,
            Freisetzung
        };

        public SubstitutionTypeResolver(IEnumerable<string> floors)
        {
            this.floors = floors;
        }

        public SubstitutionType ResolveType(Substitution substitution)
        {
            // Step 1: Test rules above
            foreach(var rule in Rules)
            {
                var parsed = rule.TryParse(substitution.RawType);
                if(parsed.WasSuccessful)
                {
                    return parsed.Value;
                }
            }

            if(ContainsAll(floors, substitution.Rooms) || ContainsAll(floors, substitution.ReplacementRooms))
            {
                return SubstitutionType.Pausenaufsicht;
            }

            if (string.IsNullOrEmpty(substitution.ReplacementTeacher))
            {
                return SubstitutionType.VertretungOhneLehrer;
            }

            var teacherChanged = NotNullAndChanged(substitution.Teacher, substitution.ReplacementTeacher);
            var subjectChanged = NotNullAndChanged(substitution.Subject, substitution.Subject);
            var roomChanged = NotNullAndCollectionChanged(substitution.Rooms, substitution.ReplacementRooms);
            var numChanged = (new[] { teacherChanged, subjectChanged, roomChanged }).Select(x => x ? 1 : 0).Sum();

            if(numChanged > 1)
            {
                return SubstitutionType.Vertretung;
            }

            if(substitution.Teacher == substitution.ReplacementTeacher && subjectChanged)
            {
                return SubstitutionType.UnterrichtGeaendert;
            }

            if(roomChanged)
            {
                return SubstitutionType.Raumvertretung;
            }

            if(Raumvertretung.TryParse(substitution.RawType).WasSuccessful)
            {
                return SubstitutionType.Raumvertretung;
            }

            return SubstitutionType.Vertretung;
        }

        private static bool NotNullAndChanged(string objectiveA, string objectiveB)
        {
            if (!string.IsNullOrEmpty(objectiveA) && !string.IsNullOrEmpty(objectiveB) && objectiveA != objectiveB)
            {
                return true;
            }

            return false;
        }

        private static bool NotNullAndCollectionChanged(IEnumerable<string> objectiveA, IEnumerable<string> objectiveB)
        {
            if(objectiveA == null || objectiveB == null)
            {
                return false;
            }

            if(objectiveA.Count() != objectiveB.Count() || objectiveA.Intersect(objectiveB).Count() != objectiveA.Count())
            {
                return true;
            }

            return false;
        }

        private static bool ContainsAll(IEnumerable<string> items, IEnumerable<string> values)
        {
            foreach(var value in values)
            {
                if(!items.Contains(value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
