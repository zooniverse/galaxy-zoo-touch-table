using System;
using System.Threading.Tasks;
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

        private async void CloseButton_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            await MoveClassifier();
            Console.MoveButton();
            Console.ClassifierOpen = !Console.ClassifierOpen;
        }

        public Task MoveClassifier()
        {
            float ConsoleHeight = Convert.ToSingle(Console.ActualHeight);
            float StartPos = Console.ClassifierOpen ? 0 : ConsoleHeight;
            float EndPos = Console.ClassifierOpen ? ConsoleHeight : 0;

            var PanelTransform = new TranslateTransform(0, StartPos);
            RenderTransform = PanelTransform;
            DoubleAnimation panelAnimation = new DoubleAnimation(EndPos, new Duration(TimeSpan.FromSeconds(0.25)));
            if (Console.ClassifierOpen)
            {
                panelAnimation.Completed += (s, e) => Console.ControlPanel.Children.Remove(this);
            }
            PanelTransform.BeginAnimation(TranslateTransform.YProperty, panelAnimation);

            return Task.Delay(300);
        }
    }
}
