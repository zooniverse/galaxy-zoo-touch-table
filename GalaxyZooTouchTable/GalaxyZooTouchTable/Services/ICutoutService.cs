using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public interface ICutoutService
    {
        Task<SpaceCutout> GetSpaceCutout(SpaceNavigation location);
    }
}
