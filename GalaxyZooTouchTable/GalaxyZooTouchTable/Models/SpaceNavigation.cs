namespace GalaxyZooTouchTable.Models
{
    public static class SpaceNavigation
    {
        public static double RA { get; set; }
        public static double DEC { get; set; }
        public static double PlateScale { get; set; } = 1.5;
    }

    public class SpacePoint
    {
        public double RightAscension { get; set; }
        public double Declination { get; set; }

        public SpacePoint(double ra, double dec)
        {
            RightAscension = ra;
            Declination = dec;
        }
    }
}
