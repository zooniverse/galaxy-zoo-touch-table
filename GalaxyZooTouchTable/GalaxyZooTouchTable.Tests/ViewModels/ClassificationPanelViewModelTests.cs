using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Tests.Extensions;
using GalaxyZooTouchTable.Tests.Mock;
using GalaxyZooTouchTable.ViewModels;
using Moq;
using PanoptesNetClient.Models;
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

        public ClassificationPanelViewModelTests()
        {
            _panoptesServiceMock.Setup(dp => dp.GetWorkflowAsync("1"))
                .ReturnsAsync(PanoptesServiceMockData.Workflow("1"));

            _panoptesServiceMock.Setup(dp => dp.GetSubjectAsync("1"))
                .ReturnsAsync(PanoptesServiceMockData.Subject());

            NameValueCollection query = new NameValueCollection
                {
                    { "workflow_id", "1" }
                };
            _panoptesServiceMock.Setup(dp => dp.GetSubjectsAsync("queued", query))
                .ReturnsAsync(PanoptesServiceMockData.Subjects());

            _panoptesServiceMock.Setup(dp => dp.CreateClassificationAsync(new Classification()))
                .Returns(Task.CompletedTask);

            _graphQLServiceMock.Setup(dp => dp.GetReductionAsync(new Workflow(), new Subject()))
                .ReturnsAsync(GraphQLServiceMockData.GraphQLResponse());

            _viewModel = new ClassificationPanelViewModel(_panoptesServiceMock.Object, _graphQLServiceMock.Object, new StarUser());
        }

        [Fact]
        public void ShouldInitializeWithDefaultValues()
        {
            Assert.False(_viewModel.CloseConfirmationVisible);
            Assert.False(_viewModel.ClassifierOpen);
            Assert.False(_viewModel.CanSendClassification);
        }

        [Fact]
        public async void ShouldLoadAWorkflow()
        {
            await _viewModel.GetWorkflow();
            _panoptesServiceMock.Verify(vm=>vm.GetWorkflowAsync("1"), Times.Once);
            Assert.NotNull(_viewModel.Workflow);
        }

        [Fact]
        public void ShouldSelectAnswerAndMakeAnnotation()
        {
            AnswerButton AnswerButton = PanoptesServiceMockData.AnswerButton();
            _viewModel.OnSelectAnswer(AnswerButton);
            Assert.Equal(AnswerButton, _viewModel.SelectedAnswer);
            Assert.Equal(AnswerButton.Index, _viewModel.CurrentAnnotation.Value);
        }

        [Fact]
        public void ShouldLoadASubject()
        {
            _viewModel.Workflow = PanoptesServiceMockData.Workflow("1");
            _viewModel.OnGetSubjectById("1");
            _panoptesServiceMock.Verify(vm => vm.GetSubjectAsync("1"), Times.Once);
            _graphQLServiceMock.Verify(vm => vm.GetReductionAsync(_viewModel.Workflow, _viewModel.CurrentSubject), Times.Once);
            Assert.NotNull(_viewModel.CurrentSubject);
        }

        [Fact]
        public void ShouldOpenClassifier()
        {
            _viewModel.Load();
            _viewModel.OnOpenClassifier(null);
            Assert.True(_viewModel.ClassifierOpen);
            Assert.True(_viewModel.User.Active);
        }

        [Fact]
        public void ShouldCloseClassifier()
        {
            _viewModel.Load();
            _viewModel.Workflow = PanoptesServiceMockData.Workflow();
            _viewModel.OnCloseClassifier(null);

            Assert.False(_viewModel.ClassifierOpen);
            Assert.False(_viewModel.CloseConfirmationVisible);
            Assert.False(_viewModel.User.Active);
        }

        [Fact]
        public void ShouldToggleCloseConfirmation()
        {
            Assert.False(_viewModel.CloseConfirmationVisible);
            _viewModel.ToggleCloseConfirmation(null);
            Assert.True(_viewModel.CloseConfirmationVisible);
        }

        [Fact]
        public void ShouldChangeView()
        {
            Assert.Equal(ClassifierViewEnum.SubjectView, _viewModel.CurrentView);
            _viewModel.OnChangeView(ClassifierViewEnum.SummaryView);
            Assert.Equal(ClassifierViewEnum.SummaryView, _viewModel.CurrentView);
        }

        [Fact]
        public void ShouldLoadSubjects()
        {
            _viewModel.Workflow = PanoptesServiceMockData.Workflow("1");
            _viewModel.GetSubjectQueue();
            NameValueCollection query = new NameValueCollection
                {
                    { "workflow_id", "1" }
                };
            _panoptesServiceMock.Verify(vm => vm.GetSubjectsAsync("queued", query), Times.Once);
            _graphQLServiceMock.Verify(vm => vm.GetReductionAsync(_viewModel.Workflow, _viewModel.CurrentSubject), Times.Once);
            Assert.NotNull(_viewModel.CurrentSubject);
        }

        [Fact]
        public void ShouldSubmitClassificationOnSubmission()
        {
            _viewModel.Load();
            Assert.Empty(_viewModel.CurrentClassification.Annotations);

            _viewModel.OnSelectAnswer(PanoptesServiceMockData.AnswerButton());
            _viewModel.OnContinueClassification(null);
            _panoptesServiceMock.Verify(vm => vm.CreateClassificationAsync(_viewModel.CurrentClassification), Times.Once);

            Assert.Equal(1, _viewModel.SelectedAnswer.AnswerCount);
            Assert.Equal(1, _viewModel.TotalVotes);
            Assert.Equal(1, _viewModel.ClassificationsThisSession);
            Assert.Single(_viewModel.CurrentClassification.Annotations);
        }

        [Fact]
        public void ShouldLoadANewSubjectWhenContinuingSummary()
        {
            _viewModel.Load();
            Assert.Empty(_viewModel.Subjects);

            _viewModel.CurrentView = ClassifierViewEnum.SummaryView;
            _viewModel.OnContinueClassification(null);

            NameValueCollection query = new NameValueCollection
                {
                    { "workflow_id", "1" }
                };
            _panoptesServiceMock.Verify(vm => vm.GetSubjectsAsync("queued", query), Times.Exactly(2));
        }
    }
}
