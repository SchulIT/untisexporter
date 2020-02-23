namespace SchulIT.UntisExport.Common.Gpu
{
    internal class DateConverter : DateConverterBase
    {
        private const string DateFormat = "yyyyMMdd";

        protected override string GetDateFormatString() => DateFormat;
    }
}
