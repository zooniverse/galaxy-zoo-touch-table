using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationSummaryViewModel : ViewModelBase
    {
        static Random random = new Random();
        public NotificationsViewModel Notifications { get; private set; }

        public ICommand RandomGalaxy { get; private set; }
        public ICommand ChooseAnotherGalaxy { get; private set; }
        public event Action RandomGalaxyDelegate = delegate { };
        public event Action ChooseAnotherGalaxyDelegate = delegate { };
        public event Action<TableSubject> DropSubjectDelegate = delegate { };

        readonly List<string> SummaryStrings = new List<string>
        {
            "Nice work! You just science'd.",
            "Thanks for your help!",
            "You're a real scientist!",
            "Don't stop now!",
            "Do this at home on adler.zooniverse.org."
        };

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
            int index = random.Next(SummaryStrings.Count);
            ClassificationSummary = new ClassificationSummary(subjectLocation, counts, currentAnswers, selectedAnswer, SummaryStrings[index]);
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

        public void DropSubject(TableSubject subject)
        {
            DropSubjectDelegate(subject);
        }
    }
}
