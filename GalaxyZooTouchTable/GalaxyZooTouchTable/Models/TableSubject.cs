using PanoptesNetClient.Models;
using System;

namespace GalaxyZooTouchTable.Models
{
    public class TableSubject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double RA { get; set; }
        public double DEC { get; set; }
        public string SubjectLocation { get; set; }
        private double PlateScale { get; set; } = 1.5;
        public Subject Subject { get; set; }

        public TableSubject(Subject subject, double TableRA, double TableDEC)
        {
            RA = Convert.ToDouble(subject.Metadata.ra);
            DEC = Convert.ToDouble(subject.Metadata.dec);
            Subject = subject;
            SubjectLocation = subject.GetSubjectLocation();
            XYConvert(TableRA, TableDEC);
        }

        private void XYConvert(double TableRA, double TableDEC)
        {
            int ScreenWidth = 1248; 
            int ScreenHeight = 432; 

            double DecRange = ScreenHeight * PlateScale / 3600;
            double RaRange = Math.Abs(ScreenWidth * PlateScale / 3600 / Math.Cos(TableDEC));

            double StartY = ((TableDEC - DEC) / DecRange * ScreenHeight) + (ScreenHeight / 2);
            double StartX = ((TableRA - RA) / RaRange * ScreenWidth) + (ScreenWidth / 2);

            Y = Convert.ToInt32(StartY);
            X = Convert.ToInt32(StartX);
        }

        private double ToRadians(double Degrees)
        {
            return (Degrees * Math.PI) / 180.0;
        }
    }
}
