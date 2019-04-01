using GalaxyZooTouchTable.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for ClassificationPanel.xaml
    /// </summary>
    public partial class ClassificationPanel : UserControl
    {
        public ClassificationPanel()
        {
            InitializeComponent();
        }

        private void ResetTimer(object sender, System.Windows.Input.TouchEventArgs e)
        {
            var Element = sender as FrameworkElement;
            ClassificationPanelViewModel Classifier = Element.DataContext as ClassificationPanelViewModel;
            Classifier.StartStillThereModalTimer();
        }

        protected override System.Windows.Media.HitTestResult HitTestCore(System.Windows.Media.PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}
