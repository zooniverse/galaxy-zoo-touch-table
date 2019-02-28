using System.Windows.Controls;
using System.Windows.Media;

namespace GalaxyZooTouchTable.Views
{
    /// <summary>
    /// Interaction logic for GalaxyTileView.xaml
    /// </summary>
    public partial class GalaxyTileView : UserControl
    {
        public GalaxyTileView()
        {
            InitializeComponent();
        }

        private void RingCollection_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            var OffsetX = (RingCollection.ActualWidth - SubjectImage.ActualWidth) / 2;
            var OffsetY = (RingCollection.ActualHeight - SubjectImage.ActualHeight) / 2;

            TranslateTransform Transform = new TranslateTransform();
            Transform.X = (OffsetX * -1) - 28;
            Transform.Y = (OffsetY * -1) - 28;
            TileGrid.RenderTransform = Transform;
        }
    }
}
