using System.Collections.Generic;

namespace SchulIT.UntisExport.Model
{
    public class Room
    {
        public List<RoomPeriod> Periods { get; } = new List<RoomPeriod>();
    }
}
