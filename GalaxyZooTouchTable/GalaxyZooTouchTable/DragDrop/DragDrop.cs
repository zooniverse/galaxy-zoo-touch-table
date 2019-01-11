using GalaxyZooTouchTable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GalaxyZooTouchTable.DragDrop
{
    public class DragDrop
    {
        private Window _topWindow;
        private TouchPoint _initialMousePosition;
        private Canvas _dragDropContainer;
        private UIElement _dropTarget;
        private Object _dragDropPreviewControlDataContext;
        private ICommand _itemDroppedCommand;
        private Boolean _mouseCaptured;
        private DragDropPreviewBase _dragDropPreviewControl;
        private Point _delta;

        public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached(
            "IsDragSource", typeof(Boolean), typeof(DragDrop), new PropertyMetadata(false, IsDragSourceChanged));

        public static readonly DependencyProperty DragDropContainerProperty = DependencyProperty.RegisterAttached(
            "DragDropContainer", typeof(Panel), typeof(DragDrop), new PropertyMetadata(default(UIElement)));

        public static readonly DependencyProperty DropTargetProperty = DependencyProperty.RegisterAttached(
            "DropTarget", typeof(UIElement), typeof(DragDrop), new PropertyMetadata(default(String)));

        public static readonly DependencyProperty DragDropPreviewControlDataContextProperty = DependencyProperty.RegisterAttached(
            "DragDropPreviewControlDataContext", typeof(Object), typeof(DragDrop), new PropertyMetadata(default(Object)));

        public static readonly DependencyProperty ItemDroppedProperty = DependencyProperty.RegisterAttached(
            "ItemDropped", typeof(ICommand), typeof(DragDrop), new PropertyMetadata(new PropertyChangedCallback(AttachOrRemoveItemDroppedEvent)));

        public static readonly DependencyProperty DragDropPreviewControlProperty = DependencyProperty.RegisterAttached(
            "DragDropPreviewControl", typeof(DragDropPreviewBase), typeof(DragDrop), new PropertyMetadata(default(UIElement)));

        private static readonly Lazy<DragDrop> _Instance = new Lazy<DragDrop>(() => new DragDrop());

        public static Boolean GetIsDragSource(DependencyObject element)
        {
            return (Boolean)element.GetValue(IsDragSourceProperty);
        }

                public static void SetIsDragSource(DependencyObject element, Boolean value)
        {
            element.SetValue(IsDragSourceProperty, value);
        }

        private static DragDrop Instance
        {
            get { return _Instance.Value; }
        }

        private static void AttachOrRemoveItemDroppedEvent(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
        }

        private static void IsDragSourceChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var dragSource = element as UIElement;
            if (dragSource == null)
            { return; }

            if (Object.Equals(e.NewValue, true))
            {
                dragSource.PreviewTouchDown += Instance.DragSource_PreviewTouchDown;
                dragSource.PreviewTouchUp += Instance.DragSource_PreviewTouchUp;
                dragSource.PreviewTouchMove += Instance.DragSource_PreviewTouchMove;
            }
            else
            {
                dragSource.PreviewTouchDown -= Instance.DragSource_PreviewTouchDown;
                dragSource.PreviewTouchUp -= Instance.DragSource_PreviewTouchUp;
                dragSource.PreviewTouchMove -= Instance.DragSource_PreviewTouchMove;
            }
        }

        private void DragSource_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (_mouseCaptured || _dragDropPreviewControlDataContext == null)
            {
                return; //we're already capturing the mouse, or we don't have a data context for the preview control
            }

            if (DragDrop.IsMovementBigEnough(_initialMousePosition, e.GetTouchPoint(_topWindow)) == false)
            {
                return; //only drag when the user moved the mouse by a reasonable amount
            }
            Console.WriteLine("PREVIEW CONTROL");
            Console.WriteLine(_dragDropPreviewControl);
            _dragDropPreviewControl = (DragDropPreviewBase)GetDragDropPreviewControl(sender as DependencyObject);
            _dragDropPreviewControl.DataContext = _dragDropPreviewControlDataContext;

            TableSubject test = _dragDropPreviewControlDataContext as TableSubject;
            Console.WriteLine(test.DEC);

            _dragDropPreviewControl.Opacity = 0.7;

            _dragDropContainer.Children.Add(_dragDropPreviewControl);
            _mouseCaptured = Mouse.Capture(_dragDropPreviewControl); //have the preview control recieve and be able to handle mouse events    

            //offset it just a bit so it looks like it's underneath the mouse
            Mouse.OverrideCursor = Cursors.Hand;

            Canvas.SetLeft(_dragDropPreviewControl, _initialMousePosition.Position.X - 20);
            Canvas.SetTop(_dragDropPreviewControl, _initialMousePosition.Position.Y - 15);

            _dragDropContainer.PreviewTouchMove += DragDropContainer_PreviewTouchMove;
            _dragDropContainer.PreviewTouchUp += DragDropContainer_PreviewTouchUp;
        }

        private void DragDropContainer_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            switch (_dragDropPreviewControl.DropState)
            {
                case DropState.CanDrop:
                    try
                    {

                        var scaleXAnim = CreateDoubleAnimation(0);
                        Storyboard.SetTargetProperty(scaleXAnim, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

                        var scaleYAnim = CreateDoubleAnimation(0);
                        Storyboard.SetTargetProperty(scaleYAnim, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

                        var opacityAnim = CreateDoubleAnimation(0);
                        Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("(UIElement.Opacity)"));

                        var canDropSb = new Storyboard() { FillBehavior = FillBehavior.Stop };
                        canDropSb.Children.Add(scaleXAnim);
                        canDropSb.Children.Add(scaleYAnim);
                        canDropSb.Children.Add(opacityAnim);
                        canDropSb.Completed += (s, args) => { FinalizePreviewControlMouseUp(); };

                        canDropSb.Begin(_dragDropPreviewControl);

                        if (_itemDroppedCommand != null)
                        { _itemDroppedCommand.Execute(_dragDropPreviewControlDataContext); }
                    }
                    catch (Exception ex)
                    { }
                    break;
                case DropState.CannotDrop:
                    try
                    {
                        var translateXAnim = CreateDoubleAnimation(_delta.X);
                        Storyboard.SetTargetProperty(translateXAnim, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"));

                        var translateYAnim = CreateDoubleAnimation(_delta.Y);
                        Storyboard.SetTargetProperty(translateYAnim, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));

                        var opacityAnim = CreateDoubleAnimation(0);
                        opacityAnim.BeginTime = TimeSpan.FromMilliseconds(150);
                        Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("(UIElement.Opacity)"));

                        var cannotDropSb = new Storyboard() { FillBehavior = FillBehavior.Stop };
                        cannotDropSb.Children.Add(translateXAnim);
                        cannotDropSb.Children.Add(translateYAnim);
                        cannotDropSb.Children.Add(opacityAnim);
                        cannotDropSb.Completed += (s, args) => { FinalizePreviewControlMouseUp(); };

                        cannotDropSb.Begin(_dragDropPreviewControl);
                    }
                    catch (Exception ex) { }
                    break;
            }

            _dragDropPreviewControlDataContext = null;
            _mouseCaptured = false;
        }

        private void FinalizePreviewControlMouseUp()
        {
            _dragDropContainer.Children.Remove(_dragDropPreviewControl);
            _dragDropContainer.PreviewTouchMove -= DragDropContainer_PreviewTouchMove;
            _dragDropContainer.PreviewTouchUp -= DragDropContainer_PreviewTouchUp;

            if (_dragDropPreviewControl != null)
            {
                _dragDropPreviewControl.ReleaseMouseCapture();
            }
            _dragDropPreviewControl = null;
            Mouse.OverrideCursor = null;
        }

        private static DoubleAnimation CreateDoubleAnimation(Double to)
        {
            var anim = new DoubleAnimation();
            anim.To = to;
            anim.Duration = TimeSpan.FromMilliseconds(250);
            anim.AccelerationRatio = 0.1;
            anim.DecelerationRatio = 0.9;

            return anim;
        }

        private void DragDropContainer_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            TouchPoint currentPoint = e.GetTouchPoint(_topWindow);

            //offset it just a bit so it looks like it's underneath the mouse
            //Mouse.OverrideCursor = Cursors.Hand;
            //currentPoint.Position = new Point(currentPoint.Position.X - 20, 10);
            //currentPoint.Position.Y = currentPoint.Position.Y - 15;

            _delta = new Point(_initialMousePosition.Position.X - currentPoint.Position.X, _initialMousePosition.Position.Y - currentPoint.Position.Y);
            var target = new Point(_initialMousePosition.Position.X - _delta.X, _initialMousePosition.Position.Y - _delta.Y);

            Canvas.SetLeft(_dragDropPreviewControl, target.X);
            Canvas.SetTop(_dragDropPreviewControl, target.Y);

            _dragDropPreviewControl.DropState = DropState.CannotDrop;

            if (_dropTarget == null)
            {
                AnimateDropState();
                return;
            }

            var transform = _dropTarget.TransformToVisual(_dragDropContainer);
            var dropBoundingBox = transform.TransformBounds(new Rect(0, 0, _dropTarget.RenderSize.Width, _dropTarget.RenderSize.Height));

            if (e.GetTouchPoint(_dragDropContainer).Position.X > dropBoundingBox.Left &&
                e.GetTouchPoint(_dragDropContainer).Position.X < dropBoundingBox.Right &&
                e.GetTouchPoint(_dragDropContainer).Position.Y > dropBoundingBox.Top &&
                e.GetTouchPoint(_dragDropContainer).Position.Y < dropBoundingBox.Bottom)
            {
                _dragDropPreviewControl.DropState = DropState.CanDrop;
            }

            //bounding box might allow us to drop, but now we need to check with the command
            if (_itemDroppedCommand != null && _itemDroppedCommand.CanExecute(_dragDropPreviewControlDataContext) == false)
            {
                _dragDropPreviewControl.DropState = DropState.CannotDrop; //commanding trumps visual.                                    
            }

            AnimateDropState();
        }

        private void DragSource_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            _dragDropPreviewControlDataContext = null;
            _mouseCaptured = false;

            if (_dragDropPreviewControl != null)
            { _dragDropPreviewControl.ReleaseMouseCapture(); }
        }

        private void DragSource_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                var visual = e.OriginalSource as Visual;
                _topWindow = (Window)DragDrop.FindAncestor(typeof(Window), visual);

                _initialMousePosition = e.GetTouchPoint(_topWindow);

                //first, determine if the outer container property is bound
                _dragDropContainer = DragDrop.GetDragDropContainer(sender as DependencyObject) as Canvas;

                if (_dragDropContainer == null)
                {
                    //set the container to the canvas ancestor of the bound visual
                    _dragDropContainer = (Canvas)DragDrop.FindAncestor(typeof(Canvas), visual);
                }

                _dropTarget = GetDropTarget(sender as DependencyObject);

                //get the data context for the preview control
                _dragDropPreviewControlDataContext = DragDrop.GetDragDropPreviewControlDataContext(sender as DependencyObject);

                if (_dragDropPreviewControlDataContext == null)
                { _dragDropPreviewControlDataContext = (sender as FrameworkElement).DataContext; }


                _itemDroppedCommand = DragDrop.GetItemDropped(sender as DependencyObject);

            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception in DragDropHelper: " + exc.InnerException.ToString());
            }
        }

        private void AnimateDropState()
        {
            //determine if we need to animate states
            switch (_dragDropPreviewControl.DropState)
            {
                case DropState.CanDrop:

                    if (_dragDropPreviewControl.Resources.Contains("canDropChanged"))
                    {
                        ((Storyboard)_dragDropPreviewControl.Resources["canDropChanged"]).Begin(_dragDropPreviewControl);
                    }

                    break;
                case DropState.CannotDrop:
                    if (_dragDropPreviewControl.Resources.Contains("cannotDropChanged"))
                    {
                        ((Storyboard)_dragDropPreviewControl.Resources["cannotDropChanged"]).Begin(_dragDropPreviewControl);
                    }
                    break;
                default:
                    break;
            }
        }

        public static FrameworkElement FindAncestor(Type ancestorType, Visual visual)
        {
            while (visual != null && !ancestorType.IsInstanceOfType(visual))
            {
                visual = (Visual)VisualTreeHelper.GetParent(visual);
            }
            return visual as FrameworkElement;
        }

        public static Panel GetDragDropContainer(DependencyObject element)
        {
            return (Panel)element.GetValue(DragDropContainerProperty);
        }

        public static void SetDragDropContainer(DependencyObject element, Panel value)
        {
            element.SetValue(DragDropContainerProperty, value);
        }

        public static UIElement GetDropTarget(DependencyObject element)
        {
            return (UIElement)element.GetValue(DropTargetProperty);
        }

        public static Object GetDragDropPreviewControlDataContext(DependencyObject element)
        {
            return (Object)element.GetValue(DragDropPreviewControlDataContextProperty);
        }

        public static ICommand GetItemDropped(DependencyObject element)
        {
            return (ICommand)element.GetValue(ItemDroppedProperty);
        }

        public static DragDropPreviewBase GetDragDropPreviewControl(DependencyObject element)
        {
            return (DragDropPreviewBase)element.GetValue(DragDropPreviewControlProperty);
        }

        public static void SetDragDropPreviewControl(DependencyObject element, DragDropPreviewBase value)
        {
            element.SetValue(DragDropPreviewControlProperty, value);
        }

        public static Boolean IsMovementBigEnough(TouchPoint initialMousePosition, TouchPoint currentPosition)
        {
            return (Math.Abs(currentPosition.Position.X - initialMousePosition.Position.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPosition.Position.Y - initialMousePosition.Position.Y) >= SystemParameters.MinimumVerticalDragDistance);
        }
    }
}
