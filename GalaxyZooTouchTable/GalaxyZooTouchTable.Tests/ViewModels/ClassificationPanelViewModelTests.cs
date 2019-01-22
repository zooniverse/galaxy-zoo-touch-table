using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Tests.Mock;
using GalaxyZooTouchTable.ViewModels;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class ClassificationPanelViewModelTests
    {
        private ClassificationPanelViewModel ViewModel { get; set; }

        public ClassificationPanelViewModelTests()
        {
            var starUser = new StarUser();
            ViewModel = new ClassificationPanelViewModel(new PanoptesServiceMock(), new GraphQLServiceMock(), starUser);
        }

        [Fact]
        public void ShouldInitializeWithDefaultValues()
        {
            Assert.Equal("1", ViewModel.Workflow.Id);
            Assert.False(ViewModel.CloseConfirmationVisible);
            Assert.False(ViewModel.ClassifierOpen);
            Assert.False(ViewModel.CanSendClassification);

            Assert.NotNull(ViewModel.Notifications);
            Assert.NotNull(ViewModel.LevelerViewModel);
            Assert.NotNull(ViewModel.StillThere);
            Assert.NotNull(ViewModel.ExamplesViewModel);
        }

        [Fact]
        public void ShouldSelectAnswer()
        {
            AnswerButton AnswerButton = PanoptesServiceMock.ConstructAnswerButton();

            ViewModel.OnSelectAnswer(AnswerButton);
            Assert.Equal(AnswerButton, ViewModel.SelectedAnswer);
            Assert.Equal(AnswerButton.Index, ViewModel.CurrentAnnotation.Value);
        }

        [Fact]
        public void ShouldOpenClassifier()
        {
            ViewModel.OnOpenClassifier(null);
            Assert.True(ViewModel.ClassifierOpen);
            Assert.True(ViewModel.User.Active);
        }

        [Fact]
        public void ShouldCloseClassifier()
        {
            ViewModel.OnCloseClassifier(null);
            Assert.False(ViewModel.Notifications.OpenNotifier);
            Assert.True(ViewModel.ExamplesViewModel.IsOpen);
            Assert.False(ViewModel.LevelerViewModel.IsOpen);
            Assert.False(ViewModel.ClassifierOpen);
            Assert.False(ViewModel.User.Active);
        }

        [Fact]
        public void ShouldPrepareNewClassification()
        {
            ViewModel.TotalVotes = 5;
            ViewModel.PrepareForNewClassification();
            Assert.Equal(0, ViewModel.TotalVotes);
        }

        [Fact]
        public void ShouldToggleCloseConfirmation()
        {
            Assert.False(ViewModel.CloseConfirmationVisible);
            ViewModel.ToggleCloseConfirmation(null);
            Assert.True(ViewModel.CloseConfirmationVisible);
        }

        [Fact]
        public void ShouldViewClassificationSummaryOnSubmit()
        {
            ViewModel.OnSelectAnswer(PanoptesServiceMock.ConstructAnswerButton());

            Assert.Equal(0, ViewModel.TotalVotes);
            Assert.Equal(ClassifierViewEnum.SubjectView, ViewModel.CurrentView);
            ViewModel.OnContinueClassification(null);
            Assert.Equal(1, ViewModel.TotalVotes);
            Assert.Equal(ClassifierViewEnum.SummaryView, ViewModel.CurrentView);
        }
    }
}
