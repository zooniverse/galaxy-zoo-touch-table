using PanoptesNetClient.Models;
using System.Collections.ObjectModel;

namespace GalaxyZooTouchTable.Models
{
    public class TableSubject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double RightAscension { get; set; }
        public double Declination { get; set; }
        public string SubjectLocation { get; set; }
        private readonly double PlateScale = 1.5;
        private readonly double Offset = 0.03;
        public Subject Subject { get; set; }
        public ObservableCollection<GalaxyRing> GalaxyRings { get; set; } = new ObservableCollection<GalaxyRing>();
        public string Location { get; set; }
        public string Id { get; set; }

        public TableSubject(Subject subject, double TableRA = 0, double TableDEC = 0)
        {
            if (subject.Metadata != null)
            {
                RightAscension = System.Convert.ToDouble(subject.Metadata.ra);
                Declination = System.Convert.ToDouble(subject.Metadata.dec);
            }
            Subject = subject;
            SubjectLocation = subject.GetSubjectLocation();
            XYConvert(TableRA, TableDEC);

            GalaxyRings.Add(new GalaxyRing());
        }

        public TableSubject(string id, string location, double ra, double dec)
        {
            Id = id;
            Location = location;
            RightAscension = ra;
            Declination = dec;
            SubjectLocation = location;
            XYConvert();
        }

        private void XYConvert()
        {
            int CutoutWidth = 1248; 
            int CutoutHeight = 432;
            const int ArcDegreeInSeconds = 3600;

            double DecRange = CutoutHeight * PlateScale / ArcDegreeInSeconds;
            double RaRange = System.Math.Abs(CutoutWidth * PlateScale / ArcDegreeInSeconds / System.Math.Cos(SpaceNavigation.DEC));

            double StartY = ((SpaceNavigation.DEC - Declination) / DecRange * CutoutHeight) + (CutoutHeight / 2);
            double StartX = ((SpaceNavigation.RA - RightAscension) / (RaRange + Offset) * CutoutWidth) + (CutoutWidth / 2);

            Y = System.Convert.ToInt32(StartY);
            X = System.Convert.ToInt32(StartX);
        }

        public void RemoveRing(TableUser user)
        {
            bool RingRemoved = false;
            foreach (GalaxyRing Ring in GalaxyRings)
            {
                if (Ring.UserName == user.Name)
                {
                    GalaxyRings.Remove(Ring);
                    RingRemoved = true;
                    break;
                }
            }

            if (RingRemoved)
            {
                int index = 0;
                foreach (GalaxyRing Ring in GalaxyRings)
                {
                    Ring.SetProperties(index);
                    index++;
                }
            }
        }

        public void DimRing(TableUser userClassifying)
        {
            foreach (GalaxyRing Ring in GalaxyRings)
            {
                if (Ring.UserName == userClassifying.Name)
                {
                    Ring.CurrentlyClassifying = false;
                }
            }
        }

        public void AddRing(TableUser user)
        {
            int NewIndex = GalaxyRings.Count;
            GalaxyRing NewRing = new GalaxyRing(NewIndex, user);
            GalaxyRings.Add(NewRing);
        }

        private double ToRadians(double Degrees)
        {
            return (Degrees * System.Math.PI) / 180.0;
        }
    }
}
