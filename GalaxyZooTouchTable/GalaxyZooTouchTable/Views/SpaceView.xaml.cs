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
        DispatcherTimer GalaxyPulseTimer = new DispatcherTimer();
        DispatcherTimer MoveMapPulseTimer = new DispatcherTimer();

        public SpaceView()
        {
            InitializeComponent();
            ViewModel = DataContext as SpaceViewModel;
            ViewModel.AnimateMovement += AnimateCutoutMovement;

            GalaxyPulseTimer.Tick += new EventHandler(PulseGalaxies);
            MoveMapPulseTimer.Tick += new EventHandler(PulseButtons);

            GalaxyPulseTimer.Interval = new TimeSpan(0, 0, 10);
            MoveMapPulseTimer.Interval = new TimeSpan(0, 0, 10);
            GalaxyPulseTimer.Start();
            MoveMapPulseTimer.Start();

            Messenger.Default.Register<object>(this, OnNewUserGalaxyPulse, "New_User_Galaxy_Pulse");
        }

        private void OnNewUserGalaxyPulse(object sender)
        {
            Messenger.Default.Send(sender, "Pulse_Galaxies");
            GalaxyPulseTimer.Stop();
            GalaxyPulseTimer.Start();
        }

        private void PulseGalaxies(object sender, EventArgs e)
        {
            Messenger.Default.Send(sender, "Pulse_Galaxies");
        }

        private void PulseButtons(object sender, EventArgs e)
        {
            DoubleAnimation expandHeight = new DoubleAnimation(24, 32, TimeSpan.FromSeconds(0.75));
            DoubleAnimation expandWidth = new DoubleAnimation(94, 110, TimeSpan.FromSeconds(0.75));
            expandHeight.AutoReverse = expandWidth.AutoReverse = true;
            expandHeight.RepeatBehavior = expandWidth.RepeatBehavior = new RepeatBehavior(2);
            expandHeight.EasingFunction = expandWidth.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseIn };

            MoveMapNorth.BeginAnimation(HeightProperty, expandHeight);
            MoveMapNorth.BeginAnimation(WidthProperty, expandWidth);

            MoveMapSouth.BeginAnimation(HeightProperty, expandHeight);
            MoveMapSouth.BeginAnimation(WidthProperty, expandWidth);

            MoveMapEast.BeginAnimation(HeightProperty, expandHeight);
            MoveMapEast.BeginAnimation(WidthProperty, expandWidth);

            MoveMapWest.BeginAnimation(HeightProperty, expandHeight);
            MoveMapWest.BeginAnimation(WidthProperty, expandWidth);
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
            GalaxyPulseTimer.Stop();
            GalaxyPulseTimer.Start();
        }
    }
}
