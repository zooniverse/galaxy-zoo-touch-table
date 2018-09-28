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
        public int ClassificationsThisSession { get; set; } = 0;
        public UserConsole Console { get; set; }
        public List<AnswerButton> CurrentAnswers { get; set; }
        public Classification CurrentClassification { get; set; }
        public Subject CurrentSubject { get; set; }
        public WorkflowTask CurrentTask { get; set; }
        public string CurrentTaskIndex { get; set; }
        public LevelerViewModel LevelerVM { get; set; } = new LevelerViewModel();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public TableUser User { get; set; }
        public Workflow Workflow { get; }

        public ICommand CloseConfirmationBox { get; set; }
        public ICommand EndSession { get; set; }
        public ICommand SelectAnswer { get; set; }
        public ICommand ShowCloseConfirmation { get; set; }
        public ICommand SubmitClassification { get; set; }

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
            CurrentClassification.Metadata.FinishedAt = DateTime.Now.ToString();
            CurrentClassification.Annotations.Add(CurrentAnnotation);
            ApiClient client = new ApiClient();
            await client.Classifications.Create(CurrentClassification);
            ClassificationsThisSession += 1;
            Messenger.Default.Send<int>(ClassificationsThisSession, User);
            GetSubject();
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
    }
}
