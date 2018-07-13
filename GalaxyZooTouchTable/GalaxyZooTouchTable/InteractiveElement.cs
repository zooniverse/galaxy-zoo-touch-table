using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GalaxyZooTouchTable
{
    public class InteractiveElement : Border
    {
        MatrixTransform transform;

        public InteractiveElement()
        {
            transform = new MatrixTransform();
            RenderTransform = transform;
            BorderThickness = new Thickness(5);
            BorderBrush = Brushes.Red;
            IsManipulationEnabled = true;
        }

        protected override void OnManipulationStarting(ManipulationStartingEventArgs e)
        {
            Canvas.SetZIndex(this, 10);
            base.OnManipulationStarting(e);
            e.ManipulationContainer = (Canvas)Parent;
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);
            Matrix matrix = transform.Matrix;
            matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
            matrix.RotateAt(e.DeltaManipulation.Rotation, e.ManipulationOrigin.X, e.ManipulationOrigin.Y);
            // matrix.ScaleAt(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Scale.Y, e.ManipulationOrigin.X, e.ManipulationOrigin.Y);
            transform.Matrix = matrix;
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            Canvas.SetZIndex(this, 0);
        }
    }
}
