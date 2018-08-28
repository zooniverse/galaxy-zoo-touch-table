using System;
using System.Windows.Input;
using System.Collections.Generic;
using PanoptesNetClient.Models;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using PanoptesNetClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using GalaxyZooTouchTable.Lib;
using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel : INotifyPropertyChanged
    {
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public Workflow Workflow { get; }
        public WorkflowTask CurrentTask { get; set; }
        public Classification CurrentClassification { get; set; }
        public List<AnswerButton> CurrentAnswers { get; set; }
        public Subject CurrentSubject { get; set; }
        public Annotation CurrentAnnotation { get; set; }
        public string CurrentTaskIndex { get; set; }

        private string _subjectImageSource;
        public string SubjectImageSource
        {
            get
            {
                return _subjectImageSource;
            }
            set
            {
                _subjectImageSource = value;
                OnPropertyRaised("SubjectImageSource");
            }
        }

        public ICommand SelectAnswer { get; set; }
        public ICommand SubmitClassification { get; set; }

        private AnswerButton _selectedItem;
        public AnswerButton SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyRaised("SelectedItem");
            }
        }

        public ClassificationPanelViewModel(Workflow workflow)
        {
            GetSubject();
            LoadCommands();
            Workflow = workflow;
            CurrentTask = workflow.Tasks[workflow.FirstTask];
            CurrentTaskIndex = workflow.FirstTask;

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
        }

        private async void SendClassification(object obj)
        {
            CurrentClassification.Metadata.FinishedAt = DateTime.Now.ToString();
            CurrentClassification.Annotations.Add(CurrentAnnotation);
            ApiClient client = new ApiClient();
            await client.Classifications.Create(CurrentClassification);
            GetSubject();
        }

        private bool CanSendClassification(object obj)
        {
            return CurrentAnnotation != null;
        }

        private void ChooseAnswer(object obj)
        {
            AnswerButton button = (AnswerButton)obj;
            SelectedItem = button;
            CurrentAnnotation = new Annotation(CurrentTaskIndex, button.Index);
        }

        private bool CanSelectAnswer(object obj)
        {
            return CurrentAnswers.Count > 0;
        }

        public List<AnswerButton> ParseTaskAnswers(List<TaskAnswer> answers)
        {
            List<AnswerButton> renderedAnswers = new List<AnswerButton>();

            for (int i = 0; i < answers.Count; i++)
            {
                AnswerButton item = new AnswerButton(answers[i], i);
                renderedAnswers.Add(item);
            }
            return renderedAnswers;
        }

        public void StartNewClassification(Subject subject)
        {
            CurrentAnnotation = null;
            SelectedItem = null;

            CurrentClassification = new Classification();
            CurrentClassification.Metadata.WorkflowVersion = Workflow.Version;
            CurrentClassification.Metadata.StartedAt = DateTime.Now.ToString();
            CurrentClassification.Metadata.UserAgent = "Galaxy Zoo Touch Table";
            CurrentClassification.Metadata.UserLanguage = "en";

            CurrentClassification.Links = new ClassificationLinks(Config.ProjectId, Config.WorkflowId);
            CurrentClassification.Links.Subjects.Add(subject.Id);
        }

        private async void GetSubject()
        {
            if (Subjects.Count > 0)
            {
                SetSubject();
            } else {
                ApiClient client = new ApiClient();
                NameValueCollection query = new NameValueCollection
                {
                    { "workflow_id", Config.WorkflowId }
                };
                Subjects = await client.Subjects.GetList("queued", query);
                SetSubject();
            }
        }

        private void SetSubject()
        {
            CurrentSubject = Subjects[0];
            StartNewClassification(CurrentSubject);
            SubjectImageSource = Utilities.GetSubjectLocation(CurrentSubject);
            Subjects.RemoveAt(0);

            //BitmapImage image = new BitmapImage();
            //image.BeginInit();
            //image.UriSource = new Uri(src, UriKind.Absolute);
            //image.EndInit();
            //SubjectImageSource = image;
        }
    }
}
