using GalaxyZooTouchTable.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for UserConsole.xaml
    /// </summary>
    public partial class UserConsole : UserControl
    {
        ClassificationPanelViewModel ViewModel { get; set; }

        public UserConsole()
        {
            InitializeComponent();
        }

        private void Console_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel = DataContext as ClassificationPanelViewModel;
            ViewModel.LevelUpAnimation += OnLevelUpAnimation;
        }

        private void OnLevelUpAnimation()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation growXAnimation = new DoubleAnimation(1, 7.5, TimeSpan.FromSeconds(0.4));
            DoubleAnimation growYAnimation = new DoubleAnimation(1, 7.5, TimeSpan.FromSeconds(0.4));
            DoubleAnimation opacityAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.4));
            opacityAnimation.AutoReverse = true;
            storyboard.Children.Add(growXAnimation);
            storyboard.Children.Add(growYAnimation);
            storyboard.Children.Add(opacityAnimation);
            Storyboard.SetTargetProperty(growXAnimation, new System.Windows.PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(growYAnimation, new System.Windows.PropertyPath("RenderTransform.ScaleY"));
            Storyboard.SetTargetProperty(opacityAnimation, new System.Windows.PropertyPath(OpacityProperty));
            Storyboard.SetTarget(growXAnimation, Aura);
            Storyboard.SetTarget(growYAnimation, Aura);
            Storyboard.SetTarget(opacityAnimation, Aura);
            storyboard.Begin();
        }
    }
}
