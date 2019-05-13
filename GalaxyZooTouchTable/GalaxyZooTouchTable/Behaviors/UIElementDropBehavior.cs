using System;
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

            AssociatedObject.TouchDown += AssociatedObject_TouchDown;
            AssociatedObject.TouchEnter += AssociatedObject_TouchEnter;
            AssociatedObject.TouchLeave += AssociatedObject_TouchLeave;
        }

        private void AssociatedObject_TouchDown(object sender, TouchEventArgs e)
        {
            ResetView(sender);
        }

        private void AssociatedObject_TouchLeave(object sender, TouchEventArgs e)
        {
            ResetView(sender);
        }

        private void ResetView(object sender)
        {
            Border element = sender as Border;
            element.Background = Brushes.Transparent;
            element.Effect = null;

            Border childBorder = element.Child as Border;
            childBorder.Background = Brushes.Transparent;
            childBorder.Effect = null;
        }

        private void AssociatedObject_TouchEnter(object sender, TouchEventArgs e)
        {
            Border element = sender as Border;
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0.5);
            brush.EndPoint = new Point(1, 0.5);
            brush.GradientStops.Add(new GradientStop(Colors.Gray, 0));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(32,32,32), 1));
            brush.GradientStops.Add(new GradientStop(Colors.Black, 1));
            element.Background = brush;

            Border childBorder = element.Child as Border;
            childBorder.Background = new SolidColorBrush(Color.FromArgb(60,229,255,77));
            DropShadowEffect childEffect = new DropShadowEffect();
            childEffect.BlurRadius = 20;
            childEffect.Color = Color.FromArgb(120,255,255,255);
            childBorder.Effect = childEffect;

            DropShadowEffect effect = new DropShadowEffect();
            effect.ShadowDepth = 0;
            effect.BlurRadius = 10;
            element.Effect = effect;
        }
    }
}
