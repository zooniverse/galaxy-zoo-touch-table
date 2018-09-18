using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for ExamplesPanel.xaml
    /// </summary>
    public partial class ExamplesPanel : UserControl
    {
        public bool IsOpen { get; set; } = false;

        public ExamplesPanel()
        {
            InitializeComponent();
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            var sb = new Storyboard();
            if (IsOpen)
            {
                sb = (Storyboard)FindResource("SlideLeft");
            }
            else
            {
                sb = (Storyboard)FindResource("SlideRight");
            }

            IsOpen = !IsOpen;
            sb.Begin();
        }
    }
}
