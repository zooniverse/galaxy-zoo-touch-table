using GalaxyZooTouchTable.ViewModels;
using System.Threading.Tasks;
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
            _viewModel.CloseClassifier.Execute(null);
            Assert.True(closeClassificationPanelFired);
        }

        [Fact]
        public void ShouldCloseModal()
        {
            var resetFiveMinuteTimerFired = false;
            _viewModel.ResetFiveMinuteTimer += () => resetFiveMinuteTimerFired = true;
            _viewModel.CloseModal.Execute(null);
            Assert.False(_viewModel.Visible);
            Assert.True(resetFiveMinuteTimerFired);
        }
    }
}
