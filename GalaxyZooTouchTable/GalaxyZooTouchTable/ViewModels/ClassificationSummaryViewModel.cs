using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationSummaryViewModel : ViewModelBase
    {
        static Random random = new Random();
        public NotificationsViewModel Notifications { get; private set; }
        bool ButtonsDisabled = false;

        public ICommand RandomGalaxy { get; private set; }
        public ICommand ChooseAnotherGalaxy { get; private set; }
        public event Action<object> RandomGalaxyDelegate = delegate { };
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
            TemporaryDisableButtons();
        }

        async void TemporaryDisableButtons()
        {
            ButtonsDisabled = true;
            await Task.Delay(TimeSpan.FromSeconds(1));
            ButtonsDisabled = false;
        }

        void LoadCommands()
        {
            RandomGalaxy = new CustomCommand(OnRandomGalaxy, CanBeginNewClassification);
            ChooseAnotherGalaxy = new CustomCommand(OnChooseAnotherGalaxy, CanBeginNewClassification);
        }

        private bool CanBeginNewClassification(object obj)
        {
            return !ButtonsDisabled;
        }

        void OnRandomGalaxy(object obj)
        {
            RandomGalaxyDelegate(null);
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
