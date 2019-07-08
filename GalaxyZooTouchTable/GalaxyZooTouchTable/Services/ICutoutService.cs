using GalaxyZooTouchTable.Models;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Services
{
    public interface ICutoutService
    {
        Task<BitmapImage> GetSpaceCutout(SpaceNavigation location);
    }
}
