using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.ViewModels;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class LevelerViewModelTests
    {
        private LevelerViewModel ViewModel { get; set; }

        [Fact]
        public void ShouldIncrementCount()
        {
            var starUser = new StarUser();
            ViewModel = new LevelerViewModel(starUser);

            ViewModel.OnIncrementCount(5);
            Assert.Equal(5, ViewModel.ClassificationsThisSession);
        }
    }
}
