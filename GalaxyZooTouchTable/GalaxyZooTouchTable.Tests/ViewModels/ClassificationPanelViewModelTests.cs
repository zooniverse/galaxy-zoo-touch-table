using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Tests.Mock;
using GalaxyZooTouchTable.ViewModels;
using Moq;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class ClassificationPanelViewModelTests
    {
        private ClassificationPanelViewModel _viewModel { get; set; }
        private Mock<IPanoptesService> _panoptesServiceMock = new Mock<IPanoptesService>();
        private Mock<IGraphQLService> _graphQLServiceMock = new Mock<IGraphQLService>();
        private Mock<ILocalDBService> _localDBServiceMock = new Mock<ILocalDBService>();

        public ClassificationPanelViewModelTests()
        {
            _panoptesServiceMock.Setup(dp => dp.GetWorkflowAsync("1"))
                .ReturnsAsync(PanoptesServiceMockData.Workflow("1"));
            _panoptesServiceMock.Setup(dp => dp.CreateClassificationAsync(new Classification()))
                .Returns(Task.CompletedTask);

            _graphQLServiceMock.Setup(dp => dp.GetReductionAsync(new Workflow(), PanoptesServiceMockData.TableSubject))
                .ReturnsAsync(GraphQLServiceMockData.GraphQLResponse());

            _localDBServiceMock.Setup(dp => dp.GetLocalSubject("1")).Returns(PanoptesServiceMockData.TableSubject);
            _localDBServiceMock.Setup(dp => dp.GetQueuedSubjects()).Returns(PanoptesServiceMockData.TableSubjects());

            _viewModel = new ClassificationPanelViewModel(_panoptesServiceMock.Object, _graphQLServiceMock.Object, _localDBServiceMock.Object, new StarUser());
        }

        [Fact]
        private void ShouldInitializeWithDefaultValues()
        {
            Assert.False(_viewModel.CloseConfirmationVisible);
            Assert.False(_viewModel.ClassifierOpen);
            Assert.False(_viewModel.CanSendClassification);
        }

        [Fact]
        private async void ShouldLoadAWorkflow()
        {
            await _viewModel.GetWorkflow();
            _panoptesServiceMock.Verify(vm=>vm.GetWorkflowAsync("1"), Times.Once);
            Assert.NotNull(_viewModel.Workflow);
        }

        [Fact]
        private void ShouldSelectAnswerAndMakeAnnotation()
        {
            AnswerButton AnswerButton = PanoptesServiceMockData.AnswerButton();
            _viewModel.SelectAnswer.Execute(AnswerButton);
            Assert.Equal(AnswerButton, _viewModel.SelectedAnswer);
            Assert.Equal(AnswerButton.Index, _viewModel.CurrentAnnotation.Value);
        }

        [Fact]
        private void ShouldLoadASubject()
        {
            _viewModel.Workflow = PanoptesServiceMockData.Workflow("1");
            _viewModel.OnGetSubjectById("1");
            _localDBServiceMock.Verify(vm => vm.GetLocalSubject("1"), Times.Once);
            _graphQLServiceMock.Verify(vm => vm.GetReductionAsync(_viewModel.Workflow, _viewModel.CurrentSubject), Times.Once);
            Assert.NotNull(_viewModel.CurrentSubject);
        }

        [Fact]
        private void ShouldOpenClassifier()
        {
            _viewModel.Load();
            _viewModel.OpenClassifier.Execute(null);
            Assert.True(_viewModel.ClassifierOpen);
            Assert.True(_viewModel.User.Active);
        }

        [Fact]
        private void ShouldCloseClassifier()
        {
            _viewModel.Load();
            _viewModel.Workflow = PanoptesServiceMockData.Workflow();
            _viewModel.CloseClassifier.Execute(null);

            Assert.False(_viewModel.ClassifierOpen);
            Assert.False(_viewModel.CloseConfirmationVisible);
            Assert.False(_viewModel.User.Active);
        }

        [Fact]
        private void ShouldToggleCloseConfirmation()
        {
            Assert.False(_viewModel.CloseConfirmationVisible);
            _viewModel.ShowCloseConfirmation.Execute(null);
            Assert.True(_viewModel.CloseConfirmationVisible);
        }

        [Fact]
        private void ShouldChangeView()
        {
            Assert.Equal(ClassifierViewEnum.SubjectView, _viewModel.CurrentView);
            _viewModel.OnChangeView(ClassifierViewEnum.SummaryView);
            Assert.Equal(ClassifierViewEnum.SummaryView, _viewModel.CurrentView);
        }

        [Fact]
        private void ShouldLoadSubjects()
        {
            _viewModel.Workflow = PanoptesServiceMockData.Workflow("1");
            _viewModel.GetSubjectQueue();
            NameValueCollection query = new NameValueCollection
                {
                    { "workflow_id", "1" }
                };
            _localDBServiceMock.Verify(vm => vm.GetQueuedSubjects(), Times.Once);
            _graphQLServiceMock.Verify(vm => vm.GetReductionAsync(_viewModel.Workflow, _viewModel.CurrentSubject), Times.Once);
            Assert.NotNull(_viewModel.CurrentSubject);
        }

        [Fact]
        private void ShouldSubmitClassificationOnSubmission()
        {
            _viewModel.Load();
            Assert.Empty(_viewModel.CurrentClassification.Annotations);

            _viewModel.SelectAnswer.Execute(PanoptesServiceMockData.AnswerButton());
            _viewModel.ContinueClassification.Execute(null);
            _panoptesServiceMock.Verify(vm => vm.CreateClassificationAsync(_viewModel.CurrentClassification), Times.Once);

            Assert.Equal(1, _viewModel.SelectedAnswer.AnswerCount);
            Assert.Equal(1, _viewModel.TotalVotes);
            Assert.Equal(1, _viewModel.ClassificationsThisSession);
            Assert.Single(_viewModel.CurrentClassification.Annotations);
        }

        [Fact]
        private void ShouldLoadANewSubjectWhenContinuingSummary()
        {
            _viewModel.Load();
            Assert.Empty(_viewModel.Subjects);

            _viewModel.CurrentView = ClassifierViewEnum.SummaryView;
            _viewModel.ContinueClassification.Execute(null);

            NameValueCollection query = new NameValueCollection
                {
                    { "workflow_id", "1" }
                };
            _localDBServiceMock.Verify(vm => vm.GetQueuedSubjects(), Times.Exactly(2));
        }

        [Fact]
        private void ShouldCreateAnAnnotationWhenSelectingAnswer()
        {
            AnswerButton SelectedAnswer = PanoptesServiceMockData.AnswerButton();
            _viewModel.ChooseAnswer(SelectedAnswer);
            Assert.Equal(SelectedAnswer.Index, _viewModel.CurrentAnnotation.Value);
        }

        [Fact]
        private void ShouldParseTaskAnswers()
        {
            List<TaskAnswer> Answers = PanoptesServiceMockData.TaskAnswerList();
            var result = _viewModel.ParseTaskAnswers(Answers);

            Assert.Equal(2, result.Count);
            Assert.Equal("First Task", result[0].Label);
            Assert.Equal(0, result[0].Index);
            Assert.Equal("Second Task", result[1].Label);
            Assert.Equal(1, result[1].Index);
        }

        [Fact]
        private async void ShouldCreateANewClassification()
        {
            await _viewModel.GetWorkflow();
            var Subject = PanoptesServiceMockData.TableSubject;

            _viewModel.StartNewClassification(Subject);
            Assert.Null(_viewModel.CurrentAnnotation);
            Assert.Null(_viewModel.SelectedAnswer);
            Assert.NotNull(_viewModel.CurrentClassification);
        }

        [Fact]
        private void ShouldResetAnswerCount()
        {
            List<TaskAnswer> Answers = PanoptesServiceMockData.TaskAnswerList();
            _viewModel.CurrentAnswers = _viewModel.ParseTaskAnswers(Answers);
            _viewModel.CurrentAnswers[0].AnswerCount = 5;
            _viewModel.CurrentAnswers[1].AnswerCount = 7;

            _viewModel.ResetAnswerCount();
            Assert.Equal(0, _viewModel.CurrentAnswers[0].AnswerCount);
            Assert.Equal(0, _viewModel.CurrentAnswers[1].AnswerCount);
        }
    }
}
