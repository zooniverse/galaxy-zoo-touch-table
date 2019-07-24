using GalaxyZooTouchTable.Lib;
using System;
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
            Messenger.Default.Register<object>(this, OnScrollExamplesToLeft, "ScrollExamplesToLeft");
        }

        private void OnScrollExamplesToLeft(object obj)
        {
            SmoothScroller.ScrollToLeftEnd();
            FeaturesScroller.ScrollToLeftEnd();
            NotAGalaxyScroller.ScrollToLeftEnd();
        }

        protected override void OnManipulationBoundaryFeedback(ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void SmoothStackPanel_TouchUp(object sender, TouchEventArgs e)
        {
            SmoothScroller.ScrollToLeftEnd();
            ItemText.ScrollToTop();
        }

        private void FeaturesStackPanel_TouchUp(object sender, TouchEventArgs e)
        {
            FeaturesScroller.ScrollToLeftEnd();
            ItemText.ScrollToTop();
        }

        private void NotAGalaxyStackPanel_TouchUp(object sender, TouchEventArgs e)
        {
            NotAGalaxyScroller.ScrollToLeftEnd();
            ItemText.ScrollToTop();
        }
    }
}
