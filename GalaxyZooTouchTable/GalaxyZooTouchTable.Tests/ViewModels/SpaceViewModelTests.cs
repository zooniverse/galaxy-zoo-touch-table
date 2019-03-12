using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Tests.Mock;
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
        private Mock<ILocalDBService> _localDBServiceMock = new Mock<ILocalDBService>();
        private NameValueCollection _query;

        public SpaceViewModelTests()
        {
            _query = new NameValueCollection
                {
                    { "workflow_id", "1" },
                    { "page_size", "25" }
                };

            _localDBServiceMock.Setup(dp => dp.GetRandomPoint()).Returns(new SpacePoint());
            _localDBServiceMock.Setup(dp => dp.GetLocalSubjects(
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
                .Returns(PanoptesServiceMockData.TableSubjects());

            _viewModel = new SpaceViewModel(_localDBServiceMock.Object);
        }

        [Fact]
        private void ShouldInitializeWithDefaultValues()
        {
            Assert.NotNull(_viewModel);
            Assert.NotNull(_viewModel.CurrentGalaxies);
            Assert.NotNull(_viewModel.SpaceCutoutUrl);
            _localDBServiceMock.Verify(vm => vm.GetRandomPoint(), Times.Once);
            _localDBServiceMock.Verify(vm => vm.GetLocalSubjects(
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Once);
        }
    }
}
