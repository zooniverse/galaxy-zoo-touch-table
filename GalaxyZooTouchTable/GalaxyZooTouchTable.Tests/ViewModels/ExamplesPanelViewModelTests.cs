using GalaxyZooTouchTable.Lib;
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
            var user = GlobalData.GetInstance().BlueUser;
            _viewModel = new ExamplesPanelViewModel(user, ClassificationPanelViewModelTests.MockClassificationPanel());
        }

        [Fact]
        private void ShouldInstantiateWithDefaultValues()
        {
            Assert.False(_viewModel.IsSelected);
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
        private void ShouldResetItself()
        {
            GalaxyExample SmoothGalaxy = GalaxyExampleFactory.Create(GalaxyType.Smooth);
            _viewModel.OnToggleItem(SmoothGalaxy);

            _viewModel.ResetExamples();
            Assert.Null(_viewModel.SelectedExample);
        }
    }
}
