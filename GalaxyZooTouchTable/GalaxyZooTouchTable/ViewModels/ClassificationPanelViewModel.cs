﻿using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Utility;
using GraphQL.Common.Response;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel : ViewModelBase
    {
        public DispatcherTimer StillThereTimer { get; set; }
        private int ClassificationsThisSession { get; set; } = 0;
        private Classification CurrentClassification { get; set; }
        private Subject CurrentSubject { get; set; }
        public string CurrentTaskIndex { get; set; }
        public ExamplesPanelViewModel ExamplesViewModel { get; private set; } = new ExamplesPanelViewModel();
        public LevelerViewModel LevelerViewModel { get; private set; }
        public NotificationsViewModel Notifications { get; private set; }
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public TableUser User { get; set; }
        public Workflow Workflow { get; set; }
        public IGraphQLRepository _graphQLRepository;
        private IPanoptesRepository _panoptesRepository;

        public ICommand CloseClassifier { get; private set; }
        public ICommand ContinueClassification { get; private set; }
        public ICommand OpenClassifier { get; private set; }
        public ICommand SelectAnswer { get; private set; }
        public ICommand ShowCloseConfirmation { get; private set; }

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

        public ClassificationPanelViewModel(IPanoptesRepository panoptesRepo, IGraphQLRepository graphQLRepo, TableUser user)
        {
            _panoptesRepository = panoptesRepo;
            _graphQLRepository = graphQLRepo;
            User = user;

            Notifications = new NotificationsViewModel(user);
            LevelerViewModel = new LevelerViewModel(user);
            StillThere = new StillThereViewModel(this);

            AddSubscribers();
            GetWorkflow();
            LoadCommands();
        }

        private void AddSubscribers()
        {
            ExamplesViewModel.PropertyChanged += ResetTimer;
            LevelerViewModel.PropertyChanged += ResetTimer;
            Notifications.GetSubjectById += OnGetSubjectById;
            Notifications.ChangeView += OnChangeView;
            Notifications.SendRequestToUser += OnSendRequestToUser;
        }

        private void OnSendRequestToUser(TableUser UserToNotify)
        {
            NotificationRequest Request = new NotificationRequest(User, CurrentSubject.Id);
            Messenger.Default.Send<NotificationRequest>(Request, $"{UserToNotify.Name}_ReceivedNotification");
        }

        private async void GetWorkflow()
        {
            Workflow = await _panoptesRepository.GetWorkflowAsync(Config.WorkflowId);
            PrepareForNewClassification();
            WorkflowTask CurrentTask = Workflow.Tasks[Workflow.FirstTask];
            CurrentTaskIndex = Workflow.FirstTask;
            CurrentAnswers = ParseTaskAnswers(CurrentTask.Answers);
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

        private async void OnGetSubjectById(string subjectID)
        {
            TotalVotes = 0;
            CurrentSubject = await _panoptesRepository.GetSubjectAsync(subjectID);
            StartNewClassification(CurrentSubject);
            SubjectImageSource = CurrentSubject.GetSubjectLocation();
            GetSubjectReductions();
        }

        private void OnOpenClassifier(object sender)
        {
            StartTimer();
            ClassifierOpen = true;
            User.Active = true;
        }

        public void OnCloseClassifier(object sender = null)
        {
            Notifications.ClearNotifications(true);
            StillThereTimer = null;
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
            OnChangeView(ClassifierViewEnum.SubjectView);
            TotalVotes = 0;
        }

        private void ToggleCloseConfirmation(object sender)
        {
            CloseConfirmationVisible = !CloseConfirmationVisible;
        }

        private void OnChangeView(ClassifierViewEnum view)
        {
            CurrentView = view;
        }

        private async void OnContinueClassification(object sender)
        {
            if (CurrentView == ClassifierViewEnum.SubjectView)
            {
                CurrentClassification.Metadata.FinishedAt = System.DateTime.Now.ToString();
                CurrentClassification.Annotations.Add(CurrentAnnotation);
                await _panoptesRepository.CreateClassificationAsync(CurrentClassification);
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

        private void StartTimer()
        {
            StillThereTimer = new DispatcherTimer();
            StillThereTimer.Tick += new System.EventHandler(ShowStillThereModal);
            ResetTimer();
        }

        private void ShowStillThereModal(object sender, System.EventArgs e)
        {
            StillThereTimer.Stop();
            StillThere.Visible = true;
        }

        private void ChooseAnswer(AnswerButton button)
        {
            CurrentAnnotation = new Annotation(CurrentTaskIndex, button.Index);
        }

        public void ResetTimer()
        {
            if (StillThereTimer != null)
            {
                StillThereTimer.Interval = new System.TimeSpan(0, 0, 10);
                StillThereTimer.Start();
            }
            if (StillThere.Visible) { StillThere.Visible = false; }
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
                NameValueCollection query = new NameValueCollection
                {
                    { "workflow_id", Config.WorkflowId }
                };
                Subjects = await _panoptesRepository.GetSubjectsAsync("queued", query);
            }
            CurrentSubject = Subjects[0];
            StartNewClassification(CurrentSubject);
            SubjectImageSource = CurrentSubject.GetSubjectLocation();
            Subjects.RemoveAt(0);
            GetSubjectReductions();
        }

        private void ResetAnswerCount()
        {
            foreach (AnswerButton Answer in CurrentAnswers)
            {
                Answer.AnswerCount = 0;
            }
        }

        private async void GetSubjectReductions()
        {
            GraphQLResponse response = await _graphQLRepository.GetReductionAsync(Workflow, CurrentSubject);

            if (response.Data != null)
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
