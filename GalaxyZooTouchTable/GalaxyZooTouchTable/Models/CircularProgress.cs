using System;
using System.ComponentModel;
using System.Windows;

namespace GalaxyZooTouchTable.Models
{
    public class CircularProgress : INotifyPropertyChanged
    {
        private readonly int Radius;

        private bool _isLargeArc;
        public bool IsLargeArc
        {
            get => _isLargeArc;
            set
            {
                _isLargeArc = value;
                OnPropertyRaised("LargeArc");
            }
        }

        private Size _arcSize;
        public Size ArcSize
        {
            get => _arcSize;
            set
            {
                _arcSize = value;
                OnPropertyRaised("ArcSize");
            }
        }

        private Point _startPoint = new Point();
        public Point StartPoint
        {
            get => _startPoint;
            set
            {
                _startPoint = value;
                OnPropertyRaised("StartPoint");
            }
        }

        private Point _arcPoint;
        public Point ArcPoint
        {
            get => _arcPoint;
            set
            {
                _arcPoint = value;
                OnPropertyRaised("ArcPoint");
            }
        }

        public CircularProgress(int radius)
        {
            Radius = radius;
        }

        private Point ComputeCartesianCoordinate(double angle, double radius)
        {
            double angleRad = (Math.PI / 180.0) * (angle - 90);
            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);
            return new Point(x, y);
        }

        public void RenderArc(int Percentage)
        {
            int Angle = -(Percentage * 360) / 100;
            StartPoint = new Point(Radius, 0);
            Point endPoint = ComputeCartesianCoordinate(Angle, Radius);
            endPoint.X += Radius;
            endPoint.Y += Radius;

            bool largeArc = -Angle > 180.0;

            Size outerArcSize = new Size(Radius, Radius);

            if (StartPoint.X == Math.Round(endPoint.X) && StartPoint.Y == Math.Round(endPoint.Y))
                endPoint.X += 0.01;

            ArcPoint = endPoint;
            ArcSize = outerArcSize;
            IsLargeArc = largeArc;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
