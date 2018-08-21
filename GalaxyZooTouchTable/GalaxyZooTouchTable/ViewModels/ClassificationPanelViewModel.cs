using PanoptesNetClient.Models;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationPanelViewModel
    {
        Workflow Workflow { get; }
        WorkflowTask CurrentTask { get; set; }
        Classification CurrentClassification { get; set; }

        public ClassificationPanelViewModel(Workflow workflow)
        {
            Workflow = workflow;
            CurrentTask = workflow.Tasks[workflow.FirstTask];
            System.Console.WriteLine(CurrentTask);
        }
    }
}
