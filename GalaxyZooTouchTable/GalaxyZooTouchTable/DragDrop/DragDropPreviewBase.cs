using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GalaxyZooTouchTable.DragDrop
{
    public class DragDropPreviewBase : UserControl
    {
        public DragDropPreviewBase()
        {
            ScaleTransform scale = new ScaleTransform(1f, 1f);
            SkewTransform skew = new SkewTransform(0f, 0f);
            RotateTransform rotate = new RotateTransform(0f);
            TranslateTransform trans = new TranslateTransform(0f, 0f);
            TransformGroup transGroup = new TransformGroup();
            transGroup.Children.Add(scale);
            transGroup.Children.Add(skew);
            transGroup.Children.Add(rotate);
            transGroup.Children.Add(trans);

            this.RenderTransform = transGroup;
        }

        public DropState DropState
        {
            get { return (DropState)GetValue(DropStateProperty); }
            set { SetValue(DropStateProperty, value); }
        }

        public static readonly DependencyProperty DropStateProperty =
            DependencyProperty.Register("DropState", typeof(DropState), typeof(DragDropPreviewBase), new UIPropertyMetadata(DropStateChanged));

        public static void DropStateChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var instance = (DragDropPreviewBase)element;
            instance.StateChangedHandler(element, e);
        }

        public virtual void StateChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        { }
    }
}
