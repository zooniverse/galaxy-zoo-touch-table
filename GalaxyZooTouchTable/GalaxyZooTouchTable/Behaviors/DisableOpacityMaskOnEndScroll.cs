using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace GalaxyZooTouchTable.Behaviors
{
    public class DisableOpacityMaskOnEndScroll : Behavior<ScrollViewer>
    {
        private LinearGradientBrush InitialOpacityMask { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.ScrollChanged += AssociatedObject_ScrollChanged;
            SetOpacityMask();
        }

        private void SetOpacityMask()
        {
            var gradientCollection = new GradientStopCollection();
            var gradientStop1 = new GradientStop(Colors.Black, 0);
            var gradientStop2 = new GradientStop(Colors.Transparent, 1);
            gradientCollection.Add(gradientStop1);
            gradientCollection.Add(gradientStop2);
            

            Point StartPoint = new Point();
            Point EndPoint = new Point();
            if (IsHorizontal)
            {
                StartPoint.X = StartPercent;
                EndPoint.X = 1;
            }
            else
            {
                StartPoint.Y = StartPercent;
                EndPoint.Y = 1;
            }

            InitialOpacityMask = new LinearGradientBrush(gradientCollection, StartPoint, EndPoint);
            AssociatedObject.OpacityMask = InitialOpacityMask;
        }

        private static readonly DependencyProperty IsHorizontalProperty =
            DependencyProperty.Register("IsHorizontal", typeof(Boolean), typeof(DisableOpacityMaskOnEndScroll));

        public bool IsHorizontal
        {
            get { return (bool)GetValue(IsHorizontalProperty); }
            set { SetValue(IsHorizontalProperty, value); }
        }

        private static readonly DependencyProperty StartPercentProperty =
            DependencyProperty.Register("StartPercent", typeof(double), typeof(DisableOpacityMaskOnEndScroll));

        public double StartPercent
        {
            get { return (double)GetValue(StartPercentProperty); }
            set { SetValue(StartPercentProperty, value); }
        }

        private void AssociatedObject_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (AssociatedObject.ScrollableHeight == AssociatedObject.VerticalOffset && !IsHorizontal)
            {
                AssociatedObject.OpacityMask = null;
            }
            else if (AssociatedObject.ScrollableWidth == AssociatedObject.HorizontalOffset && IsHorizontal)
            {
                AssociatedObject.OpacityMask = null;
            }
            else
            {
                AssociatedObject.OpacityMask = InitialOpacityMask;
            }
        }
    }
}
