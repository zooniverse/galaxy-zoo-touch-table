using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class AskAFriendViewModel : INotifyPropertyChanged
    {
        public ICommand NotifyUser { get; set; }

        private AskAFriend _askAFriend;
        public AskAFriend AskAFriend
        {
            get { return _askAFriend; }
            set
            {
                _askAFriend = value;
                OnPropertyRaised("AskAFriend");
            }
        }

        public AskAFriendViewModel(TableUser user, ObservableCollection<TableUser> allUsers)
        {
            AskAFriend = new AskAFriend(user, allUsers);
            LoadCommands();
        }

        private void LoadCommands()
        {
            NotifyUser = new CustomCommand(OnNotifyUser);
        }

        private void OnNotifyUser(object sender)
        {
            TableUser user = sender as TableUser;
            user.HelpNotification = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
