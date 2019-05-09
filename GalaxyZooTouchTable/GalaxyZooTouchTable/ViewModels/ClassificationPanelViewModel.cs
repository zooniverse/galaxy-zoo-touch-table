﻿using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Utility;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
<<<<<<< HEAD
=======
using System.Collections.ObjectModel;
using System.Collections.Specialized;
>>>>>>> Use Random Summary String
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel : ViewModelBase
    {
        private IPanoptesService _panoptesService;
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
<<<<<<< HEAD
        public List<TableSubject> Subjects = new List<TableSubject>();
=======
        public List<TableSubject> Subjects { get; set; } = new List<TableSubject>();
        static Random random = new Random();
>>>>>>> Use Random Summary String

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

        private string _summaryString;
        public string SummaryString
        {
            get => _summaryString;
            set => SetProperty(ref _summaryString, value);
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
            ChooseAnotherGalaxy = new CustomCommand(PrepareForNewClassification);
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

        private void PrepareForNewClassification(object sender = null)
        {
            TotalVotes = 0;
            CurrentSubject = null;
            OnChangeView(ClassifierViewEnum.SubjectView);
            CurrentAnnotation = null;
            SelectedAnswer = null;
        }

        private void OnSelectAnswer(object sender)
        {
            AnswerButton Button = sender as AnswerButton;
            SelectedAnswer = Button;
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

<<<<<<< HEAD
=======
        private void PrepareForNewClassification()
        {
            SummaryString = SelectSummaryString();
            GetSubjectQueue();
            OnChangeView(ClassifierViewEnum.SubjectView);
            TotalVotes = 0;
        }

>>>>>>> Use Random Summary String
        public void OnChangeView(ClassifierViewEnum view)
        {
            CurrentView = view;
        }

        private async void OnContinueClassification(object sender)
        {
            if (CurrentView == ClassifierViewEnum.SubjectView)
            {
                NotifySpaceView(RingNotifierStatus.IsSubmitting);
                CurrentClassification.Annotations.Add(CurrentAnnotation);
<<<<<<< HEAD
                ClassificationCounts counts = await _panoptesService.CreateClassificationAsync(CurrentClassification);
                TotalVotes = counts.Total;
=======
                //await _panoptesService.CreateClassificationAsync(CurrentClassification);
>>>>>>> Use Random Summary String
                SelectedAnswer.AnswerCount += 1;
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
                GetSubjectQueue();
            }
        }

        string SelectSummaryString()
        {
            ObservableCollection<string> summaryStrings = Application.Current.FindResource("SummaryStrings") as ObservableCollection<string>;
            int index = random.Next(summaryStrings.Count);
            return summaryStrings[index];
        }

        private void NotifySpaceView(RingNotifierStatus Status)
        {
            ClassificationRingNotifier Notification = new ClassificationRingNotifier(CurrentSubject, User, Status);
            Messenger.Default.Send(Notification);
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

        public void ResetAnswerCount()
        {
            foreach (AnswerButton Answer in CurrentAnswers)
            {
                Answer.AnswerCount = 0;
            }
        }
    }
}
