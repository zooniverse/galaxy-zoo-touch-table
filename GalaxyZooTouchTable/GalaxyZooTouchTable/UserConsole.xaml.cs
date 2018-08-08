using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for UserConsole.xaml
    /// </summary>
    public partial class UserConsole : UserControl
    {
        ClassificationPanel Classifier { get; set; }
        public bool ClassifierOpen { get; set; } = false;

        public UserConsole()
        {
            InitializeComponent();
        }

        private async void StartButton_TouchUp(object sender, TouchEventArgs e)
        {
            await ToggleClassifier();
            AddClassifier();
            ClassifierOpen = !ClassifierOpen;
        }

        public Task ToggleClassifier()
        {
            MoveButton();
            return Task.Delay(300);
        }

        public void MoveButton()
        {
            float ButtonHeight = Convert.ToSingle(StartButton.ActualHeight);
            float EndPos = ClassifierOpen ? 0 : ButtonHeight;
            float StartPos = ClassifierOpen ? ButtonHeight : 0;
            TranslateTransform Translate = new TranslateTransform(0, StartPos);
            DoubleAnimation animation = new DoubleAnimation(EndPos, new Duration(TimeSpan.FromSeconds(0.25)));
            StartButton.RenderTransform = Translate;
            Translate.BeginAnimation(TranslateTransform.YProperty, animation);
        }

        private void AddClassifier()
        {
            ClassificationPanel panel = new ClassificationPanel(this);
            Classifier = panel;
            ControlPanel.Children.Add(panel);
            panel.MoveClassifier();
        }
    }
}
