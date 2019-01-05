using GalaxyZooTouchTable.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GalaxyZooTouchTable.Lib
{
    public class CommonData : INotifyPropertyChanged
    {
        private static CommonData _instance = null;
        public ObservableCollection<TableUser> AllUsers { get; set; } = new ObservableCollection<TableUser>();
        public TableUser PersonUser = TableUserFactory.Create(UserType.Person);
        public TableUser LightUser = TableUserFactory.Create(UserType.Light);
        public TableUser StarUser = TableUserFactory.Create(UserType.Star);
        public TableUser HeartUser = TableUserFactory.Create(UserType.Heart);
        public TableUser FaceUser = TableUserFactory.Create(UserType.Face);
        public TableUser EarthUser = TableUserFactory.Create(UserType.Earth);

        protected CommonData()
        {
            PopulateUsers();
        }

        public static CommonData GetInstance()
        {
            if (_instance == null)
                _instance = new CommonData();

            return _instance;
        }

        private void PopulateUsers()
        {
            AllUsers.Add(PersonUser);
            AllUsers.Add(LightUser);
            AllUsers.Add(StarUser);
            AllUsers.Add(HeartUser);
            AllUsers.Add(FaceUser);
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
