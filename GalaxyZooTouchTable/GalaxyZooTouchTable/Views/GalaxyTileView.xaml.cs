using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GalaxyZooTouchTable.Views
{
    /// <summary>
    /// Interaction logic for GalaxyTileView.xaml
    /// </summary>
    public partial class GalaxyTileView : UserControl
    {
        readonly int INITIAL_RING_WIDTH = 56;

        public GalaxyTileView()
        {
            InitializeComponent();
            AddPulse();
        }

        private void AddPulse()
        {
            Storyboard storyboard = new Storyboard();
            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            MainItem.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            MainItem.RenderTransform = scale;

            DoubleAnimation growAnimation = new DoubleAnimation();
            growAnimation.Duration = TimeSpan.FromSeconds(0.5);
            growAnimation.From = 1;
            growAnimation.To = 1.1;
            growAnimation.RepeatBehavior = RepeatBehavior.Forever;
            growAnimation.AutoReverse = true;

            DoubleAnimation growYAnimation = new DoubleAnimation();
            growYAnimation.Duration = TimeSpan.FromSeconds(0.5);
            growYAnimation.From = 1;
            growYAnimation.To = 1.1;
            growYAnimation.RepeatBehavior = RepeatBehavior.Forever;
            growYAnimation.AutoReverse = true;

            storyboard.Children.Add(growAnimation);
            storyboard.Children.Add(growYAnimation);

            Storyboard.SetTargetProperty(growAnimation, new System.Windows.PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(growYAnimation, new System.Windows.PropertyPath("RenderTransform.ScaleY"));
            Storyboard.SetTarget(growAnimation, MainItem);
            Storyboard.SetTarget(growYAnimation, MainItem);
            storyboard.Begin();
        }

        /// <summary>
        /// When a galaxy gain or loses rings, the tile must change position so the center point is always over the 
        /// subject.
        /// </summary>
        private void RingCollection_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            //var OffsetX = (RingCollection.ActualWidth - INITIAL_RING_WIDTH) / 2;
            //var OffsetY = (RingCollection.ActualHeight - INITIAL_RING_WIDTH) / 2;

            //TranslateTransform Transform = new TranslateTransform();
            //Transform.X = (OffsetX * -1) - 28;
            //Transform.Y = (OffsetY * -1) - 28;
            //TileGrid.RenderTransform = Transform;
        }
    }
}
