using System.Windows;
using System.Windows.Controls;

namespace GalaxyZooTouchTable
{
    public class DragCanvas : Canvas
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void AddDraggableElement(DraggableElement element)
        {
            Children.Add(element);
            element.DragParent = this;
            // This is where subscription occurs
            element.ElementDragged += Element_ElementDragged;
            element.ElementDropped += Element_ElementDropped;
        }

        /// <summary>
        /// This function occurs when any element on the canvas is dropped
        /// </summary>
        /// <param name="sender">Originating element</param>
        /// <param name="args">Event args</param>
        private void Element_ElementDropped(object sender, DraggableElementEventArgs args)
        {
            IDroppableArea finalDropTarget = null;
            foreach (UIElement child in Children)
            {
                if (child is IDroppableArea)
                {
                    IDroppableArea dropTarget = child as IDroppableArea;
                    if (dropTarget != args.Element && dropTarget.IsUnder(args.DragPosition))
                    {
                        finalDropTarget = dropTarget;
                    }
                }
            }
            if (finalDropTarget != null)
            {
                finalDropTarget.Drop(args.Element);
            }
        }

        private void Element_ElementDragged(object sender, DraggableElementEventArgs args)
        {
        }
    }
}
