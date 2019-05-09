using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Threading;
using System.Linq;

namespace GalaxyZooTouchTable.ViewModels
{
    public class CenterpieceViewModel : ViewModelBase
    {
        public DispatcherTimer Timer = new DispatcherTimer();
        public ObservableCollection<TableUser> AllUsers { get; set; } = new ObservableCollection<TableUser>();

        private bool _isDormant = true;
        public bool IsDormant
        {
            get => _isDormant;
            set
            {
                Messenger.Default.Send(value, "TableStateChanged");
                SetProperty(ref _isDormant, value);
            }
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
            AllUsers.Add(GlobalData.GetInstance().AquaUser);
            AllUsers.Add(GlobalData.GetInstance().BlueUser);
            AllUsers.Add(GlobalData.GetInstance().PinkUser);
            AllUsers.Add(GlobalData.GetInstance().FaceUser);
            AllUsers.Add(GlobalData.GetInstance().GreenUser);
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
            IsDormant = !AllUsers.Any(user => user.Active == true);
        }

        private void CreateTimer()
        {
            Timer.Tick += new System.EventHandler(OnFlipCenterpiece);
            Timer.Interval = new System.TimeSpan(0, 1, 0);
            Timer.Start();
        }

        private void OnFlipCenterpiece(object sender, System.EventArgs e)
        {
            CenterpieceIsFlipped = !CenterpieceIsFlipped;
        }
    }
}
