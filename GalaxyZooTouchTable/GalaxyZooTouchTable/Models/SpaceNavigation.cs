namespace GalaxyZooTouchTable.Models
{
    public class SpaceNavigation
    {
        public const double PlateScale = 1.5;
        const int CutoutWidth = 1248;
        const int CutoutHeight = 432;
        const int ArcDegreeInSeconds = 3600;
        public double DecRange { get; set; } = CutoutHeight * PlateScale / ArcDegreeInSeconds;
        public double RaRange { get; set; }
        public double MinRa { get; set; }
        public double MaxRa { get; set; }
        public double MinDec { get; set; }
        public double MaxDec { get; set; }

        private SpacePoint _center;
        public SpacePoint Center
        {
            get => _center;
            set
            {
                _center = value;
                UpdateBounds();
            }
        }

        public SpaceNavigation(SpacePoint center)
        {
            Center = center;
            UpdateBounds();
        }

        public double RaRangeFunc()
        {
            RaRange = (CutoutWidth * PlateScale / ArcDegreeInSeconds) / System.Math.Abs(System.Math.Cos((ToRadians(Center.Declination))));
            return RaRange;
        }

        private void UpdateBounds()
        {
            MinRa = Center.RightAscension - (RaRangeFunc() / 2);
            MaxRa = Center.RightAscension + (RaRangeFunc() / 2);
            MinDec = Center.Declination - (DecRange / 2);
            MaxDec = Center.Declination + (DecRange / 2);
        }

        private double ToRadians(double Degrees)
        {
            return (Degrees * System.Math.PI) / 180.0;
        }

        public SpacePoint NextNorthernPoint()
        {
            return new SpacePoint(Center.RightAscension, Center.Declination + DecRange);
        }

        public SpacePoint NextSouthernPoint()
        {
            return new SpacePoint(Center.RightAscension, Center.Declination - DecRange);
        }

        public SpacePoint NextEasternPoint()
        {
            return new SpacePoint(Center.RightAscension - RaRangeFunc(), Center.Declination);
        }

        public SpacePoint NextWesternPoint()
        {
            return new SpacePoint(Center.RightAscension + RaRangeFunc(), Center.Declination);
        }
    }
}
