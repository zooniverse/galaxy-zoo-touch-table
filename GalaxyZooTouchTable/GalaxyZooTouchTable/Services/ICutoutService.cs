using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public interface ICutoutService
    {
        Task<string> GetSpaceCutout(double ra, double dec, double plateScale);
    }
}
