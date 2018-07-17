using System;
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
        public bool ClassifierOpen { get => classifierOpen; set => classifierOpen = value; }

        private bool classifierOpen = false;

        public UserConsole()
        {
            InitializeComponent();
        }

        private void StartButton_TouchUp(object sender, TouchEventArgs e)
        {
            ToggleClassifier();
        }

        public void ToggleClassifier()
        {
            MoveButton();
            AddClassifier();

            ClassifierOpen = !ClassifierOpen;
        }

        public void MoveButton()
        {
            float EndPos = ClassifierOpen ? 0 : 250;
            float StartPos = ClassifierOpen ? 250 : 0;
            TranslateTransform Translate = new TranslateTransform(0, StartPos);
            DoubleAnimation animation = new DoubleAnimation(EndPos, new Duration(TimeSpan.FromSeconds(0.25)));
            StartButton.RenderTransform = Translate;
            Translate.BeginAnimation(TranslateTransform.YProperty, animation);
        }

        private void AddClassifier()
        {
            ClassificationPanel panel = new ClassificationPanel(this);
            Classifier = panel;
            Canvas.SetTop(panel, 300);
            ControlPanel.Children.Add(panel);
            panel.MoveClassifier();
        }
    }
}
