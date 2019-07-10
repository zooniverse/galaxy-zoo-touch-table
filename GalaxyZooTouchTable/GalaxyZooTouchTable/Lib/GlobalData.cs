using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.ViewModels;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace GalaxyZooTouchTable.Lib
{
    public class GlobalData : ViewModelBase
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
        }

        private void PopulateOfflineWorkflow()
        {
            OfflineWorkflow = new Workflow();

            OfflineWorkflow.Version = "1.4";
            OfflineWorkflow.FirstTask = "T0";

            List<TaskAnswer> answers = new List<TaskAnswer>();
            answers.Add(new TaskAnswer("![smooth_triple_flat.png](https://panoptes-uploads.zooniverse.org/staging/project_attached_image/810958cc-54bc-40f5-8871-628a5b0257ea.png) Smooth _Gradually fades from the center_"));
            answers.Add(new TaskAnswer("![features_or_disk.png](https://panoptes-uploads.zooniverse.org/staging/project_attached_image/4a4333a4-3e8b-42d3-9b81-8d6a2d296d16.png) Features _Irregularities; not smooth_"));
            answers.Add(new TaskAnswer("![star_inverted.png](https://panoptes-uploads.zooniverse.org/staging/project_attached_image/f0900323-83f1-4971-829c-6437ba6dd63a.png) Not a Galaxy _Star or artifact_"));

            WorkflowTask offlineTask = new WorkflowTask("Choose an Answer", answers);
            OfflineWorkflow.Tasks = new Dictionary<string, WorkflowTask>();
            OfflineWorkflow.Tasks.Add("T0", offlineTask);
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

        public void EstablishLog()
        {
            string date = DateTime.Now.ToString("MM-dd-yyyy");
            Logger = new Log($"log_{date}");
        }
    }
}
