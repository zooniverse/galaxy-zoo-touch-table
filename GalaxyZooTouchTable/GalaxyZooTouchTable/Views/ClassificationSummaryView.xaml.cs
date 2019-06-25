using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for ClassificationSummary.xaml
    /// </summary>
    public partial class ClassificationSummaryView : UserControl, IDroppableArea
    {
        public ClassificationSummaryView()
        {
            InitializeComponent();
        }

        public bool IsUnder(Point p)
        {
            throw new System.NotImplementedException();
        }

        void IDroppableArea.Drop(FrameworkElement element)
        {
            TableSubject passedSubject = element.DataContext as TableSubject;
            ClassificationSummaryViewModel viewModel = DataContext as ClassificationSummaryViewModel;
            viewModel.DropSubject(passedSubject);
        }

        protected override System.Windows.Media.HitTestResult HitTestCore(System.Windows.Media.PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}
