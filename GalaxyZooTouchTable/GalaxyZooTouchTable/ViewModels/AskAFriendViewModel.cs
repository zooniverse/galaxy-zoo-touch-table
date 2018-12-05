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
        public ICommand CloseNotifier { get; set; }

        public ClassificationPanelViewModel Classifier;

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

        private TableUser _selectedItem;
        public TableUser SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyRaised("SelectedItem");
            }
        }

        public AskAFriendViewModel(TableUser user, ObservableCollection<TableUser> allUsers, ClassificationPanelViewModel classificationPanel)
        {
            Classifier = classificationPanel;
            AskAFriend = new AskAFriend(user, allUsers);
            Messenger.Default.Register<TableUser>(this, OnHelpRequested, user);
            LoadCommands();
        }

        private void OnHelpRequested(TableUser user)
        {
            System.Console.WriteLine(user);
        }

        private void LoadCommands()
        {
            NotifyUser = new CustomCommand(OnNotifyUser);
        }

        private void OnNotifyUser(object sender)
        {
            TableUser user = sender as TableUser;
            Messenger.Default.Send<TableUser>(AskAFriend.User, user);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
