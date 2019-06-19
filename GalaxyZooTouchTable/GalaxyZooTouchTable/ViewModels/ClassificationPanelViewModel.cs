using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Utility;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel : ViewModelBase
    {
        private IPanoptesService _panoptesService;
        private ILocalDBService _localDBService;
        private List<CompletedClassification> CompletedClassifications { get; set; } = new List<CompletedClassification>();
        private DispatcherTimer StillThereTimer { get; set; } = new DispatcherTimer();

        public Classification CurrentClassification { get; set; } = new Classification();
        public List<TableSubject> Subjects = new List<TableSubject>();
        public TableUser User { get; set; }
        public Workflow Workflow { get; set; }
        public ICommand CloseClassifier { get; private set; }
        public ICommand SubmitClassification { get; private set; }
        public ICommand OpenClassifier { get; private set; }
        public ICommand SelectAnswer { get; private set; }
        public ICommand ShowCloseConfirmation { get; private set; }

        private TableSubject _currentSubject;
        public TableSubject CurrentSubject
        {
            get => _currentSubject;
            set
            {
                if (value != null)
                {
                    TableSubject NewSubject = value as TableSubject;
                    Notifications.ReceivedNewSubject(NewSubject);
                }
                Messenger.Default.Send(value != null, $"{User.Name}_SubjectStatus");
                AllowSelection = value != null;
                SetProperty(ref _currentSubject, value);
            }
        }

        private ClassificationSummaryViewModel _classificationSummaryViewModel;
        public ClassificationSummaryViewModel ClassificationSummaryViewModel
        {
            get => _classificationSummaryViewModel;
            set => SetProperty(ref _classificationSummaryViewModel, value);
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

        private AnswerButton _selectedAnswer;
        public AnswerButton SelectedAnswer
        {
            get => _selectedAnswer;
            set => SetProperty(ref _selectedAnswer, value);
        }

        private bool _allowSelection;
        public bool AllowSelection
        {
            get => _allowSelection;
            set => SetProperty(ref _allowSelection, value);
        }

        public ClassificationPanelViewModel(IPanoptesService panoptesService, ILocalDBService localDBService, TableUser user)
        {
            _panoptesService = panoptesService;
            _localDBService = localDBService;
            User = user;
            
            LevelerViewModel = new LevelerViewModel(User);
            Notifications = new NotificationsViewModel(User);
            ClassificationSummaryViewModel = new ClassificationSummaryViewModel(Notifications);

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
            ClassificationSummaryViewModel.RandomGalaxyDelegate += GetSubjectQueue;
            ClassificationSummaryViewModel.ChooseAnotherGalaxyDelegate += PrepareForNewClassification;
            ClassificationSummaryViewModel.DropSubjectDelegate += DropSubject;
        }

        private void OnShouldShowOverlay()
        {
            ShowOverlay = CloseConfirmationViewModel.IsVisible || StillThere.IsVisible;
        }

        public async void Load() { await GetWorkflow(); }

        public async Task GetWorkflow()
        {
            Workflow = await _panoptesService.GetWorkflowAsync(Config.WorkflowId);
            WorkflowTask CurrentTask = Workflow.Tasks[Workflow.FirstTask];
            CurrentAnswers = ParseTaskAnswers(CurrentTask.Answers);
        }

        private void LoadCommands()
        {
            CloseClassifier = new CustomCommand(OnCloseClassifier);
            SubmitClassification = new CustomCommand(OnSubmitClassification);
            OpenClassifier = new CustomCommand(OnOpenClassifier);
            SelectAnswer = new CustomCommand(OnSelectAnswer);
            ShowCloseConfirmation = new CustomCommand(OnShowCloseConfirmation);
        }

        private void OnShowCloseConfirmation(object obj)
        {
            CloseConfirmationViewModel.OnToggleCloseConfirmation();
        }

        private void PrepareForNewClassification()
        {
            CurrentSubject = null;
            OnChangeView(ClassifierViewEnum.SubjectView);
            CurrentAnnotation = null;
            SelectedAnswer = null;
        }

        public void OnSelectAnswer(object sender)
        {
            SelectedAnswer = sender as AnswerButton;
            CurrentAnnotation = new Annotation(Workflow.FirstTask, SelectedAnswer.Index);
        }

        public void OnGetSubjectById(string subjectID)
        {
            NotifySpaceView(RingNotifierStatus.IsHelping);
            TableSubject subject = _localDBService.GetLocalSubject(subjectID);
            LoadSubject(subject);
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
            PrepareForNewClassification();
            LevelerViewModel.CloseLeveler();
            ExamplesViewModel.ResetExamples();
            Notifications.NotifyLeaving();

            ClassifierOpen = false;
            CloseConfirmationViewModel.OnToggleCloseConfirmation(false);
            User.Active = false;
            NotifySpaceView(RingNotifierStatus.IsLeaving);
            CompletedClassifications.Clear();
        }

        public void OnChangeView(ClassifierViewEnum view) { CurrentView = view; }

        private async void OnSubmitClassification(object sender)
        {
            CurrentClassification.Annotations.Add(CurrentAnnotation);
            ClassificationCounts counts = await _panoptesService.CreateClassificationAsync(CurrentClassification);
            ClassificationSummaryViewModel.ProcessNewClassification(CurrentSubject.SubjectLocation, counts, CurrentAnswers, SelectedAnswer);

            NotifySpaceView(RingNotifierStatus.IsSubmitting);
            LevelerViewModel.OnIncrementCount();
            OnChangeView(ClassifierViewEnum.SummaryView);
            HandleCompletedClassification();
        }

        void HandleCompletedClassification()
        {
            CompletedClassification FinishedClassification = new CompletedClassification(SelectedAnswer, User, CurrentSubject.Id);
            CompletedClassifications.Add(FinishedClassification);
            Messenger.Default.Send(FinishedClassification, $"{User.Name}_AddCompletedClassification");
            Notifications.HandleAnswer(FinishedClassification);
        }

        private void NotifySpaceView(RingNotifierStatus Status)
        {
            Messenger.Default.Send(new ClassificationRingNotifier(CurrentSubject, User, Status));
        }

        private void SetTimer()
        {
            StillThereTimer.Tick += new System.EventHandler(ShowStillThereModal);
            StillThereTimer.Interval = new System.TimeSpan(0, 2, 30);
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
            CurrentClassification.Metadata.StartedAt = System.DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            CurrentClassification.Metadata.UserAgent = "Galaxy_Zoo_Touch_Table";
            CurrentClassification.Metadata.UserLanguage = "en";
            CurrentClassification.Metadata.UtcOffset = System.Math.Abs(System.TimeZoneInfo.Local.GetUtcOffset(System.DateTime.UtcNow).TotalSeconds).ToString();

            CurrentClassification.Links = new ClassificationLinks(Config.ProjectId, Config.WorkflowId);
            CurrentClassification.Links.Subjects.Add(subject.Id);
            CommandManager.InvalidateRequerySuggested();
        }

        public void LoadSubject(TableSubject subject)
        {
            PrepareForNewClassification();
            CurrentSubject = subject;
            StartNewClassification(subject);
        }

        public void GetSubjectQueue()
        {
            PrepareForNewClassification();
            if (Subjects.Count == 0)
                Subjects = _localDBService.GetQueuedSubjects();
            LoadSubject(Subjects[0]);
            Subjects.RemoveAt(0);
        }

        public void DropSubject(TableSubject subject)
        {
            if (CheckAlreadyCompleted(subject)) return;
            if (CurrentView == ClassifierViewEnum.SummaryView) CurrentView = ClassifierViewEnum.SubjectView;
            LoadSubject(subject);
            NotifySpaceView(RingNotifierStatus.IsCreating);
        }

        private bool CheckAlreadyCompleted(TableSubject subject)
        {
            bool AlreadyCompleted = CompletedClassifications.Any(x => x.SubjectId == subject.Id);
            if (AlreadyCompleted) Notifications.AlreadySeen();
            return AlreadyCompleted;
        }
    }
}
