using GalaxyZooTouchTable.Models;

namespace GalaxyZooTouchTable.ViewModels
{
    interface INotificationsViewModel
    {
        void ClearNotifications(bool UserLeaving = false);
        void FilterCurrentUser();
        void OnAcceptGalaxy(object sender = null);
        void OnAnswerReceived(AnswerButton Answer);
        void OnDeclineGalaxy(object sender);
        void OnNotificationReceived(NotificationRequest Request);
        void OnNotifyUser(object sender);
        void OnPeerLeaving(TableUser user = null);
        void OnResetNotifications(object sender = null);
        void OnToggleButtonNotification(object sender);
        void OnToggleNotifier(object sender);
        void RegisterMessengerActions(TableUser user);
        void SendAnswerToUser(AnswerButton SelectedItem);
    }
}
