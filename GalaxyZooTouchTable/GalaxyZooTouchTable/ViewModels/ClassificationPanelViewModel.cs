using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using PanoptesNetClient;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel : INotifyPropertyChanged
    {
        private const int SUBJECT_VIEW = 0;
        private const int SUMMARY_VIEW = 1;

        public ObservableCollection<TableUser> ActiveUsers { get; set; }
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
        public ICommand OpenClassifier { get; set; }
        public ICommand ShowCloseConfirmation { get; set; }

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
                if (value != null)
                {
                    ChooseAnswer(value);
                }
                OnPropertyRaised("SelectedItem");
            }
        }

        public ClassificationPanelViewModel(Workflow workflow, TableUser user, ObservableCollection<TableUser> activeUsers)
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
            LevelerViewModel = new LevelerViewModel(user);
            ActiveUsers = activeUsers;

            if (CurrentTask.Answers != null)
            {
                CurrentAnswers = ParseTaskAnswers(CurrentTask.Answers);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

        private void LoadCommands()
        {
            ContinueClassification = new CustomCommand(OnContinueClassification, CanSendClassification);
            ShowCloseConfirmation = new CustomCommand(ToggleCloseConfirmation);
            CloseClassifier = new CustomCommand(OnCloseClassifier);
            OpenClassifier = new CustomCommand(OnOpenClassifier);
        }

        private void OnOpenClassifier(object sender)
        {
            ClassifierOpen = true;
            ActiveUsers.Add(User);
        }

        private void OnCloseClassifier(object sender)
        {
            LevelerViewModel.IsOpen = false;
            ExamplesViewModel.IsOpen = true;
            ExamplesViewModel.SelectedExample = null;
            PrepareForNewClassification();
            ClassifierOpen = false;
            CloseConfirmationVisible = false;
            foreach (TableUser ActiveUser in ActiveUsers)
            {
                if (User == ActiveUser)
                {
                    ActiveUsers.Remove(ActiveUser);
                    break;
                }
            }
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
                CurrentClassification.Metadata.FinishedAt = DateTime.Now.ToString();
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

        private bool CanSendClassification(object sender)
        {
            return CurrentAnnotation != null;
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
            //var graphQLResponse = await GraphQLClient.SendQueryAsync(answersRequest);
            //var reductions = graphQLResponse.Data.workflow.subject_reductions;
            ResetAnswerCount();
            //if (reductions.Count > 0)
            //{
            //    var data = reductions.First.data;
            //    foreach (var count in data)
            //    {
            //        var index = System.Convert.ToInt32(count.Name);
            //        AnswerButton Answer = CurrentAnswers[index];


            //        int answerCount = (int)count.Value;
            //        Answer.AnswerCount = answerCount;

            //        TotalVotes += answerCount;
            //    }
            //}
        }
    }
}
