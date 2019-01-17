using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.ViewModels;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class LevelerViewModelTests
    {
        [Fact]
        public void ShouldIncrementCount()
        {
            var starUser = new StarUser();
            var viewModel = new LevelerViewModel(starUser);

            viewModel.OnIncrementCount(5);
            Assert.Equal(5, viewModel.ClassificationsThisSession);
        }
    }
}
