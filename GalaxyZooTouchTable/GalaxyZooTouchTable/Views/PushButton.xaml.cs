using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GalaxyZooTouchTable.Views
{
    public delegate void PressedChangedEventHandler(object sender, bool isPressed);

    public partial class PushButton : Button
    {
        public event PressedChangedEventHandler PressedChanged;

        public static readonly DependencyProperty PressChangedCommandProperty = DependencyProperty.Register(
            "PressChangedCommand", typeof(ICommand), typeof(PushButton), new PropertyMetadata(default(ICommand)));
        public static readonly DependencyProperty PressChangedCommandParameterProperty = DependencyProperty.Register(
            "PressChangedCommandParameter", typeof(object), typeof(PushButton), new PropertyMetadata(default(object)));
        public static readonly DependencyProperty IsTouchedProperty = DependencyProperty.Register(
            "IsTouched", typeof(bool), typeof(PushButton), new PropertyMetadata(default(bool)));

        public ICommand PressChangedCommand
        {
            get { return (ICommand)GetValue(PressChangedCommandProperty); }
            set { SetValue(PressChangedCommandProperty, value); }
        }

        /// <summary>
        /// Parameter for <see cref="PressChangedCommand"/>
        /// </summary>
        public object PressChangedCommandParameter
        {
            get { return (object)GetValue(PressChangedCommandParameterProperty); }
            set { SetValue(PressChangedCommandParameterProperty, value); }
        }

        public bool IsTouched
        {
            get { return (bool)GetValue(IsTouchedProperty); }
            set { SetValue(IsTouchedProperty, value); }
        }

        public PushButton()
        {
            InitializeComponent();

            TouchDown += AssociatedObject_TouchOn;
            TouchEnter += AssociatedObject_TouchOn;

            TouchUp += AssociatedObject_TouchOff;
            TouchLeave += AssociatedObject_TouchOff;
        }

        private void AssociatedObject_TouchOff(object sender, TouchEventArgs e)
        {
            IsTouched = false;
        }

        private void AssociatedObject_TouchOn(object sender, TouchEventArgs e)
        {
            IsTouched = true;
        }

        protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPressedChanged();
            base.OnIsPressedChanged(e);
        }

        private void OnPressedChanged()
        {
            PressedChanged?.Invoke(this, IsPressed);

            if (IsPressed)
            {
                FireCommand();
            }
        }

        void FireCommand()
        {
            if (PressChangedCommand != null && PressChangedCommand.CanExecute(PressChangedCommandParameter))
            {
                PressChangedCommand.Execute(PressChangedCommandParameter);
            }
        }
    }
}
