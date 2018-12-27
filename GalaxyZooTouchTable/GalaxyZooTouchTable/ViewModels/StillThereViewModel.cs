using GalaxyZooTouchTable.Utility;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class StillThereViewModel : ViewModelBase
    {
        public ClassificationPanelViewModel Classifier { get; set; }
        public DispatcherTimer SecondTimer { get; set; }
        public DispatcherTimer ThirtySecondTimer { get; set; }
        public int Radius { get; set; } = 41;
        public int Percentage { get; set; } = 100;
        public int StrokeThickness { get; set; } = 4;

        public ICommand CloseClassifier { get; set; }
        public ICommand CloseModal { get; set; }

        private decimal _currentSeconds = 30;
        public decimal CurrentSeconds
        {
            get => _currentSeconds;
            set
            {
                _currentSeconds = value;
                OnPropertyChanged();
            }
        }

        private bool _isLargeArc;
        public bool IsLargeArc
        {
            get => _isLargeArc;
            set
            {
                _isLargeArc = value;
                OnPropertyChanged();
            }
        }

        private bool _visible = false;
        public bool Visible
        {
            get => _visible;
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
                OnPropertyChanged();
            }
        }

        private Size _arcSize;
        public Size ArcSize
        {
            get => _arcSize;
            set
            {
                _arcSize = value;
                OnPropertyChanged();
            }
        }

        private Point _startPoint = new Point();
        public Point StartPoint
        {
            get => _startPoint; 
            set
            {
                _startPoint = value;
                OnPropertyChanged();
            }
        }

        private Point _arcPoint;
        public Point ArcPoint
        {
            get => _arcPoint;
            set
            {
                _arcPoint = value;
                OnPropertyChanged();
            }
        }

        private Thickness _margin;
        public Thickness Margin
        {
            get => _margin;
            set
            {
                _margin = value;
                OnPropertyChanged();
            }
        }

        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        private double _height;
        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        public StillThereViewModel(ClassificationPanelViewModel classifier)
        {
            Classifier = classifier;
            LoadCommands();
            RenderArc();
        }

        private void LoadCommands()
        {
            CloseClassifier = new CustomCommand(OnCloseClassifier);
            CloseModal = new CustomCommand(OnCloseModal);
        }

        private void OnCloseClassifier(object sender)
        {
            Classifier.OnCloseClassifier();
        }

        private void OnCloseModal(object sender)
        {
            Visible = false;
            Classifier.ResetTimer();
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
    }
}
