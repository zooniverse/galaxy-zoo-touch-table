using PanoptesNetClient.Models;

namespace GalaxyZooTouchTable.Models
{
    public class TableSubject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double RA { get; set; }
        public double DEC { get; set; }
        public string SubjectLocation { get; set; }
        private readonly double PlateScale = 1.5;
        private readonly double Offset = 0.03;
        public Subject Subject { get; set; }

        public TableSubject(Subject subject, double TableRA, double TableDEC)
        {
            RA = System.Convert.ToDouble(subject.Metadata.ra);
            DEC = System.Convert.ToDouble(subject.Metadata.dec);
            Subject = subject;
            SubjectLocation = subject.GetSubjectLocation();
            XYConvert(TableRA, TableDEC);
        }

        private void XYConvert(double TableRA, double TableDEC)
        {
            int ScreenWidth = 1248; 
            int ScreenHeight = 432; 

            double DecRange = ScreenHeight * PlateScale / 3600;
            double RaRange = System.Math.Abs(ScreenWidth * PlateScale / 3600 / System.Math.Cos(TableDEC));

            double StartY = ((TableDEC - DEC) / DecRange * ScreenHeight) + (ScreenHeight / 2);
            double StartX = ((TableRA - RA) / (RaRange + Offset) * ScreenWidth) + (ScreenWidth / 2);

            Y = System.Convert.ToInt32(StartY);
            X = System.Convert.ToInt32(StartX);
        }

        private double ToRadians(double Degrees)
        {
            return (Degrees * System.Math.PI) / 180.0;
        }
    }
}
