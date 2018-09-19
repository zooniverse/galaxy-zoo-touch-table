using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for ExamplesPanel.xaml
    /// </summary>
    public partial class ExamplesPanel : UserControl
    {
        public bool IsOpen { get; set; } = false;

        public ExamplesPanel()
        {
            InitializeComponent();
            DataContext = new ExamplesPanelViewModel();
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            var sb = new Storyboard();
            if (IsOpen)
            {
                sb = (Storyboard)FindResource("SlideLeft");
            }
            else
            {
                sb = (Storyboard)FindResource("SlideRight");
            }

            RotateArrow();
            IsOpen = !IsOpen;
            sb.Begin();
        }

        private void RotateArrow()
        {
            double RotateFrom = IsOpen ? 180 : 0;
            double RotateTo = IsOpen ? 0 : 180;
            var doubleAnimation = new DoubleAnimation(RotateFrom, RotateTo, new Duration(TimeSpan.FromSeconds(0.2)));
            var rotateTransform = new RotateTransform();
            RightArrow.RenderTransform = rotateTransform;
            RightArrow.RenderTransformOrigin = new Point(0.5, 0.5);
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, doubleAnimation);
        }

        private void UIElement_TouchDown(object sender, TouchEventArgs e)
        {
            var element = sender as Border;
            if (element !=null)
            {
                GeneralTransform generalTransform = SelectedElement.TransformToVisual(element);
                Point point = generalTransform.Transform(new Point());

                TranslateTransform translateTransform
                    = element.RenderTransform as TranslateTransform;
                if(translateTransform==null)
                {
                    translateTransform = new TranslateTransform();
                    element.RenderTransform = translateTransform;
                }
                bool elementReachedOrigin = point.X == 0 && point.Y == 0;
                if(elementReachedOrigin)
                {
                    translateTransform.AnimateTo(new Point());
                }
                else
                {
                    translateTransform.AnimateTo(point);
                }
            }
        }
    }
}
