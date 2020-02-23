using System.Collections.Generic;

namespace SchulIT.UntisExport.Substitutions.Gpu
{
    public class DefaultGermanSubstitutionTypeConverter : DefaultSubstitutionTypeConverterBase
    {
        private readonly Dictionary<string, string> Types = new Dictionary<string, string>
        {
            { "T", "verlegt" },
            { "F", "verlegt von" },
            { "W", "Tausch" },
            { "S", "Betreuung" },
            { "A", "Sondereinsatz" },
            { "C", "Entfall" },
            { "L", "Freisetzung" },
            { "P", "Teil-Vertretung" },
            { "B", "Pausenaufsichtsvertretung" },
            { "~", "Lehrertausch" },
            { "E", "Klausur" }
        };

        protected override string GetDefaultType() => "Vertretung";

        protected override Dictionary<string, string> GetTypeDictionary() => Types;
    }
}