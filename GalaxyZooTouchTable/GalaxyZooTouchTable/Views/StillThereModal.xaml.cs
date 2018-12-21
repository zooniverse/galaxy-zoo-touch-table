using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.Views
{
    /// <summary>
    /// Interaction logic for StillThereModal.xaml
    /// </summary>
    public partial class StillThereModal : UserControl
    {
        public DispatcherTimer ThirtySecondTimer { get; set; } = new DispatcherTimer();
        public int Radius { get; set; } = 41;
        public int Percentage { get; set; } = 100;
        public int StrokeThickness { get; set; } = 4;
        public decimal CurrentSeconds { get; set; } = 30;

        public StillThereModal()
        {
            InitializeComponent();
            StartTimer();
            RenderArc();
        }

        private void StartTimer()
        {
            ThirtySecondTimer.Tick += new System.EventHandler(OneSecondElapsed);
            ThirtySecondTimer.Interval = new System.TimeSpan(0, 0, 1);
            ThirtySecondTimer.Start();
        }

        private void OneSecondElapsed(object sender, System.EventArgs e)
        {
            decimal StartingSeconds = 30;
            decimal test = CurrentSeconds / StartingSeconds;
            decimal PercentOfSeconds = (CurrentSeconds / StartingSeconds) * 100;
            Percentage = Convert.ToInt16(Math.Floor(PercentOfSeconds));
            RenderArc();
            CurrentSeconds--;
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
            Point startPoint = new Point(Radius, 0);
            Point endPoint = ComputeCartesianCoordinate(Angle, Radius);
            endPoint.X += Radius;
            endPoint.Y += Radius;

            pathRoot.Width = Radius * 2 + StrokeThickness + 10;
            pathRoot.Height = Radius * 2 + StrokeThickness + 10;
            pathRoot.Margin = new Thickness(StrokeThickness, StrokeThickness, 0, 0);

            bool largeArc = -Angle > 180.0;

            Size outerArcSize = new Size(Radius, Radius);

            pathFigure.StartPoint = startPoint;

            if (startPoint.X == Math.Round(endPoint.X) && startPoint.Y == Math.Round(endPoint.Y))
                endPoint.X += 0.01;

            arcSegment.Point = endPoint;
            arcSegment.Size = outerArcSize;
            arcSegment.IsLargeArc = largeArc;
        }
    }
}
