using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class NotificationsViewModel : INotifyPropertyChanged
    {
        public ICommand AcceptGalaxy { get; set; }
        public ICommand DeclineGalaxy { get; set; }
        public ICommand NotifyUser { get; set; }
        public ICommand ResetNotifications { get; set; }
        public ICommand ToggleButtonNotification { get; set; }
        public ICommand ToggleNotifier { get; set; }

        private ObservableCollection<TableUser> _availableUsers;
        public ObservableCollection<TableUser> AvailableUsers
        {
            get { return _availableUsers; }
            set
            {
                _availableUsers = value;
                OnPropertyRaised("AvailableUsers");
            }
        }

        private TableUser _cooperatingPeer;
        public TableUser CooperatingPeer
        {
            get { return _cooperatingPeer; }
            set
            {
                _cooperatingPeer = value;
                OnPropertyRaised("CooperatingPeer");
            }
        }

        private bool _hideButtonNotification = false;
        public bool HideButtonNotification
        {
            get { return _hideButtonNotification; }
            set
            {
                _hideButtonNotification = value;
                OnPropertyRaised("HideButtonNotification");
            }
        }

        private NotificationStatus _status = NotificationStatus.ClearNotifications;
        public NotificationStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyRaised("Status");
            }
        }

        private bool _openNotifier = false;
        public bool OpenNotifier
        {
            get { return _openNotifier; }
            set
            {
                _openNotifier = value;
                OnPropertyRaised("OpenNotifier");
            }
        }

        private string _suggestedAnswer;
        public string SuggestedAnswer
        {
            get { return _suggestedAnswer; }
            set
            {
                _suggestedAnswer = value;
                OnPropertyRaised("SuggestedAnswer");
            }
        }

        private TableUser _user;
        public TableUser User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyRaised("User");
            }
        }

        public NotificationsViewModel(TableUser user, ObservableCollection<TableUser> allUsers)
        {
            LoadCommands();
            User = user;
            FilterCurrentUser(allUsers);
        }

        private void FilterCurrentUser(ObservableCollection<TableUser> allUsers)
        {
            ObservableCollection<TableUser> allUsersCopy = new ObservableCollection<TableUser>(allUsers);
            foreach (TableUser tableUser in allUsersCopy)
            {
                if (User == tableUser)
                {
                    allUsersCopy.Remove(User);
                    break;
                }
            }
            AvailableUsers = allUsersCopy;
        }

        private void LoadCommands()
        {
            AcceptGalaxy = new CustomCommand(OnAcceptGalaxy);
            DeclineGalaxy = new CustomCommand(OnDeclineGalaxy);
            NotifyUser = new CustomCommand(OnNotifyUser);
            ResetNotifications = new CustomCommand(OnResetNotifications);
            ToggleButtonNotification = new CustomCommand(OnToggleButtonNotification);
            ToggleNotifier = new CustomCommand(OnToggleNotifier);
        }

        private void OnAcceptGalaxy(object sender)
        {
            OpenNotifier = false;
            CooperatingPeer.Classifier.Notifications.Status = NotificationStatus.AcceptedHelp;
            Status = NotificationStatus.HelpingUser;

            string NewSubjectId = CooperatingPeer.Classifier.CurrentSubject.Id;
            User.Classifier.GetSubjectById(NewSubjectId);
        }

        private void OnDeclineGalaxy(object sender)
        {
            OpenNotifier = false;
            CooperatingPeer.Classifier.Notifications.Status = NotificationStatus.DeclinedHelp;

            Status = NotificationStatus.ClearNotifications;
            CooperatingPeer = null;
        }

        private void OnNotifyUser(object sender)
        {
            TableUser UserToNotify = sender as TableUser;

            CooperatingPeer = UserToNotify;
            UserToNotify.Classifier.Notifications.CooperatingPeer = User;

            Status = NotificationStatus.HelpRequestSent;
            UserToNotify.Classifier.Notifications.Status = NotificationStatus.HelpRequestReceived;

            UserToNotify.Classifier.Notifications.OpenNotifier = true;
        }

        public void OnResetNotifications(object sender)
        {
            CooperatingPeer = null;
            OpenNotifier = false;
            Status = NotificationStatus.ClearNotifications;
            SuggestedAnswer = null;
        }

        private void OnToggleButtonNotification(object sender)
        {
            HideButtonNotification = !HideButtonNotification;
        }

        private void OnToggleNotifier(object sender)
        {
            OpenNotifier = !OpenNotifier;
        }

        public void SendAnswerToUser(AnswerButton SelectedItem)
        {
            CooperatingPeer.Classifier.Notifications.SuggestedAnswer = SelectedItem.Label;
            CooperatingPeer.Classifier.Notifications.HideButtonNotification = false;
            CooperatingPeer.Classifier.Notifications.Status = NotificationStatus.AnswerGiven;
            CooperatingPeer.Classifier.Notifications.OpenNotifier = true;
            Status = NotificationStatus.ClearNotifications;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
