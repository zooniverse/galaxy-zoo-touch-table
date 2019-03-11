using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace GalaxyZooTouchTable.Behaviors
{
    /// <summary>
    /// This class defines a tap behavior as a TouchDown and TouchUp on a single UIElement.
    /// Previously, only using TouchUp led to commands being triggered when a touch movement
    /// across the app surface was lifted on an unintended UIElement.
    /// </summary>
    public class TapBehavior : Behavior<UIElement>
    {
        private bool IsTouchDown { get; set; } = false;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TouchDown += AssociatedObject_TouchDown;
            AssociatedObject.TouchUp += AssociatedObject_TouchUp;
        }

        /// <summary>
        /// We check to make sure the TouchUp element is the same as the one that received
        /// the TouchDown action. If so, execute the defined command
        /// </summary>
        private void AssociatedObject_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = Handle;
            if (IsTouchDown && Command.CanExecute(sender))
            {
                Command.Execute(CommandParameter);
            }
            IsTouchDown = false;
        }

        private void AssociatedObject_TouchDown(object sender, TouchEventArgs e)
        {
            IsTouchDown = true;
        }

        public static readonly DependencyProperty HandleProperty =
            DependencyProperty.Register("Handle", typeof(bool), typeof(TapBehavior), new UIPropertyMetadata(true));

        public bool Handle
        {
            get { return (bool)GetValue(HandleProperty); }
            set { SetValue(HandleProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TapBehavior));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(TapBehavior));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
    }
}
