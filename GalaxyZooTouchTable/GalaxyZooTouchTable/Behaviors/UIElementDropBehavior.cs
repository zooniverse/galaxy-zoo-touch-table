using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace GalaxyZooTouchTable.Behaviors
{
    public class UIElementDropBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TouchEnter += AssociatedObject_TouchEnter;
            AssociatedObject.TouchLeave += AssociatedObject_TouchLeave;
        }

        private void AssociatedObject_TouchLeave(object sender, TouchEventArgs e)
        {
            Border element = sender as Border;
            element.Background = Brushes.Transparent;
        }

        private void AssociatedObject_TouchEnter(object sender, TouchEventArgs e)
        {
            Border element = sender as Border;
            element.Background = Brushes.Red;
        }
    }
}
