using GalaxyZooTouchTable.Models;
using PanoptesNetClient;
using PanoptesNetClient.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for UserConsole.xaml
    /// </summary>
    public partial class UserConsole : UserControl
    {
        public ClassificationPanel Classifier { get; set; }
        public bool ClassifierOpen { get; set; } = false;
        public Workflow Workflow { get; set; }

        public UserConsole()
        {
            InitializeComponent();
            GetWorkflow();
        }

        private async void GetWorkflow()
        {
            ApiClient client = new ApiClient();
            Workflow = await client.Workflows.Get(Config.WorkflowId);
        }

        private async void StartButton_TouchUp(object sender, TouchEventArgs e)
        {
            //await ToggleClassifier();
            //AddClassifier();
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
            TableUser user = DataContext as TableUser;
            ClassificationPanel panel = new ClassificationPanel(this, Workflow, user);
            Classifier = panel;
            ControlPanel.Children.Add(panel);
            panel.MoveClassifier();
        }
    }
}
