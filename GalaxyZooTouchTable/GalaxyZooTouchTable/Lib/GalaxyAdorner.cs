using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GalaxyZooTouchTable.Lib
{
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
