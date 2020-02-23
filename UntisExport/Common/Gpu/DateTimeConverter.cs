namespace SchulIT.UntisExport.Common.Gpu
{
    internal class DateTimeConverter : DateConverterBase
    {
        private const string DateFormat = "yyyyMMddHHmm";

        protected override string GetDateFormatString() => DateFormat;
    }
}
