using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace GalaxyZooTouchTable.ViewModels
{
    public class NotificationAvatarViewModel : ViewModelBase
    {
        public TableUser User { get; set; }
        public NotificationAvatar NotificationAvatar { get; set; } = new NotificationAvatar();
        private List<CompletedClassification> CompletedClassifications { get; set; } = new List<CompletedClassification>();

        public NotificationAvatarViewModel(TableUser user)
        {
            User = user;
            User.PropertyChanged += UpdateUserProperties;
            Messenger.Default.Register<CompletedClassification>(this, OnCompletedClassification, $"{user.Name}_AddCompletedClassification");
        }

        private void UpdateUserProperties(object sender, PropertyChangedEventArgs e)
        {
            if (!User.Active) CompletedClassifications.Clear();
            if (!User.Busy) ResetIcons();
        }

        public void ResetIcons()
        {
            NotificationAvatar.ShowCircle = false;
            NotificationAvatar.ShowQuestion = false;
            NotificationAvatar.ShowExclamationPoint = false;
        }

        public void ShowDisabled()
        {
            NotificationAvatar.Disabled = true;
        }

        public void ShowEnabled()
        {
            NotificationAvatar.Disabled = false;
        }

        public void ShowQuestionMark()
        {
            NotificationAvatar.ShowQuestion = true;
        }

        public void ShowExclamationPoint(TableUser UserHelping)
        {
            NotificationAvatar.ShowExclamationPoint = UserHelping == null;
            if (UserHelping != null) NotificationAvatar.ShowCircle = true;
        }

        private void OnCompletedClassification(CompletedClassification Classification)
        {
            CompletedClassifications.Add(Classification);
        }

        internal CompletedClassification HasAlreadyClassified(string currentSubjectId)
        {
            return CompletedClassifications.Find(x => x.SubjectId == currentSubjectId);
        }
    }
}
