using GalaxyZooTouchTable.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GalaxyZooTouchTable.Models
{
    public class NotificationAvatar
    {
        public TableUser User { get; set; }
        public bool ShowCircle { get; set; } 
        public bool ShowExclamationPoint { get; set; } 
        public bool ShowQuestion { get; set; } 
        public bool ShowDisable { get; set; }
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
        }

        internal CompletedClassification HasAlreadySeen(string currentSubjectId)
        {
            return CompletedClassifications.Find(x => x.SubjectId == currentSubjectId);
        }
    }
}
