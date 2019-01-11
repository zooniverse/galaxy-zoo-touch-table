using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace GalaxyZooTouchTable.Behaviors
{
    public class UIElementDropBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AllowDrop = true;
            AssociatedObject.DragEnter += AssociatedObject_DragEnter;
            AssociatedObject.DragOver += AssociatedObject_DragOver;
            AssociatedObject.DragLeave += AssociatedObject_DragLeave;
            AssociatedObject.Drop += AssociatedObject_Drop;
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void AssociatedObject_DragLeave(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void AssociatedObject_DragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}
