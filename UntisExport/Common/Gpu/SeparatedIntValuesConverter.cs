using System.Collections.Generic;
using System.Linq;

namespace SchulIT.UntisExport.Common.Gpu
{
    internal class SeparatedIntValuesConverter : SeparatedValuesConverter
    {
        public override object StringToField(string from)
        {
            var stringList = base.StringToField(from) as IEnumerable<string>;
            return stringList?.Select(x => int.Parse(x)).ToList();
        }
    }
}
