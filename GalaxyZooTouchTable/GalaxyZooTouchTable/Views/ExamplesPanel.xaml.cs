using System.Windows.Controls;
using System.Windows.Input;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for ExamplesPanel.xaml
    /// </summary>
    public partial class ExamplesPanel : UserControl
    {
        public ExamplesPanel()
        {
            InitializeComponent();
        }

        protected override void OnManipulationBoundaryFeedback(ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void SmoothStackPanel_TouchUp(object sender, TouchEventArgs e)
        {
            SmoothScroller.ScrollToLeftEnd();
        }

        private void FeaturesStackPanel_TouchUp(object sender, TouchEventArgs e)
        {
            FeaturesScroller.ScrollToLeftEnd();
        }

        private void NotAGalaxyStackPanel_TouchUp(object sender, TouchEventArgs e)
        {
            NotAGalaxyScroller.ScrollToLeftEnd();
        }
    }
}
