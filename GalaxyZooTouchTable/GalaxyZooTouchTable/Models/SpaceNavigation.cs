namespace GalaxyZooTouchTable.Models
{
    public class SpaceNavigation
    {
        public const double PlateScale = 1.5;
        const int CutoutWidth = 1248;
        const int CutoutHeight = 432;
        const int ArcDegreeInSeconds = 3600;
        public double DecRange { get; set; } = CutoutHeight * PlateScale / ArcDegreeInSeconds;
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

        public double RaRange()
        {
            return (CutoutWidth * PlateScale / ArcDegreeInSeconds) / System.Math.Abs(System.Math.Cos((ToRadians(Center.Declination))));
        }

        private void UpdateBounds()
        {
            MinRa = Center.RightAscension - (RaRange() / 2);
            MaxRa = Center.RightAscension + (RaRange() / 2);
            MinDec = Center.Declination - (DecRange / 2);
            MaxDec = Center.Declination + (DecRange / 2);
        }

        private double ToRadians(double Degrees)
        {
            return (Degrees * System.Math.PI) / 180.0;
        }

        public void MoveNorth()
        {
            Center.Declination += DecRange;
            UpdateBounds();
        }

        public void MoveSouth()
        {
            Center.Declination -= DecRange;
            UpdateBounds();
        }

        public void MoveEast()
        {
            Center.RightAscension -= RaRange();
            UpdateBounds();
        }

        public void MoveWest()
        {
            Center.RightAscension += RaRange();
            UpdateBounds();
        }
    }
}
