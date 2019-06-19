using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationSummaryViewModel : ViewModelBase
    {
        public NotificationsViewModel Notifications { get; private set; }

        public ICommand RandomGalaxy { get; private set; }
        public ICommand ChooseAnotherGalaxy { get; private set; }
        public event Action RandomGalaxyDelegate = delegate { };
        public event Action ChooseAnotherGalaxyDelegate = delegate { };

        private ClassificationSummary classificationSummary;
        public ClassificationSummary ClassificationSummary
        {
            get => classificationSummary;
            set => SetProperty(ref classificationSummary, value);
        }

        public ClassificationSummaryViewModel(NotificationsViewModel notifications)
        {
            Notifications = notifications;
            LoadCommands();
        }

        public void ProcessNewClassification(string subjectLocation, ClassificationCounts counts, List<AnswerButton> currentAnswers, AnswerButton selectedAnswer)
        {
            ClassificationSummary = new ClassificationSummary(subjectLocation, counts, currentAnswers, selectedAnswer);
        }

        void LoadCommands()
        {
            RandomGalaxy = new CustomCommand(OnRandomGalaxy);
            ChooseAnotherGalaxy = new CustomCommand(OnChooseAnotherGalaxy);
        }

        void OnRandomGalaxy(object obj)
        {
            RandomGalaxyDelegate();
        }

        void OnChooseAnotherGalaxy(object obj)
        {
            ChooseAnotherGalaxyDelegate();
        }
    }
}
