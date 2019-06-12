using GalaxyZooTouchTable.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

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

        protected GlobalData()
        {
            PopulateUsers();
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
