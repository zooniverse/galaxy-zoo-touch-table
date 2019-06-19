using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.ViewModels;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class LevelerViewModelTests
    {
        private LevelerViewModel _viewModel;

        public LevelerViewModelTests()
        {
            var User = GlobalData.GetInstance().BlueUser;
            _viewModel = new LevelerViewModel(User);
        }

        [Fact]
        public void ShouldInitializeWithDefaultValues()
        {
            var ExpectedUser = GlobalData.GetInstance().BlueUser;
            Assert.Equal(5, _viewModel.ClassificationsUntilUpgrade);
            Assert.Equal(0, _viewModel.ClassificationsThisSession);
            Assert.Equal(ExpectedUser, _viewModel.User);
            Assert.Equal("One", _viewModel.ClassificationLevel);
            Assert.False(_viewModel.IsOpen);
        }

        [Fact]
        public void ShouldIncrementCount()
        {
            _viewModel.OnIncrementCount();
            Assert.Equal(1, _viewModel.ClassificationsThisSession);
        }

        [Fact]
        public void ShouldCloseLeveler()
        {
            _viewModel.IsOpen = true;
            _viewModel.CloseLeveler();
            Assert.False(_viewModel.IsOpen);
        }

        [Fact]
        public void ShouldToggleLeveler()
        {
            Assert.False(_viewModel.IsOpen);
            _viewModel.SlideLeveler(null);
            Assert.True(_viewModel.IsOpen);
        }

        [Fact]
        public void ShouldLevelUpAccordingly()
        {
            Assert.Equal("One", _viewModel.ClassificationLevel);

            _viewModel.ClassificationsThisSession = 6;
            _viewModel.ClassificationsUntilUpgrade = 0;
            Assert.Equal("Two", _viewModel.ClassificationLevel);

            _viewModel.ClassificationsThisSession = 12;
            _viewModel.ClassificationsUntilUpgrade = 0;
            Assert.Equal("Three", _viewModel.ClassificationLevel);

            _viewModel.ClassificationsThisSession = 18;
            _viewModel.ClassificationsUntilUpgrade = 0;
            Assert.Equal("Four", _viewModel.ClassificationLevel);

            _viewModel.ClassificationsThisSession = 24;
            _viewModel.ClassificationsUntilUpgrade = 0;
            Assert.Equal("Five", _viewModel.ClassificationLevel);
        }
    }
}
