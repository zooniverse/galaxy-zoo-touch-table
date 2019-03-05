using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Utility;
using GraphQL.Common.Response;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel : ViewModelBase
    {
        private IGraphQLService _graphQLService;
        private IPanoptesService _panoptesService;
        private string CurrentTaskIndex { get; set; }
        private DispatcherTimer StillThereTimer { get; set; } = new DispatcherTimer();

        public Classification CurrentClassification { get; set; } = new Classification();
        public Subject CurrentSubject { get; set; }
        public Workflow Workflow { get; set; }
        public TableUser User { get; set; }

        public int ClassificationsThisSession { get; set; } = 0;
        public ICommand ChooseAnotherGalaxy { get; set; }
        public ICommand CloseClassifier { get; private set; }
        public ICommand ContinueClassification { get; private set; }
        public ICommand OpenClassifier { get; private set; }
        public ICommand SelectAnswer { get; private set; }
        public ICommand ShowCloseConfirmation { get; private set; }
        public List<Subject> Subjects { get; set; } = new List<Subject>();

        public NotificationsViewModel Notifications { get; private set; }
        public ExamplesPanelViewModel ExamplesViewModel { get; private set; }
        public LevelerViewModel LevelerViewModel { get; private set; }

        private List<AnswerButton> _currentAnswers;
        public List<AnswerButton> CurrentAnswers
        {
            get => _currentAnswers;
            set => SetProperty(ref _currentAnswers, value);
        }

        private StillThereViewModel _stillThere;
        public StillThereViewModel StillThere
        {
            get => _stillThere;
            set => SetProperty(ref _stillThere, value);
        }

        private int _totalVotes = 0;
        public int TotalVotes
        {
            get => _totalVotes;
            set => SetProperty(ref _totalVotes, value);
        }

        private ClassifierViewEnum _currentView = ClassifierViewEnum.SubjectView;
        public ClassifierViewEnum CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        } 

        private SubjectViewEnum _subjectView = SubjectViewEnum.DragSubject;
        public SubjectViewEnum SubjectView
        {
            get => _subjectView;
            set
            {
                Messenger.Default.Send<SubjectViewEnum>(value, $"{User.Name}_SubjectStatus");
                AllowSelection = value == SubjectViewEnum.MatchedSubject;
                SetProperty(ref _subjectView, value);
            }
        }

        private bool _closeConfirmationVisible = false;
        public bool CloseConfirmationVisible
        {
            get => _closeConfirmationVisible;
            set => SetProperty(ref _closeConfirmationVisible, value);
        }

        private bool _classifierOpen = false;
        public bool ClassifierOpen
        {
            get => _classifierOpen;
            set => SetProperty(ref _classifierOpen, value);
        }

        private Annotation _currentAnnotation;
        public Annotation CurrentAnnotation
        {
            get => _currentAnnotation;
            set
            {
                CanSendClassification = value != null;
                SetProperty(ref _currentAnnotation, value);
            }
        }

        private bool _canSendClassification = false;
        public bool CanSendClassification
        {
            get => _canSendClassification;
            set => SetProperty(ref _canSendClassification, value);
        }

        private string _subjectImageSource;
        public string SubjectImageSource
        {
            get => _subjectImageSource;
            set => SetProperty(ref _subjectImageSource, value);
        }

        private AnswerButton _selectedAnswer;
        public AnswerButton SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                if (value != null)
                {
                    ChooseAnswer(value);
                }
                SetProperty(ref _selectedAnswer, value);
            }
        }

        private bool _allowSelection;
        public bool AllowSelection
        {
            get => _allowSelection;
            set => SetProperty(ref _allowSelection, value);
        }

        public ClassificationPanelViewModel(IPanoptesService panoptesService, IGraphQLService graphQLService, TableUser user)
        {
            _panoptesService = panoptesService;
            _graphQLService = graphQLService;
            User = user;

            ExamplesViewModel = new ExamplesPanelViewModel();
            LevelerViewModel = new LevelerViewModel(User);
            Notifications = new NotificationsViewModel(User);
            StillThere = new StillThereViewModel();

            LoadCommands();
            AddSubscribers();
            SetTimer();
        }

        private void AddSubscribers()
        {
            ExamplesViewModel.PropertyChanged += ResetStillThereModalTimer;
            LevelerViewModel.PropertyChanged += ResetStillThereModalTimer;
            Notifications.GetSubjectById += OnGetSubjectById;
            Notifications.ChangeView += OnChangeView;
            Notifications.SendRequestToUser += OnSendRequestToUser;
            StillThere.ResetFiveMinuteTimer += StartStillThereModalTimer;
            StillThere.CloseClassificationPanel += OnCloseClassifier;
        }

        public async void Load()
        {
            await GetWorkflow();
            PrepareForNewClassification();
        }

        private void OnSendRequestToUser(TableUser UserToNotify)
        {
            NotificationRequest Request = new NotificationRequest(User, CurrentSubject.Id);
            Messenger.Default.Send<NotificationRequest>(Request, $"{UserToNotify.Name}_ReceivedNotification");
        }

        public async Task GetWorkflow()
        {
            Workflow = await _panoptesService.GetWorkflowAsync(Config.WorkflowId);
            WorkflowTask CurrentTask = Workflow.Tasks[Workflow.FirstTask];
            CurrentTaskIndex = Workflow.FirstTask;
            CurrentAnswers = ParseTaskAnswers(CurrentTask.Answers);
        }

        private void LoadCommands()
        {
            ChooseAnotherGalaxy = new CustomCommand(OnChooseAnotherGalaxy);
            CloseClassifier = new CustomCommand(OnCloseClassifier);
            ContinueClassification = new CustomCommand(OnContinueClassification);
            OpenClassifier = new CustomCommand(OnOpenClassifier);
            SelectAnswer = new CustomCommand(OnSelectAnswer);
            ShowCloseConfirmation = new CustomCommand(ToggleCloseConfirmation);
        }

        private void OnChooseAnotherGalaxy(object sender)
        {
            SubjectView = SubjectViewEnum.DragSubject;
            PrepareForNewClassification();
        }

        private void OnSelectAnswer(object sender)
        {
            AnswerButton Button = sender as AnswerButton;
            SelectedAnswer = Button;
        }

        public async void OnGetSubjectById(string subjectID)
        {
            TotalVotes = 0;
            Subject newSubject = await _panoptesService.GetSubjectAsync(subjectID);
            Subjects.Insert(0, newSubject);
            GetSubjectQueue();
            SubjectView = SubjectViewEnum.MatchedSubject;
        }

        private void OnOpenClassifier(object sender)
        {
            StartStillThereModalTimer();
            ClassifierOpen = true;
            User.Active = true;
            LevelerViewModel = new LevelerViewModel(User);
        }

        private void OnCloseClassifier(object sender)
        {
            SubjectView = SubjectViewEnum.DragSubject;
            bool UserIsLeaving = true;
            LevelerViewModel.CloseLeveler();
            ExamplesViewModel.ResetExamples();
            PrepareForNewClassification();
            Notifications.ClearNotifications(UserIsLeaving);

            ClassifierOpen = false;
            CloseConfirmationVisible = false;
            User.Active = false;
        }

        private void PrepareForNewClassification()
        {
            GetSubjectQueue();
            OnChangeView(ClassifierViewEnum.SubjectView);
            TotalVotes = 0;
        }

        private void ToggleCloseConfirmation(object sender)
        {
            CloseConfirmationVisible = !CloseConfirmationVisible;
        }

        public void OnChangeView(ClassifierViewEnum view)
        {
            CurrentView = view;
        }

        private async void OnContinueClassification(object sender)
        {
            if (CurrentView == ClassifierViewEnum.SubjectView)
            {
                CurrentClassification.Metadata.FinishedAt = System.DateTime.Now.ToString();
                CurrentClassification.Annotations.Add(CurrentAnnotation);
                await _panoptesService.CreateClassificationAsync(CurrentClassification);
                SelectedAnswer.AnswerCount += 1;
                TotalVotes += 1;
                ClassificationsThisSession += 1;
                LevelerViewModel.OnIncrementCount(ClassificationsThisSession);
                OnChangeView(ClassifierViewEnum.SummaryView);
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

        private void SetTimer()
        {
            StillThereTimer.Tick += new System.EventHandler(ShowStillThereModal);
            StillThereTimer.Interval = new System.TimeSpan(0, 0, 10);
            StartStillThereModalTimer();
        }

        public void StartStillThereModalTimer()
        {
            StillThereTimer.Stop();
            StillThereTimer.Start();
            if (StillThere.Visible) { StillThere.Visible = false; }
        }

        private void ShowStillThereModal(object sender, System.EventArgs e)
        {
            StillThereTimer.Stop();
            StillThere.Visible = true;
        }

        public void ChooseAnswer(AnswerButton button)
        {
            CurrentAnnotation = new Annotation(CurrentTaskIndex, button.Index);
        }

        private void ResetStillThereModalTimer(object sender, PropertyChangedEventArgs e)
        {
            StartStillThereModalTimer();
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

        public async void GetSubjectQueue()
        {
            if (Subjects.Count <= 0)
            {
                NameValueCollection query = new NameValueCollection
                {
                    { "workflow_id", Config.WorkflowId }
                };
                Subjects = await _panoptesService.GetSubjectsAsync("queued", query);
            }
            if (Subjects.Count > 0)
            {
                CurrentSubject = Subjects[0];
                StartNewClassification(CurrentSubject);
                Subjects.RemoveAt(0);

                if (CurrentSubject != null && CurrentSubject.Locations != null)
                {
                    SubjectImageSource = CurrentSubject.GetSubjectLocation();
                    GetSubjectReductions();
                }
            }
        }

        public void DropSubject(TableSubject subject)
        {
            TotalVotes = 0;
            Subjects.Insert(0, subject.Subject);
            GetSubjectQueue();
            SubjectView = SubjectViewEnum.MatchedSubject;
        }

        public void ResetAnswerCount()
        {
            foreach (AnswerButton Answer in CurrentAnswers)
            {
                Answer.AnswerCount = 0;
            }
        }

        private async void GetSubjectReductions()
        {
            GraphQLResponse response = await _graphQLService.GetReductionAsync(Workflow, CurrentSubject);

            if (response != null && response.Data != null)
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
}
