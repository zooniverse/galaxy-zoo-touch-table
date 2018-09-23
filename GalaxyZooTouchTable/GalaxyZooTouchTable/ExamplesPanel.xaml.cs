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
        public ExamplesPanel()
        {
            InitializeComponent();
            DataContext = new ExamplesPanelViewModel();
        }

        public void UIElement_TouchDown(object sender, TouchEventArgs e)
        {
            var source = sender as UIElement;
            var element = FindParent<Border>(source);

            var selectedItem = element.DataContext;


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
                    Console.WriteLine("TO ORIGIN");
                    ListItems.SelectedItem = null;
                    translateTransform.AnimateTo(new Point());
                }
                else
                {
                    Console.WriteLine("TO TOP");
                    translateTransform.AnimateTo(point);
                }
            }
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}
