using GalaxyZooTouchTable.Models;
using GraphQL.Client;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ClassificationSummaryViewModel : INotifyPropertyChanged
    {
        public List<AnswerButton> Answers {get;set;}
        public GraphQLHttpClient GraphQLClient { get; set; } = new GraphQLHttpClient("https://caesar-staging.zooniverse.org/graphql");
        public Subject Subject { get; set; }
        public Workflow Workflow { get; set; }

        private int _answerOneCount = 0;
        public int AnswerOneCount
        {
            get { return _answerOneCount; }
            set
            {
                _answerOneCount = value;
                OnPropertyRaised("AnswerOneCount");
            }
        }

        private int _answerTwoCount = 0;
        public int AnswerTwoCount
        {
            get { return _answerTwoCount; }
            set
            {
                _answerTwoCount = value;
                OnPropertyRaised("AnswerTwoCount");
            }
        }

        private int _answerThreeCount = 0;
        public int AnswerThreeCount
        {
            get { return _answerThreeCount; }
            set
            {
                _answerThreeCount = value;
                OnPropertyRaised("AnswerThreeCount");
            }
        }

        private string _subjectImageSource;
        public string SubjectImageSource
        {
            get { return _subjectImageSource; }
            set
            {
                _subjectImageSource = value;
                OnPropertyRaised("SubjectImageSource");
            }
        }

        private int _totalVotes = 0;
        public int TotalVotes
        {
            get { return _totalVotes; }
            set
            {
                _totalVotes = value;
                OnPropertyRaised("TotalVotes");
            }
        }

        public ClassificationSummaryViewModel(Workflow workflow, Subject subject, List<AnswerButton> answers)
        {
            Answers = answers;
            Workflow = workflow;
            Subject = subject;
            SubjectImageSource = Subject.GetSubjectLocation();

            GraphQLRequest();
        }

        private async void GraphQLRequest()
        {
            var answersRequest = new GraphQLRequest
            {
                Query = @"
                    query AnswerCount($workflowId: ID!, $subjectId: ID!) {
                      workflow(id: $workflowId) {
                        subject_reductions(subjectId: $subjectId, reducerKey: T0_Stats) {
                            data
                        }
                      }
                    }",
                OperationName = "AnswerCount",
                Variables = new
                {
                    workflowId = Workflow.Id,
                    subjectId = Subject.Id
                }
            };
            var graphQLResponse = await GraphQLClient.SendQueryAsync(answersRequest);
            var reductions = graphQLResponse.Data.workflow.subject_reductions;
            var data = reductions.First.data;
            TotalVotes = 0;
            foreach (var count in data)
            {
                var index = System.Convert.ToInt32(count.Name);
                AnswerButton test = Answers[index];

                int answerCount = (int)count.Value;
                test.AnswerCount = answerCount;

                TotalVotes += answerCount;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {   if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
