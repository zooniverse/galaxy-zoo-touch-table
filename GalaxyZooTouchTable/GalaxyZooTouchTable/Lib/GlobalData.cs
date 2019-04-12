using GalaxyZooTouchTable.Models;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace GalaxyZooTouchTable.Lib
{
    public class GlobalData
    {
        private static GlobalData _instance = null;
        public ObservableCollection<TableUser> AllUsers { get; set; } = new ObservableCollection<TableUser>();
        public TableUser PurpleUser = new PurpleUser();
        public TableUser AquaUser = new AquaUser();
        public TableUser BlueUser = new BlueUser();
        public TableUser PinkUser = new PinkUser();
        public TableUser PeachUser = new PeachUser();
        public TableUser GreenUser = new GreenUser();
        public Workflow OfflineWorkflow = new Workflow();

        protected GlobalData()
        {
            PopulateUsers();
            PopulateOfflineWorkflow();
        }

        private void PopulateOfflineWorkflow()
        {
            OfflineWorkflow = new Workflow();
            foreach (XElement element in XElement.Load("../../Data/OfflineWorkflow.xml").Elements("Workflow"))
            {
                List<TaskAnswer> answers = new List<TaskAnswer>();
                OfflineWorkflow.Version = element.Element("version").Value;
                OfflineWorkflow.FirstTask = element.Element("firstTask").Value;
                var task = element.Element("tasks").Element("task");
                foreach (XElement answer in task.Elements())
                {
                    answers.Add(new TaskAnswer(answer.Value));
                }
                WorkflowTask offlineTask = new WorkflowTask("Choose an Answer", answers);
                OfflineWorkflow.Tasks.Add("T0", offlineTask);
            }
        }

        public static GlobalData GetInstance()
        {
            if (_instance == null)
                _instance = new GlobalData();

            return _instance;
        }

        private void PopulateUsers()
        {
            AllUsers.Add(PurpleUser);
            AllUsers.Add(PeachUser);
            AllUsers.Add(AquaUser);
            AllUsers.Add(BlueUser);
            AllUsers.Add(PinkUser);
            AllUsers.Add(GreenUser);
        }
    }
}
