using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace GalaxyZooTouchTable.Behaviors
{
    public class UIElementDragBehavior : Behavior<UIElement>
    {
        private bool isTouchDown = false;
        List<DependencyObject> hitResultsList = new List<DependencyObject>();
        DragCanvas DragOverlay { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TouchDown += AssociatedObject_TouchDown;
            AssociatedObject.TouchLeave += AssociatedObject_TouchLeave;
            AssociatedObject.TouchUp += AssociatedObject_TouchUp;
        }

        public static FrameworkElement FindAncestor(Type ancestorType, Visual visual)
        {
            while (visual != null && !ancestorType.IsInstanceOfType(visual))
            {
                visual = (Visual)VisualTreeHelper.GetParent(visual);
            }
            return visual as FrameworkElement;
        }

        private void AssociatedObject_TouchUp(object sender, TouchEventArgs e)
        {
            isTouchDown = false;
        }

        private void AssociatedObject_TouchDown(object sender, TouchEventArgs e)
        {
            isTouchDown = true;
        }

        private void AssociatedObject_TouchLeave(object sender, TouchEventArgs e)
        {
            if (DragOverlay == null)
            {
                var visual = e.OriginalSource as Visual;
                DragOverlay = (DragCanvas)UIElementDragBehavior.FindAncestor(typeof(DragCanvas), visual);
            }

            if (isTouchDown)
            {
                TouchPoint touchPosition = e.GetTouchPoint(DragOverlay);
                Point initialPoint = new Point(touchPosition.Position.X, touchPosition.Position.Y);
                FrameworkElement adornedElement = sender as FrameworkElement;

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
                GalaxyAdorner adorner = new GalaxyAdorner(adornedElement, initialPoint);
                adornerLayer.Add(adorner);

                EventHandler<TouchEventArgs> moveHandler = new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_TouchMove(s, e, adorner));
                EventHandler<TouchEventArgs> upHandler = new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_TouchUp(s, evt, adorner, adornedElement));
                EventHandler<TouchEventArgs> unsubscribeHandler = new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_Unsubscribe(s, e, moveHandler, upHandler, adorner));
                adorner.UnsubscribeEvent = unsubscribeHandler;

                DragOverlay.TouchMove += moveHandler;
                DragOverlay.TouchUp += upHandler;
                DragOverlay.TouchUp += unsubscribeHandler;
            }
            isTouchDown = false;
        }

        private void DragDropContainer_Unsubscribe(object s, TouchEventArgs e, EventHandler<TouchEventArgs> moveHandler, EventHandler<TouchEventArgs> upHandler, GalaxyAdorner adorner)
        {
            if (adorner.Parent == null)
            {
                DragOverlay.TouchMove -= moveHandler;
                DragOverlay.TouchUp -= upHandler;
                DragOverlay.TouchUp -= adorner.UnsubscribeEvent;
            }
        }

        private void DragDropContainer_TouchMove(object sender, TouchEventArgs e, GalaxyAdorner adorner)
        {
            var touchPosition = e.GetTouchPoint(DragOverlay);
            var adornerLocation = new Point(touchPosition.Position.X, touchPosition.Position.Y);
            adorner.UpdatePosition(adornerLocation);
        }

        private void DragDropContainer_TouchUp(object sender, TouchEventArgs e, GalaxyAdorner adorner, UIElement element)
        {
            var touchPosition = e.GetTouchPoint(DragOverlay);
            Point touchPoint = new Point(touchPosition.Position.X, touchPosition.Position.Y);

            if (touchPoint.X >= adorner.Location.X && touchPoint.X <= adorner.Location.X + adorner.ActualWidth
                && touchPoint.Y >= adorner.Location.Y && touchPoint.Y <= adorner.Location.Y + adorner.ActualHeight)
            {
                AdornerLayer.GetAdornerLayer(sender as Visual).Remove(adorner);

                VisualTreeHelper.HitTest(DragOverlay, null,
                    new HitTestResultCallback(MyHitTestResult),
                    new PointHitTestParameters(touchPoint));
                
                foreach (UIElement item in hitResultsList)
                {
                    if (item is IDroppableArea)
                    {
                        IDroppableArea area = item as IDroppableArea;
                        area.Drop(adorner);
                    }
                }
                hitResultsList.Clear();
            }
        }

        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            hitResultsList.Add(result.VisualHit);
            return HitTestResultBehavior.Continue;
        }

        public class GalaxyAdorner : Adorner
        {
            public Point Location;
            public Point Offset;
            public EventHandler<TouchEventArgs> UnsubscribeEvent { get; set; }

            public GalaxyAdorner(FrameworkElement adornedElement, Point offset)
              : base(adornedElement)
            {
                Offset = offset;
                DataContext = adornedElement.DataContext;
                IsHitTestVisible = false;
            }

            public void UpdatePosition(Point location)
            {
                Location = location;
                this.InvalidateVisual();
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                var adornerLocation = Location;
                adornerLocation.Offset(-Offset.X, -Offset.Y);

                Rect adornedElementRect = new Rect(adornerLocation, AdornedElement.DesiredSize);

                var visualBrush = new VisualBrush(AdornedElement);
                Pen border = new Pen(new SolidColorBrush(Color.FromRgb(229,255,77)), 1.5);

                drawingContext.DrawRoundedRectangle(
                    visualBrush, border, adornedElementRect,
                    this.AdornedElement.DesiredSize.Width / 2,
                    this.AdornedElement.DesiredSize.Height / 2);
            }
        }
    }
}
