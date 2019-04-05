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
        public TableUser PersonUser = new PersonUser();
        public TableUser LightUser = new LightUser();
        public TableUser StarUser = new StarUser();
        public TableUser HeartUser = new HeartUser();
        public TableUser FaceUser = new FaceUser();
        public TableUser EarthUser = new EarthUser();

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
            AllUsers.Add(PersonUser);
            AllUsers.Add(FaceUser);
            AllUsers.Add(LightUser);
            AllUsers.Add(StarUser);
            AllUsers.Add(HeartUser);
            AllUsers.Add(EarthUser);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
