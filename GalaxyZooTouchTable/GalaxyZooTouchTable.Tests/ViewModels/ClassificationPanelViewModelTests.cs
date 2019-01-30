using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Tests.Mock;
using GalaxyZooTouchTable.ViewModels;
using Moq;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class ClassificationPanelViewModelTests
    {
        private ClassificationPanelViewModel _viewModel { get; set; }
        private Mock<IPanoptesService> _panoptesServiceMock = new Mock<IPanoptesService>();

        public ClassificationPanelViewModelTests()
        {
            var starUser = new StarUser();
            _panoptesServiceMock.Setup(dp => dp.GetWorkflowAsync("1"))
                .ReturnsAsync(PanoptesServiceMockData.Workflow("1"));

            _viewModel = new ClassificationPanelViewModel(_panoptesServiceMock.Object, new GraphQLServiceMockData(), starUser);
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
        public void ShouldLoadAWorkflow()
        {
            _viewModel.Load();
            _panoptesServiceMock.Verify(vm=>vm.GetWorkflowAsync("1"), Times.Once);
            Assert.NotNull(_viewModel.Workflow);
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
