using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.ViewModels;
using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.Views
{
    /// <summary>
    /// Interaction logic for SpaceView.xaml
    /// </summary>
    public partial class SpaceView : UserControl
    {
        SpaceViewModel ViewModel { get; set; }
        DispatcherTimer Timer = new DispatcherTimer();

        public SpaceView()
        {
            InitializeComponent();
            ViewModel = DataContext as SpaceViewModel;
            ViewModel.AnimateMovement += AnimateCutoutMovement;

            Timer.Tick += new EventHandler(PulseGalaxies);
            Timer.Interval = new TimeSpan(0, 0, 10);
            Timer.Start();
        }

        private void PulseGalaxies(object sender, EventArgs e)
        {
            Messenger.Default.Send(sender, "Pulse_Galaxies");
        }

        void AnimateCutoutMovement(CardinalDirectionEnum direction)
        {
            Point transitionPoint;

            switch (direction) {
                case CardinalDirectionEnum.North:
                    transitionPoint = new Point(0, (int)CurrentCutout.Height);
                    break;
                case CardinalDirectionEnum.South:
                    transitionPoint = new Point(0, (int)CurrentCutout.Height * -1);
                    break;
                case CardinalDirectionEnum.East:
                    transitionPoint = new Point((int)CurrentCutout.Width * -1, 0);
                    break;
                case CardinalDirectionEnum.West:
                    transitionPoint = new Point((int)CurrentCutout.Width, 0);
                    break;
                default:
                    return;
            }

            PerformScaleTransforms();
            PerformTranslateTransforms(transitionPoint);
            PerformFade();
        }

        void PerformTranslateTransforms(Point transitionPoint)
        {
            TransformGroup previousCutoutBorderGroup = new TransformGroup();
            TransformGroup currentCutoutBorderGroup = new TransformGroup();

            TranslateTransform moveAside = new TranslateTransform();
            DoubleAnimation animateAsideX = new DoubleAnimation(transitionPoint.X, TimeSpan.FromSeconds(0.25));
            DoubleAnimation animateAsideY = new DoubleAnimation(transitionPoint.Y, TimeSpan.FromSeconds(0.25));
            animateAsideX.BeginTime = TimeSpan.FromSeconds(0.5);
            animateAsideY.BeginTime = TimeSpan.FromSeconds(0.5);
            moveAside.BeginAnimation(TranslateTransform.XProperty, animateAsideX);
            moveAside.BeginAnimation(TranslateTransform.YProperty, animateAsideY);

            TranslateTransform startAside = new TranslateTransform(transitionPoint.X * -1, transitionPoint.Y * -1);
            TranslateTransform moveInto = new TranslateTransform();
            DoubleAnimation animateIntoX = new DoubleAnimation(transitionPoint.X, TimeSpan.FromSeconds(0.25));
            DoubleAnimation animateIntoY = new DoubleAnimation(transitionPoint.Y, TimeSpan.FromSeconds(0.25));
            animateIntoX.BeginTime = TimeSpan.FromSeconds(0.75);
            animateIntoY.BeginTime = TimeSpan.FromSeconds(0.75);
            moveInto.BeginAnimation(TranslateTransform.XProperty, animateIntoX);
            moveInto.BeginAnimation(TranslateTransform.YProperty, animateIntoY);

            previousCutoutBorderGroup.Children.Add(moveAside);
            currentCutoutBorderGroup.Children.Add(startAside);
            currentCutoutBorderGroup.Children.Add(moveInto);

            PreviousCutout.RenderTransform = previousCutoutBorderGroup;
            CurrentCutout.RenderTransform = currentCutoutBorderGroup;
        }

        void PerformScaleTransforms()
        {
            TransformGroup previousCutoutImageGroup = new TransformGroup();
            TransformGroup currentCutoutImageGroup = new TransformGroup();

            ScaleTransform scaleSmall = new ScaleTransform();
            DoubleAnimation animateScaleSmall = new DoubleAnimation(1, 0.5, TimeSpan.FromSeconds(0.25));
            animateScaleSmall.BeginTime = TimeSpan.FromSeconds(0.25);
            scaleSmall.BeginAnimation(ScaleTransform.ScaleXProperty, animateScaleSmall);
            scaleSmall.BeginAnimation(ScaleTransform.ScaleYProperty, animateScaleSmall);

            ScaleTransform scaleLarge = new ScaleTransform();
            DoubleAnimation animateScaleLarge = new DoubleAnimation(1, 2, TimeSpan.FromSeconds(0.25));
            animateScaleLarge.BeginTime = TimeSpan.FromSeconds(1);
            scaleLarge.BeginAnimation(ScaleTransform.ScaleXProperty, animateScaleLarge);
            scaleLarge.BeginAnimation(ScaleTransform.ScaleYProperty, animateScaleLarge);

            currentCutoutImageGroup.Children.Add(scaleSmall);
            currentCutoutImageGroup.Children.Add(scaleLarge);

            previousCutoutImageGroup.Children.Add(scaleSmall);
            previousCutoutImageGroup.Children.Add(scaleLarge);

            CurrentCutoutImage.RenderTransform = currentCutoutImageGroup;
            PreviousCutoutImage.RenderTransform = previousCutoutImageGroup;
        }

        void PerformFade()
        {
            DoubleAnimation animateOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0));
            DoubleAnimation animateIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            animateIn.BeginTime = TimeSpan.FromSeconds(1.5);

            Storyboard storyboard = new Storyboard();
            Storyboard.SetTargetName(animateOut, VisibleGalaxies.Name);
            Storyboard.SetTargetName(animateIn, VisibleGalaxies.Name);

            Storyboard.SetTargetProperty(animateOut,
                new System.Windows.PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(animateIn,
                new System.Windows.PropertyPath(OpacityProperty));

            storyboard.Children.Add(animateOut);
            storyboard.Children.Add(animateIn);

            storyboard.Begin(this);
        }

        private void ResetGalaxyPulseTimer(object sender, System.Windows.Input.TouchEventArgs e)
        {
            Timer.Stop();
            Timer.Start();
        }
    }
}
