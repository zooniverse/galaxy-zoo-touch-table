using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace GalaxyZooTouchTable.Behaviors
{
    public class UIElementDropBehavior : Behavior<UIElement>
    {
        private List<DependencyObject> hitResultsList = new List<DependencyObject>();

        public DragCanvas DraggingOverlay
        {
            get { return (DragCanvas)GetValue(DragOverlayProperty); }
            set { SetValue(DragOverlayProperty, value); }
        }

        public static readonly DependencyProperty DragOverlayProperty = DependencyProperty.Register(
            "DraggingOverlay", typeof(DragCanvas), typeof(UIElementDropBehavior));

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TouchEnter += AssociatedObject_TouchEnter;
            AssociatedObject.TouchLeave += AssociatedObject_TouchLeave;
        }

        private void AssociatedObject_TouchLeave(object sender, TouchEventArgs e)
        {
            Border element = sender as Border;
            element.BorderBrush = Brushes.White;
        }

        private void AssociatedObject_TouchEnter(object sender, TouchEventArgs e)
        {
            if (DraggingOverlay == null)
            {
                var visual = e.OriginalSource as Visual;
                DraggingOverlay = (DragCanvas)UIElementDragBehavior.FindAncestor(typeof(DragCanvas), visual);
            }

            //var touchPosition = e.GetTouchPoint(sender as FrameworkElement);
            //var point = new Point(touchPosition.Position.X, touchPosition.Position.Y);

            //AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(DraggingOverlay);
            

            //UIElement elements = sender as UIElement;
            //Point relativePoint = elements.TransformToAncestor(DraggingOverlay)
            //                  .Transform(new Point(0, 0));
            //AdornerHitTestResult result = myAdornerLayer.AdornerHitTest(relativePoint);

            //var test = myAdornerLayer.GetAdorners(DraggingOverlay);

            //Point testing = DraggingOverlay.PointFromScreen(relativePoint);

            Border element = sender as Border;
            element.BorderBrush = Brushes.Red;

            //VisualTreeHelper.HitTest(myAdornerLayer, null,
            //    new HitTestResultCallback(MyHitTestResult),
            //    new PointHitTestParameters(point));
        }

        public static FrameworkElement FindAncestor(Type ancestorType, Visual visual)
        {
            while (visual != null && !ancestorType.IsInstanceOfType(visual))
            {
                visual = (Visual)VisualTreeHelper.GetParent(visual);
            }
            return visual as FrameworkElement;
        }

        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            hitResultsList.Add(result.VisualHit);

            return HitTestResultBehavior.Continue;
        }
    }
}
