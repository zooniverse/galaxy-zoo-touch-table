using GalaxyZooTouchTable.ViewModels;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GalaxyZooTouchTable.Views
{
    /// <summary>
    /// Interaction logic for SpaceView.xaml
    /// </summary>
    public partial class SpaceView : UserControl
    {
        SpaceViewModel ViewModel { get; set; }

        public SpaceView()
        {
            InitializeComponent();
            ViewModel = DataContext as SpaceViewModel;
            ViewModel.AnimateMovement += AnimateCutoutMovement;
        }

        private void AnimateCutoutMovement(string direction)
        {
            Point startingPoint = new Point();
            Point endingPoint = new Point();

            switch (direction) {
                case "North":
                    startingPoint = new Point(0, (int)CurrentCutout.Height);
                    endingPoint = new Point(0, (int)CurrentCutout.Height * -1);
                    break;
                case "South":
                    startingPoint = new Point(0, (int)CurrentCutout.Height * -1);
                    endingPoint = new Point(0, (int)CurrentCutout.Height);
                    break;
                case "East":
                    startingPoint = new Point((int)CurrentCutout.Width * -1, 0);
                    endingPoint = new Point((int)CurrentCutout.Width, 0);
                    break;
                case "West":
                    startingPoint = new Point((int)CurrentCutout.Width, 0);
                    endingPoint = new Point((int)CurrentCutout.Width * -1, 0);
                    break;
                default:
                    Console.WriteLine("Unknown Direction");
                    break;
            }
            TransformGroup previousCutoutImageGroup = new TransformGroup();
            TransformGroup previousCutoutBorderGroup = new TransformGroup();
            ScaleTransform scaleDown = new ScaleTransform();
            DoubleAnimation animateScaleDown = new DoubleAnimation(1, 0.5, TimeSpan.FromSeconds(1));
            animateScaleDown.BeginTime = TimeSpan.FromSeconds(0.5);
            scaleDown.BeginAnimation(ScaleTransform.ScaleXProperty, animateScaleDown);
            scaleDown.BeginAnimation(ScaleTransform.ScaleYProperty, animateScaleDown);

            ScaleTransform scaleUp = new ScaleTransform();
            DoubleAnimation animateScaleUp = new DoubleAnimation(1, 2, TimeSpan.FromSeconds(1));
            animateScaleUp.BeginTime = TimeSpan.FromSeconds(2.5);
            scaleUp.BeginAnimation(ScaleTransform.ScaleXProperty, animateScaleUp);
            scaleUp.BeginAnimation(ScaleTransform.ScaleYProperty, animateScaleUp);

            TranslateTransform moveAside = new TranslateTransform();
            DoubleAnimation animateToX = new DoubleAnimation(endingPoint.X, TimeSpan.FromSeconds(1));
            DoubleAnimation animateToY = new DoubleAnimation(endingPoint.Y, TimeSpan.FromSeconds(1));
            animateToX.BeginTime = TimeSpan.FromSeconds(1.5);
            animateToY.BeginTime = TimeSpan.FromSeconds(1.5);
            moveAside.BeginAnimation(TranslateTransform.XProperty, animateToX);
            moveAside.BeginAnimation(TranslateTransform.YProperty, animateToY);

            previousCutoutImageGroup.Children.Add(scaleDown);
            previousCutoutImageGroup.Children.Add(scaleUp);
            previousCutoutBorderGroup.Children.Add(moveAside);

            PreviousCutoutImage.RenderTransform = previousCutoutImageGroup;
            PreviousCutout.RenderTransform = previousCutoutBorderGroup;

            TransformGroup currentCutoutBorderGroup = new TransformGroup();
            TransformGroup currentCutoutImageGroup = new TransformGroup();
            TranslateTransform startAside = new TranslateTransform(startingPoint.X, startingPoint.Y);
            TranslateTransform moveInto = new TranslateTransform();
            DoubleAnimation animateIntoX = new DoubleAnimation(endingPoint.X, TimeSpan.FromSeconds(1));
            DoubleAnimation animateIntoY = new DoubleAnimation(endingPoint.Y, TimeSpan.FromSeconds(1));
            animateIntoX.BeginTime = TimeSpan.FromSeconds(1.5);
            animateIntoY.BeginTime = TimeSpan.FromSeconds(1.5);
            moveInto.BeginAnimation(TranslateTransform.XProperty, animateIntoX);
            moveInto.BeginAnimation(TranslateTransform.YProperty, animateIntoY);

            currentCutoutBorderGroup.Children.Add(startAside);
            currentCutoutBorderGroup.Children.Add(moveInto);
            currentCutoutImageGroup.Children.Add(scaleDown);
            currentCutoutImageGroup.Children.Add(scaleUp);
            CurrentCutoutImage.RenderTransform = currentCutoutBorderGroup;
            CurrentCutout.RenderTransform = currentCutoutImageGroup;

            FadeOutGalaxies();
        }

        private void FadeOutGalaxies()
        {
            DoubleAnimation anim = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
            anim.AutoReverse = true;
            VisibleGalaxies.BeginAnimation(OpacityProperty, anim);
        }
    }
}
