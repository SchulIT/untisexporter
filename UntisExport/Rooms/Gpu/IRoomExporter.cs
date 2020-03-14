using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchulIT.UntisExport.Rooms.Gpu
{
    public interface IRoomExporter
    {
        Task<IEnumerable<Room>> ParseGpuAsync(RoomExportSettings settings, string gpu);
    }
}
