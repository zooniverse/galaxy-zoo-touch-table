using System;
using System.Windows;
using System.Windows.Input;

namespace GalaxyZooTouchTable
{
    public delegate void DraggableElementEventDelegate(object sender, DraggableElementEventArgs args);

    public class DraggableElementEventArgs : EventArgs
    {
        public DraggableElement Element { get; protected set; }
        public Point DragPosition { get; set; }

        public DraggableElementEventArgs(DraggableElement element, Point dragPosition)
        {
            Element = element;
            DragPosition = dragPosition;
        }
    }

    public class DraggableElement : InteractiveElement
    {
        public DragCanvas DragParent { get; set; }

        public event DraggableElementEventDelegate ElementDragged;
        public event DraggableElementEventDelegate ElementDropped;

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);

            ElementDragged?.Invoke(this, new DraggableElementEventArgs(this, e.ManipulationOrigin));
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);

            ElementDropped?.Invoke(this, new DraggableElementEventArgs(this, DragParent.PointToScreen(e.ManipulationOrigin)));
        }
    }
}
