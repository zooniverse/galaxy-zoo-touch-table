using GalaxyZooTouchTable.Models;
using PanoptesNetClient.Models;
using System.Collections.Generic;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel
    {
        public Workflow Workflow { get; }
        public WorkflowTask CurrentTask { get; set; }
        public Classification CurrentClassification { get; set; }
        public List<AnswerButton> CurrentAnswers { get; set; }
        public Subject CurrentSubject { get; set; }

        public ClassificationPanelViewModel(Workflow workflow, Subject subject)
        {
            CurrentSubject = subject;
            Workflow = workflow;
            CurrentTask = workflow.Tasks[workflow.FirstTask];
            StartNewClassification();

            if (CurrentTask.Answers != null)
            {
                CurrentAnswers = ParseTaskAnswers(CurrentTask.Answers);
            }
        }

        public List<AnswerButton> ParseTaskAnswers(List<TaskAnswer> answers)
        {
            List<AnswerButton> renderedAnswers = new List<AnswerButton>();

            foreach (var answer in answers)
            {
                AnswerButton item = new AnswerButton(answer);
                renderedAnswers.Add(item);
            }
            return renderedAnswers;
        }

        public void StartNewClassification()
        {
            CurrentClassification = new Classification();
            CurrentClassification.Metadata.WorkflowVersion = Workflow.Version;
            CurrentClassification.Metadata.StartedAt = "";
            CurrentClassification.Metadata.UserAgent = "Galaxy Zoo Touch Table";
            CurrentClassification.Metadata.UserLanguage = "en";

            CurrentClassification.Links = new ClassificationLinks(Config.ProjectId, Config.WorkflowId);
            CurrentClassification.Links.Subjects.Add(CurrentSubject.Id);
        }
    }
}
