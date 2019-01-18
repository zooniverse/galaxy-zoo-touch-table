using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.ViewModels;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class ExamplesPanelViewModelTests
    {
        private ExamplesPanelViewModel ViewModel = new ExamplesPanelViewModel();

        [Fact]
        private void ShouldInstantiateWithDefaultValues()
        {
            Assert.False(ViewModel.IsSelected);
            Assert.True(ViewModel.IsOpen);
        }

        [Fact]
        private void ShouldSelectAGalaxy()
        {
            GalaxyExample SmoothGalaxy = GalaxyExampleFactory.Create(GalaxyType.Smooth);
            ViewModel.OnToggleItem(SmoothGalaxy);
            Assert.Equal(SmoothGalaxy, ViewModel.SelectedExample);
        }

        [Fact]
        private void ShouldToggleSlidePanel()
        {
            ViewModel.SlidePanel(null);
            Assert.False(ViewModel.IsOpen);
        }
    }
}
