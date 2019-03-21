using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;

namespace GalaxyZooTouchTable.Models
{
    public class NotificationAvatar : ViewModelBase
    {
        public TableUser User { get; set; }

        public bool ShowCircle { get; set; } 

        public bool ShowExclamationPoint { get; set; } 

        public bool ShowQuestion { get; set; }

        private bool _disabled;
        public bool Disabled
        {
            get => _disabled;
            set => SetProperty(ref _disabled, value);
        }

        private List<CompletedClassification> CompletedClassifications { get; set; } = new List<CompletedClassification>();

        public NotificationAvatar(TableUser user)
        {
            User = user;
            User.PropertyChanged += ShouldClearClassifications;
            Messenger.Default.Register<CompletedClassification>(this, OnCompletedClassification, $"{User.Name}_AddCompletedClassification");
        }

        private void OnCompletedClassification(CompletedClassification Classification)
        {
            CompletedClassifications.Add(Classification);
        }

        private void ShouldClearClassifications(object sender, PropertyChangedEventArgs e)
        {
            if (!User.Active)
            {
                CompletedClassifications.Clear();
            }
            Disabled = User.Busy;
        }

        internal CompletedClassification HasAlreadySeen(string currentSubjectId)
        {
            return CompletedClassifications.Find(x => x.SubjectId == currentSubjectId);
        }
    }
}
