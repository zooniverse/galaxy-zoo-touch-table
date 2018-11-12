using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using PanoptesNetClient;
using PanoptesNetClient.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TableUser> ActiveUsers { get; set; } = new ObservableCollection<TableUser>();
        public Workflow Workflow { get; set; }
        public Project Project { get; set; }
        public ICommand WindowLoaded { get; set; }

        private bool _showJoinMessage = true;
        public bool ShowJoinMessage
        {
            get { return _showJoinMessage; }
            set
            {
                _showJoinMessage = value;
                OnPropertyRaised("ShowJoinMessage");
            }
        }

        private ClassificationPanelViewModel _personUserVM;
        public ClassificationPanelViewModel PersonUserVM
        {
            get { return _personUserVM; }
            set
            {
                _personUserVM = value;
                OnPropertyRaised("PersonUserVM");
            }
        }

        private ClassificationPanelViewModel _faceUserVM;
        public ClassificationPanelViewModel FaceUserVM
        {
            get { return _faceUserVM; }
            set
            {
                _faceUserVM = value;
                OnPropertyRaised("FaceUserVM");
            }
        }

        private ClassificationPanelViewModel _lightUserVM;
        public ClassificationPanelViewModel LightUserVM
        {
            get { return _lightUserVM; }
            set
            {
                _lightUserVM = value;
                OnPropertyRaised("LightUserVM");
            }
        }

        private ClassificationPanelViewModel _starUserVM;
        public ClassificationPanelViewModel StarUserVM
        {
            get { return _starUserVM; }
            set
            {
                _starUserVM = value;
                OnPropertyRaised("StarUserVM");
            }
        }

        private ClassificationPanelViewModel _heartUserVM;
        public ClassificationPanelViewModel HeartUserVM
        {
            get { return _heartUserVM; }
            set
            {
                _heartUserVM = value;
                OnPropertyRaised("HeartUserVM");
            }
        }

        private ClassificationPanelViewModel _earthUserVM;
        public ClassificationPanelViewModel EarthUserVM
        {
            get { return _earthUserVM; }
            set
            {
                _earthUserVM = value;
                OnPropertyRaised("EarthUserVM");
            }
        }

        public MainWindowViewModel()
        {
            LoadCommands();

            ActiveUsers.CollectionChanged += ActiveUsersChanged;
        }

        private void LoadCommands()
        {
            WindowLoaded = new CustomCommand(GetWorkflow);
        }

        private async void GetWorkflow(object sender)
        {
            ApiClient client = new ApiClient();
            Workflow = await client.Workflows.Get(Config.WorkflowId);

            SetChildDataContext();
        }

        private void ActiveUsersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ShowJoinMessage = ActiveUsers.Count <= 0;
        }

        private void SetChildDataContext()
        {
            TableUser personUser = TableUserFactory.Create(UserType.Person);
            TableUser lightUser = TableUserFactory.Create(UserType.Light);
            TableUser starUser = TableUserFactory.Create(UserType.Star);
            TableUser heartUser = TableUserFactory.Create(UserType.Heart);
            TableUser faceUser = TableUserFactory.Create(UserType.Face);
            TableUser earthUser = TableUserFactory.Create(UserType.Earth);

            if (Workflow != null)
            {
                PersonUserVM = new ClassificationPanelViewModel(Workflow, personUser, ActiveUsers);
                LightUserVM = new ClassificationPanelViewModel(Workflow, lightUser, ActiveUsers);
                StarUserVM = new ClassificationPanelViewModel(Workflow, starUser, ActiveUsers);
                HeartUserVM = new ClassificationPanelViewModel(Workflow, heartUser, ActiveUsers);
                FaceUserVM = new ClassificationPanelViewModel(Workflow, faceUser, ActiveUsers);
                EarthUserVM = new ClassificationPanelViewModel(Workflow, earthUser, ActiveUsers);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
