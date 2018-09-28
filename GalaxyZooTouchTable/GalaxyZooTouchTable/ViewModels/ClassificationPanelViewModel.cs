using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using PanoptesNetClient;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel : INotifyPropertyChanged
    {
        public UserConsole Console { get; set; }
        public List<AnswerButton> CurrentAnswers { get; set; }
        public Classification CurrentClassification { get; set; }
        public Subject CurrentSubject { get; set; }
        public WorkflowTask CurrentTask { get; set; }
        public string CurrentTaskIndex { get; set; }
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public TableUser User { get; set; }
        public Workflow Workflow { get; }

        public ICommand CloseConfirmationBox { get; set; }
        public ICommand EndSession { get; set; }
        public ICommand SelectAnswer { get; set; }
        public ICommand ShowCloseConfirmation { get; set; }
        public ICommand SubmitClassification { get; set; }
        public ICommand ToggleLeveler { get; set; }

        public LevelerViewModel LevelerVM { get; set; } = new LevelerViewModel();

        private double _levelTwoOpacity { get; set; } = 0.5;
        public double LevelTwoOpacity
        {
            get { return _levelTwoOpacity; }
            set
            {
                _levelTwoOpacity = value;
                OnPropertyRaised("LevelTwoOpacity");
            }
        }

        private double _levelThreeOpacity { get; set; } = 0.5;
        public double LevelThreeOpacity
        {
            get { return _levelThreeOpacity; }
            set
            {
                _levelThreeOpacity = value;
                OnPropertyRaised("LevelThreeOpacity");
            }
        }

        private double _levelFourOpacity { get; set; } = 0.5;
        public double LevelFourOpacity
        {
            get { return _levelFourOpacity; }
            set
            {
                _levelFourOpacity = value;
                OnPropertyRaised("LevelFourOpacity");
            }
        }

        private double _levelFiveOpacity { get; set; } = 0.5;
        public double LevelFiveOpacity
        {
            get { return _levelFiveOpacity; }
            set
            {
                _levelFiveOpacity = value;
                OnPropertyRaised("LevelFiveOpacity");
            }
        }

        private string _classificationLevel { get; set; } = "One";
        public string ClassificationLevel
        {
            get { return _classificationLevel; }
            set
            {
                _classificationLevel = value;
                OnPropertyRaised("ClassificationLevel");
            }
        }

        private int _classificationsUntilUpgrade { get; set; } = 5;
        public int ClassificationsUntilUpgrade
        {
            get { return _classificationsUntilUpgrade; }
            set
            {
                if (value == 0)
                {
                    value = 5;
                    LevelUp();
                }
                _classificationsUntilUpgrade = value;
                OnPropertyRaised("ClassificationsUntilUpgrade");
            }
        }

        private int _classificationsThisSession = 0;
        public int ClassificationsThisSession
        {
            get { return _classificationsThisSession; }
            set
            {
                if (ClassificationLevel != "Five")
                {
                    ClassificationsUntilUpgrade -= 1;
                }
                _classificationsThisSession = value;
                OnPropertyRaised("ClassificationsThisSession");
            }
        }

        private bool _levelerIsOpen = false;
        public bool LevelerIsOpen
        {
            get { return _levelerIsOpen; }
            set
            {
                _levelerIsOpen = value;
                OnPropertyRaised("LevelerIsOpen");
            }
        }

        private bool _closeConfirmationVisible = false;
        public bool CloseConfirmationVisible
        {
            get { return _closeConfirmationVisible; }
            set
            {
                _closeConfirmationVisible = value;
                OnPropertyRaised("CloseConfirmationVisible");
            }
        }

        private Annotation _currentAnnotation;
        public Annotation CurrentAnnotation
        {
            get { return _currentAnnotation; }
            set
            {
                _currentAnnotation = value;
                OnPropertyRaised("CurrentAnnotation");
            }
        }

        private string _subjectImageSource;
        public string SubjectImageSource
        {
            get { return _subjectImageSource; }
            set
            {
                _subjectImageSource = value;
                OnPropertyRaised("SubjectImageSource");
            }
        }

        private AnswerButton _selectedItem;
        public AnswerButton SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyRaised("SelectedItem");
            }
        }

        public ClassificationPanelViewModel(Workflow workflow, UserConsole console, TableUser user)
        {
            GetSubject();
            LoadCommands();
            Workflow = workflow;
            User = user;
            CurrentTask = workflow.Tasks[workflow.FirstTask];
            CurrentTaskIndex = workflow.FirstTask;
            Console = console;
            LevelerVM = new LevelerViewModel(user);

            if (CurrentTask.Answers != null)
            {
                CurrentAnswers = ParseTaskAnswers(CurrentTask.Answers);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

        private void LoadCommands()
        {
            SelectAnswer = new CustomCommand(ChooseAnswer, CanSelectAnswer);
            SubmitClassification = new CustomCommand(SendClassification, CanSendClassification);
            ShowCloseConfirmation = new CustomCommand(ToggleCloseConfirmation);
            EndSession = new CustomCommand(CloseClassifier);
            ToggleLeveler = new CustomCommand(SlideLeveler);
        }

        private void SlideLeveler(object sender)
        {
            LevelerIsOpen = !LevelerIsOpen;
        }

        private async void CloseClassifier(object sender)
        {
            await Console.Classifier.MoveClassifier();
            Console.MoveButton();
            Console.ClassifierOpen = !Console.ClassifierOpen;
        }

        private void ToggleCloseConfirmation(object sender)
        {
            CloseConfirmationVisible = !CloseConfirmationVisible;
        }

        private async void SendClassification(object sender)
        {
            //CurrentClassification.Metadata.FinishedAt = DateTime.Now.ToString();
            //CurrentClassification.Annotations.Add(CurrentAnnotation);
            //ApiClient client = new ApiClient();
            //await client.Classifications.Create(CurrentClassification);
            System.Console.WriteLine(ClassificationsThisSession);
            ClassificationsThisSession += 1;
            Messenger.Default.Send<int>(ClassificationsThisSession);
            //GetSubject();
        }

        private bool CanSendClassification(object sender)
        {
            return CurrentAnnotation != null;
        }

        private void ChooseAnswer(object sender)
        {
            AnswerButton button = sender as AnswerButton;
            SelectedItem = button;
            CurrentAnnotation = new Annotation(CurrentTaskIndex, button.Index);
        }

        private bool CanSelectAnswer(object sender)
        {
            return CurrentAnswers.Count > 0;
        }

        public List<AnswerButton> ParseTaskAnswers(List<TaskAnswer> answers)
        {
            List<AnswerButton> renderedAnswers = new List<AnswerButton>();

            for (int index = 0; index < answers.Count; index++)
            {
                AnswerButton item = new AnswerButton(answers[index], index);
                renderedAnswers.Add(item);
            }
            return renderedAnswers;
        }

        public void StartNewClassification(Subject subject)
        {
            CurrentClassification = new Classification();
            CurrentClassification.Metadata.WorkflowVersion = Workflow.Version;
            CurrentClassification.Metadata.StartedAt = DateTime.Now.ToString();
            CurrentClassification.Metadata.UserAgent = "Galaxy Zoo Touch Table";
            CurrentClassification.Metadata.UserLanguage = "en";

            CurrentClassification.Links = new ClassificationLinks(Config.ProjectId, Config.WorkflowId);
            CurrentClassification.Links.Subjects.Add(subject.Id);

            CurrentAnnotation = null;
            SelectedItem = null;
            CommandManager.InvalidateRequerySuggested();
        }

        private async void GetSubject()
        {
            if (Subjects.Count <= 0)
            {
                ApiClient client = new ApiClient();
                NameValueCollection query = new NameValueCollection
                {
                    { "workflow_id", Config.WorkflowId }
                };
                Subjects = await client.Subjects.GetList("queued", query);
            }
            CurrentSubject = Subjects[0];
            StartNewClassification(CurrentSubject);
            SubjectImageSource = CurrentSubject.GetSubjectLocation();
            Subjects.RemoveAt(0);
        }

        private void LevelUp()
        {
            if (ClassificationsThisSession > 25)
            {
                return;
            }
            switch (ClassificationsThisSession)
            {
                case int n when (n <= 5):
                    ClassificationLevel = "Two";
                    LevelTwoOpacity = 1;
                    break;
                case int n when (n <= 10):
                    ClassificationLevel = "Three";
                    LevelThreeOpacity = 1;
                    break;
                case int n when (n <= 15):
                    ClassificationLevel = "Four";
                    LevelFourOpacity = 1;
                    break;
                case int n when (n <= 20):
                    ClassificationLevel = "Five";
                    LevelFiveOpacity = 1;
                    break;
                default:
                    ClassificationLevel = "One";
                    break;
            }
        }
    }
}
