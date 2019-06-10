using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public List<NotificationAvatarViewModel> AvailableUsers { get; private set; } = new List<NotificationAvatarViewModel>();
        public event Action<ClassifierViewEnum> ChangeView = delegate { };
        public event Action<string> GetSubjectById = delegate { };
        public ObservableCollection<PendingRequest> PendingRequests { get; set; } = new ObservableCollection<PendingRequest>(); 
        public TableUser User { get; set; }

        string CurrentSubjectId { get; set; }
        bool CurrentlyClassifying { get; set; }
        ObservableCollection<TableUser> UsersAlreadyAsked { get; set; } = new ObservableCollection<TableUser>();

        private TableUser _userHelping;
        public TableUser UserHelping
        {
            get => _userHelping;
            set => SetProperty(ref _userHelping, value);
        }

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

        public NotificationsViewModel(TableUser user)
        {
            User = user;
            RegisterMessengerActions(user);
            LoadCommands();
            FilterCurrentUserAndOrganize();
            PendingRequests.CollectionChanged += UpdateAvatarsOnHelpRequest;
            UsersAlreadyAsked.CollectionChanged += UpdateIconsWhenAsked;
        }

        void UpdateIconsWhenAsked(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (NotificationAvatarViewModel Avatar in AvailableUsers)
            {
                bool AvatarAlreadyAsked = UsersAlreadyAsked.Any(x => x == Avatar.User);
                if (AvatarAlreadyAsked)
                {
                    Avatar.ShowDisabled();
                } else
                {
                    Avatar.ShowEnabled();
                }
            }
        }

        void UpdateAvatarsOnHelpRequest(object sender, NotifyCollectionChangedEventArgs e)
        {
            User.Busy = PendingRequests.Count > 0;
            UpdateAvatarsViaRequests();
        }

        void UpdateAvatarsViaRequests()
        {
            foreach (NotificationAvatarViewModel Avatar in AvailableUsers)
            {
                Avatar.ResetIcons();
                PendingRequest Request = PendingRequests.SingleOrDefault(x => x.CooperatingPeer == Avatar.User);
                if (Request != null && !Request.Assisting)
                {
                    Avatar.ShowQuestionMark();
                }
                else if (Request != null)
                {
                    Avatar.ShowExclamationPoint(UserHelping);
                }
            } 
        }

        void RegisterMessengerActions(TableUser user)
        {
            Messenger.Default.Register<bool>(this, OnSubjectStatusChange, $"{user.Name}_SubjectStatus");
            Messenger.Default.Register<HelpNotification>(this, OnReceiveNotification, $"{user.Name}_PostNotification");
            Messenger.Default.Register<HelpNotification>(this, OnReceiveNotification, $"UserLeaving");
        }

        void LoadCommands()
        {
            AcceptGalaxy = new CustomCommand(OnAcceptGalaxy);
            ClearOverlay = new CustomCommand(OnClearOverlay);
            DeclineGalaxy = new CustomCommand(OnDeclineGalaxy);
            NotifyUser = new CustomCommand(OnNotifyUser);
            ToggleNotifier = new CustomCommand(OnToggleNotifier);
        }

        void OnReceiveNotification(HelpNotification notification)
        {
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

        void OnAskForHelp(HelpNotification notification)
        {
            PendingRequests.Add(new PendingRequest(notification.SentBy, true, notification.SubjectId));
            NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowRequest, notification.SentBy.Avatar);
        }

        void OnDeclinedHelp(HelpNotification notification)
        {
            PendingRequests.Remove(PendingRequests.SingleOrDefault(x => x.CooperatingPeer == notification.SentBy));
            Overlay = new NotificationOverlay("Sorry,", "declined your invitation. Ask someone else?", notification.SentBy.Avatar);
        }

        void OnAcceptedHelp(HelpNotification notification)
        {
            Overlay = new NotificationOverlay("Hooray", "accepted your invitation!", notification.SentBy.Avatar);
        }

        void OnReceiveAnswer(HelpNotification notification)
        {
            Overlay = new NotificationOverlay("Check it out,", "made a classification!", notification.SentBy.Avatar);
            NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowAnswer, notification.SentBy.Avatar, notification.Classification.Answer);
            PendingRequests.Remove(PendingRequests.SingleOrDefault(x => x.CooperatingPeer == notification.SentBy));
        }

        void OnUserLeaving(HelpNotification notification)
        {
            PendingRequest Request = PendingRequests.SingleOrDefault(x => x.CooperatingPeer == notification.SentBy);
            if (Request != null)
            {
                Overlay = new NotificationOverlay("Sorry,", "has left the table.", notification.SentBy.Avatar);
                PendingRequests.Remove(Request);
                NotificationPanel = null;
            }
            UsersAlreadyAsked.Remove(notification.SentBy);
        }

        /// <summary>
        /// This method serves two functions, filter the current user from notification avatars
        /// and reorganize the remaining avatars for how they would appear clockwise around the table
        /// for the current user.
        /// </summary>
        void FilterCurrentUserAndOrganize()
        {
            int IndexOfUser = GlobalData.GetInstance().AllUsers.IndexOf(User);
            for (int i = IndexOfUser; i < GlobalData.GetInstance().AllUsers.Count - 1; i ++)
            {
                TableUser CurrentUser = GlobalData.GetInstance().AllUsers[i + 1];
                AvailableUsers.Add(new NotificationAvatarViewModel(CurrentUser));
            }
            for (int i = 0; i < IndexOfUser; i++)
            {
                TableUser CurrentUser = GlobalData.GetInstance().AllUsers[i];
                AvailableUsers.Add(new NotificationAvatarViewModel(CurrentUser));
            }
        }

        void ResetNotifications(bool hardReset = true)
        {
            if (hardReset || !(NotificationPanel != null && NotificationPanel.Status == NotificationPanelStatus.ShowRequest))
                NotificationPanel = null;
            Overlay = null;
            if (hardReset) PendingRequests.Clear();
            UsersAlreadyAsked.Clear();
            UserHelping = null;
        }

        void OnNotifyUser(object sender)
        {
            TableUser UserToNotify = sender as TableUser;
            GlobalData.GetInstance().Logger.AddEntry("Ask_For_Help");

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
        }

        bool IsCurrentlyWorkingWith(TableUser UserToNotify)
        {
            bool AlreadyWorkingWith = PendingRequests.Any(x => x.CooperatingPeer == UserToNotify);
            if (AlreadyWorkingWith)
            {
                Overlay = new NotificationOverlay("You are already working with", null, UserToNotify.Avatar);
            }
            return AlreadyWorkingWith;
        }

        bool CannotAskForHelp()
        {
            if (!CurrentlyClassifying)
            {
                Overlay = new NotificationOverlay("Drag a galaxy above into your classifier to begin!");
            }
            return !CurrentlyClassifying;
        }

        bool UserIsUnavailable(TableUser userToNotify)
        {
            if (!userToNotify.Active)
            {
                Overlay = new NotificationOverlay("Sorry,", "is not at the table. Ask someone else?", userToNotify.Avatar);
            }
            return !userToNotify.Active;
        }

        bool AlreadyAskedUser(TableUser userToNotify)
        {
            bool AlreadyAsked = UsersAlreadyAsked.Any(x => x == userToNotify);
            if (AlreadyAsked)
            {
                Overlay = new NotificationOverlay("Sorry, you have already asked", "for help.", userToNotify.Avatar);
            }
            return AlreadyAsked;
        }

        bool UserHasAlreadySeen(TableUser userToNotify)
        {
            NotificationAvatarViewModel UserToQuestion = AvailableUsers.Find(x => x.User.Name == userToNotify.Name);
            CompletedClassification FinishedClassification = UserToQuestion.HasAlreadyClassified(CurrentSubjectId);
            if (FinishedClassification != null)
            {
                Overlay = new NotificationOverlay("Sorry,", "has already classified that galaxy.", userToNotify.Avatar);
                NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowAnswer, userToNotify.Avatar, FinishedClassification.Answer);
            }
            return FinishedClassification != null;
        }

        bool UserIsBusy(TableUser userToNotify)
        {
            if (userToNotify.Busy)
            {
                Overlay = new NotificationOverlay("Sorry,", "is busy working with another user.", userToNotify.Avatar);
            }
            return userToNotify.Busy;
        }

        bool PendingRequest()
        {
            bool RequestIsPending = NotificationPanel != null && NotificationPanel.Status == NotificationPanelStatus.ShowRequest;
            if (RequestIsPending)
            {
                Overlay = new NotificationOverlay("You must respond to your current help request.");
            }
            return RequestIsPending;
        }

        void OnAcceptGalaxy(object sender)
        {
            if (CurrentlyClassifying && NotificationPanel.Status != NotificationPanelStatus.ShowWarning)
            {
                NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowWarning);
                return;
            }

            PendingRequest Request = PendingRequests.First();
            UserHelping = Request.CooperatingPeer;
            UpdateAvatarsViaRequests();
            Overlay = null;
            NotificationPanel = null;
            ChangeView(ClassifierViewEnum.SubjectView);
            GetSubjectById(Request.SubjectId);
            HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.Accepted);
            Messenger.Default.Send(Notification, $"{Request.CooperatingPeer.Name}_PostNotification");
            GlobalData.GetInstance().Logger.AddEntry("Accept_Galaxy");
        }

        void OnDeclineGalaxy(object sender)
        {
            PendingRequest Request = PendingRequests.First();
            HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.Decline);
            Messenger.Default.Send(Notification, $"{Request.CooperatingPeer.Name}_PostNotification");
            ResetNotifications();
            GlobalData.GetInstance().Logger.AddEntry("Decline_Galaxy");
        }

        void OnClearOverlay(object sender)
        {
            Overlay = null;
        }

        void OnToggleNotifier(object sender)
        {
            NotifierIsOpen = !NotifierIsOpen;
        }

        public void AlreadySeen()
        {
            Overlay = new NotificationOverlay("You have already classified this galaxy.");
        }

        public void ReceivedNewSubject(TableSubject subject)
        {
            bool HardReset = false;
            ResetNotifications(HardReset);
            CurrentSubjectId = subject.Id;
        }

        public void HandleAnswer(CompletedClassification classification)
        {
            PendingRequest AwaitingRequest = PendingRequests.SingleOrDefault(x => x.SubjectId == classification.SubjectId && x.Assisting);
            if (AwaitingRequest != null)
            {
                HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.SendAnswer, classification);
                Messenger.Default.Send(Notification, $"{AwaitingRequest.CooperatingPeer.Name}_PostNotification");
                PendingRequests.Remove(AwaitingRequest);
                GlobalData.GetInstance().Logger.AddEntry("Helped_User");
            }
            bool HardReset = false;
            ResetNotifications(HardReset);
        }

        public void NotifyLeaving()
        {
            HelpNotification Notification = new HelpNotification(User, HelpNotificationStatus.Leaving);
            Messenger.Default.Send(Notification, $"UserLeaving");
            ResetNotifications();
        }

        public void OnSubjectStatusChange(bool isClassifying)
        {
            Overlay = null;
            CurrentlyClassifying = isClassifying;
        }
    }
}
