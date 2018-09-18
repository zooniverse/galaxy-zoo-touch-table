using GalaxyZooTouchTable.Lib;
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
            var element = e.OriginalSource as FrameworkElement;
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
                translateTransform.AnimateTo(point);
            }
        }
    }
}
