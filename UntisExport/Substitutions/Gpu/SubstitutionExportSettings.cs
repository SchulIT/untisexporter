namespace SchulIT.UntisExport.Substitutions.Gpu
{
    public class SubstitutionExportSettings
    {
        /// <summary>
        /// Type converter for converting the type and substitution type into a human
        /// readable string. Defaults to the <see cref="DefaultGermanSubstitutionTypeConverter"/> which
        /// provides a basic converter for German users.
        /// 
        /// <see cref="ISubstitutionTypeConverter" />, <see cref="DefaultSubstitutionTypeConverterBase" /> and
        /// <see cref="DefaultGermanSubstitutionTypeConverter"/>.
        /// </summary>
        public ISubstitutionTypeConverter TypeConverter { get; set; } = new DefaultGermanSubstitutionTypeConverter();
    }
}
