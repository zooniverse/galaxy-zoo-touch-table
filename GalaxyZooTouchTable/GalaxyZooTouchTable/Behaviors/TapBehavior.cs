using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace GalaxyZooTouchTable.Behaviors
{
    public class TapBehavior : Behavior<UIElement>
    {
        private bool IsTouchDown { get; set; } = false;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TouchDown += AssociatedObject_TouchDown;
            AssociatedObject.TouchUp += AssociatedObject_TouchUp;
        }

        private void AssociatedObject_TouchUp(object sender, TouchEventArgs e)
        {
            if (IsTouchDown)
            {
                Command.Execute(sender);
            }
            IsTouchDown = false;
        }

        private void AssociatedObject_TouchDown(object sender, TouchEventArgs e)
        {
            IsTouchDown = true;
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TapBehavior));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
    }
}
