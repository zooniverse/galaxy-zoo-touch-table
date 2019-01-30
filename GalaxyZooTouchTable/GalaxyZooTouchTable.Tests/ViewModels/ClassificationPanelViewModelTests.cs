using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
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

            Assert.NotNull(_viewModel.Notifications);
            Assert.NotNull(_viewModel.LevelerViewModel);
            Assert.NotNull(_viewModel.StillThere);
            Assert.NotNull(_viewModel.ExamplesViewModel);
        }

        [Fact]
        public async void ShouldLoadAWorkflow()
        {
            await _viewModel.GetWorkflow();
            _panoptesServiceMock.Verify(vm=>vm.GetWorkflowAsync("1"), Times.Once);
            Assert.NotNull(_viewModel.Workflow);
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
        public void ShouldLoadASubject()
        {
            _viewModel.Workflow = PanoptesServiceMockData.Workflow("1");
            _viewModel.OnGetSubjectById("1");
            _panoptesServiceMock.Verify(vm => vm.GetSubjectAsync("1"), Times.Once);
            _graphQLServiceMock.Verify(vm => vm.GetReductionAsync(_viewModel.Workflow, _viewModel.CurrentSubject), Times.Once);
            Assert.NotNull(_viewModel.CurrentSubject);
        }

        [Fact]
        public void ShouldSubmitClassificationOnSubmission()
        {
            _viewModel.OnSelectAnswer(PanoptesServiceMockData.AnswerButton());
            _viewModel.OnContinueClassification(null);
            _panoptesServiceMock.Verify(vm => vm.CreateClassificationAsync(_viewModel.CurrentClassification), Times.Once);
        }

        //[Fact]
        //public void ShouldSelectAnswer()
        //{
        //    AnswerButton AnswerButton = PanoptesServiceMockData.ConstructAnswerButton();

        //    ViewModel.OnSelectAnswer(AnswerButton);
        //    Assert.Equal(AnswerButton, ViewModel.SelectedAnswer);
        //    Assert.Equal(AnswerButton.Index, ViewModel.CurrentAnnotation.Value);
        //}

        [Fact]
        public void ShouldOpenClassifier()
        {
            _viewModel.OnOpenClassifier(null);
            Assert.True(_viewModel.ClassifierOpen);
            Assert.True(_viewModel.User.Active);
        }

        //[Fact]
        //public void ShouldCloseClassifier()
        //{
        //    ViewModel.OnCloseClassifier(null);
        //    Assert.False(ViewModel.Notifications.OpenNotifier);
        //    Assert.True(ViewModel.ExamplesViewModel.IsOpen);
        //    Assert.False(ViewModel.LevelerViewModel.IsOpen);
        //    Assert.False(ViewModel.ClassifierOpen);
        //    Assert.False(ViewModel.User.Active);
        //}

        //[Fact]
        //public void ShouldPrepareNewClassification()
        //{
        //    ViewModel.TotalVotes = 5;
        //    ViewModel.PrepareForNewClassification();
        //    Assert.Equal(0, ViewModel.TotalVotes);
        //}

        [Fact]
        public void ShouldToggleCloseConfirmation()
        {
            Assert.False(_viewModel.CloseConfirmationVisible);
            _viewModel.ToggleCloseConfirmation(null);
            Assert.True(_viewModel.CloseConfirmationVisible);
        }

        //[Fact]
        //public void ShouldViewClassificationSummaryOnSubmit()
        //{
        //    ViewModel.OnSelectAnswer(PanoptesServiceMockData.ConstructAnswerButton());

        //    Assert.Equal(0, ViewModel.TotalVotes);
        //    Assert.Equal(ClassifierViewEnum.SubjectView, ViewModel.CurrentView);
        //    ViewModel.OnContinueClassification(null);
        //    Assert.Equal(1, ViewModel.TotalVotes);
        //    Assert.Equal(ClassifierViewEnum.SummaryView, ViewModel.CurrentView);
        //}
    }
}
