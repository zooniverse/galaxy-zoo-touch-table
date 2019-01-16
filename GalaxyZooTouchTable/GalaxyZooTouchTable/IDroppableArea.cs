using System.Windows;

namespace GalaxyZooTouchTable
{
    interface IDroppableArea
    {
        bool IsUnder(Point p);
        void Drop(FrameworkElement element);
    }
}
