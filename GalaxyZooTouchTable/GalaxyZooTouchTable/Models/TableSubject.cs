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
        private double PlateScale { get; set; } = 0.4;
        public Subject Subject { get; set; }

        public TableSubject(Subject subject, double TableRA, double TableDEC)
        {
            RA = Convert.ToDouble(subject.Metadata.ra);
            DEC = Convert.ToDouble(subject.Metadata.dec);
            var test = (RA - TableRA) * (PlateScale / Math.Cos(DEC));
            Subject = subject;
            SubjectLocation = subject.GetSubjectLocation();
            EQConvert(TableRA, TableDEC);
        }

        private void EQConvert(double TableRA, double TableDEC)
        {
            double PI = 3.1415926535897932384626433832795;
            double TwoPI = 6.283185307179586476925286766559;
            double PI2 = 1.5707963267948966192313216916398;
            double Rads = 0.01745329251994329576923690768489;

            double Az, Alt, tmpval, nz, az2, tx, ty, xyscale;

            double DisplayFOV = 1.4;
            double DisplayROT = 0.0;

            int ScreenWidth = 1248;
            int ScreenHeight = 432;

            double RadRA = RA * Rads;
            double RadDEC = DEC * Rads;
            double FieldRA = TableRA * Rads;
            double FieldDEC = TableDEC * Rads;
            double FieldFOV = DisplayFOV * Rads;
            double FieldROT = DisplayROT * Rads;

            Az = Math.Atan2(Math.Sin(FieldRA - RadRA), Math.Cos(FieldRA - RadRA) * Math.Sin(FieldDEC) - Math.Tan(RadDEC) * Math.Cos(FieldDEC));
            tmpval = Math.Sin(FieldDEC) * Math.Sin(RadDEC) + Math.Cos(FieldDEC) * Math.Cos(RadDEC) * Math.Cos(FieldRA - RadRA);

            if (tmpval >= 1.0)
            {
                Alt = PI2;
            } else
            {
                Alt = Math.Asin(tmpval);
            }

            nz = 1.0 - 2.0 * Alt / PI;
            az2 = Az - PI2 + FieldROT;
            tx = (nz * Math.Cos(az2)) * PI / FieldFOV;
            ty = -(nz * Math.Sin(az2)) * PI / FieldFOV;

            xyscale = ((double)ScreenWidth / FieldFOV) / (120.0 / DisplayFOV);

            X = (int)(((double)ScreenWidth / 2.0) + (tx * xyscale));
            Y = (int)(((double)ScreenHeight / 2.0) + (ty * xyscale));
        }

        private double ToRadians(double Degrees)
        {
            return (Degrees * Math.PI) / 180.0;
        }
    }
}
