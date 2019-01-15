using GalaxyZooTouchTable.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GalaxyZooTouchTable.Behaviors
{
    public class UIElementDragBehavior : Behavior<UIElement>
    {
        private bool isTouchDown = false;
        private Rectangle _rectangle { get; set; }

        public List<FrameworkElement> DroppableAreas
        {
            get { return (List<FrameworkElement>)GetValue(DroppableAreasProperty); }
            set { SetValue(DroppableAreasProperty, value); }
        }

        public DragCanvas DragOverlay
        {
            get { return (DragCanvas)GetValue(DragOverlayProperty); }
            set { SetValue(DragOverlayProperty, value); }
        }

        public static readonly DependencyProperty DroppableAreasProperty = DependencyProperty.Register(
            "DroppableAreas", typeof(List<FrameworkElement>), typeof(UIElementDragBehavior));

        public static readonly DependencyProperty DragOverlayProperty = DependencyProperty.Register(
            "DragOverlay", typeof(DragCanvas), typeof(UIElementDragBehavior));

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

        private void AssociatedObject_TouchLeave(object sender, TouchEventArgs e)
        {
            if (DragOverlay == null)
            {
                var visual = e.OriginalSource as Visual;
                DragOverlay = (DragCanvas)UIElementDragBehavior.FindAncestor(typeof(DragCanvas), visual);
            }
            Border test = sender as Border;
            var DataContext = test.DataContext;

            if (isTouchDown)
            {
                var touchPosition = e.GetTouchPoint(DragOverlay);
                var initialPoint = new Point(touchPosition.Position.X, touchPosition.Position.Y);
                UIElement element = sender as UIElement;

                var myAdornerLayer = AdornerLayer.GetAdornerLayer(element);
                SimpleCircleAdorner adorner = new SimpleCircleAdorner(element, initialPoint);
                myAdornerLayer.Add(adorner);

                EventHandler<TouchEventArgs> moveHandler = new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_PreviewTouchMove(s, e, adorner));
                DragOverlay.PreviewTouchMove += moveHandler;

                EventHandler<TouchEventArgs> upHandler = new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_PreviewTouchUp(s, evt, adorner, element));
                DragOverlay.PreviewTouchUp += upHandler;

                DragOverlay.PreviewTouchUp += new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_Unsubscribe(s, e, moveHandler, upHandler, adorner));
            }
            isTouchDown = false;
        }

        private void DragDropContainer_Unsubscribe(object s, TouchEventArgs e, EventHandler<TouchEventArgs> moveHandler, EventHandler<TouchEventArgs> upHandler, SimpleCircleAdorner adorner)
        {
            if (adorner.Parent == null)
            {
                DragOverlay.PreviewTouchMove -= moveHandler;
                DragOverlay.PreviewTouchUp -= upHandler;
            }
        }

        private void DragDropContainer_PreviewTouchMove(object sender, TouchEventArgs e, SimpleCircleAdorner adorner)
        {
            var touchPosition = e.GetTouchPoint(DragOverlay);
            var point = new Point(touchPosition.Position.X, touchPosition.Position.Y);
            adorner.UpdatePosition(point);
            //DragCanvas.SetLeft(rectangle, touchPosition.Position.X - 50);
            //DragCanvas.SetTop(rectangle, touchPosition.Position.Y - 50);
        }

        private void DragDropContainer_PreviewTouchUp(object sender, TouchEventArgs e, SimpleCircleAdorner adorner, UIElement element)
        {
            var touchPosition = e.GetTouchPoint(DragOverlay);
            Point touchPoint = new Point(touchPosition.Position.X, touchPosition.Position.Y);

            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(element);

            Point adornerTopLeft = adorner.location;
            Point adornerTopRight = new Point(adorner.location.X + adorner.ActualWidth, adorner.location.Y);
            Point adornerBottomLeft = new Point(adorner.location.X, adorner.location.Y + adorner.ActualHeight);
            Point adornerBottomRight = new Point(adorner.location.X + adorner.ActualWidth, adorner.location.Y + adorner.ActualHeight);

            if (touchPoint.X >= adornerTopLeft.X && touchPoint.X <= adornerTopRight.X
                && touchPoint.Y >= adornerTopLeft.Y && touchPoint.Y <= adornerBottomLeft.Y)
            {
                AdornerLayer.GetAdornerLayer(element).Remove(adorner);
            }

            //DragOverlay.Children.Remove(rectangle);

            //VisualTreeHelper.HitTest(DragOverlay, null,
            //    new HitTestResultCallback(MyHitTestResult),
            //    new PointHitTestParameters(point));
        }

        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            List<DependencyObject> hitResultsList = new List<DependencyObject>();
            // Add the hit test result to the list that will be processed after the enumeration.
            hitResultsList.Add(result.VisualHit);

            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }

        private void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AssociatedObject_TouchDown(object sender, TouchEventArgs e)
        {
            isTouchDown = true;
        }

        public class SimpleCircleAdorner : Adorner
        {
            public Point location;
            public Point offset;

            public static DependencyProperty SubjectProperty =
                DependencyProperty.Register("Subject", typeof(TableSubject), typeof(SimpleCircleAdorner));

            // Be sure to call the base class constructor.
            public SimpleCircleAdorner(UIElement adornedElement, Point offset)
              : base(adornedElement)
            {
                this.offset = offset;
                this.DataContext = this.AdornedElement;

                this.SetUpBindings();
            }

            private void SetUpBindings()
            {
                BindingOperations.SetBinding(this,
                    SimpleCircleAdorner.SubjectProperty,
                    new Binding()
                    {
                        Path = new PropertyPath("DataContext"),
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });
            }

            public void UpdatePosition(Point location)
            {
                this.location = location;

                this.InvalidateVisual();
            }

            public TableSubject Subject
            {
                get { return (TableSubject)this.GetValue(SimpleCircleAdorner.SubjectProperty); }
                set { this.SetValue(SimpleCircleAdorner.SubjectProperty, value); }
            }

            // A common way to implement an adorner's rendering behavior is to override the OnRender
            // method, which is called by the layout system as part of a rendering pass.
            protected override void OnRender(DrawingContext drawingContext)
            {
                var p = location;
                p.Offset(-offset.X, -offset.Y);

                Rect adornedElementRect = new Rect(p, this.AdornedElement.DesiredSize);

                // Some arbitrary drawing implements.
                SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
                renderBrush.Opacity = 0.2;
                Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
                double renderRadius = 5.0;

                // Draw a circle at each corner.
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
            }
        }
    }
}
