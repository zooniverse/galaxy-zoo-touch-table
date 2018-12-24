using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class StillThereViewModel : INotifyPropertyChanged
    {
        public ClassificationPanelViewModel Classifier { get; set; }
        public DispatcherTimer SecondTimer { get; set; }
        public DispatcherTimer ThirtySecondTimer { get; set; }
        public int Radius { get; set; } = 41;
        public int Percentage { get; set; } = 100;
        public int StrokeThickness { get; set; } = 4;

        private decimal _currentSeconds = 30;
        public decimal CurrentSeconds
        {
            get { return _currentSeconds; }
            set
            {
                _currentSeconds = value;
                OnPropertyRaised("CurrentSeconds");
            }
        }

        private bool _isLargeArc;
        public bool IsLargeArc
        {
            get { return _isLargeArc; }
            set
            {
                _isLargeArc = value;
                OnPropertyRaised("IsLargeArc");
            }
        }

        private bool _visible = false;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                if (value == true)
                {
                    StartTimers();
                } else
                {
                    StopTimers();
                }
                OnPropertyRaised("Visible");
            }
        }

        private Size _arcSize;
        public Size ArcSize
        {
            get { return _arcSize; }
            set
            {
                _arcSize = value;
                OnPropertyRaised("ArcSize");
            }
        }

        private Point _startPoint = new Point();
        public Point StartPoint
        {
            get { return _startPoint; }
            set
            {
                _startPoint = value;
                OnPropertyRaised("StartPoint");
            }
        }

        private Point _arcPoint;
        public Point ArcPoint
        {
            get { return _arcPoint; }
            set
            {
                _arcPoint = value;
                OnPropertyRaised("ArcPoint");
            }
        }

        private Thickness _margin;
        public Thickness Margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                OnPropertyRaised("Margin");
            }
        }

        private double _width;
        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyRaised("Width");
            }
        }

        private double _height;
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyRaised("Height");
            }
        }

        public StillThereViewModel(ClassificationPanelViewModel classifier)
        {
            Classifier = classifier;
            RenderArc();
        }

        private void StartTimers()
        {
            CurrentSeconds = 30;
            Percentage = 100;
            SecondTimer = new DispatcherTimer();
            SecondTimer.Tick += new System.EventHandler(OneSecondElapsed);
            SecondTimer.Interval = new System.TimeSpan(0, 0, 1);
            SecondTimer.Start();

            ThirtySecondTimer = new DispatcherTimer();
            ThirtySecondTimer.Tick += new System.EventHandler(ThirtySecondsElapsed);
            ThirtySecondTimer.Interval = new System.TimeSpan(0, 0, 31);
            ThirtySecondTimer.Start();
            RenderArc();
        }

        private void StopTimers()
        {
            SecondTimer.Stop();
            ThirtySecondTimer.Stop();
        }

        private void ThirtySecondsElapsed(object sender, EventArgs e)
        {
            Classifier.OnCloseClassifier();
            Visible = false;
        }

        private void OneSecondElapsed(object sender, System.EventArgs e)
        {
            CurrentSeconds--;
            decimal StartingSeconds = 30;
            decimal test = CurrentSeconds / StartingSeconds;
            decimal PercentOfSeconds = (CurrentSeconds / StartingSeconds) * 100;
            Percentage = Convert.ToInt16(Math.Floor(PercentOfSeconds));
            RenderArc();
        }

        private Point ComputeCartesianCoordinate(double angle, double radius)
        {
            double angleRad = (Math.PI / 180.0) * (angle - 90);
            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);
            return new Point(x, y);
        }

        public void RenderArc()
        {
            int Angle = -(Percentage * 360) / 100;
            StartPoint = new Point(Radius, 0);
            Point endPoint = ComputeCartesianCoordinate(Angle, Radius);
            endPoint.X += Radius;
            endPoint.Y += Radius;

            Width = Radius * 2 + StrokeThickness + 10;
            Height = Radius * 2 + StrokeThickness + 10;
            Margin = new Thickness(StrokeThickness, StrokeThickness, 0, 0);

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
