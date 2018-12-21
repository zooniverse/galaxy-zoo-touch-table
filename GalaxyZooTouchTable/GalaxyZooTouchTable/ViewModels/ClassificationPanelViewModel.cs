using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using PanoptesNetClient;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TableUser> AllUsers { get; set; }
        public DispatcherTimer StillThereTimer { get; set; } = new DispatcherTimer();
        public int ClassificationsThisSession { get; set; } = 0;
        public List<AnswerButton> CurrentAnswers { get; set; }
        public Classification CurrentClassification { get; set; }
        public Subject CurrentSubject { get; set; }
        public WorkflowTask CurrentTask { get; set; }
        public string CurrentTaskIndex { get; set; }
        public GraphQLHttpClient GraphQLClient { get; set; } = new GraphQLHttpClient(Config.CaesarHost);
        public LevelerViewModel LevelerViewModel { get; set; } = new LevelerViewModel();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public ExamplesPanelViewModel ExamplesViewModel { get; set; } = new ExamplesPanelViewModel();
        public TableUser User { get; set; }
        public Workflow Workflow { get; }

        public ICommand CloseClassifier { get; set; }
        public ICommand ContinueClassification { get; set; }
        public ICommand OpenClassifier { get; set; }
        public ICommand SelectAnswer { get; set; }
        public ICommand ShowCloseConfirmation { get; set; }

        private NotificationsViewModel _notifications;
        public NotificationsViewModel Notifications
        {
            get { return _notifications; }
            set
            {
                _notifications = value;
                OnPropertyRaised("Notifications");
            }
        }

        private int _totalVotes = 0;
        public int TotalVotes
        {
            get { return _totalVotes; }
            set
            {
                _totalVotes = value;
                OnPropertyRaised("TotalVotes");
            }
        }

        private ClassifierViewEnum _currentView = ClassifierViewEnum.SubjectView;
        public ClassifierViewEnum CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyRaised("CurrentView");
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

        private bool _classifierOpen = false;
        public bool ClassifierOpen
        {
            get { return _classifierOpen; }
            set
            {
                _classifierOpen = value;
                OnPropertyRaised("ClassifierOpen");
            }
        }

        private Annotation _currentAnnotation;
        public Annotation CurrentAnnotation
        {
            get { return _currentAnnotation; }
            set
            {
                _currentAnnotation = value;
                CanSendClassification = value != null;
                OnPropertyRaised("CurrentAnnotation");
            }
        }

        private bool _canSendClassification = false;
        public bool CanSendClassification
        {
            get { return _canSendClassification; }
            set
            {
                _canSendClassification = value;
                OnPropertyRaised("CanSendClassification");
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

        private AnswerButton _selectedAnswer;
        public AnswerButton SelectedAnswer
        {
            get { return _selectedAnswer; }
            set
            {
                _selectedAnswer = value;
                if (value != null)
                {
                    ChooseAnswer(value);
                }
                OnPropertyRaised("SelectedAnswer");
            }
        }

        public ClassificationPanelViewModel(Workflow workflow, TableUser user, ObservableCollection<TableUser> allUsers)
        {
            if (workflow != null)
            {
                PrepareForNewClassification();
            }
            LoadCommands();
            Workflow = workflow;
            User = user;
            CurrentTask = workflow.Tasks[workflow.FirstTask];
            CurrentTaskIndex = workflow.FirstTask;
            Notifications = new NotificationsViewModel(user, allUsers, this);
            LevelerViewModel = new LevelerViewModel(user);
            ExamplesViewModel.PropertyChanged += ResetTimer;
            LevelerViewModel.PropertyChanged += ResetTimer;

            if (CurrentTask.Answers != null)
            {
                CurrentAnswers = ParseTaskAnswers(CurrentTask.Answers);
            }
        }

        private void LoadCommands()
        {
            CloseClassifier = new CustomCommand(OnCloseClassifier);
            ContinueClassification = new CustomCommand(OnContinueClassification);
            OpenClassifier = new CustomCommand(OnOpenClassifier);
            SelectAnswer = new CustomCommand(OnSelectAnswer);
            ShowCloseConfirmation = new CustomCommand(ToggleCloseConfirmation);
        }

        private void OnSelectAnswer(object sender)
        {
            AnswerButton Button = sender as AnswerButton;
            SelectedAnswer = Button;
            CurrentAnnotation = new Annotation(CurrentTaskIndex, Button.Index);
        }

        public async void GetSubjectById(string subjectID)
        {
            TotalVotes = 0;
            ApiClient client = new ApiClient();
            CurrentSubject = await client.Subjects.Get(subjectID);
            StartNewClassification(CurrentSubject);
            SubjectImageSource = CurrentSubject.GetSubjectLocation();
            GraphQLRequest();
        }

        private void OnOpenClassifier(object sender)
        {
            StartTimer();
            ClassifierOpen = true;
            User.Active = true;
        }

        private void OnCloseClassifier(object sender)
        {
            Notifications.ClearNotifications(true);
            StopTimer();
            LevelerViewModel.IsOpen = false;
            ExamplesViewModel.IsOpen = true;
            ExamplesViewModel.SelectedExample = null;
            PrepareForNewClassification();
            ClassifierOpen = false;
            CloseConfirmationVisible = false;
            User.Active = false;
        }

        private void PrepareForNewClassification()
        {
            GetSubject();
            CurrentView = ClassifierViewEnum.SubjectView;
            TotalVotes = 0;
        }

        private void ToggleCloseConfirmation(object sender)
        {
            CloseConfirmationVisible = !CloseConfirmationVisible;
        }

        private async void OnContinueClassification(object sender)
        {
            if (CurrentView == ClassifierViewEnum.SubjectView)
            {
                CurrentClassification.Metadata.FinishedAt = System.DateTime.Now.ToString();
                CurrentClassification.Annotations.Add(CurrentAnnotation);
                ApiClient client = new ApiClient();
                await client.Classifications.Create(CurrentClassification);
                SelectedAnswer.AnswerCount += 1;
                TotalVotes += 1;
                ClassificationsThisSession += 1;
                Messenger.Default.Send<int>(ClassificationsThisSession, $"{User.Name}_IncrementCount");
                CurrentView = ClassifierViewEnum.SummaryView;
                HandleNotificationsOnSubmit();
            }
            else
            {
                PrepareForNewClassification();
            }
        }

        private void HandleNotificationsOnSubmit()
        {
            if (User.Status == NotificationStatus.HelpingUser)
            {
                Notifications.SendAnswerToUser(SelectedAnswer);
            }

            if (User.Status != NotificationStatus.HelpRequestReceived &&
                User.Status != NotificationStatus.HelpRequestSent &&
                User.Status != NotificationStatus.AcceptedHelp)
            {
                Notifications.ClearNotifications();
            }
        }

        private void StartTimer()
        {
            StillThereTimer.Tick += new System.EventHandler(ShowStillThereModal);
            ResetTimer();
        }

        private void StopTimer()
        {
            StillThereTimer.Stop();
        }

        private void ShowStillThereModal(object sender, System.EventArgs e)
        {
            System.Console.WriteLine("Show Modal Now");
        }

        private void ChooseAnswer(AnswerButton button)
        {
            CurrentAnnotation = new Annotation(CurrentTaskIndex, button.Index);
        }

        private void ResetTimer()
        {
            StillThereTimer.Interval = new System.TimeSpan(0, 0, 5);
            StillThereTimer.Start();
        }

        private void ResetTimer(object sender, PropertyChangedEventArgs e)
        {
            ResetTimer();
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
            CurrentClassification.Metadata.StartedAt = System.DateTime.Now.ToString();
            CurrentClassification.Metadata.UserAgent = "Galaxy Zoo Touch Table";
            CurrentClassification.Metadata.UserLanguage = "en";

            CurrentClassification.Links = new ClassificationLinks(Config.ProjectId, Config.WorkflowId);
            CurrentClassification.Links.Subjects.Add(subject.Id);

            CurrentAnnotation = null;
            SelectedAnswer = null;
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
            GraphQLRequest();
        }

        private void ResetAnswerCount()
        {
            foreach (AnswerButton Answer in CurrentAnswers)
            {
                Answer.AnswerCount = 0;
            }
        }

        private async void GraphQLRequest()
        {
            var answersRequest = new GraphQLRequest
            {
                Query = @"
                    query AnswerCount($workflowId: ID!, $subjectId: ID!) {
                      workflow(id: $workflowId) {
                        subject_reductions(subjectId: $subjectId, reducerKey: T0_Stats) {
                            data
                        }
                      }
                    }",
                OperationName = "AnswerCount",
                Variables = new
                {
                    workflowId = Workflow.Id,
                    subjectId = CurrentSubject.Id
                }
            };

            ResetAnswerCount();
            GraphQLResponse response = new GraphQLResponse();

            try {
                response = await GraphQLClient.SendQueryAsync(answersRequest);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Graph QL Error: {0}", e.Message);
            }

            if (response.Data != null)
            {
                GetReductions(response);
            }
        }

        private void GetReductions(GraphQLResponse response)
        {
            var reductions = response.Data.workflow.subject_reductions;

            if (reductions.Count > 0)
            {
                var data = reductions.First.data;
                foreach (var count in data)
                {
                    var index = System.Convert.ToInt32(count.Name);
                    AnswerButton Answer = CurrentAnswers[index];


                    int answerCount = (int)count.Value;
                    Answer.AnswerCount = answerCount;

                    TotalVotes += answerCount;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (StillThereTimer != null && StillThereTimer.IsEnabled)
            {
                ResetTimer();
            }
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
