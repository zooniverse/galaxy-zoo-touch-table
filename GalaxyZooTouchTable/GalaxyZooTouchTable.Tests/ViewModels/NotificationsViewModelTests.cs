using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Tests.Mock;
using GalaxyZooTouchTable.ViewModels;
using PanoptesNetClient.Models;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class NotificationsViewModelTests
    {
        private NotificationsViewModel ViewModel { get; set; }

        public NotificationsViewModelTests()
        {
            TableUser StarUser = GlobalData.GetInstance().StarUser;
            ViewModel = new NotificationsViewModel(StarUser);
        }

        [Fact]
        public void ShouldInitializeWithDefaultValues()
        {
            Assert.False(ViewModel.HideButtonNotification);
            Assert.False(ViewModel.OpenNotifier);
            Assert.Null(ViewModel.SuggestedAnswer);
            Assert.Null(ViewModel.CooperatingPeer);
        }

        [Fact]
        public void ShouldFilterUserFromAvailableUsers()
        {
            TableUser StarUser = GlobalData.GetInstance().StarUser;
            Assert.DoesNotContain(StarUser, ViewModel.AvailableUsers);
        }

        [Fact]
        public void ShouldCloseNotifierWhenPeerHasLeft()
        {
            ViewModel.OnPeerLeaving();
            Assert.False(ViewModel.OpenNotifier);
            Assert.Equal(NotificationStatus.PeerHasLeft, ViewModel.User.Status);
        }

        [Fact]
        public void ShouldHandleNotificationRequest()
        {
            TableUser HeartUser = new HeartUser();
            string SubjectID = "1";
            NotificationRequest Request = new NotificationRequest(HeartUser, SubjectID);

            ViewModel.OnNotificationReceived(Request);
            Assert.Equal(HeartUser, ViewModel.CooperatingPeer);
            Assert.True(ViewModel.OpenNotifier);
            Assert.Equal(SubjectID, ViewModel.SubjectIdToExamine);
            Assert.Equal(NotificationStatus.HelpRequestReceived, ViewModel.User.Status);
        }

        [Fact]
        public void ShouldReceiveAnswerRequest()
        {
            AnswerButton ItemTouched = PanoptesServiceMock.ConstructAnswerButton();
            ViewModel.OnAnswerReceived(ItemTouched);
            Assert.False(ViewModel.HideButtonNotification);
            Assert.True(ViewModel.OpenNotifier);
            Assert.Equal(ItemTouched.Label, ViewModel.SuggestedAnswer);
            Assert.Equal(NotificationStatus.AnswerGiven, ViewModel.User.Status);
        }

        [Fact]
        public void ShouldAcceptNotifications()
        {
            TableUser HeartUser = new HeartUser();
            ViewModel.CooperatingPeer = HeartUser;

            var ChangeViewCalled = false;
            var GetSubjectByIdCalled = false;
            ViewModel.ChangeView += (s) => ChangeViewCalled = true;
            ViewModel.GetSubjectById += (s) => GetSubjectByIdCalled = true;
            ViewModel.OnAcceptGalaxy(null);

            Assert.True(ChangeViewCalled);
            Assert.True(GetSubjectByIdCalled);
            Assert.False(ViewModel.OpenNotifier);
            Assert.Equal(NotificationStatus.HelpingUser, ViewModel.User.Status);
            Assert.Equal(NotificationStatus.AcceptedHelp, HeartUser.Status);
        }

        [Fact]
        public void ShouldDeclineNotifications()
        {
            TableUser HeartUser = new HeartUser();
            ViewModel.CooperatingPeer = HeartUser;

            ViewModel.OnDeclineGalaxy(null);
            Assert.Null(ViewModel.CooperatingPeer);
            Assert.False(ViewModel.OpenNotifier);
            Assert.Equal(NotificationStatus.Idle, ViewModel.User.Status);
        }

        [Fact]
        public void ShouldNotifyUser()
        {
            TableUser HeartUser = new HeartUser();
            var SendRequestToUserCalled = false;
            ViewModel.SendRequestToUser += (s) => SendRequestToUserCalled = true;
            ViewModel.OnNotifyUser(HeartUser);

            Assert.Equal(HeartUser, ViewModel.CooperatingPeer);
            Assert.True(SendRequestToUserCalled);
            Assert.False(ViewModel.OpenNotifier);
            Assert.Equal(NotificationStatus.HelpRequestSent, ViewModel.User.Status);
        }

        [Fact]
        public void ShouldClearNotifications()
        {
            ViewModel.ClearNotifications();

            Assert.Null(ViewModel.CooperatingPeer);
            Assert.False(ViewModel.OpenNotifier);
            Assert.Null(ViewModel.SuggestedAnswer);
            Assert.Equal(NotificationStatus.Idle, ViewModel.User.Status);
        }

        [Fact]
        public void ShouldToggleNotificationWhenAnswerGiven()
        {
            GlobalData.GetInstance().StarUser.Status = NotificationStatus.AnswerGiven;
            ViewModel.OnToggleButtonNotification(null);
            Assert.False(ViewModel.HideButtonNotification);
            Assert.Equal(NotificationStatus.Idle, ViewModel.User.Status);
        }

        [Fact]
        public void ShouldToggleNotificationWhenPeerHasLeft()
        {
            Assert.False(ViewModel.HideButtonNotification);
            GlobalData.GetInstance().StarUser.Status = NotificationStatus.PeerHasLeft;
            ViewModel.OnToggleButtonNotification(null);
            Assert.True(ViewModel.HideButtonNotification);
        }

        [Fact]
        public void ShouldToggleNotifier()
        {
            Assert.False(ViewModel.OpenNotifier);
            ViewModel.OnToggleNotifier(null);
            Assert.True(ViewModel.OpenNotifier);
        }

        [Fact]
        public void ShouldSendAnswerToUser()
        {
            ViewModel.CooperatingPeer = new HeartUser();
            ViewModel.SendAnswerToUser(null);
            Assert.Equal(NotificationStatus.Idle, ViewModel.User.Status);
        }
    }
}
