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
        public ICommand ResetNotifications { get; private set; }
        public ICommand ToggleNotifier { get; private set; }
        public event Action<string> GetSubjectById = delegate { };
        public event Action<ClassifierViewEnum> ChangeView = delegate { };
        public event Action<TableUser> SendRequestToUser = delegate { };
        string CurrentSubjectId { get; set; }
        public TableUser User { get; private set; }
        private bool CurrentlyClassifying { get; set; }

        public List<NotificationAvatar> AvailableUsers { get; private set; } = new List<NotificationAvatar>();

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
            CurrentlyClassifying = status == SubjectViewEnum.MatchedSubject;
        }

        private void OnPeerLeaving(TableUser user = null)
        {
            User.Status = NotificationStatus.PeerHasLeft;
            OpenNotifier = false;
        }

        private void OnNotificationReceived(NotificationRequest Request)
        {
            CooperatingPeer = Request.User;
            OpenNotifier = true;
            CurrentSubjectId = Request.SubjectID;
            User.Status = NotificationStatus.HelpRequestReceived;
        }

        private void OnAnswerReceived(AnswerButton Answer)
        {
            OpenNotifier = true;
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
            ResetNotifications = new CustomCommand(OnResetNotifications);
            ToggleNotifier = new CustomCommand(OnToggleNotifier);
        }

        private void OnAcceptGalaxy(object sender)
        {
            ChangeView(ClassifierViewEnum.SubjectView);
            GetSubjectById(CurrentSubjectId);
            CooperatingPeer.Status = NotificationStatus.AcceptedHelp;
            OpenNotifier = false;
            User.Status = NotificationStatus.HelpingUser;
        }

        private void OnDeclineGalaxy(object sender)
        {
            CooperatingPeer.Status = NotificationStatus.DeclinedHelp;
            CooperatingPeer = null;
            OpenNotifier = false;
            CurrentSubjectId = null;
            User.Status = NotificationStatus.Idle;
        }

        private void OnNotifyUser(object sender)
        {
            TableUser UserToNotify = sender as TableUser;

            if (CannotAskForHelp()) return;
            if (UserIsUnavailable(UserToNotify)) return;
            if (UserHasAlreadySeen(UserToNotify)) return;

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
            bool HasAlreadySeen = UserInQuestion.HasAlreadySeen(CurrentSubjectId);

            if (HasAlreadySeen)
            {
                string firstMessage = "Sorry,";
                string secondMessage = "has already classified that galaxy";
                Overlay = new NotificationOverlay(firstMessage, secondMessage, userToNotify.Avatar);
                //NotificationPanel = new NotificationPanelInfo(NotificationPanelEnum.ShowAnswer, avatar, Classification.Answer);
            }

            return HasAlreadySeen;
        }

        private void OnClearOverlay(object sender)
        {
            Overlay = null;
        }

        private void OnResetNotifications(object sender = null)
        {
            ClearNotifications(false);
        }

        public void ClearNotifications(bool UserLeaving = false)
        {
            if (UserLeaving && CooperatingPeer != null && User.Status != NotificationStatus.PeerHasLeft)
            {
                Messenger.Default.Send(User, $"{CooperatingPeer.Name}_PeerLeaving");
            }

            CooperatingPeer = null;
            OpenNotifier = false;
            SuggestedAnswer = null;
            User.Status = NotificationStatus.Idle;
        }

        private void OnToggleNotifier(object sender)
        {
            OpenNotifier = !OpenNotifier;
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
    }
}
