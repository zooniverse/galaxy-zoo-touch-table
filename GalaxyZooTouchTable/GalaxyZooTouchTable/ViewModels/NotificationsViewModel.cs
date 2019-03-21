using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<TableUser> UsersAlreadyAsked { get; set; } = new List<TableUser>();
        private List<PendingRequest> PendingRequests { get; set; } = new List<PendingRequest>(); 

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
            Messenger.Default.Register<SubjectViewEnum>(this, OnSubjectStatusChange, $"{user.Name}_SubjectStatus");
            Messenger.Default.Register<HelpNotification>(this, OnReceiveNotification, $"{user.Name}_PostNotification");
            Messenger.Default.Register<HelpNotification>(this, OnReceiveNotification, $"UserLeaving");
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
                    OnReceiveAnswer(notification);
                    return;
                case HelpNotificationStatus.Leaving:
                    OnUserLeaving(notification);
                    return;
            }
        }

        /// <summary>
        /// Notification that the peer you are working with is leaving the table
        /// </summary>
        void OnUserLeaving(HelpNotification notification)
        {
            PendingRequest Request = PendingRequests.Find(x => x.CooperatingPeer == notification.SentBy);

            if (Request != null)
            {
                string firstMessage = "Sorry,";
                string secondMessage = "has left the table";
                Overlay = new NotificationOverlay(firstMessage, secondMessage, notification.SentBy.Avatar);
                PendingRequests.Remove(Request);
                User.Busy = PendingRequests.Count > 0;
                if (!User.Busy) NotificationPanel = null;
            }
            UsersAlreadyAsked.Remove(notification.SentBy);
        }

        bool IsWaitingForResponse(HelpNotification notification, bool IsWaitingForAnswer = false)
        {
            PendingRequest Request = PendingRequests.Find(x => x.CooperatingPeer == notification.SentBy);
            bool MatchingRequest = Request != null && Request.SubjectId == notification.SubjectId;

            if (IsWaitingForAnswer) return MatchingRequest && !Request.Assisting;
            return MatchingRequest;
        }

        bool IsWaitingForResponse(CompletedClassification classification, bool IsWaitingForAnswer = false)
        {
            PendingRequest Request = PendingRequests.Find(x => x.CooperatingPeer == classification.User);
            bool MatchingRequest = Request != null && Request.SubjectId == classification.SubjectId;

            if (IsWaitingForAnswer) return MatchingRequest && !Request.Assisting;
            return MatchingRequest;
        }

        TableUser FindPendingRequest(CompletedClassification classification)
        {
            PendingRequest Request = PendingRequests.Find(x => x.SubjectId == classification.SubjectId);
            return Request.CooperatingPeer;
        }

        void OnAcceptedHelp(HelpNotification notification)
        {
            string firstMessage = "Hooray";
            string secondMessage = "accepted your invitation!";
            Overlay = new NotificationOverlay(firstMessage, secondMessage, notification.SentBy.Avatar);
            User.Busy = PendingRequests.Count > 0;
        }

        private void OnReceiveAnswer(HelpNotification notification)
        {
            bool IsWaitingForAnswer = true;
            if (IsWaitingForResponse(notification.Classification, IsWaitingForAnswer))
            {
                string firstMessage = "Check it out,";
                string secondMessage = "made a classification!";
                Overlay = new NotificationOverlay(firstMessage, secondMessage, notification.SentBy.Avatar);
                NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowAnswer, notification.SentBy.Avatar, notification.Classification.Answer);
            }
            PendingRequests.RemoveAll(x => x.CooperatingPeer == notification.SentBy);
            User.Busy = PendingRequests.Count > 0;
        }

        private void OnDeclinedHelp(HelpNotification notification)
        {
            PendingRequests.RemoveAll(x => x.CooperatingPeer == notification.SentBy);
            User.Busy = PendingRequests.Count > 0;
            string firstMessage = "Sorry,";
            string secondMessage = "declined your invitation. Ask someone else?";
            Overlay = new NotificationOverlay(firstMessage, secondMessage, notification.SentBy.Avatar);
        }

        private void OnAskForHelp(HelpNotification notification)
        {
            PendingRequest Request = new PendingRequest(notification.SentBy, true, notification.SubjectId);
            PendingRequests.Add(Request);
            User.Busy = PendingRequests.Count > 0;
            NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowRequest, notification.SentBy.Avatar);
        }

        private void OnSubjectStatusChange(SubjectViewEnum status)
        {
            CurrentlyClassifying = status == SubjectViewEnum.MatchedSubject;
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
            User.Busy = false;
            NotificationPanel = null;
            Overlay = null;
            PendingRequests.Clear();
        }

        private void OnAcceptGalaxy(object sender)
        {
            if (CurrentlyClassifying && NotificationPanel.Status != NotificationPanelStatus.ShowWarning)
            {
                NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowWarning);
                return;
            }

            PendingRequest Request = PendingRequests.First();
            Overlay = null;
            NotificationPanel = null;
            ChangeView(ClassifierViewEnum.SubjectView);
            GetSubjectById(Request.SubjectId);
            HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.Accepted, Request.SubjectId);
            Messenger.Default.Send(Notification, $"{Request.CooperatingPeer.Name}_PostNotification");
        }

        private void OnDeclineGalaxy(object sender)
        {
            PendingRequest Request = PendingRequests.First();
            PendingRequests.Clear();
            HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.Decline);
            Messenger.Default.Send(Notification, $"{Request.CooperatingPeer.Name}_PostNotification");
            ResetNotifications();
        }

        private void OnNotifyUser(object sender)
        {
            TableUser UserToNotify = sender as TableUser;

            if (IsCurrentlyWorkingWith(UserToNotify)) return;
            if (CannotAskForHelp()) return;
            if (UserIsUnavailable(UserToNotify)) return;
            if (AlreadyAskedUser(UserToNotify)) return;
            if (UserHasAlreadySeen(UserToNotify)) return;
            if (UserIsBusy(UserToNotify)) return;
            if (PendingRequest()) return;

            UsersAlreadyAsked.Add(UserToNotify);
            HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.AskForHelp, CurrentSubjectId);
            Messenger.Default.Send(Notification, $"{UserToNotify.Name}_PostNotification");
            PendingRequest Request = new PendingRequest(UserToNotify, false, CurrentSubjectId);
            PendingRequests.Add(Request);
            User.Busy = PendingRequests.Count > 0;
        }

        private bool IsCurrentlyWorkingWith(TableUser UserToNotify)
        {
            bool AlreadyWorkingWith = PendingRequests.Any(x => x.CooperatingPeer == UserToNotify);
            if (AlreadyWorkingWith)
            {
                string firstMessage = "You are already working with";
                Overlay = new NotificationOverlay(firstMessage, null, UserToNotify.Avatar);
            }
            return AlreadyWorkingWith;
        }

        private bool PendingRequest()
        {
            bool RequestIsPending = NotificationPanel != null && NotificationPanel.Status == NotificationPanelStatus.ShowRequest;
            if (RequestIsPending)
            {
                string firstMessage = "You must respond to your current help request";
                Overlay = new NotificationOverlay(firstMessage);
            }
            return RequestIsPending;
        }

        private bool UserIsBusy(TableUser userToNotify)
        {
            if (userToNotify.Busy)
            {
                string firstMessage = "Sorry,";
                string secondMessage = "is busy working with another user";
                Overlay = new NotificationOverlay(firstMessage, secondMessage, userToNotify.Avatar);
            }
            return userToNotify.Busy;
        }

        private bool AlreadyAskedUser(TableUser userToNotify)
        {
            bool AlreadyAsked = UsersAlreadyAsked.Any(x => x == userToNotify);

            if (AlreadyAsked)
            {
                string firstMessage = "Sorry, you have already asked";
                string secondMessage = "for help";
                Overlay = new NotificationOverlay(firstMessage, secondMessage, userToNotify.Avatar);
            }

            return AlreadyAsked;
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

        private void OnToggleNotifier(object sender)
        {
            NotifierIsOpen = !NotifierIsOpen;
        }

        public void ReceivedNewSubject(TableSubject subject)
        {
            UsersAlreadyAsked.Clear();
            CurrentSubjectId = subject.Id;
        }

        public void HandleAnswer(CompletedClassification classification)
        {
            TableUser UserAwaitingHelp = FindPendingRequest(classification);
            if (UserAwaitingHelp != null)
            {
                HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.SendAnswer, classification);
                Messenger.Default.Send(Notification, $"{UserAwaitingHelp.Name}_PostNotification");
            }
            ResetNotifications();
        }

        public void NotifyLeaving()
        {
            HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.Leaving);
            Messenger.Default.Send(Notification, $"UserLeaving");
            ResetNotifications();
        }
    }
}
