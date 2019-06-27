using GalaxyZooTouchTable.Models;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Linq;

namespace GalaxyZooTouchTable.Lib
{
    public class GlobalData : INotifyPropertyChanged
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
        public Log Logger { get; private set; }

        protected GlobalData()
        {
            PopulateUsers();
            PopulateOfflineWorkflow();

            string date = DateTime.Now.ToString("MM-dd-yyyy");
            Logger = new Log($"log_{date}");
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
                    TaskAnswer taskAnswer = new TaskAnswer(answer.Value);
                    answers.Add(taskAnswer);
                }
                WorkflowTask offlineTask = new WorkflowTask("Choose an Answer", answers);
                OfflineWorkflow.Tasks = new Dictionary<string, WorkflowTask>();
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
