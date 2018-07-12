using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for UserConsole.xaml
    /// </summary>
    public partial class UserConsole : UserControl
    {
        ClassificationPanel Classifier { get; set; }

        public UserConsole()
        {
            InitializeComponent();
        }

        private void StartButton_TouchUp(object sender, TouchEventArgs e)
        {
            ClassificationPanel panel = new ClassificationPanel(this);
            ControlPanel.Children.Add(panel);
            this.Classifier = panel;
            this.StartButton.Visibility = Visibility.Hidden;
        }
    }
}
