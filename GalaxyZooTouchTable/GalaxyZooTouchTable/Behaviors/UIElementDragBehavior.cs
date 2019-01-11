using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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

        public Grid DragOverlay
        {
            get { return (Grid)GetValue(DragOverlayProperty); }
            set { SetValue(DragOverlayProperty, value); }
        }

        public static readonly DependencyProperty DragOverlayProperty = DependencyProperty.Register(
            "DragOverlay", typeof(Grid), typeof(UIElementDragBehavior));

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TouchDown += AssociatedObject_TouchDown;
            AssociatedObject.TouchLeave += AssociatedObject_TouchLeave;
            AssociatedObject.TouchUp += AssociatedObject_TouchUp;
            AssociatedObject.PreviewTouchMove += AssociatedObject_PreviewTouchMove;
            AssociatedObject.PreviewTouchDown += AssociatedObject_PreviewTouchDown;
            AssociatedObject.PreviewTouchUp += AssociatedObject_PreviewTouchUp;
        }


        private void AssociatedObject_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            Console.WriteLine("Preview Touch Up");
        }

        private void AssociatedObject_PreviewTouchDown(object sender, TouchEventArgs e)
        {
        }

        private void AssociatedObject_PreviewTouchMove(object sender, TouchEventArgs e)
        {

        }

        private void AssociatedObject_TouchUp(object sender, TouchEventArgs e)
        {
            isTouchDown = false;
        }

        private void AssociatedObject_TouchLeave(object sender, TouchEventArgs e)
        {
            Console.WriteLine("TOUCH LEAVE");
            if (isTouchDown)
            {
                e.Handled = true;
                //Label l = e.Source as Label;
                //l.DoDragDrop(this.AssociatedObject, System.Windows.Forms.DragDropEffects.Copy);

                _rectangle = new Rectangle();
                _rectangle.Width = 100;
                _rectangle.Height = 100;
                _rectangle.Fill = Brushes.Blue;

                DragOverlay.Children.Add(_rectangle);

                DragOverlay.PreviewTouchMove += DragDropContainer_PreviewTouchMove;
                DragOverlay.PreviewTouchUp += DragDropContainer_PreviewTouchUp;
            }
            isTouchDown = false;
        }

        private void FinalizePreviewControlMouseUp()
        {
            DragOverlay.Children.Remove(_rectangle);
            DragOverlay.PreviewTouchMove -= DragDropContainer_PreviewTouchMove;
            DragOverlay.PreviewTouchUp -= DragDropContainer_PreviewTouchUp;
        }

        private void DragDropContainer_PreviewTouchMove(object sender, TouchEventArgs e)
        {
        }

        private void DragDropContainer_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            // TODO: Fix this to handle multiple subscribers. It currently remove subscription for any in progress drag
            FinalizePreviewControlMouseUp();
        }

        private void AssociatedObject_TouchDown(object sender, TouchEventArgs e)
        {
            isTouchDown = true;
        }

        //private async Task<Point> DragAsync(UIElement shape, TouchEventArgs e, IProgress<Point> progress = null)
        //{
        //    Point point = new Point(Canvas.GetLeft(shape), Canvas.GetTop(shape));
        //    Point position = e.GetTouchPoint(null).Position;

        //    Task<RoutedEventArgs> stopTracking = shape.WhenPointerLost();

        //    while (!stopTracking.IsCompleted)
        //    {
        //        Task<RoutedEventArgs> moved = shape.WhenPointerMoved();
        //        await Task.WhenAny(stopTracking, moved);
        //        {
        //            if (moved.IsCompleted)
        //            {
        //                Point pt = moved.Result.GetCurrentPoint(null).Position;
        //                var x = point.X + pt.X - position.X;
        //                var y = point.Y + pt.Y - position.Y;
        //                Canvas.SetLeft(shape, x);
        //                Canvas.SetTop(shape, y);
        //            }
        //        }
        //    }
        //}
    }
}
