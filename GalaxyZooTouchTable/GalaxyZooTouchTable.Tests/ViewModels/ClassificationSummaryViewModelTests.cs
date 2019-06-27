using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Tests.Mock;
using GalaxyZooTouchTable.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class ClassificationSummaryViewModelTests
    {
        ClassificationSummaryViewModel _viewModel;
        TableSubject Subject = PanoptesServiceMockData.TableSubject();
        ClassificationCounts Counts = GraphQLServiceMockData.ClassificationCounts();
        List<AnswerButton> Answers = PanoptesServiceMockData.AnswerButtons();

        public ClassificationSummaryViewModelTests()
        {
            NotificationsViewModel notifications = new NotificationsViewModel(new BlueUser());
            _viewModel = new ClassificationSummaryViewModel(notifications);
        }

        [Fact]
        public void ShouldProcessAClassification()
        {
            _viewModel.ProcessNewClassification(Subject.Location, Counts, Answers, Answers[0]);

            Assert.NotNull(_viewModel.ClassificationSummary.SelectedAnswer);
            Assert.NotNull(_viewModel.ClassificationSummary.CurrentAnswers);
            Assert.NotNull(_viewModel.ClassificationSummary.SubjectLocation);
            Assert.NotNull(_viewModel.ClassificationSummary.SummaryString);
            Assert.True(_viewModel.ClassificationSummary.TotalVotes > 0);
        }

        [Fact]
        public async void ShouldTemporarilyDisableButtons()
        {
            _viewModel.ProcessNewClassification(Subject.Location, Counts, Answers, Answers[0]);
            Assert.False(_viewModel.RandomGalaxy.CanExecute(null));
            Assert.False(_viewModel.ChooseAnotherGalaxy.CanExecute(null));
            await Task.Delay(TimeSpan.FromSeconds(1.1));
            Assert.True(_viewModel.RandomGalaxy.CanExecute(null));
            Assert.True(_viewModel.ChooseAnotherGalaxy.CanExecute(null));
        }
    }
}
