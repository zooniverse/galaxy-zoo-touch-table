using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Tests.Mock;
using GalaxyZooTouchTable.ViewModels;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class SpaceViewModelTests
    {
        private SpaceViewModel _viewModel { get; set; }
        private Mock<ILocalDBService> _localDBServiceMock = new Mock<ILocalDBService>();

        public SpaceViewModelTests()
        {
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
            Assert.False(_viewModel.ShowError);
            _localDBServiceMock.Verify(vm => vm.GetRandomPoint(), Times.Once);
            _localDBServiceMock.Verify(vm => vm.GetLocalSubjects(
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Once);
        }

        public class EmptyResultsAtNextTile
        {
            private Mock<ILocalDBService> _localDBServiceMock = new Mock<ILocalDBService>();
            private SpaceViewModel _viewModel { get; set; }

            public EmptyResultsAtNextTile()
            {
                _localDBServiceMock.Setup(dp => dp.FindNextAscendingDec(It.IsAny<double>())).Returns(new SpacePoint(0,10));
                _localDBServiceMock.Setup(dp => dp.FindNextDescendingDec(It.IsAny<double>())).Returns(new SpacePoint(0,-10));
                _localDBServiceMock.Setup(dp => dp.FindNextAscendingRa(It.IsAny<double>())).Returns(new SpacePoint(10,0));
                _localDBServiceMock.Setup(dp => dp.FindNextDescendingRa(It.IsAny<double>())).Returns(new SpacePoint(-10,0));
                _localDBServiceMock.Setup(dp => dp.GetRandomPoint()).Returns(new SpacePoint());
                _localDBServiceMock.Setup(dp => dp.GetLocalSubjects(
                    It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
                    .Returns(new List<TableSubject>());

                _viewModel = new SpaceViewModel(_localDBServiceMock.Object);
            }

            [Fact]
            private void ShouldMoveUp()
            {
                double OldDec = SpaceNavigation.DEC;
                _viewModel.MoveViewNorth.Execute(null);
                Assert.True(OldDec < SpaceNavigation.DEC);
                _localDBServiceMock.Verify(vm => vm.FindNextAscendingDec(It.IsAny<double>()), Times.Once);
            }

            [Fact]
            private void ShouldMoveRight()
            {
                double OldRa = SpaceNavigation.RA;
                _viewModel.MoveViewEast.Execute(null);
                Assert.True(OldRa > SpaceNavigation.RA);
                _localDBServiceMock.Verify(vm => vm.FindNextDescendingRa(It.IsAny<double>()), Times.Once);
            }

            [Fact]
            private void ShouldMoveDown()
            {
                double OldDec = SpaceNavigation.DEC;
                _viewModel.MoveViewSouth.Execute(null);
                Assert.True(OldDec > SpaceNavigation.DEC);
                _localDBServiceMock.Verify(vm => vm.FindNextDescendingDec(It.IsAny<double>()), Times.Once);
            }

            [Fact]
            private void ShouldMoveLeft()
            {
                double Ra = SpaceNavigation.RA;
                _viewModel.MoveViewWest.Execute(null);
                Assert.True(Ra < SpaceNavigation.RA);
                _localDBServiceMock.Verify(vm => vm.FindNextAscendingRa(It.IsAny<double>()), Times.Once);
            }
        }
    }
}
