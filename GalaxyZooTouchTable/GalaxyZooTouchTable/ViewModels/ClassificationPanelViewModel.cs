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

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// User needing help:
        /// DeclinedHelp: Shows in center that cooperating user has declined to help
        /// AcceptedHelp: Shows in center that cooperating user has accepted help
        /// AnswerGiven: Shows in center that cooperating user has answered and answer in left panel
        /// HelpRequestSent: Shows in center that request was sent
        /// 
        /// User helping:
        /// HelpRequetReceived: Opens panel with accept or decline buttons
        /// HelpingUser: Shows circle around user that is being helped
        /// </summary>
        enum HelpStatus
        {
            ClearNotifications,
            HelpRequestReceived,
            DeclinedHelp,
            AcceptedHelp,
            HelpRequestSent,
            HelpingUser,
            AnswerGiven
        }

        private const int SUBJECT_VIEW = 0;
        private const int SUMMARY_VIEW = 1;

        public ObservableCollection<TableUser> AllUsers { get; set; }
        public int ClassificationsThisSession { get; set; } = 0;
        public List<AnswerButton> CurrentAnswers { get; set; }
        public Classification CurrentClassification { get; set; }
        public Subject CurrentSubject { get; set; }
        public WorkflowTask CurrentTask { get; set; }
        public string CurrentTaskIndex { get; set; }
        public GraphQLHttpClient GraphQLClient { get; set; } = new GraphQLHttpClient(Config.CaesarHost);
        public LevelerViewModel LevelerViewModel { get; set; } = new LevelerViewModel();
        public ExamplesPanelViewModel ExamplesViewModel { get; set; } = new ExamplesPanelViewModel();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public TableUser User { get; set; }
        public Workflow Workflow { get; }

        public ICommand CloseClassifier { get; set; }
        public ICommand ContinueClassification { get; set; }
        public ICommand NotifyUser { get; set; }
        public ICommand OpenClassifier { get; set; }
        public ICommand SelectAnswer { get; set; }
        public ICommand CloseNotifier { get; set; }
        public ICommand ShowCloseConfirmation { get; set; }
        public ICommand DeclineGalaxy { get; set; }
        public ICommand AcceptGalaxy { get; set; }
        public ICommand ResetNotifications { get; set; }
        public ICommand ToggleButtonNotification { get; set; }

        private Classification _suggestedClassification;
        public Classification SuggestedClassification
        {
            get { return _suggestedClassification; }
            set
            {
                _suggestedClassification = value;
                OnPropertyRaised("SuggestedClassification");
            }
        }

        private int _notificationStatus = 0;
        public int NotificationStatus
        {
            get { return _notificationStatus; }
            set
            {
                _notificationStatus = value;
                OnPropertyRaised("NotificationStatus");
            }
        }

        private bool _hideButtonNotification = false;
        public bool HideButtonNotification
        {
            get { return _hideButtonNotification; }
            set
            {
                _hideButtonNotification = value;
                OnPropertyRaised("HideButtonNotification");
            }
        }

        private bool _openNotifier { get; set; } = false;
        public bool OpenNotifier
        {
            get { return _openNotifier; }
            set
            {
                _openNotifier = value;
                OnPropertyRaised("OpenNotifier");
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

        private int _currentView = SUBJECT_VIEW;
        public int CurrentView
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

        private ClassificationPanelViewModel _cooperatingClassifier;
        public ClassificationPanelViewModel CooperatingClassifier
        {
            get { return _cooperatingClassifier; }
            set
            {
                _cooperatingClassifier = value;
                OnPropertyRaised("CooperatingClassifier");
            }
        }

        private AnswerButton _selectedItem;
        public AnswerButton SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (value != null)
                {
                    ChooseAnswer(value);
                }
                OnPropertyRaised("SelectedItem");
            }
        }

        private ObservableCollection<TableUser> _helpfulUsers;
        public ObservableCollection<TableUser> HelpfulUsers
        {
            get { return _helpfulUsers; }
            set
            {
                _helpfulUsers = value;
                OnPropertyRaised("HelpfulUsers");
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
            user.Classifier = this;
            CurrentTask = workflow.Tasks[workflow.FirstTask];
            CurrentTaskIndex = workflow.FirstTask;
            FilterCurrentUser(allUsers);
            AllUsers = allUsers;
            LevelerViewModel = new LevelerViewModel(user);
            Messenger.Default.Register<ClassificationPanelViewModel>(this, OnHelpRequested, user);

            if (CurrentTask.Answers != null)
            {
                CurrentAnswers = ParseTaskAnswers(CurrentTask.Answers);
            }
        }

        private void OnHelpRequested(ClassificationPanelViewModel Classifier)
        {
            OpenNotifier = true;
            CooperatingClassifier = Classifier;

            Classifier.NotificationStatus = (int)HelpStatus.HelpRequestSent;
            NotificationStatus = (int)HelpStatus.HelpRequestReceived;
            //Classifier.User.CoClassification = true;
        }

        private void FilterCurrentUser(ObservableCollection<TableUser> allUsers)
        {
            ObservableCollection<TableUser> allUsersCopy = new ObservableCollection<TableUser>(allUsers);
            foreach (TableUser tableUser in allUsersCopy)
            {
                if (User == tableUser)
                {
                    allUsersCopy.Remove(User);
                    break;
                }
            }
            HelpfulUsers = allUsersCopy;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

        private void LoadCommands()
        {
            ContinueClassification = new CustomCommand(OnContinueClassification);
            ShowCloseConfirmation = new CustomCommand(ToggleCloseConfirmation);
            CloseClassifier = new CustomCommand(OnCloseClassifier);
            NotifyUser = new CustomCommand(OnNotifyUser);
            OpenClassifier = new CustomCommand(OnOpenClassifier);
            CloseNotifier = new CustomCommand(OnCloseNotifier);
            DeclineGalaxy = new CustomCommand(OnDeclineGalaxy);
            AcceptGalaxy = new CustomCommand(OnAcceptGalaxy);
            ResetNotifications = new CustomCommand(OnResetNotifications);
            ToggleButtonNotification = new CustomCommand(OnToggleButtonNotification);
        }

        private void OnToggleButtonNotification(object sender)
        {
            HideButtonNotification = !HideButtonNotification;
        }

        private void OnAcceptGalaxy(object sender)
        {
            OpenNotifier = false;

            // Let cooperating user know of acceptance
            CooperatingClassifier.NotificationStatus = (int)HelpStatus.AcceptedHelp;

            // You're helping now
            NotificationStatus = (int)HelpStatus.HelpingUser;

            // Additional functionality is needed to fetch the right subject and start a new classification
            string NewSubjectID = CooperatingClassifier.CurrentSubject.Id;
            GetSubjectById(NewSubjectID);
        }

        private async void GetSubjectById(string subjectID)
        {
            ApiClient client = new ApiClient();
            CurrentSubject = await client.Subjects.Get(subjectID);
            StartNewClassification(CurrentSubject);
            SubjectImageSource = CurrentSubject.GetSubjectLocation();
            GraphQLRequest();
        }

        private void OnDeclineGalaxy(object sender)
        {
            OpenNotifier = false;

            // Let cooperating user know of declination
            CooperatingClassifier.NotificationStatus = (int)HelpStatus.DeclinedHelp;
            CooperatingClassifier.User.CoClassification = false;

            // You're single now
            NotificationStatus = (int)HelpStatus.ClearNotifications;
            CooperatingClassifier = null;
            User.CoClassification = false;
        }

        private void OnResetNotifications(object sender)
        {
            NotificationStatus = (int)HelpStatus.ClearNotifications;
            SuggestedClassification = null;
            CooperatingClassifier = null;
        }

        private void OnCloseNotifier(object sender)
        {
            OpenNotifier = false;
        }

        private void OnNotifyUser(object sender)
        {
            TableUser UserToNotify = sender as TableUser;

            // These users are busy now
            UserToNotify.CoClassification = true;
            User.CoClassification = true;

            // Who are you speaking to?
            CooperatingClassifier = UserToNotify.Classifier;

            // What is your status?
            NotificationStatus = (int)HelpStatus.HelpRequestReceived;
            Messenger.Default.Send<ClassificationPanelViewModel>(this, UserToNotify);
        }

        private void OnOpenClassifier(object sender)
        {
            ClassifierOpen = true;
            User.Active = true;
        }

        private void OnCloseClassifier(object sender)
        {
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
            CurrentView = SUBJECT_VIEW;
            TotalVotes = 0;
        }

        private void ToggleCloseConfirmation(object sender)
        {
            CloseConfirmationVisible = !CloseConfirmationVisible;
        }

        private async void OnContinueClassification(object sender)
        {
            if (CurrentView == SUBJECT_VIEW)
            {
                CurrentClassification.Metadata.FinishedAt = System.DateTime.Now.ToString();
                CurrentClassification.Annotations.Add(CurrentAnnotation);
                ApiClient client = new ApiClient();
                //await client.Classifications.Create(CurrentClassification);
                SelectedItem.AnswerCount += 1;
                TotalVotes += 1;
                ClassificationsThisSession += 1;
                Messenger.Default.Send<int>(ClassificationsThisSession, User);
                CurrentView = SUMMARY_VIEW;
            }
            else
            {
                PrepareForNewClassification();
            }
        }

        private void ChooseAnswer(AnswerButton button)
        {
            CurrentAnnotation = new Annotation(CurrentTaskIndex, button.Index);
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
    }
}
