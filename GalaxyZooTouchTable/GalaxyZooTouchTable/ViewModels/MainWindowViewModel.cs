using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Utility;
using PanoptesNetClient.Models;
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
        private IPanoptesRepository _panoptesRepository = new PanoptesRepository();

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
            SetChildDataContext();
            AllUsers.CollectionChanged += AllUsersCollectionChanged;
        }

        private void AllUsersCollectionChanged(object sender, NotifyCollectionChangedEventArgs changedEventArgs)
        {

            if (changedEventArgs.NewItems != null)
            {
                foreach (object item in changedEventArgs.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (changedEventArgs.OldItems != null)
            {
                foreach (object item in changedEventArgs.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs changedEventArgs)
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

            PersonUserVM = new ClassificationPanelViewModel(_panoptesRepository, personUser, AllUsers);
            LightUserVM = new ClassificationPanelViewModel(_panoptesRepository, lightUser, AllUsers);
            StarUserVM = new ClassificationPanelViewModel(_panoptesRepository, starUser, AllUsers);
            HeartUserVM = new ClassificationPanelViewModel(_panoptesRepository, heartUser, AllUsers);
            FaceUserVM = new ClassificationPanelViewModel(_panoptesRepository, faceUser, AllUsers);
            EarthUserVM = new ClassificationPanelViewModel(_panoptesRepository, earthUser, AllUsers);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
