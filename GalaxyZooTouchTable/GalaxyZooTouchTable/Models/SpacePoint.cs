namespace GalaxyZooTouchTable.Models
{
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
