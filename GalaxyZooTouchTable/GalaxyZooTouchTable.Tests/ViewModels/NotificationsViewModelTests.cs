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
        TableUser BlueUser = new BlueUser();
        TableUser PinkUser = new PinkUser();
        TableUser PersonUser = new PersonUser();

        public NotificationsViewModelTests()
        {
            _viewModel = new NotificationsViewModel(BlueUser);
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
            NotificationAvatarViewModel Avatar = new NotificationAvatarViewModel(BlueUser);
            Assert.DoesNotContain(Avatar, _viewModel.AvailableUsers);
        }

        [Fact]
        void ShouldNotifyDragToStart()
        {
            _viewModel.NotifyUser.Execute(PinkUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Drag a galaxy above into your classifier to begin!", _viewModel.Overlay.MessageOne);
        }

        [Fact]
        void ShouldntNotifyIfUserInactive()
        {
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            PinkUser.Busy = true;
            _viewModel.NotifyUser.Execute(PinkUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Sorry,", _viewModel.Overlay.MessageOne);
            Assert.Equal("is not at the table. Ask someone else?", _viewModel.Overlay.MessageTwo);
        }

        [Fact]
        void ShouldNotifyIfUserAlreadySeen()
        {
            Messenger.Default.Send(PanoptesServiceMockData.CompletedClassification(), "PinkUser_AddCompletedClassification");
            PinkUser.Active = true;
            _viewModel.ReceivedNewSubject(PanoptesServiceMockData.TableSubject());
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            NotificationAvatarViewModel PinkAvatar = _viewModel.AvailableUsers.Find(x => x.User.Name == "PinkUser");
            _viewModel.NotifyUser.Execute(PinkUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Sorry,", _viewModel.Overlay.MessageOne);
            Assert.Equal("has already classified that galaxy.", _viewModel.Overlay.MessageTwo);
        }

        [Fact]
        void ShouldNotifyIfUserBusy()
        {
            PinkUser.Active = true;
            PinkUser.Busy = true;
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            _viewModel.NotifyUser.Execute(PinkUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Sorry,", _viewModel.Overlay.MessageOne);
            Assert.Equal("is busy working with another user.", _viewModel.Overlay.MessageTwo);
        }

        [Fact]
        void ShouldNotifyIfAlreadyWorkingWith()
        {
            HelpNotification Notification = new HelpNotification(PinkUser, HelpNotificationStatus.AskForHelp, "1");
            Messenger.Default.Send(Notification, "BlueUser_PostNotification");
            _viewModel.NotifyUser.Execute(PinkUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("You are already working with", _viewModel.Overlay.MessageOne);
        }

        [Fact]
        void ShouldNotifyIfPendingRequest()
        {
            PersonUser.Active = true;
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            HelpNotification Notification = new HelpNotification(PinkUser, HelpNotificationStatus.AskForHelp, "1");
            Messenger.Default.Send(Notification, "BlueUser_PostNotification");
            _viewModel.NotifyUser.Execute(PersonUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("You must respond to your current help request.", _viewModel.Overlay.MessageOne);
        }

        [Fact]
        void ShouldNotifyIfAlreadyAsked()
        {
            PinkUser.Active = true;
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            _viewModel.NotifyUser.Execute(PinkUser);
            Assert.Null(_viewModel.Overlay);
            HelpNotification Notification = new HelpNotification(PinkUser, HelpNotificationStatus.Decline);
            Messenger.Default.Send(Notification, "BlueUser_PostNotification");
            _viewModel.NotifyUser.Execute(PinkUser);
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Sorry, you have already asked", _viewModel.Overlay.MessageOne);
            Assert.Equal("for help.", _viewModel.Overlay.MessageTwo);
        }

        [Fact]
        void ShouldClearRequestsWhenDecliningGalaxies()
        {
            HelpNotification Notification = new HelpNotification(PinkUser, HelpNotificationStatus.AskForHelp);
            Messenger.Default.Send(Notification, "BlueUser_PostNotification");
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
            HelpNotification Notification = new HelpNotification(PinkUser, HelpNotificationStatus.AskForHelp, "1");
            Messenger.Default.Send(Notification, "BlueUser_PostNotification");
            Assert.NotNull(_viewModel.NotificationPanel);
            _viewModel.AcceptGalaxy.Execute(null);
            Assert.Equal(_viewModel.UserHelping, PinkUser);
            Assert.Null(_viewModel.NotificationPanel);
        }

        [Fact]
        void ShouldWarnAcceptingInviteClearsGalaxy()
        {
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            HelpNotification Notification = new HelpNotification(PinkUser, HelpNotificationStatus.AskForHelp, "1");
            Messenger.Default.Send(Notification, "BlueUser_PostNotification");
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
            _viewModel.UserHelping = PinkUser;
            _viewModel.ReceivedNewSubject(PanoptesServiceMockData.TableSubject());
            Assert.Null(_viewModel.UserHelping);
            Assert.Null(_viewModel.Overlay);
            Assert.Null(_viewModel.NotificationPanel);
        }

        [Fact]
        void ShouldNotifyWhenUserIsLeaving()
        {
            HelpNotification Notification = new HelpNotification(PinkUser, HelpNotificationStatus.AskForHelp);
            Messenger.Default.Send(Notification, "BlueUser_PostNotification");
            HelpNotification LeavingNotification = new HelpNotification(PinkUser, HelpNotificationStatus.Leaving);
            Messenger.Default.Send(LeavingNotification, "UserLeaving");
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Sorry,", _viewModel.Overlay.MessageOne);
            Assert.Equal("has left the table.", _viewModel.Overlay.MessageTwo);
        }

        [Fact]
        void NotifyWhenUserHasAnswered()
        {
            NotificationsViewModel PinkNotifier = new NotificationsViewModel(PinkUser);
            PinkUser.Active = true;
            _viewModel.OnSubjectStatusChange(SubjectViewEnum.MatchedSubject);
            _viewModel.ReceivedNewSubject(PanoptesServiceMockData.TableSubject());
            _viewModel.NotifyUser.Execute(PinkUser);
            PinkNotifier.AcceptGalaxy.Execute(null);
            PinkNotifier.HandleAnswer(PanoptesServiceMockData.CompletedClassification());
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("Check it out,", _viewModel.Overlay.MessageOne);
            Assert.Equal("made a classification!", _viewModel.Overlay.MessageTwo);
            Assert.NotNull(_viewModel.NotificationPanel);
            Assert.Equal(NotificationPanelStatus.ShowAnswer, _viewModel.NotificationPanel.Status);
        }

        [Fact]
        void ShouldNotifyAlreadyAnswered()
        {
            _viewModel.AlreadySeen();
            Assert.NotNull(_viewModel.Overlay);
            Assert.Equal("You have already classified this galaxy.", _viewModel.Overlay.MessageOne);
        }
    }
}
