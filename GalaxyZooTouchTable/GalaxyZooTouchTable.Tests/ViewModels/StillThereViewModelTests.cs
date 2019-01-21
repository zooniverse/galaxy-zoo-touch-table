using GalaxyZooTouchTable.ViewModels;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class StillThereViewModelTests
    {
        private StillThereViewModel ViewModel = new StillThereViewModel();

        [Fact]
        public void ShouldInitializeWithDefaultValues()
        {
            Assert.Equal(30, ViewModel.CurrentSeconds);
            Assert.False(ViewModel.Visible);
        }

        [Fact]
        public void ShouldCloseParentClassifier()
        {
            var CloseClassificationPanelCalled = false;
            ViewModel.CloseClassificationPanel += (s) => CloseClassificationPanelCalled = true;
            ViewModel.OnCloseClassifier(null);

            Assert.True(CloseClassificationPanelCalled);
        }

        [Fact]
        public void ShouldCloseModal()
        {
            var ResetFiveMinuteTimerCalled = false;
            ViewModel.ResetFiveMinuteTimer += () => ResetFiveMinuteTimerCalled = true;
            ViewModel.OnCloseModal(null);

            Assert.False(ViewModel.Visible);
            Assert.True(ResetFiveMinuteTimerCalled);
        }

        [Fact]
        public void ShouldClosePanelAfterThirtySeconds()
        {
            var CloseClassificationPanelCalled = false;
            ViewModel.CloseClassificationPanel += (s) => CloseClassificationPanelCalled = true;

            ViewModel.ThirtySecondsElapsed(null, new System.EventArgs());
            Assert.True(CloseClassificationPanelCalled);
            Assert.False(ViewModel.Visible);
        }

        [Fact]
        public void ShouldCalculatePercentageAtEachSecond()
        {
            ViewModel.CurrentSeconds = 16;
            ViewModel.OneSecondElapsed(null, new System.EventArgs());
            Assert.Equal(50, ViewModel.Percentage);
        }
    }
}
