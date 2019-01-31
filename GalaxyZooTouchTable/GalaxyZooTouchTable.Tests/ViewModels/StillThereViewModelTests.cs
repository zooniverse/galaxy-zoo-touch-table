using GalaxyZooTouchTable.ViewModels;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class StillThereViewModelTests
    {
        private StillThereViewModel _viewModel;

        public StillThereViewModelTests()
        {
            _viewModel = new StillThereViewModel();
        }

        [Fact]
        public void ShouldInitializeWithDefaultValues()
        {
            Assert.Equal(30, _viewModel.CurrentSeconds);
            Assert.False(_viewModel.Visible);
        }

        [Fact]
        public void ShouldCloseParentClassifier()
        {
            var closeClassificationPanelFired = false;
            _viewModel.CloseClassificationPanel += (s) => closeClassificationPanelFired = true;
            _viewModel.OnCloseClassifier(null);
            Assert.True(closeClassificationPanelFired);
        }

        [Fact]
        public void ShouldCloseModal()
        {
            var resetFiveMinuteTimerFired = false;
            _viewModel.ResetFiveMinuteTimer += () => resetFiveMinuteTimerFired = true;
            _viewModel.OnCloseModal(null);
            Assert.False(_viewModel.Visible);
            Assert.True(resetFiveMinuteTimerFired);
        }

        [Fact]
        public void ShouldClosePanelAfterThirtySeconds()
        {
            var CloseClassificationPanelCalled = false;
            _viewModel.CloseClassificationPanel += (s) => CloseClassificationPanelCalled = true;
            _viewModel.ThirtySecondsElapsed(null, new System.EventArgs());
            Assert.True(CloseClassificationPanelCalled);
            Assert.False(_viewModel.Visible);
        }

        [Fact]
        public void ShouldCalculatePercentageAtEachSecond()
        {
            _viewModel.CurrentSeconds = 16;
            _viewModel.OneSecondElapsed(null, new System.EventArgs());
            Assert.Equal(50, _viewModel.Percentage);
        }
    }
}
