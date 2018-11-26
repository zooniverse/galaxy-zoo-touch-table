using GalaxyZooTouchTable.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GalaxyZooTouchTable.ViewModels
{
    public class AskAFriendViewModel : INotifyPropertyChanged
    {
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
