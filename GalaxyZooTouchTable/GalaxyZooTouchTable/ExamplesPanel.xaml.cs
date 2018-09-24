using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.ViewModels;
using System;
using System.Collections;
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

        private void ExampleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ExampleList.SelectedItem;

            // if the user selects more than one item, remove that item and deny animation when the method runs again
            if (ExampleList.SelectedItems.Count > 1)
            {
                foreach (var example in new ArrayList(ExampleList.SelectedItems))
                {
                    if (example != selectedItem)
                    {
                        ExampleList.SelectedItems.Remove(example);
                    }
                }
                return;
            } else if (e.AddedItems.Count == 0 && selectedItem != null)
            {
                return;
            }

            ListBoxItem element = (ListBoxItem)(ExampleList.ItemContainerGenerator.ContainerFromItem(selectedItem));

            // A selected item exists, move it to top
            if (element != null)
            {
                HideItems();
                GeneralTransform generalTransform = SelectedElement.TransformToVisual(element);
                Point point = generalTransform.Transform(new Point());

                TranslateTransform translateTransform
                    = element.RenderTransform as TranslateTransform;
                if (translateTransform == null)
                {
                    translateTransform = new TranslateTransform();
                    element.RenderTransform = translateTransform;
                }
                translateTransform.AnimateTo(point);
            }
            // A selected item doesn't exist, reset all items
            else
            {
                ResetItems();
            }
        }

        private void ResetItems()
        {
            foreach(var item in ExampleList.Items)
            {
                ListBoxItem element = (ListBoxItem)(ExampleList.ItemContainerGenerator.ContainerFromItem(item));
                GeneralTransform generalTransform = SelectedElement.TransformToVisual(element);
                Point point = generalTransform.Transform(new Point());

                TranslateTransform translateTransform
                    = element.RenderTransform as TranslateTransform;
                if (translateTransform == null)
                {
                    translateTransform = new TranslateTransform();
                    element.RenderTransform = translateTransform;
                }
                bool elementReachedOrigin = point.X == 0 && point.Y == 0;
                if (elementReachedOrigin)
                {
                    translateTransform.AnimateTo(new Point());
                } else
                {
                    DoubleAnimation animation = new DoubleAnimation();
                    animation.From = 0.0;
                    animation.To = 1.0;
                    animation.Duration = new Duration(TimeSpan.FromSeconds(0.25));
                    element.BeginAnimation(OpacityProperty, animation);
                }
            }
        }

        private void HideItems()
        {
            var item = ExampleList.SelectedItem;

            foreach (var example in ExampleList.Items)
            {
                ListBoxItem boxItem = (ListBoxItem)(ExampleList.ItemContainerGenerator.ContainerFromItem(example));

                if (item != example)
                {
                    DoubleAnimation animation = new DoubleAnimation();
                    animation.From = 1.0;
                    animation.To = 0.0;
                    animation.Duration = new Duration(TimeSpan.FromSeconds(0.25));
                    boxItem.BeginAnimation(OpacityProperty, animation);
                }
            }
        }
    }
}
