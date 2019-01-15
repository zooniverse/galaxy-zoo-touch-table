using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace GalaxyZooTouchTable.Behaviors
{
    public class UIElementDropBehavior : Behavior<UIElement>
    {
        private List<DependencyObject> hitResultsList = new List<DependencyObject>();

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TouchUp += AssociatedObject_TouchUp;
            AssociatedObject.TouchEnter += AssociatedObject_TouchEnter;
            AssociatedObject.TouchLeave += AssociatedObject_TouchLeave;
        }

        private void AssociatedObject_TouchUp(object sender, TouchEventArgs e)
        {
            Console.WriteLine("TOUCH Up");
        }

        private void AssociatedObject_TouchLeave(object sender, TouchEventArgs e)
        {
            Border element = sender as Border;
            element.BorderBrush = Brushes.White;
        }

        private void AssociatedObject_TouchEnter(object sender, TouchEventArgs e)
        {
            var touchPosition = e.GetTouchPoint(sender as FrameworkElement);
            var point = new Point(touchPosition.Position.X, touchPosition.Position.Y);

            VisualTreeHelper.HitTest(sender as FrameworkElement, null,
                new HitTestResultCallback(MyHitTestResult),
                new PointHitTestParameters(point));

            Console.WriteLine(hitResultsList);

            Border element = sender as Border;
            element.BorderBrush = Brushes.Red;
        }

        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            hitResultsList.Add(result.VisualHit);

            return HitTestResultBehavior.Continue;
        }
    }
}
