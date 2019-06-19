using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationSummaryViewModel : ViewModelBase
    {
        static Random random = new Random();

        public ClassificationCounts Counts { get; private set; }
        public string SubjectLocation { get; private set; }
        public string SummaryString { get; private set; }
        public NotificationsViewModel Notifications { get; private set; }
        public int TotalVotes { get; private set; }
        public string UserName { get; private set; }
        public AnswerButton SelectedAnswer { get; private set; }
        public List<AnswerButton> CurrentAnswers { get; private set; }

        public ICommand RandomGalaxy { get; private set; }
        public ICommand ChooseAnotherGalaxy { get; private set; }

        public ClassificationSummaryViewModel(string subjectLocation, NotificationsViewModel notifications, ClassificationCounts counts, string userName, List<AnswerButton> currentAnswers, AnswerButton selectedAnswer)
        {
            Counts = counts;
            CurrentAnswers = currentAnswers;
            Notifications = notifications;
            SelectedAnswer = selectedAnswer;
            SubjectLocation = subjectLocation;
            SummaryString = SelectSummaryString();
            TotalVotes = counts.Total;
            UserName = userName;

            ParseAnswerCounts(currentAnswers, counts);
            LoadCommands();
        }

        void ParseAnswerCounts(List<AnswerButton> answers, ClassificationCounts counts)
        {
            foreach (AnswerButton answer in answers)
            {
                switch (answer.Label)
                {
                    case "Smooth":
                        answer.AnswerCount = counts.Smooth;
                        break;
                    case "Features":
                        answer.AnswerCount = counts.Features;
                        break;
                    default:
                        answer.AnswerCount = counts.Star;
                        break;
                }
            }
        }

        void LoadCommands()
        {
            RandomGalaxy = new CustomCommand(OnRandomGalaxy);
            ChooseAnotherGalaxy = new CustomCommand(OnChooseAnotherGalaxy);
        }

        void OnRandomGalaxy(object obj)
        {
            Messenger.Default.Send(obj, $"{UserName}_RandomGalaxy");
        }

        void OnChooseAnotherGalaxy(object obj)
        {
            Messenger.Default.Send(obj, $"{UserName}_ChooseAnother");
        }

        string SelectSummaryString()
        {
            ObservableCollection<string> summaryStrings = Application.Current.FindResource("SummaryStrings") as ObservableCollection<string>;
            int index = random.Next(summaryStrings.Count);
            return summaryStrings[index];
        }
    }
}
