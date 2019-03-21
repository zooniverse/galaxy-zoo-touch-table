using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class NotificationsViewModel : ViewModelBase
    {
        public ICommand AcceptGalaxy { get; private set; }
        public ICommand ClearOverlay { get; private set; }
        public ICommand DeclineGalaxy { get; private set; }
        public ICommand NotifyUser { get; private set; }
        public ICommand ToggleNotifier { get; private set; }
        public event Action<string> GetSubjectById = delegate { };
        public event Action<ClassifierViewEnum> ChangeView = delegate { };
        public event Action<TableUser> SendRequestToUser = delegate { };
        string CurrentSubjectId { get; set; }
        public TableUser User { get; private set; }
        private bool CurrentlyClassifying { get; set; }
        private HelpNotification CurrentNotification { get; set; }
        private bool WaitingForAnswer { get; set; } = false;

        public List<NotificationAvatar> AvailableUsers { get; private set; } = new List<NotificationAvatar>();

        private NotificationPanel _notificationPanel;
        public NotificationPanel NotificationPanel
        {
            get => _notificationPanel;
            set
            {
                NotifierIsOpen = value != null;
                SetProperty(ref _notificationPanel, value);
            }
        }

        private NotificationOverlay _overlay;
        public NotificationOverlay Overlay
        {
            get => _overlay;
            set => SetProperty(ref _overlay, value);
        }

        private TableUser _cooperatingPeer;
        public TableUser CooperatingPeer
        {
            get => _cooperatingPeer;
            set => SetProperty(ref _cooperatingPeer, value);
        }

        private bool _notifierIsOpen = false;
        public bool NotifierIsOpen
        {
            get => _notifierIsOpen;
            set => SetProperty(ref _notifierIsOpen, value);
        }

        private string _suggestedAnswer;
        public string SuggestedAnswer
        {
            get => _suggestedAnswer;
            set => SetProperty(ref _suggestedAnswer, value);
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
            Messenger.Default.Register<HelpNotification>(this, OnReceiveNotification, $"{user.Name}_PostNotification");
        }

        private void OnReceiveNotification(HelpNotification notification)
        {
            CurrentNotification = notification;

            switch (notification.Status)
            {
                case HelpNotificationStatus.AskForHelp:
                    OnAskForHelp(notification);
                    return;
                case HelpNotificationStatus.Decline:
                    OnDeclinedHelp(notification);
                    return;
                case HelpNotificationStatus.Accepted:
                    OnAcceptedHelp(notification);
                    return;
                case HelpNotificationStatus.SendAnswer:
                    if (WaitingForAnswer) OnReceiveAnswer(notification);
                    return;
            }
        }

        private void OnAcceptedHelp(HelpNotification notification)
        {
            WaitingForAnswer = true;
            string firstMessage = "Horray";
            string secondMessage = "accepted your invitation!";
            Overlay = new NotificationOverlay(firstMessage, secondMessage, notification.SentBy.Avatar);
        }

        private void OnReceiveAnswer(HelpNotification notification)
        {
            string firstMessage = "Check it out,";
            string secondMessage = "made a classification!";
            Overlay = new NotificationOverlay(firstMessage, secondMessage, notification.SentBy.Avatar);
            NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowAnswer, notification.SentBy.Avatar, notification.Classification.Answer);
        }

        private void OnDeclinedHelp(HelpNotification notification)
        {
            ResetNotifications();
            CooperatingPeer = null;
            string firstMessage = "Sorry,";
            string secondMessage = "declined your invitation. Ask someone else?";
            Overlay = new NotificationOverlay(firstMessage, secondMessage, CooperatingPeer.Avatar);
        }

        private void OnAskForHelp(HelpNotification notification)
        {
            CooperatingPeer = notification.SentBy;
            NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowRequest, notification.SentBy.Avatar);
        }

        private void OnSubjectStatusChange(SubjectViewEnum status)
        {
            CurrentlyClassifying = status == SubjectViewEnum.MatchedSubject;
        }

        private void OnPeerLeaving(TableUser user = null)
        {
            User.Status = NotificationStatus.PeerHasLeft;
            NotifierIsOpen = false;
        }

        private void OnNotificationReceived(NotificationRequest Request)
        {
            CooperatingPeer = Request.User;
            NotifierIsOpen = true;
            CurrentSubjectId = Request.SubjectID;
            User.Status = NotificationStatus.HelpRequestReceived;
        }

        private void OnAnswerReceived(AnswerButton Answer)
        {
            NotifierIsOpen = true;
            SuggestedAnswer = Answer.Label;
            User.Status = NotificationStatus.AnswerGiven;
        }

        private void FilterCurrentUser()
        {
            foreach (TableUser tableUser in GlobalData.GetInstance().AllUsers)
            {
                if (User != tableUser)
                {
                    AvailableUsers.Add(new NotificationAvatar(tableUser));
                }
            }
        }

        private void LoadCommands()
        {
            AcceptGalaxy = new CustomCommand(OnAcceptGalaxy);
            ClearOverlay = new CustomCommand(OnClearOverlay);
            DeclineGalaxy = new CustomCommand(OnDeclineGalaxy);
            NotifyUser = new CustomCommand(OnNotifyUser);
            ToggleNotifier = new CustomCommand(OnToggleNotifier);
        }

        private void ResetNotifications()
        {
            NotificationPanel = null;
            Overlay = null;
            WaitingForAnswer = false;
        }

        private void OnAcceptGalaxy(object sender)
        {
            ChangeView(ClassifierViewEnum.SubjectView);
            GetSubjectById(CurrentNotification.SubjectId);
            ResetNotifications();
            HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.Accepted);
            Messenger.Default.Send(Notification, $"{CooperatingPeer.Name}_PostNotification");
            //CooperatingPeer.Status = NotificationStatus.AcceptedHelp;
            //NotifierIsOpen = false;
            //User.Status = NotificationStatus.HelpingUser;
        }

        private void OnDeclineGalaxy(object sender)
        {
            HelpNotification Notification = new HelpNotification(CooperatingPeer, HelpNotificationStatus.Decline);
            Messenger.Default.Send(Notification, $"{CooperatingPeer.Name}_PostNotification");
            NotificationPanel = null;
            //CooperatingPeer.Status = NotificationStatus.DeclinedHelp;
            //CooperatingPeer = null;
            //NotifierIsOpen = false;
            //CurrentSubjectId = null;
            //User.Status = NotificationStatus.Idle;
        }

        private void OnNotifyUser(object sender)
        {
            TableUser UserToNotify = sender as TableUser;

            if (CannotAskForHelp()) return;
            if (UserIsUnavailable(UserToNotify)) return;
            if (UserHasAlreadySeen(UserToNotify)) return;

            CooperatingPeer = UserToNotify;
            HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.AskForHelp, CurrentSubjectId);
            Messenger.Default.Send(Notification, $"{UserToNotify.Name}_PostNotification");
            //SendRequestToUser(UserToNotify);
            //CooperatingPeer = UserToNotify;
            //OpenNotifier = false;
            //User.Status = NotificationStatus.HelpRequestSent;
        }

        private bool CannotAskForHelp()
        {
            if (!CurrentlyClassifying)
            {
                string message = "Drag a galaxy above into your classifier to begin!";
                Overlay = new NotificationOverlay(message);
            }
            return !CurrentlyClassifying;
        }

        private bool UserIsUnavailable(TableUser userToNotify)
        {
            if (!userToNotify.Active)
            {
                string firstMessage = "Sorry,";
                string secondMessage = "is not at the table. Ask someone else?";
                Overlay = new NotificationOverlay(firstMessage, secondMessage, userToNotify.Avatar);
            }
            return !userToNotify.Active;
        }

        private bool UserHasAlreadySeen(TableUser userToNotify)
        {
            NotificationAvatar UserInQuestion = AvailableUsers.Find(x => x.User == userToNotify);
            CompletedClassification FinishedClassification = UserInQuestion.HasAlreadySeen(CurrentSubjectId);

            if (FinishedClassification != null)
            {
                string firstMessage = "Sorry,";
                string secondMessage = "has already classified that galaxy";
                Overlay = new NotificationOverlay(firstMessage, secondMessage, userToNotify.Avatar);
                NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowAnswer, userToNotify.Avatar, FinishedClassification.Answer);
            }

            return FinishedClassification != null;
        }

        private void OnClearOverlay(object sender)
        {
            Overlay = null;
        }

        public void ClearNotifications(bool UserLeaving = false)
        {
            if (UserLeaving && CooperatingPeer != null && User.Status != NotificationStatus.PeerHasLeft)
            {
                Messenger.Default.Send(User, $"{CooperatingPeer.Name}_PeerLeaving");
            }

            CooperatingPeer = null;
            NotifierIsOpen = false;
            SuggestedAnswer = null;
            User.Status = NotificationStatus.Idle;
        }

        private void OnToggleNotifier(object sender)
        {
            NotifierIsOpen = !NotifierIsOpen;
        }

        public void SendAnswerToUser(AnswerButton SelectedItem)
        {
            Messenger.Default.Send(SelectedItem, $"{CooperatingPeer.Name}_ReceivedAnswer");
            User.Status = NotificationStatus.Idle;
        }

        public void ReceivedNewSubject(TableSubject subject)
        {
            CurrentSubjectId = subject.Id;
        }

        public void HandleAnswer(CompletedClassification classification)
        {
            ResetNotifications();

            if (CooperatingPeer != null)
            {
                HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.SendAnswer, classification);
                Messenger.Default.Send(Notification, $"{CooperatingPeer.Name}_PostNotification");
            }
        }
    }
}
