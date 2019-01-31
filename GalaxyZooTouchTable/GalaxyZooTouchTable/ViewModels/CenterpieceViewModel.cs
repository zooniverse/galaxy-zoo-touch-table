using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Threading;
using System.Linq;

namespace GalaxyZooTouchTable.ViewModels
{
    public class CenterpieceViewModel : ViewModelBase, ICenterpieceViewModel
    {
        public DispatcherTimer Timer = new DispatcherTimer();
        public ObservableCollection<TableUser> AllUsers { get; set; } = new ObservableCollection<TableUser>();

        private bool _showJoinMessage = true;
        public bool ShowJoinMessage
        {
            get => _showJoinMessage;
            set => SetProperty(ref _showJoinMessage, value);
        }

        private bool _centerpieceIsFlipped = false;
        public bool CenterpieceIsFlipped
        {
            get => _centerpieceIsFlipped;
            set => SetProperty(ref _centerpieceIsFlipped, value);
        }

        public CenterpieceViewModel()
        {
            CreateTimer();

            AllUsers.CollectionChanged += AllUsersCollectionChanged;

            AllUsers.Add(GlobalData.GetInstance().PersonUser);
            AllUsers.Add(GlobalData.GetInstance().LightUser);
            AllUsers.Add(GlobalData.GetInstance().StarUser);
            AllUsers.Add(GlobalData.GetInstance().HeartUser);
            AllUsers.Add(GlobalData.GetInstance().FaceUser);
            AllUsers.Add(GlobalData.GetInstance().EarthUser);
        }

        public void AllUsersCollectionChanged(object sender, NotifyCollectionChangedEventArgs changedEventArgs)
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

        public void ItemPropertyChanged(object sender, PropertyChangedEventArgs changedEventArgs)
        {
            ShowJoinMessage = !AllUsers.Any(user => user.Active == true);
        }

        public void CreateTimer()
        {
            Timer.Tick += new System.EventHandler(OnFlipCenterpiece);
            Timer.Interval = new System.TimeSpan(0, 1, 0);
            Timer.Start();
        }

        public void OnFlipCenterpiece(object sender, System.EventArgs e)
        {
            CenterpieceIsFlipped = !CenterpieceIsFlipped;
        }
    }
}
