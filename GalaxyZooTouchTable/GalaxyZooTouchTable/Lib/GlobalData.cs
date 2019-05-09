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
        public TableUser AquaUser = new AquaUser();
        public TableUser StarUser = new StarUser();
        public TableUser PinkUser = new PinkUser();
        public TableUser FaceUser = new FaceUser();
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
            AllUsers.Add(PersonUser);
            AllUsers.Add(FaceUser);
            AllUsers.Add(AquaUser);
            AllUsers.Add(StarUser);
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
