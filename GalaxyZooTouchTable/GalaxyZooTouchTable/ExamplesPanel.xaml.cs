using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.ViewModels;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
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
            // if the user selects more than one item, remove that item and deny animation when the method runs again
            if (ExampleList.SelectedItems.Count > 1)
            {
                RemoveExtraItem();
                return;
            } else if (e.AddedItems.Count == 0 && ExampleList.SelectedItem != null)
            {
                return;
            }

            ListBoxItem SelectedUIElement = (ListBoxItem)(ExampleList.ItemContainerGenerator.ContainerFromItem(ExampleList.SelectedItem));

            // A selected item exists, move it to top
            if (SelectedUIElement != null)
            {
                HideItems();
                GeneralTransform generalTransform = SelectedElement.TransformToVisual(SelectedUIElement);
                Point point = generalTransform.Transform(new Point());

                TranslateTransform translateTransform
                    = SelectedUIElement.RenderTransform as TranslateTransform;
                if (translateTransform == null)
                {
                    translateTransform = new TranslateTransform();
                    SelectedUIElement.RenderTransform = translateTransform;
                }
                translateTransform.AnimateTo(point);
            }
            // The item has been unselected, reset all items
            else
            {
                ResetItems();
            }
        }

        private void RemoveExtraItem()
        {
            foreach (var example in new ArrayList(ExampleList.SelectedItems))
            {
                if (example != ExampleList.SelectedItem)
                {
                    ExampleList.SelectedItems.Remove(example);
                }
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
                    ToggleOpacity(element, false);
                }
            }
        }

        private void ToggleOpacity(ListBoxItem Element, bool HideItem)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = HideItem ? 1.0 : 0.0;
            animation.To = HideItem ? 0.0 : 1.0;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.25));
            Element.BeginAnimation(OpacityProperty, animation);
        }

        private void HideItems()
        {
            foreach (var example in ExampleList.Items)
            {
                ListBoxItem boxItem = (ListBoxItem)(ExampleList.ItemContainerGenerator.ContainerFromItem(example));

                if (ExampleList.SelectedItem != example)
                {
                    ToggleOpacity(boxItem, true);
                }
            }
        }
    }
}
