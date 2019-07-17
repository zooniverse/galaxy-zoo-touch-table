using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
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
            if (isTouchDown && DragOverlay != null)
            {
                TouchPoint touchPosition = e.GetTouchPoint(DragOverlay);
                if (touchPosition != null)
                {
                    Point initialPoint = new Point(touchPosition.Position.X, touchPosition.Position.Y);
                    FrameworkElement adornedElement = sender as FrameworkElement;
                    if (initialPoint != null && adornedElement != null)
                    {
                        ConstructGhostAdornerWithHandlers(initialPoint, adornedElement, e);
                        using (TableSubject subject = adornedElement.DataContext as TableSubject)
                            GlobalData.GetInstance().Logger?.AddEntry(entry: "Drag_Galaxy", subjectId: subject?.Id);
                    }
                }
            }
            isTouchDown = false;
        }

        private void ConstructGhostAdornerWithHandlers(Point initialPoint, FrameworkElement adornedElement, TouchEventArgs e)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            GalaxyAdorner adorner = new GalaxyAdorner(adornedElement, initialPoint);
            if (adornerLayer != null && adorner != null)
                adornerLayer.Add(adorner);
            else return;

            EventHandler<TouchEventArgs> moveHandler = new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_TouchMove(s, e, adorner));
            EventHandler<TouchEventArgs> upHandler = new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_TouchUp(s, evt, adorner, adornedElement));
            EventHandler<TouchEventArgs> unsubscribeHandler = new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_Unsubscribe(s, e, moveHandler, upHandler, adorner));
            adorner.UnsubscribeEvent = unsubscribeHandler;

            DragOverlay.TouchMove += moveHandler;
            DragOverlay.TouchUp += upHandler;
            DragOverlay.TouchUp += unsubscribeHandler;
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

            bool TouchUpIsOverAdorner = touchPoint.X >= adorner.Location.X && touchPoint.X <= adorner.Location.X + adorner.ActualWidth
                && touchPoint.Y >= adorner.Location.Y && touchPoint.Y <= adorner.Location.Y + adorner.ActualHeight;

            if (TouchUpIsOverAdorner)
            {
                AdornerLayer.GetAdornerLayer(sender as Visual).Remove(adorner);
                CheckAndExecuteDrop(adorner, touchPoint);
            }
        }

        private void CheckAndExecuteDrop(GalaxyAdorner adorner, Point touchPoint)
        {
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

        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            hitResultsList.Add(result.VisualHit);
            return HitTestResultBehavior.Continue;
        }
    }
}
