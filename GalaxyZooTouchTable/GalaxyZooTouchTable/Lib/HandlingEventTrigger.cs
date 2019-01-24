using System;
using System.Windows;

namespace GalaxyZooTouchTable.Lib
{
    public class HandlingEventTrigger : System.Windows.Interactivity.EventTrigger
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            
        }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var routedEventArgs = eventArgs as RoutedEventArgs;
            if (routedEventArgs != null)
                routedEventArgs.Handled = true;

            base.OnEvent(eventArgs);
        }
    }
}
