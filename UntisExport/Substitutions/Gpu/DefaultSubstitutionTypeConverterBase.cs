using System.Collections.Generic;

namespace SchulIT.UntisExport.Substitutions.Gpu
{
    /// <summary>
    /// This is the base class for language-dependent type converters that only concider
    /// the substitution type. The converter checks whether the subtitution type is avaialable
    /// in the dictionary from the <see cref="GetTypeDictionary"/> method. If so, it returns the
    /// specified value. Otherwise it returns the value from the <see cref="GetDefaultType"/> method.
    /// 
    /// Also see the <see cref="DefaultGermanSubstitutionTypeConverter"/>.
    /// </summary>
    public abstract class DefaultSubstitutionTypeConverterBase : ISubstitutionTypeConverter
    {
        protected abstract Dictionary<string, string> GetTypeDictionary();

        protected abstract string GetDefaultType();

        public string ConvertType(int type, string substitutionType)
        {
            var types = GetTypeDictionary();

            if (!string.IsNullOrWhiteSpace(substitutionType) && types.ContainsKey(substitutionType))
            {
                return types[substitutionType];
            }

            return GetDefaultType();
        }
    }
}
