using System.Windows;

namespace GalaxyZooTouchTable
{
    public class UserConsole : DraggableElement, IDroppableArea
    {
        public UserConsole()
        {
            IntroUI ui = new IntroUI();
            Child = ui;

            // Data Binding below
            //ClassificationData sampleData = new ClassificationData()
            //{
            //    Name = "This is a test",
            //    Color = Brushes.Red
            //};
            //ui.DataContext = sampleData;
        }

        public bool IsUnder(Point p)
        {
            Point localPoint = PointFromScreen(p);
            return localPoint.X >= 0 && localPoint.X <= ActualWidth && localPoint.Y >= 0 && localPoint.Y <= ActualHeight;
        }

        void IDroppableArea.Drop(DraggableElement element)
        {
            Child = new ClassificationUI();
        }
    }
}
