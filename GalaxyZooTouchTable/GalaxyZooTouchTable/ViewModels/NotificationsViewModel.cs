using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class NotificationsViewModel : ViewModelBase
    {
        public ICommand AcceptGalaxy { get; private set; }
        public ICommand DeclineGalaxy { get; private set; }
        public ICommand NotifyUser { get; private set; }
        public ICommand ResetNotifications { get; private set; }
        public ICommand ToggleButtonNotification { get; private set; }
        public ICommand ToggleNotifier { get; private set; }
        public event Action<string> GetSubjectById = delegate { };
        public event Action<ClassifierViewEnum> ChangeView = delegate { };
        public event Action<TableUser> SendRequestToUser = delegate { };
        string SubjectIdToExamine { get; set; }
        public TableUser User { get; private set; }

        public ObservableCollection<TableUser> AvailableUsers { get; private set; }

        private TableUser _cooperatingPeer;
        public TableUser CooperatingPeer
        {
            get => _cooperatingPeer;
            set => SetProperty(ref _cooperatingPeer, value);
        }

        private bool _hideButtonNotification = false;
        public bool HideButtonNotification
        {
            get => _hideButtonNotification;
            set => SetProperty(ref _hideButtonNotification, value);
        }

        private bool _openNotifier = false;
        public bool OpenNotifier
        {
            get => _openNotifier;
            set => SetProperty(ref _openNotifier, value);
        }

        private string _suggestedAnswer;
        public string SuggestedAnswer
        {
            get => _suggestedAnswer;
            set => SetProperty(ref _suggestedAnswer, value);
        }

        private SubjectViewEnum _subjectView = SubjectViewEnum.DragSubject;
        public SubjectViewEnum SubjectView
        {
            get => _subjectView;
            set => SetProperty(ref _subjectView, value);
        }

        public NotificationsViewModel(TableUser user)
        {
            User = user;
            RegisterMessengerActions(user);
            FilterCurrentUser();
            LoadCommands();
        }

        private void RegisterMessengerActions(TableUser user)
        {
            Messenger.Default.Register<AnswerButton>(this, OnAnswerReceived, $"{user.Name}_ReceivedAnswer");
            Messenger.Default.Register<NotificationRequest>(this, OnNotificationReceived, $"{user.Name}_ReceivedNotification");
            Messenger.Default.Register<TableUser>(this, OnPeerLeaving, $"{user.Name}_PeerLeaving");
            Messenger.Default.Register<SubjectViewEnum>(this, OnSubjectStatusChange, $"{user.Name}_SubjectStatus");
        }

        private void OnSubjectStatusChange(SubjectViewEnum status)
        {
            SubjectView = status;
        }

        private void OnPeerLeaving(TableUser user)
        {
            User.Status = NotificationStatus.PeerHasLeft;
            OpenNotifier = false;
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

        private void FilterCurrentUser()
        {
            ObservableCollection<TableUser> allUsersCopy = new ObservableCollection<TableUser>(GlobalData.GetInstance().AllUsers);
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
            ChangeView(ClassifierViewEnum.SubjectView);
            GetSubjectById(SubjectIdToExamine);
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
            SendRequestToUser(UserToNotify);
            CooperatingPeer = UserToNotify;
            OpenNotifier = false;
            User.Status = NotificationStatus.HelpRequestSent;
        }

        private void OnResetNotifications(object sender = null)
        {
            ClearNotifications(false);
        }

        public void ClearNotifications(bool UserLeaving = false)
        {
            if (UserLeaving && CooperatingPeer != null && User.Status != NotificationStatus.PeerHasLeft)
            {
                Messenger.Default.Send<TableUser>(User, $"{CooperatingPeer.Name}_PeerLeaving");
            }

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
            if (User.Status == NotificationStatus.PeerHasLeft)
            {
                ClearNotifications();
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
    }
}
