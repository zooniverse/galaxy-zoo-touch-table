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

        public NotificationsViewModelTests()
        {
            TableUser StarUser = GlobalData.GetInstance().StarUser;
            _viewModel = new NotificationsViewModel(StarUser);
        }

        [Fact]
        public void ShouldInitializeWithDefaultValues()
        {
            Assert.False(_viewModel.HideButtonNotification);
            Assert.False(_viewModel.OpenNotifier);
            Assert.Null(_viewModel.SuggestedAnswer);
            Assert.Null(_viewModel.CooperatingPeer);
        }

        [Fact]
        public void ShouldFilterUserFromAvailableUsers()
        {
            TableUser StarUser = GlobalData.GetInstance().StarUser;
            Assert.DoesNotContain(StarUser, _viewModel.AvailableUsers);
        }

        [Fact]
        public void ShouldCloseNotifierWhenPeerHasLeft()
        {
            _viewModel.OnPeerLeaving();
            Assert.False(_viewModel.OpenNotifier);
            Assert.Equal(NotificationStatus.PeerHasLeft, _viewModel.User.Status);
        }

        [Fact]
        public void ShouldHandleNotificationRequest()
        {
            TableUser HeartUser = new HeartUser();
            string SubjectID = "1";
            NotificationRequest Request = new NotificationRequest(HeartUser, SubjectID);

            _viewModel.OnNotificationReceived(Request);
            Assert.Equal(HeartUser, _viewModel.CooperatingPeer);
            Assert.True(_viewModel.OpenNotifier);
            Assert.Equal(SubjectID, _viewModel.SubjectIdToExamine);
            Assert.Equal(NotificationStatus.HelpRequestReceived, _viewModel.User.Status);
        }

        [Fact]
        public void ShouldReceiveAnswerRequest()
        {
            AnswerButton AnswerSelected = PanoptesServiceMockData.AnswerButton();
            _viewModel.OnAnswerReceived(AnswerSelected);
            Assert.False(_viewModel.HideButtonNotification);
            Assert.True(_viewModel.OpenNotifier);
            Assert.Equal(AnswerSelected.Label, _viewModel.SuggestedAnswer);
            Assert.Equal(NotificationStatus.AnswerGiven, _viewModel.User.Status);
        }

        [Fact]
        public void ShouldAcceptNotifications()
        {
            TableUser HeartUser = new HeartUser();
            _viewModel.CooperatingPeer = HeartUser;

            var ChangeViewCalled = false;
            var GetSubjectByIdCalled = false;
            _viewModel.ChangeView += (s) => ChangeViewCalled = true;
            _viewModel.GetSubjectById += (s) => GetSubjectByIdCalled = true;
            _viewModel.OnAcceptGalaxy(null);

            Assert.True(ChangeViewCalled);
            Assert.True(GetSubjectByIdCalled);
            Assert.False(_viewModel.OpenNotifier);
            Assert.Equal(NotificationStatus.HelpingUser, _viewModel.User.Status);
            Assert.Equal(NotificationStatus.AcceptedHelp, HeartUser.Status);
        }

        [Fact]
        public void ShouldDeclineNotifications()
        {
            TableUser HeartUser = new HeartUser();
            _viewModel.CooperatingPeer = HeartUser;

            _viewModel.OnDeclineGalaxy(null);
            Assert.Null(_viewModel.CooperatingPeer);
            Assert.False(_viewModel.OpenNotifier);
            Assert.Equal(NotificationStatus.Idle, _viewModel.User.Status);
        }

        [Fact]
        public void ShouldNotifyUser()
        {
            TableUser HeartUser = new HeartUser();
            var SendRequestToUserCalled = false;
            _viewModel.SendRequestToUser += (s) => SendRequestToUserCalled = true;
            _viewModel.OnNotifyUser(HeartUser);

            Assert.True(SendRequestToUserCalled);
            Assert.Equal(HeartUser, _viewModel.CooperatingPeer);
            Assert.False(_viewModel.OpenNotifier);
            Assert.Equal(NotificationStatus.HelpRequestSent, _viewModel.User.Status);
        }

        [Fact]
        public void ShouldClearNotifications()
        {
            _viewModel.ClearNotifications();

            Assert.Null(_viewModel.CooperatingPeer);
            Assert.False(_viewModel.OpenNotifier);
            Assert.Null(_viewModel.SuggestedAnswer);
            Assert.Equal(NotificationStatus.Idle, _viewModel.User.Status);
        }

        [Fact]
        public void ShouldToggleNotificationWhenAnswerGiven()
        {
            GlobalData.GetInstance().StarUser.Status = NotificationStatus.AnswerGiven;
            _viewModel.OnToggleButtonNotification(null);
            Assert.False(_viewModel.HideButtonNotification);
            Assert.Equal(NotificationStatus.Idle, _viewModel.User.Status);
        }

        [Fact]
        public void ShouldToggleNotificationWhenPeerHasLeft()
        {
            Assert.False(_viewModel.HideButtonNotification);
            GlobalData.GetInstance().StarUser.Status = NotificationStatus.PeerHasLeft;
            _viewModel.OnToggleButtonNotification(null);
            Assert.True(_viewModel.HideButtonNotification);
        }

        [Fact]
        public void ShouldToggleNotifier()
        {
            Assert.False(_viewModel.OpenNotifier);
            _viewModel.OnToggleNotifier(null);
            Assert.True(_viewModel.OpenNotifier);
        }

        [Fact]
        public void ShouldSendAnswerToUser()
        {
            _viewModel.CooperatingPeer = new HeartUser();
            _viewModel.SendAnswerToUser(null);
            Assert.Equal(NotificationStatus.Idle, _viewModel.User.Status);
        }
    }
}
