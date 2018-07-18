using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for ClassificationPanel.xaml
    /// </summary>
    public partial class ClassificationPanel : UserControl
    {
        public UserConsole Console { get; set; }

        public ClassificationPanel(UserConsole parent)
        {
            InitializeComponent();
            Console = parent;
        }

        private void CloseButton_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            MoveClassifier();
            Console.MoveButton();
            Console.ClassifierOpen = !Console.ClassifierOpen;
        }

        public void MoveClassifier()
        {
            float StartPos = Console.ClassifierOpen ? -600 : 0;
            float EndPos = Console.ClassifierOpen ? 400 : -600;

            var PanelTransform = new TranslateTransform(0, StartPos);
            RenderTransform = PanelTransform;
            DoubleAnimation panelAnimation = new DoubleAnimation(EndPos, new Duration(TimeSpan.FromSeconds(0.25)));
            if (Console.ClassifierOpen)
            {
                panelAnimation.Completed += (s, e) => Console.ControlPanel.Children.Remove(this);
            }
            PanelTransform.BeginAnimation(TranslateTransform.YProperty, panelAnimation);
        }
    }
}
