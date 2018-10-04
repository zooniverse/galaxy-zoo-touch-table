using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GalaxyZooTouchTable.Lib
{
    public static class TranslateTransformExtensions
    {
        public static void AnimateTo(this TranslateTransform translateTransform,
            Point point)
        {
            StartAnimation(translateTransform, TranslateTransform.XProperty, point.X);
            StartAnimation(translateTransform, TranslateTransform.YProperty, point.Y);
        }

        private static void StartAnimation(TranslateTransform translateTransform,
            DependencyProperty dependencyProperty,
            double toValue)
        {
            var animation = new DoubleAnimation
            {
                To = toValue,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                EasingFunction = new QuadraticEase()
            };
            translateTransform.BeginAnimation(dependencyProperty, animation);
        }
    }
}
