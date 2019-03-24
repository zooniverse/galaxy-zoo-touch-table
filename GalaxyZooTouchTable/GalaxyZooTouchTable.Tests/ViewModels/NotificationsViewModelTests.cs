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
        public void ShouldInitializeWithDefaultValues()
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
            Assert.Equal("has already classified that galaxy", _viewModel.Overlay.MessageTwo);
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
            Assert.Equal("is busy working with another user", _viewModel.Overlay.MessageTwo);
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
            Assert.Equal("You must respond to your current help request", _viewModel.Overlay.MessageOne);
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
            Assert.Equal("for help", _viewModel.Overlay.MessageTwo);
        }

        //[Fact]
        //public void ShouldDeclineNotifications()
        //{
        //    TableUser HeartUser = new HeartUser();
        //    _viewModel.CooperatingPeer = HeartUser;

        //    _viewModel.DeclineGalaxy.Execute(null);
        //    Assert.Null(_viewModel.CooperatingPeer);
        //    Assert.False(_viewModel.NotifierIsOpen);
        //    Assert.Equal(NotificationStatus.Idle, _viewModel.User.Status);
        //}

        //[Fact]
        //public void ShouldNotifyUser()
        //{
        //    TableUser HeartUser = new HeartUser();
        //    var SendRequestToUserCalled = false;
        //    _viewModel.SendRequestToUser += (s) => SendRequestToUserCalled = true;
        //    _viewModel.NotifyUser.Execute(HeartUser);

        //    Assert.True(SendRequestToUserCalled);
        //    Assert.Equal(HeartUser, _viewModel.CooperatingPeer);
        //    Assert.False(_viewModel.NotifierIsOpen);
        //    Assert.Equal(NotificationStatus.HelpRequestSent, _viewModel.User.Status);
        //}

        //[Fact]
        //public void ShouldClearNotifications()
        //{
        //    _viewModel.ClearNotifications();

        //    Assert.Null(_viewModel.CooperatingPeer);
        //    Assert.False(_viewModel.NotifierIsOpen);
        //    Assert.Null(_viewModel.SuggestedAnswer);
        //    Assert.Equal(NotificationStatus.Idle, _viewModel.User.Status);
        //}

        //[Fact]
        //public void ShouldToggleNotificationWhenAnswerGiven()
        //{
        //    GlobalData.GetInstance().StarUser.Status = NotificationStatus.AnswerGiven;
        //    _viewModel.ToggleButtonNotification.Execute(null);
        //    Assert.False(_viewModel.HideButtonNotification);
        //    Assert.Equal(NotificationStatus.Idle, _viewModel.User.Status);
        //}

        //[Fact]
        //public void ShouldToggleNotificationWhenPeerHasLeft()
        //{
        //    Assert.False(_viewModel.HideButtonNotification);
        //    GlobalData.GetInstance().StarUser.Status = NotificationStatus.PeerHasLeft;
        //    _viewModel.ToggleButtonNotification.Execute(null);
        //    Assert.True(_viewModel.HideButtonNotification);
        //}

        //[Fact]
        //public void ShouldToggleNotifier()
        //{
        //    Assert.False(_viewModel.NotifierIsOpen);
        //    _viewModel.ToggleNotifier.Execute(null);
        //    Assert.True(_viewModel.NotifierIsOpen);
        //}

        //[Fact]
        //public void ShouldSendAnswerToUser()
        //{
        //    _viewModel.CooperatingPeer = new HeartUser();
        //    _viewModel.SendAnswerToUser(null);
        //    Assert.Equal(NotificationStatus.Idle, _viewModel.User.Status);
        //}
    }
}
