using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.ViewModels;
using Moq;
using System.Collections.Specialized;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class SpaceViewModelTests
    {
        private SpaceViewModel _viewModel { get; set; }
        private Mock<IPanoptesService> _panoptesServiceMock = new Mock<IPanoptesService>();
        private NameValueCollection _query;

        public SpaceViewModelTests()
        {
            _query = new NameValueCollection
                {
                    { "workflow_id", "1" },
                    { "page_size", "25" }
                };

            _viewModel = new SpaceViewModel(_panoptesServiceMock.Object);
        }

        [Fact]
        private void ShouldInitializeWithDefaultValues()
        {
            Assert.NotNull(_viewModel.CurrentGalaxies);
            Assert.NotNull(_viewModel.SpaceCutoutUrl);
            _panoptesServiceMock.Verify(vm => vm.GetSubjectsAsync("queued", _query), Times.Once);
        }
    }
}
