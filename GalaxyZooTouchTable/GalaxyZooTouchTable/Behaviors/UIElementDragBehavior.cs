using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace GalaxyZooTouchTable.Behaviors
{
    public class UIElementDragBehavior : Behavior<UIElement>
    {
        private bool isTouchDown = false;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TouchDown += AssociatedObject_TouchDown;
            AssociatedObject.TouchLeave += AssociatedObject_TouchLeave;
            AssociatedObject.TouchUp += AssociatedObject_TouchUp;
        }

        private void AssociatedObject_TouchUp(object sender, TouchEventArgs e)
        {
            isTouchDown = false;
        }

        private void AssociatedObject_TouchLeave(object sender, TouchEventArgs e)
        {
            if (isTouchDown)
            {
                e.Handled = true;
                //Label l = e.Source as Label;
                //l.DoDragDrop(this.AssociatedObject, System.Windows.Forms.DragDropEffects.Copy);
            }
        }

        private async void AssociatedObject_TouchDown(object sender, TouchEventArgs e)
        {
            //Point result = await DragAsync((UIElement)sender, e);
            isTouchDown = true;
        }

        private async Task<Point> DragAsync(UIElement shape, TouchEventArgs e, IProgress<Point> progress = null)
        {
            Point point = new Point(Canvas.GetLeft(shape), Canvas.GetTop(shape));
            Point position = e.GetTouchPoint(null).Position;

            Task<RoutedEventArgs> stopTracking = shape.WhenPointerLost();

            while (!stopTracking.IsCompleted)
            {
                Task<RoutedEventArgs> moved = shape.WhenPointerMoved();
                await Task.WhenAny(stopTracking, moved);
                {
                    if (moved.IsCompleted)
                    {
                        Point pt = moved.Result.GetCurrentPoint(null).Position;
                        var x = point.X + pt.X - position.X;
                        var y = point.Y + pt.Y - position.Y;
                        Canvas.SetLeft(shape, x);
                        Canvas.SetTop(shape, y);
                    }
                }
            }
        }
    }
}
