using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Tests.Mock;
using GalaxyZooTouchTable.ViewModels;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class ClassificationPanelViewModelTests
    {
        private ClassificationPanelViewModel ViewModel { get; set; }

        [Fact]
        public void ShouldFetchWorkflow()
        {
            var starUser = new StarUser();
            ViewModel = new ClassificationPanelViewModel(new PanoptesServiceMock(), new GraphQLServiceMock(), starUser);

            Assert.Equal("1", ViewModel.Workflow.Id);
        }
    }
}
