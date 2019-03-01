using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.ViewModels;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class ExamplesPanelViewModelTests
    {
        private ExamplesPanelViewModel _viewModel;

        public ExamplesPanelViewModelTests()
        {
            _viewModel = new ExamplesPanelViewModel();
        }

        [Fact]
        private void ShouldInstantiateWithDefaultValues()
        {
            Assert.False(_viewModel.IsSelected);
            Assert.True(_viewModel.IsOpen);
            Assert.Null(_viewModel.SelectedExample);
        }

        [Fact]
        private void ShouldSelectAGalaxy()
        {
            GalaxyExample SmoothGalaxy = GalaxyExampleFactory.Create(GalaxyType.Smooth);
            _viewModel.OnToggleItem(SmoothGalaxy);
            Assert.Equal(SmoothGalaxy, _viewModel.SelectedExample);
        }

        [Fact]
        private void ShouldToggleSlidePanel()
        {
            _viewModel.SlidePanel(null);
            Assert.False(_viewModel.IsOpen);
        }

        [Fact]
        private void ShouldResetItself()
        {
            _viewModel.SlidePanel(null);
            GalaxyExample SmoothGalaxy = GalaxyExampleFactory.Create(GalaxyType.Smooth);
            _viewModel.OnToggleItem(SmoothGalaxy);

            _viewModel.ResetExamples();
            Assert.True(_viewModel.IsOpen);
            Assert.Null(_viewModel.SelectedExample);
        }
    }
}
