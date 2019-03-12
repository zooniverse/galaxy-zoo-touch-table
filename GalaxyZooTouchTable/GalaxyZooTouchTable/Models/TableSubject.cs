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
        private readonly double WidenedPlateScale = 1.8;
        public Subject Subject { get; set; }
        public ObservableCollection<GalaxyRing> GalaxyRings { get; set; } = new ObservableCollection<GalaxyRing>();
        public string Location { get; set; }
        public string Id { get; set; }

        public TableSubject(string id, string location, double ra, double dec)
        {
            GalaxyRings.Add(new GalaxyRing());

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

            double DecRange = CutoutHeight * WidenedPlateScale / ArcDegreeInSeconds;
            double RaRange = (CutoutWidth * WidenedPlateScale / ArcDegreeInSeconds) / System.Math.Abs(System.Math.Cos((ToRadians(SpaceNavigation.DEC))));

            double StartY = ((SpaceNavigation.DEC - Declination) / DecRange * CutoutHeight) + (CutoutHeight / 2);
            double StartX = System.Math.Abs(((SpaceNavigation.RA - RightAscension) / RaRange * CutoutWidth) + (CutoutWidth / 2));

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
