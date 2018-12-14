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
        public string SubjectIdToExamine { get; set; }

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

        private ClassificationPanelViewModel _classifier;
        public ClassificationPanelViewModel Classifier
        {
            get { return _classifier; }
            set
            {
                _classifier = value;
                OnPropertyRaised("Classifier");
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

        public NotificationsViewModel(TableUser user, ObservableCollection<TableUser> allUsers, ClassificationPanelViewModel classifier)
        {
            Classifier = classifier;
            User = user;

            RegisterMessengerActions(user);
            FilterCurrentUser(allUsers);
            LoadCommands();
        }

        private void RegisterMessengerActions(TableUser user)
        {
            Messenger.Default.Register<AnswerButton>(this, OnAnswerReceived, $"{user.Name}_ReceivedAnswer");
            Messenger.Default.Register<NotificationRequest>(this, OnNotificationReceived, $"{user.Name}_ReceivedNotification");
        }

        private void OnNotificationReceived(NotificationRequest Request)
        {
            CooperatingPeer = Request.User;
            OpenNotifier = true;
            SubjectIdToExamine = Request.SubjectID;
            User.Status = NotificationStatus.HelpRequestReceived;
        }

        private void OnAnswerReceived(AnswerButton Answer)
        {
            HideButtonNotification = false;
            OpenNotifier = true;
            SuggestedAnswer = Answer.Label;
            User.Status = NotificationStatus.AnswerGiven;
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
            Classifier.GetSubjectById(SubjectIdToExamine);
            CooperatingPeer.Status = NotificationStatus.AcceptedHelp;
            OpenNotifier = false;
            User.Status = NotificationStatus.HelpingUser;
        }

        private void OnDeclineGalaxy(object sender)
        {
            CooperatingPeer.Status = NotificationStatus.DeclinedHelp;
            CooperatingPeer = null;
            OpenNotifier = false;
            SubjectIdToExamine = null;
            User.Status = NotificationStatus.Idle;
        }

        private void OnNotifyUser(object sender)
        {
            TableUser UserToNotify = sender as TableUser;
            NotificationRequest Request = new NotificationRequest(User, Classifier.CurrentSubject.Id);

            Messenger.Default.Send<NotificationRequest>(Request, $"{UserToNotify.Name}_ReceivedNotification");
            CooperatingPeer = UserToNotify;
            OpenNotifier = false;
            User.Status = NotificationStatus.HelpRequestSent;
        }

        public void OnResetNotifications(object sender = null)
        {
            CooperatingPeer = null;
            OpenNotifier = false;
            SuggestedAnswer = null;
            User.Status = NotificationStatus.Idle;
        }

        private void OnToggleButtonNotification(object sender)
        {
            HideButtonNotification = !HideButtonNotification;
            if (User.Status == NotificationStatus.AnswerGiven)
            {
                User.Status = NotificationStatus.Idle;
                HideButtonNotification = false;
            }
        }

        private void OnToggleNotifier(object sender)
        {
            OpenNotifier = !OpenNotifier;
        }

        public void SendAnswerToUser(AnswerButton SelectedItem)
        {
            Messenger.Default.Send<AnswerButton>(SelectedItem, $"{CooperatingPeer.Name}_ReceivedAnswer");
            User.Status = NotificationStatus.Idle;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
