using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class CenterpieceViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TableUser> AllUsers { get; set; } = new ObservableCollection<TableUser>();

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

        public CenterpieceViewModel()
        {
            CreateTimer();

            AllUsers.CollectionChanged += AllUsersCollectionChanged;

            AllUsers.Add(CommonData.GetInstance().PersonUser);
            AllUsers.Add(CommonData.GetInstance().LightUser);
            AllUsers.Add(CommonData.GetInstance().StarUser);
            AllUsers.Add(CommonData.GetInstance().HeartUser);
            AllUsers.Add(CommonData.GetInstance().FaceUser);
            AllUsers.Add(CommonData.GetInstance().EarthUser);
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
            dispatcherTimer.Tick += new System.EventHandler(OnFlipCenterpiece);
            dispatcherTimer.Interval = new System.TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
        }

        private void OnFlipCenterpiece(object sender, System.EventArgs e)
        {
            FlipCenterpiece = !FlipCenterpiece;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
