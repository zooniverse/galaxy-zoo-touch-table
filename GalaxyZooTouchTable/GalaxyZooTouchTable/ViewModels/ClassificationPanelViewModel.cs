using System;
using System.Windows.Input;
using System.Collections.Generic;
using PanoptesNetClient.Models;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using PanoptesNetClient;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel
    {
        public Workflow Workflow { get; }
        public WorkflowTask CurrentTask { get; set; }
        public Classification CurrentClassification { get; set; }
        public List<AnswerButton> CurrentAnswers { get; set; }
        public Subject CurrentSubject { get; set; }
        public Annotation CurrentAnnotation { get; set; }
        public string CurrentTaskIndex { get; set; }

        public ICommand SelectAnswer { get; set; }
        public ICommand SubmitClassification { get; set; }

        public ClassificationPanelViewModel(Workflow workflow, Subject subject)
        {
            LoadCommands();
            CurrentSubject = subject;
            Workflow = workflow;
            CurrentTask = workflow.Tasks[workflow.FirstTask];
            CurrentTaskIndex = workflow.FirstTask;
            StartNewClassification();

            if (CurrentTask.Answers != null)
            {
                CurrentAnswers = ParseTaskAnswers(CurrentTask.Answers);
            }
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
        }

        private bool CanSendClassification(object obj)
        {
            return CurrentAnnotation != null;
        }

        private void ChooseAnswer(object obj)
        {
            AnswerButton button = (AnswerButton)obj;
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

        public void StartNewClassification()
        {
            CurrentClassification = new Classification();
            CurrentClassification.Metadata.WorkflowVersion = Workflow.Version;
            CurrentClassification.Metadata.StartedAt = DateTime.Now.ToString();
            CurrentClassification.Metadata.UserAgent = "Galaxy Zoo Touch Table";
            CurrentClassification.Metadata.UserLanguage = "en";

            CurrentClassification.Links = new ClassificationLinks(Config.ProjectId, Config.WorkflowId);
            CurrentClassification.Links.Subjects.Add(CurrentSubject.Id);
        }
    }
}
