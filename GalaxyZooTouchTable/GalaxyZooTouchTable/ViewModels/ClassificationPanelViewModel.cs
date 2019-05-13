using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Utility;
using GraphQL.Common.Response;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel : ViewModelBase
    {
        private IGraphQLService _graphQLService;
        private IPanoptesService _panoptesService;
        private TableSubject CurrentGalaxy { get; set; }
        private ILocalDBService _localDBService;

        private string CurrentTaskIndex { get; set; }
        private DispatcherTimer StillThereTimer { get; set; } = new DispatcherTimer();

        public Classification CurrentClassification { get; set; } = new Classification();
        public Workflow Workflow { get; set; }
        public TableUser User { get; set; }
        private List<CompletedClassification> CompletedClassifications { get; set; } = new List<CompletedClassification>();

        public int ClassificationsThisSession { get; set; } = 0;
        public ICommand ChooseAnotherGalaxy { get; set; }
        public ICommand CloseClassifier { get; private set; }
        public ICommand ContinueClassification { get; private set; }
        public ICommand OpenClassifier { get; private set; }
        public ICommand SelectAnswer { get; private set; }
        public ICommand ShowCloseConfirmation { get; private set; }
        public List<TableSubject> Subjects { get; set; } = new List<TableSubject>();

        private TableSubject _currentSubject;
        public TableSubject CurrentSubject
        {
            get => _currentSubject;
            set
            {
                if (CurrentSubject != null)
                {
                    TableSubject NewSubject = value as TableSubject;
                    Notifications.ReceivedNewSubject(NewSubject);
                }
                _currentSubject = value;
            }
        }

        public CloseConfirmationViewModel CloseConfirmationViewModel { get; private set; } = new CloseConfirmationViewModel();
        public ExamplesPanelViewModel ExamplesViewModel { get; private set; } = new ExamplesPanelViewModel();
        public NotificationsViewModel Notifications { get; private set; }
        public StillThereViewModel StillThere { get; private set; } = new StillThereViewModel();

        private LevelerViewModel _levelerViewModel;
        public LevelerViewModel LevelerViewModel
        {
            get => _levelerViewModel;
            set => SetProperty(ref _levelerViewModel, value);
        }

        private List<AnswerButton> _currentAnswers;
        public List<AnswerButton> CurrentAnswers
        {
            get => _currentAnswers;
            set => SetProperty(ref _currentAnswers, value);
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

        private bool _showOverlay = false;
        public bool ShowOverlay
        {
            get => _showOverlay;
            set => SetProperty(ref _showOverlay, value);
        }

        private SubjectViewEnum _subjectView = SubjectViewEnum.DragSubject;
        public SubjectViewEnum SubjectView
        {
            get => _subjectView;
            set
            {
                Messenger.Default.Send(value, $"{User.Name}_SubjectStatus");
                AllowSelection = value == SubjectViewEnum.MatchedSubject;
                SetProperty(ref _subjectView, value);
            }
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

        public ClassificationPanelViewModel(IPanoptesService panoptesService, IGraphQLService graphQLService, ILocalDBService localDBService, TableUser user)
        {
            _panoptesService = panoptesService;
            _graphQLService = graphQLService;
            _localDBService = localDBService;
            User = user;
            
            LevelerViewModel = new LevelerViewModel(User);
            Notifications = new NotificationsViewModel(User);

            LoadCommands();
            AddSubscribers();
            SetTimer();
        }

        private void AddSubscribers()
        {
            CloseConfirmationViewModel.EndSession += OnCloseClassifier;
            ExamplesViewModel.PropertyChanged += ResetStillThereModalTimer;
            LevelerViewModel.PropertyChanged += ResetStillThereModalTimer;
            Notifications.PropertyChanged += ResetStillThereModalTimer;

            Notifications.GetSubjectById += OnGetSubjectById;
            Notifications.ChangeView += OnChangeView;
            StillThere.ResetFiveMinuteTimer += StartStillThereModalTimer;
            StillThere.CloseClassificationPanel += OnCloseClassifier;

            CloseConfirmationViewModel.CheckOverlay += OnShouldShowOverlay;
            StillThere.CheckOverlay += OnShouldShowOverlay;
        }

        private void OnShouldShowOverlay()
        {
            ShowOverlay = CloseConfirmationViewModel.IsVisible || StillThere.IsVisible;
        }

        public async void Load()
        {
            await GetWorkflow();
            PrepareForNewClassification();
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
            ShowCloseConfirmation = new CustomCommand(OnShowCloseConfirmation);
        }

        private void OnShowCloseConfirmation(object obj)
        {
            CloseConfirmationViewModel.OnToggleCloseConfirmation();
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

        public void OnGetSubjectById(string subjectID)
        {
            TotalVotes = 0;
            TableSubject newSubject = _localDBService.GetLocalSubject(subjectID);
            Subjects.Insert(0, newSubject);
            GetSubjectQueue();
            SubjectView = SubjectViewEnum.MatchedSubject;
            NotifySpaceView(RingNotifierStatus.IsCreating);
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
            LevelerViewModel.CloseLeveler();
            ExamplesViewModel.ResetExamples();
            PrepareForNewClassification();
            Notifications.NotifyLeaving();

            ClassifierOpen = false;
            CloseConfirmationViewModel.OnToggleCloseConfirmation(false);
            User.Active = false;
            NotifySpaceView(RingNotifierStatus.IsLeaving);
            CompletedClassifications.Clear();
        }

        private void PrepareForNewClassification()
        {
            GetSubjectQueue();
            OnChangeView(ClassifierViewEnum.SubjectView);
            TotalVotes = 0;
        }

        public void OnChangeView(ClassifierViewEnum view)
        {
            CurrentView = view;
        }

        private async void OnContinueClassification(object sender)
        {
            if (CurrentView == ClassifierViewEnum.SubjectView)
            {
                NotifySpaceView(RingNotifierStatus.IsSubmitting);
                CurrentClassification.Metadata.FinishedAt = System.DateTime.Now.ToString();
                CurrentClassification.Annotations.Add(CurrentAnnotation);
                await _panoptesService.CreateClassificationAsync(CurrentClassification);
                SelectedAnswer.AnswerCount += 1;
                TotalVotes += 1;
                ClassificationsThisSession += 1;
                LevelerViewModel.OnIncrementCount(ClassificationsThisSession);
                OnChangeView(ClassifierViewEnum.SummaryView);
                CompletedClassification FinishedClassification = new CompletedClassification(SelectedAnswer, User, CurrentSubject.Id);
                CompletedClassifications.Add(FinishedClassification);
                Messenger.Default.Send(FinishedClassification, $"{User.Name}_AddCompletedClassification");
                Notifications.HandleAnswer(FinishedClassification);
            }
            else
            {
                PrepareForNewClassification();
            }
        }

        private void NotifySpaceView(RingNotifierStatus Status)
        {
            ClassificationRingNotifier Notification = new ClassificationRingNotifier(CurrentSubject, User, Status);
            Messenger.Default.Send(Notification);
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
            if (StillThere.IsVisible) { StillThere.IsVisible = false; }
        }

        private void ShowStillThereModal(object sender, System.EventArgs e)
        {
            StillThereTimer.Stop();
            StillThere.IsVisible = true;
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

        public void StartNewClassification(TableSubject subject)
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

        public void GetSubjectQueue()
        {
            if (Subjects.Count <= 0)
            {
                NameValueCollection query = new NameValueCollection
                {
                    { "workflow_id", Config.WorkflowId }
                };
                Subjects = _localDBService.GetQueuedSubjects();
            }
            if (Subjects.Count > 0)
            {
                CurrentSubject = Subjects[0];
                StartNewClassification(CurrentSubject);
                Subjects.RemoveAt(0);

                if (CurrentSubject != null && CurrentSubject.Location != null)
                {
                    SubjectImageSource = CurrentSubject.Location;
                    GetSubjectReductions();
                }
            }
        }

        public void DropSubject(TableSubject subject)
        {
            if (CheckAlreadyCompleted(subject)) return;
            if (CurrentView == ClassifierViewEnum.SummaryView) CurrentView = ClassifierViewEnum.SubjectView;
            TotalVotes = 0;
            Subjects.Insert(0, subject);
            GetSubjectQueue();
            SubjectView = SubjectViewEnum.MatchedSubject;
            NotifySpaceView(RingNotifierStatus.IsCreating);
        }

        private bool CheckAlreadyCompleted(TableSubject subject)
        {
            bool AlreadyCompleted = CompletedClassifications.Any(x => x.SubjectId == subject.Id);
            if (AlreadyCompleted) Notifications.AlreadySeen();
            return AlreadyCompleted;
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
