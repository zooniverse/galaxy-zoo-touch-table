namespace GalaxyZooTouchTable.Models
{
    public class SpacePoint
    {
        public double RightAscension { get; set; }
        public double Declination { get; set; }

        public SpacePoint(double ra = 0, double dec = 0)
        {
            RightAscension = ra;
            Declination = dec;
        }
    }
}
