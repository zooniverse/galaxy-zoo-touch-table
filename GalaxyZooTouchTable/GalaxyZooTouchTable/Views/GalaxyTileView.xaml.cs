using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.Views
{
    /// <summary>
    /// Interaction logic for GalaxyTileView.xaml
    /// </summary>
    public partial class GalaxyTileView : UserControl
    {
        readonly int INITIAL_RING_WIDTH = 56;
        DispatcherTimer Timer = new DispatcherTimer();

        public GalaxyTileView()
        {
            InitializeComponent();

            Timer.Tick += new EventHandler(AddPulse);
            Timer.Interval = new TimeSpan(0, 0, 10);
            Timer.Start();
        }

        private void AddPulse(object sender, EventArgs e)
        {
            Storyboard storyboard = new Storyboard();
            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            TileGrid.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            TileGrid.RenderTransform = scale;

            DoubleAnimation growAnimation = new DoubleAnimation();
            growAnimation.Duration = TimeSpan.FromSeconds(0.5);
            growAnimation.From = 1;
            growAnimation.To = 1.1;
            growAnimation.AutoReverse = true;
            growAnimation.RepeatBehavior = new RepeatBehavior(2);

            DoubleAnimation growYAnimation = new DoubleAnimation();
            growYAnimation.Duration = TimeSpan.FromSeconds(0.5);
            growYAnimation.From = 1;
            growYAnimation.To = 1.1;
            growYAnimation.AutoReverse = true;
            growYAnimation.RepeatBehavior = new RepeatBehavior(2);

            storyboard.Children.Add(growAnimation);
            storyboard.Children.Add(growYAnimation);

            Storyboard.SetTargetProperty(growAnimation, new System.Windows.PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(growYAnimation, new System.Windows.PropertyPath("RenderTransform.ScaleY"));
            Storyboard.SetTarget(growAnimation, TileGrid);
            Storyboard.SetTarget(growYAnimation, TileGrid);
            storyboard.Begin();
        }

        /// <summary>
        /// When a galaxy gain or loses rings, the tile must change position so the center point is always over the 
        /// subject.
        /// </summary>
        private void RingCollection_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            var OffsetX = (RingCollection.ActualWidth - INITIAL_RING_WIDTH) / 2;
            var OffsetY = (RingCollection.ActualHeight - INITIAL_RING_WIDTH) / 2;

            TranslateTransform Transform = new TranslateTransform();
            Transform.X = (OffsetX * -1) - 28;
            Transform.Y = (OffsetY * -1) - 28;
            MainTile.RenderTransform = Transform;
        }
    }
}
