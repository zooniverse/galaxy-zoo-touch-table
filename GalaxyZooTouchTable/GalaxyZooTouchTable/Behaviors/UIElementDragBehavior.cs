using System;
using System.Collections.Generic;
using System.Windows;
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

            if (isTouchDown)
            {
                var rectangle = new Rectangle();
                rectangle.Width = 100;
                rectangle.Height = 100;
                rectangle.Fill = Brushes.Blue;
                rectangle.Tag = new Random();

                var touchPosition = e.GetTouchPoint(DragOverlay);

                DragOverlay.Children.Add(rectangle);
                DragCanvas.SetLeft(rectangle, touchPosition.Position.X - 50);
                DragCanvas.SetTop(rectangle, touchPosition.Position.Y - 50);


                EventHandler<TouchEventArgs> moveHandler = new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_PreviewTouchMove(s, e, rectangle));
                rectangle.PreviewTouchMove += moveHandler;

                EventHandler<TouchEventArgs> upHandler = new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_PreviewTouchUp(s, e, rectangle));
                rectangle.PreviewTouchUp += upHandler;

                rectangle.PreviewTouchUp += new EventHandler<TouchEventArgs>((s, evt) => DragDropContainer_Unsubscribe(s, e, moveHandler, upHandler));
            }
            isTouchDown = false;
        }

        private void DragDropContainer_Unsubscribe(object s, TouchEventArgs e, EventHandler<TouchEventArgs> moveHandler, EventHandler<TouchEventArgs> upHandler)
        {
            DragOverlay.PreviewTouchMove -= moveHandler;
            DragOverlay.PreviewTouchUp -= upHandler;
        }

        private void DragDropContainer_PreviewTouchMove(object sender, TouchEventArgs e, Rectangle rectangle)
        {
            var touchPosition = e.GetTouchPoint(DragOverlay);
            DragCanvas.SetLeft(rectangle, touchPosition.Position.X - 50);
            DragCanvas.SetTop(rectangle, touchPosition.Position.Y - 50);
        }

        private void DragDropContainer_PreviewTouchUp(object sender, TouchEventArgs e, Rectangle rectangle)
        {
            DragOverlay.Children.Remove(rectangle);
        }

        private void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AssociatedObject_TouchDown(object sender, TouchEventArgs e)
        {
            isTouchDown = true;
        }
    }
}
