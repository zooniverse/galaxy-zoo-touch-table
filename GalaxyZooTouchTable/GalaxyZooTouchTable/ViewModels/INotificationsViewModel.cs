using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using System;
using System.ComponentModel;

namespace GalaxyZooTouchTable.ViewModels
{
    public interface INotificationsViewModel : INotifyPropertyChanged
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

        event Action<string> GetSubjectById;
        event Action<ClassifierViewEnum> ChangeView;
        event Action<TableUser> SendRequestToUser;
    }
}
