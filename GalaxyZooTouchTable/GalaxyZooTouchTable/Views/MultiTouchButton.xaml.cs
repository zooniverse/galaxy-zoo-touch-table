using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GalaxyZooTouchTable.Views
{
    public partial class MultiTouchButton : Button
    {
        bool IsDeliberate { get; set; } = false;

        public static readonly DependencyProperty PressCommandProperty = DependencyProperty.Register(
            "PressCommand", typeof(ICommand), typeof(MultiTouchButton), new PropertyMetadata(default(ICommand)));
        public static readonly DependencyProperty PressCommandParameterProperty = DependencyProperty.Register(
            "PressCommandParameter", typeof(object), typeof(MultiTouchButton), new PropertyMetadata(default(object)));
        public static readonly DependencyProperty IsTouchedProperty = DependencyProperty.Register(
            "IsTouched", typeof(bool), typeof(MultiTouchButton), new PropertyMetadata(default(bool)));

        public ICommand PressCommand
        {
            get { return (ICommand)GetValue(PressCommandProperty); }
            set { SetValue(PressCommandProperty, value); }
        }

        /// <summary>
        /// Parameter for <see cref="PressCommand"/>
        /// </summary>
        public object PressCommandParameter
        {
            get { return (object)GetValue(PressCommandParameterProperty); }
            set { SetValue(PressCommandParameterProperty, value); }
        }

        public bool IsTouched
        {
            get { return (bool)GetValue(IsTouchedProperty); }
            set { SetValue(IsTouchedProperty, value); }
        }

        public MultiTouchButton()
        {
            InitializeComponent();

            TouchDown += MultiTouchButton_TouchDown;
            TouchEnter += MultiTouchButton_TouchEnter;

            TouchUp += MultiTouchButton_TouchUp;
            TouchLeave += MultiTouchButton_TouchLeave;
        }

        private void MultiTouchButton_TouchDown(object sender, TouchEventArgs e)
        {
            SetTouchState(true);
        }

        private void MultiTouchButton_TouchLeave(object sender, TouchEventArgs e)
        {
            SetTouchState(false);
        }

        private void MultiTouchButton_TouchUp(object sender, TouchEventArgs e)
        {
            if (IsTouched && IsDeliberate)
            {
                FireCommand();
            }
            SetTouchState(false);
        }

        private void MultiTouchButton_TouchEnter(object sender, TouchEventArgs e)
        {
            IsTouched = true;
        }

        private void SetTouchState(bool state)
        {
            IsDeliberate = IsTouched = state;
        }

        void FireCommand()
        {
            if (PressCommand != null && PressCommand.CanExecute(PressCommandParameter))
            {
                PressCommand.Execute(PressCommandParameter);
            }
        }
    }
}
