namespace SchulIT.UntisExport.Substitutions.Gpu
{
    public interface ISubstitutionTypeConverter
    {
        /// <summary>
        /// Converts the type bit field and subtitution type to a human readable type.
        /// For details see (German): https://platform.untis.at/HTML/WebHelp/de/untis/hid_export_vertretung.htm
        /// </summary>
        /// <param name="type"></param>
        /// <param name="substitutionType"></param>
        /// <returns></returns>
        string ConvertType(int type, string substitutionType);
    }
}
