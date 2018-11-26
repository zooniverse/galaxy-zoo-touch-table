using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using PanoptesNetClient;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TableUser> AllUsers { get; set; } = new ObservableCollection<TableUser>();
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

        private bool _flipCenterpiece;
        public bool FlipCenterpiece
        {
            get { return _flipCenterpiece; }
            set
            {
                _flipCenterpiece = value;
                OnPropertyRaised("FlipCenterpiece");
            }
        }

        public MainWindowViewModel()
        {
            CreateTimer();
            LoadCommands();

            AllUsers.CollectionChanged += AllUsersCollectionChanged;
        }

        private void AllUsersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (TableUser user in AllUsers)
            {
                if (user.Active)
                {
                    ShowJoinMessage = false;
                    return;
                }
            }
            ShowJoinMessage = true;
        }

        private void CreateTimer()
        {
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new System.EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new System.TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            FlipCenterpiece = !FlipCenterpiece;
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

        private void SetChildDataContext()
        {
            TableUser personUser = TableUserFactory.Create(UserType.Person);
            TableUser lightUser = TableUserFactory.Create(UserType.Light);
            TableUser starUser = TableUserFactory.Create(UserType.Star);
            TableUser heartUser = TableUserFactory.Create(UserType.Heart);
            TableUser faceUser = TableUserFactory.Create(UserType.Face);
            TableUser earthUser = TableUserFactory.Create(UserType.Earth);
            AllUsers.Add(personUser);
            AllUsers.Add(lightUser);
            AllUsers.Add(starUser);
            AllUsers.Add(heartUser);
            AllUsers.Add(faceUser);
            AllUsers.Add(earthUser);

            if (Workflow != null)
            {
                PersonUserVM = new ClassificationPanelViewModel(Workflow, personUser, AllUsers);
                LightUserVM = new ClassificationPanelViewModel(Workflow, lightUser, AllUsers);
                StarUserVM = new ClassificationPanelViewModel(Workflow, starUser, AllUsers);
                HeartUserVM = new ClassificationPanelViewModel(Workflow, heartUser, AllUsers);
                FaceUserVM = new ClassificationPanelViewModel(Workflow, faceUser, AllUsers);
                EarthUserVM = new ClassificationPanelViewModel(Workflow, earthUser, AllUsers);
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
