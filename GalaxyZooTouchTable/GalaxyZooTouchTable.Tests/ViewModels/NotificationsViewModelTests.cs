using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Tests.Mock;
using GalaxyZooTouchTable.ViewModels;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class NotificationsViewModelTests
    {
        private NotificationsViewModel _viewModel;
        TableUser StarUser = new StarUser();
        TableUser HeartUser = new HeartUser();
        TableUser PersonUser = new PersonUser();

        public NotificationsViewModelTests()
        {
            _viewModel = new NotificationsViewModel(StarUser);
        }

        [Fact]
        void ShouldInitializeWithDefaultValues()
        {
            Assert.False(_viewModel.NotifierIsOpen);
            Assert.False(_viewModel.NotifierIsOpen);
            Assert.NotNull(_viewModel.AvailableUsers);
        }

        [Fact]
        void ShouldFilterUserFromAvailableUsers()
        {
            NotificationAvatarViewModel Avatar = new NotificationAvatarViewModel(StarUser);
            Assert.DoesNotContain(Avatar, _viewModel.AvailableUsers);
        }

        [Fact]
        void ShouldNotifyDragToStart()
        {
            _viewModel.NotifyUser.Execute(HeartUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Drag a galaxy above into your classifier to begin!", _viewModel.Overlay.MessageOne);
        }

        [Fact]
        void ShouldntNotifyIfUserInactive()
        {
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            HeartUser.Busy = true;
            _viewModel.NotifyUser.Execute(HeartUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Sorry,", _viewModel.Overlay.MessageOne);
            Assert.Equal("is not at the table. Ask someone else?", _viewModel.Overlay.MessageTwo);
        }

        [Fact]
        void ShouldNotifyIfUserAlreadySeen()
        {
            Messenger.Default.Send(PanoptesServiceMockData.CompletedClassification(), "HeartUser_AddCompletedClassification");
            HeartUser.Active = true;
            _viewModel.ReceivedNewSubject(PanoptesServiceMockData.Subject());
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            NotificationAvatarViewModel HeartAvatar = _viewModel.AvailableUsers.Find(x => x.User.Name == "HeartUser");
            _viewModel.NotifyUser.Execute(HeartUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Sorry,", _viewModel.Overlay.MessageOne);
            Assert.Equal("has already classified that galaxy.", _viewModel.Overlay.MessageTwo);
        }

        [Fact]
        void ShouldNotifyIfUserBusy()
        {
            HeartUser.Active = true;
            HeartUser.Busy = true;
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            _viewModel.NotifyUser.Execute(HeartUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Sorry,", _viewModel.Overlay.MessageOne);
            Assert.Equal("is busy working with another user.", _viewModel.Overlay.MessageTwo);
        }

        [Fact]
        void ShouldNotifyIfAlreadyWorkingWith()
        {
            HelpNotification Notification = new HelpNotification(HeartUser, HelpNotificationStatus.AskForHelp, "1");
            Messenger.Default.Send(Notification, "StarUser_PostNotification");
            _viewModel.NotifyUser.Execute(HeartUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("You are already working with", _viewModel.Overlay.MessageOne);
        }

        [Fact]
        void ShouldNotifyIfPendingRequest()
        {
            PersonUser.Active = true;
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            HelpNotification Notification = new HelpNotification(HeartUser, HelpNotificationStatus.AskForHelp, "1");
            Messenger.Default.Send(Notification, "StarUser_PostNotification");
            _viewModel.NotifyUser.Execute(PersonUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("You must respond to your current help request.", _viewModel.Overlay.MessageOne);
        }

        [Fact]
        void ShouldNotifyIfAlreadyAsked()
        {
            HeartUser.Active = true;
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            _viewModel.NotifyUser.Execute(HeartUser);
            Assert.Null(_viewModel.Overlay);
            HelpNotification Notification = new HelpNotification(HeartUser, HelpNotificationStatus.Decline);
            Messenger.Default.Send(Notification, "StarUser_PostNotification");
            _viewModel.NotifyUser.Execute(HeartUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Sorry, you have already asked", _viewModel.Overlay.MessageOne);
            Assert.Equal("for help.", _viewModel.Overlay.MessageTwo);
        }

        [Fact]
        void ShouldClearRequestsWhenDecliningGalaxies()
        {
            HelpNotification Notification = new HelpNotification(HeartUser, HelpNotificationStatus.AskForHelp);
            Messenger.Default.Send(Notification, "StarUser_PostNotification");
            Assert.NotNull(_viewModel.NotificationPanel);
            _viewModel.DeclineGalaxy.Execute(null);
            Assert.Empty(_viewModel.PendingRequests);
            Assert.Null(_viewModel.NotificationPanel);
        }

        [Fact]
        void ShouldClearOverlay()
        {
            NotificationOverlay TestOverlay = new NotificationOverlay("Message One", "Message Two");
            _viewModel.Overlay = TestOverlay;
            _viewModel.ClearOverlay.Execute(null);
        }

        [Fact]
        void ShouldAcceptRequestForHelp()
        {
            HelpNotification Notification = new HelpNotification(HeartUser, HelpNotificationStatus.AskForHelp, "1");
            Messenger.Default.Send(Notification, "StarUser_PostNotification");
            Assert.NotNull(_viewModel.NotificationPanel);
            _viewModel.AcceptGalaxy.Execute(null);
            Assert.Equal(_viewModel.UserHelping, HeartUser);
            Assert.Null(_viewModel.NotificationPanel);
        }

        [Fact]
        void ShouldWarnAcceptingInviteClearsGalaxy()
        {
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            HelpNotification Notification = new HelpNotification(HeartUser, HelpNotificationStatus.AskForHelp, "1");
            Messenger.Default.Send(Notification, "StarUser_PostNotification");
            Assert.NotNull(_viewModel.NotificationPanel);
            _viewModel.AcceptGalaxy.Execute(null);
            Assert.NotNull(_viewModel.NotificationPanel);
            Assert.Equal(NotificationPanelStatus.ShowWarning, _viewModel.NotificationPanel.Status);
            _viewModel.AcceptGalaxy.Execute(null);
            Assert.Null(_viewModel.NotificationPanel);
        }

        [Fact]
        void ShouldToggleNotification()
        {
            _viewModel.NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowRequest);
            Assert.True(_viewModel.NotifierIsOpen);
            _viewModel.ToggleNotifier.Execute(null);
            Assert.False(_viewModel.NotifierIsOpen);
        }

        [Fact]
        void ShouldSetNotificationsWhenNewSubjectReceived()
        {
            _viewModel.Overlay = new NotificationOverlay("This is a new overlay");
            _viewModel.NotificationPanel = new NotificationPanel(NotificationPanelStatus.ShowAnswer);
            _viewModel.UserHelping = HeartUser;
            _viewModel.ReceivedNewSubject(PanoptesServiceMockData.Subject());
            Assert.Null(_viewModel.UserHelping);
            Assert.Null(_viewModel.Overlay);
            Assert.Null(_viewModel.NotificationPanel);
        }

        [Fact]
        void ShouldNotifyWhenUserIsLeaving()
        {
            HelpNotification Notification = new HelpNotification(HeartUser, HelpNotificationStatus.AskForHelp);
            Messenger.Default.Send(Notification, "StarUser_PostNotification");
            HelpNotification LeavingNotification = new HelpNotification(HeartUser, HelpNotificationStatus.Leaving);
            Messenger.Default.Send(LeavingNotification, "UserLeaving");
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Sorry,", _viewModel.Overlay.MessageOne);
            Assert.Equal("has left the table.", _viewModel.Overlay.MessageTwo);
        }

        [Fact]
        void NotifyWhenUserHasAnswered()
        {
            NotificationsViewModel HeartNotifier = new NotificationsViewModel(HeartUser);
            HeartUser.Active = true;
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            _viewModel.ReceivedNewSubject(PanoptesServiceMockData.Subject());
            _viewModel.NotifyUser.Execute(HeartUser);
            HeartNotifier.AcceptGalaxy.Execute(null);
            HeartNotifier.HandleAnswer(PanoptesServiceMockData.CompletedClassification());
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Check it out,", _viewModel.Overlay.MessageOne);
            Assert.Equal("made a classification!", _viewModel.Overlay.MessageTwo);
            Assert.NotNull(_viewModel.NotificationPanel);
            Assert.Equal(NotificationPanelStatus.ShowAnswer, _viewModel.NotificationPanel.Status);
        }
    }
}
