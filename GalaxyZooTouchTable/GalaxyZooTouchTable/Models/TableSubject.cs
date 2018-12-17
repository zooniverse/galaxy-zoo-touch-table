using PanoptesNetClient.Models;
using System;

namespace GalaxyZooTouchTable.Models
{
    public class TableSubject
    {
        public string X { get; set; }
        public string Y { get; set; }
        public double RA { get; set; }
        public double DEC { get; set; }
        public string SubjectLocation { get; set; }
        private double PlateScale { get; set; } = 0.4;
        public Subject Subject { get; set; }

        public TableSubject(Subject subject, double tableRA, double tableDec)
        {
            RA = Convert.ToDouble(subject.Metadata.ra);
            DEC = Convert.ToDouble(subject.Metadata.dec);
            var test = (RA - tableRA) * (PlateScale / Math.Cos(DEC));
            Subject = subject;
            SubjectLocation = subject.GetSubjectLocation();
        }
    }
}
