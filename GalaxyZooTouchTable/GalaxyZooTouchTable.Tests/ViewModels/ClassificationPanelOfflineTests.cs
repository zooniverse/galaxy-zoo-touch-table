using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.ViewModels;
using Moq;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class ClassificationPanelOfflineTests
    {
        private ClassificationPanelViewModel _viewModel { get; set; }
        private Mock<IPanoptesService> _panoptesServiceMock = new Mock<IPanoptesService>();
        private Mock<ILocalDBService> _localDBServiceMock = new Mock<ILocalDBService>();

        public ClassificationPanelOfflineTests()
        {
            _panoptesServiceMock.Setup(dp => dp.GetWorkflowAsync(It.IsAny<string>()))
                .ReturnsAsync(GlobalData.GetInstance().OfflineWorkflow);

            _viewModel = new ClassificationPanelViewModel(_panoptesServiceMock.Object, _localDBServiceMock.Object, new BlueUser());
        }

        [Fact]
        private async void ShouldLoadAnOfflineWorkflow()
        {
            await _viewModel.GetWorkflow();
            _panoptesServiceMock.Verify(vm => vm.GetWorkflowAsync("1"), Times.Once);
            Assert.NotNull(_viewModel.Workflow);
        }
    }
}
