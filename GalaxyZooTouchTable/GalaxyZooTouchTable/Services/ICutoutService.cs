namespace GalaxyZooTouchTable.Services
{
    public interface ICutoutService
    {
        bool GetSpaceCutout(double ra, double dec, double plateScale);
    }
}
