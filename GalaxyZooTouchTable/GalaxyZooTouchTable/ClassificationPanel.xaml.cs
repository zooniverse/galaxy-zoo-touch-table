using System.Windows.Controls;

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
            this.Console = parent;
        }

        private void CloseButton_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            this.Console.StartButton.Visibility = System.Windows.Visibility.Visible;
            this.Console.ControlPanel.Children.Remove(this);
        }
    }
}
