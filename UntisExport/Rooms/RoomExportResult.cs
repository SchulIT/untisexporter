using System.Collections.Generic;

namespace SchulIT.UntisExport.Rooms
{
    public class RoomExportResult
    {
        public IReadOnlyList<Room> Rooms { get; private set; }

        public RoomExportResult(IReadOnlyList<Room> rooms)
        {
            Rooms = rooms;
        }
    }
}
