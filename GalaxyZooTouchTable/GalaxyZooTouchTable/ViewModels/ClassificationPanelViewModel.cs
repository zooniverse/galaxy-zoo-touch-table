﻿using GalaxyZooTouchTable.Lib;
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
        readonly int RETIRED_LIMIT = 10;
        IPanoptesService _panoptesService;
        ILocalDBService _localDBService;
        List<CompletedClassification> CompletedClassifications { get; set; } = new List<CompletedClassification>();
        DispatcherTimer StillThereTimer { get; set; } = new DispatcherTimer();
        bool IsSubmittingClassification;
        Session Session { get; set; } = new Session();

        public Classification CurrentClassification { get; set; } = new Classification();
        public List<TableSubject> Subjects = new List<TableSubject>();
        public TableUser User { get; set; }
        public Workflow Workflow { get; set; }
        public ICommand CloseClassifier { get; private set; }
        public ICommand SubmitClassification { get; private set; }
        public ICommand TapDropZone { get; private set; }
        public ICommand OpenClassifier { get; private set; }
        public ICommand SelectAnswer { get; private set; }
        public ICommand ShowCloseConfirmation { get; private set; }
        public ICommand HideRetirementModal { get; private set; }
        public event Action LevelUpAnimation = delegate { };

        private bool _showRetirementModal;
        public bool ShowRetirementModal
        {
            get => _showRetirementModal;
            set
            {
                ShowOverlay = value;
                SetProperty(ref _showRetirementModal, value);
            }
        }

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

        public CloseConfirmationViewModel CloseConfirmationViewModel { get; private set; }
        public ExamplesPanelViewModel ExamplesViewModel { get; private set; }
        public NotificationsViewModel Notifications { get; private set; }
        public StillThereViewModel StillThere { get; private set; }

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
            
            CloseConfirmationViewModel = new CloseConfirmationViewModel(User, this);
            ExamplesViewModel = new ExamplesPanelViewModel(User, this);
            LevelerViewModel = new LevelerViewModel(User, this);
            Notifications = new NotificationsViewModel(User, this);
            ClassificationSummaryViewModel = new ClassificationSummaryViewModel(Notifications);
            StillThere = new StillThereViewModel(User, this);

            LoadCommands();
            AddSubscribers();
            SetTimer();
        }

        public void LogClosure(string message)
        {
            GlobalData.GetInstance().Logger?.AddEntry(message, User?.Name, CurrentSubject?.Id, CurrentView, Session?.Duration());
            OnCloseClassifier();
        }

        private void AddSubscribers()
        {
            ExamplesViewModel.PropertyChanged += ResetStillThereModalTimer;
            LevelerViewModel.PropertyChanged += ResetStillThereModalTimer;
            LevelerViewModel.LevelUpAnimation += OnLevelUpAnimation;
            Notifications.PropertyChanged += ResetStillThereModalTimer;

            Notifications.GetSubjectById += OnGetSubjectById;
            Notifications.ChangeView += OnChangeView;
            StillThere.ResetFiveMinuteTimer += StartStillThereModalTimer;

            CloseConfirmationViewModel.CheckOverlay += OnShouldShowOverlay;
            StillThere.CheckOverlay += OnShouldShowOverlay;
            ClassificationSummaryViewModel.RandomGalaxyDelegate += OnGetRandomGalaxy;
            ClassificationSummaryViewModel.ChooseAnotherGalaxyDelegate += OnChooseAnotherGalaxy;
            ClassificationSummaryViewModel.DropSubjectDelegate += DropSubject;
        }

        private void OnLevelUpAnimation() { LevelUpAnimation(); }

        private void OnChooseAnotherGalaxy()
        {
            GlobalData.GetInstance().Logger?.AddEntry("Choose_Galaxy", User.Name);
            PrepareForNewClassification();
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
            TapDropZone = new CustomCommand(OnTapDropZone);
            OpenClassifier = new CustomCommand(OnOpenClassifier, CanOpenClassifier);
            SelectAnswer = new CustomCommand(OnSelectAnswer);
            ShowCloseConfirmation = new CustomCommand(OnShowCloseConfirmation);
            SubmitClassification = new CustomCommand(OnSubmitClassification, CanSubmitClassification);
            HideRetirementModal = new CustomCommand(OnHideRetirementModal);
        }

        private bool CanOpenClassifier(object obj) { return !ClassifierOpen; }
        private bool CanSubmitClassification(object obj) { return !IsSubmittingClassification; }

        private void OnHideRetirementModal(object obj)
        {
            ShowRetirementModal = false;
            StartStillThereModalTimer();
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
            Session.Begin();
            Messenger.Default.Send(this, "New_User_Galaxy_Pulse");
            StillThereTimer.Start();
            ClassifierOpen = true;
            User.Active = true;
            LevelerViewModel.Reset();
            GlobalData.GetInstance().Logger?.AddEntry("Open_Classifier", User.Name);
        }

        private void OnCloseClassifier(object sender = null)
        {
            PrepareForNewClassification();
            LevelerViewModel.CloseLeveler();
            ExamplesViewModel.ResetExamples();
            Notifications.NotifyLeaving();

            StillThereTimer.Stop();
            ClassifierOpen = false;
            CloseConfirmationViewModel.OnToggleCloseConfirmation(false);
            User.Active = false;
            ShowRetirementModal = false;
            NotifySpaceView(RingNotifierStatus.IsLeaving);
            CompletedClassifications.Clear();
            StillThere.Reset();
        }

        public void OnChangeView(ClassifierViewEnum view) { CurrentView = view; }

        private async void OnSubmitClassification(object sender)
        {
            IsSubmittingClassification = true; 
            CurrentClassification.Annotations.Add(CurrentAnnotation);
            HandleCompletedClassification();
            ClassificationCounts counts = await _panoptesService.CreateClassificationAsync(CurrentClassification);
            ClassificationSummaryViewModel.ProcessNewClassification(CurrentSubject.SubjectLocation, counts, CurrentAnswers, SelectedAnswer);
            if (counts.Total >= RETIRED_LIMIT) CurrentSubject.IsRetired = true;
            NotifySpaceView(RingNotifierStatus.IsSubmitting);
            LevelerViewModel.OnIncrementCount();
            GlobalData.GetInstance().Logger?.AddEntry(
                "Submit_Classification", User.Name, CurrentSubject.Id, CurrentView, LevelerViewModel.ClassificationsThisSession.ToString(), answer: SelectedAnswer.Label);
            OnChangeView(ClassifierViewEnum.SummaryView);
            IsSubmittingClassification = false;
        }

        void HandleCompletedClassification()
        {
            if (SelectedAnswer == null || User == null || CurrentSubject == null) return;
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
            StillThereTimer.Interval = new System.TimeSpan(0, 1, 30);
        }

        public void StartStillThereModalTimer()
        {
            StillThereTimer.Stop();
            if (ClassifierOpen) StillThereTimer.Start();
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

        void OnGetRandomGalaxy()
        {
            GetNewSubject("Random_Galaxy");
        }

        void OnTapDropZone(object sender)
        {
            GetNewSubject("Tap_Drop_Zone");
        }

        public void GetNewSubject(string logStatement)
        {
            PrepareForNewClassification();
            if (Subjects.Count == 0)
                Subjects = _localDBService.GetQueuedSubjects();
            GlobalData.GetInstance().Logger?.AddEntry(logStatement, User.Name, Subjects[0].Id);
            LoadSubject(Subjects[0]);
            Subjects.RemoveAt(0);
        }

        public void DropSubject(TableSubject subject)
        {
            GlobalData.GetInstance().Logger?.AddEntry("Drop_Galaxy", User.Name, subject.Id, CurrentView);
            if (CheckAlreadyCompleted(subject)) return;
            if (CurrentView == ClassifierViewEnum.SummaryView) CurrentView = ClassifierViewEnum.SubjectView;
            if (subject.IsRetired)
            {
                GlobalData.GetInstance().Logger?.AddEntry("Show_Retirement_Modal", User.Name, subject.Id, CurrentView);
                ShowRetirementModal = true;
            }
            else
            {
                LoadSubject(subject);
                NotifySpaceView(RingNotifierStatus.IsCreating);
            }
        }

        private bool CheckAlreadyCompleted(TableSubject subject)
        {
            bool AlreadyCompleted = CompletedClassifications.Any(x => x.SubjectId == subject.Id);
            if (AlreadyCompleted) Notifications.AlreadySeen();
            return AlreadyCompleted;
        }
    }
}
