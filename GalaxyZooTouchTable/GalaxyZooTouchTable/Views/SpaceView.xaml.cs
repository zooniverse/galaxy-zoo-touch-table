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

            ScaleTransform scale = new ScaleTransform(0.5, 0.5);
            ScaleTransform scaleTo = new ScaleTransform();
            DoubleAnimation animateScale = new DoubleAnimation(1,2, TimeSpan.FromSeconds(1));
            animateScale.BeginTime = TimeSpan.FromSeconds(1);
            scaleTo.BeginAnimation(ScaleTransform.ScaleXProperty, animateScale);
            scaleTo.BeginAnimation(ScaleTransform.ScaleYProperty, animateScale);

            //// SET PREVIOUS Location
            TranslateTransform translateTo = new TranslateTransform();
            DoubleAnimation animateToX = new DoubleAnimation(endingPoint.X, TimeSpan.FromSeconds(1));
            DoubleAnimation animateToY = new DoubleAnimation(endingPoint.Y, TimeSpan.FromSeconds(1));
            translateTo.BeginAnimation(TranslateTransform.XProperty, animateToX);
            translateTo.BeginAnimation(TranslateTransform.YProperty, animateToY);
            TransformGroup previousCutoutBorderGroup = new TransformGroup();
            previousCutoutBorderGroup.Children.Add(new TranslateTransform(startingPoint.X, startingPoint.Y));
            previousCutoutBorderGroup.Children.Add(translateTo);
            PreviousCutout.RenderTransform = previousCutoutBorderGroup;

            TransformGroup previousCutoutImageGroup = new TransformGroup();
            previousCutoutImageGroup.Children.Add(scale);
            previousCutoutImageGroup.Children.Add(scaleTo);
            PreviousCutoutImage.RenderTransform = previousCutoutImageGroup;
            ////

            FadeOutGalaxies();

            //ScaleTransform trans = new ScaleTransform();
            //CurrentCutoutImage.RenderTransform = trans;

            //DoubleAnimation anim = new DoubleAnimation(1, 0.5, TimeSpan.FromMilliseconds(1000));
            //trans.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            //trans.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }

        private void FadeOutGalaxies()
        {
            DoubleAnimation anim = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
            anim.AutoReverse = true;
            VisibleGalaxies.BeginAnimation(OpacityProperty, anim);
        }
    }
}
