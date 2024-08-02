using System.Collections.Generic;

namespace GalaxyZooTouchTable.Models
{
    public class ClassificationSummary
    {
        public List<AnswerButton> CurrentAnswers { get; private set; }
        public AnswerButton SelectedAnswer { get; private set; }
        public string SubjectLocation { get; private set; }
        public string SummaryString { get; private set; }
        public int TotalVotes { get; private set; }
        public int VoteLimit { get; private set; } = 25;

        public ClassificationSummary(string subjectLocation, ClassificationCounts counts, List<AnswerButton> currentAnswers, AnswerButton selectedAnswer, string summaryString)
        {
            CurrentAnswers = ParseAnswerCounts(currentAnswers, counts);
            SelectedAnswer = selectedAnswer;
            SubjectLocation = subjectLocation;
            SummaryString = summaryString;
            TotalVotes = counts.Total;
            if (TotalVotes > 25)
                VoteLimit = counts.Total;
        }

        List<AnswerButton> ParseAnswerCounts(List<AnswerButton> answers, ClassificationCounts counts)
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
            return answers;
        }
    }
}
