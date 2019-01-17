using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace GalaxyZooTouchTable.Behaviors
{
    public class UIElementDropBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TouchEnter += AssociatedObject_TouchEnter;
            AssociatedObject.TouchLeave += AssociatedObject_TouchLeave;
        }

        private void AssociatedObject_TouchLeave(object sender, TouchEventArgs e)
        {
            Border element = sender as Border;
            element.Background = Brushes.Transparent;
            element.Effect = null;
        }

        private void AssociatedObject_TouchEnter(object sender, TouchEventArgs e)
        {
            Border element = sender as Border;
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0.5);
            brush.EndPoint = new Point(1, 0.5);
            brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#727272"), 0));
            brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#202020"), 1));
            brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#000000"), 1));
            element.Background = brush;

            //Border childBorder = element.Child as Border;
            //childBorder.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E5FF4D"));

            DropShadowEffect effect = new DropShadowEffect();
            effect.ShadowDepth = 0;
            effect.BlurRadius = 10;
            element.Effect = effect;
        }
    }
}
