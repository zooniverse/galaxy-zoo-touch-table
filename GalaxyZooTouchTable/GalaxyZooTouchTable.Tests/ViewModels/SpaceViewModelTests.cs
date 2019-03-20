﻿using GalaxyZooTouchTable.Models;
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
            _localDBServiceMock.Setup(dp => dp.GetRandomPoint()).Returns(SpaceNavigationMockData.Center());
            _localDBServiceMock.Setup(dp => dp.GetLocalSubjects(It.IsAny<SpaceNavigation>()))
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
            _localDBServiceMock.Verify(vm => vm.GetLocalSubjects(It.IsAny<SpaceNavigation>()), Times.Once);
        }

        public class EmptyResultsAtNextTile
        {
            private Mock<ILocalDBService> _localDBServiceMock = new Mock<ILocalDBService>();
            private SpaceViewModel _viewModel { get; set; }
            SpaceNavigation CurrentLocation { get; set; } = SpaceNavigationMockData.CurrentLocation();

            public EmptyResultsAtNextTile()
            {
                _localDBServiceMock.Setup(dp => dp.FindNextAscendingDec(It.IsAny<double>())).Returns(new SpacePoint(0,20));
                _localDBServiceMock.Setup(dp => dp.FindNextDescendingDec(It.IsAny<double>())).Returns(new SpacePoint(0,-20));
                _localDBServiceMock.Setup(dp => dp.FindNextAscendingRa(It.IsAny<double>())).Returns(new SpacePoint(20,0));
                _localDBServiceMock.Setup(dp => dp.FindNextDescendingRa(It.IsAny<double>())).Returns(new SpacePoint(-20,0));
                _localDBServiceMock.Setup(dp => dp.GetRandomPoint()).Returns(SpaceNavigationMockData.Center());
                _localDBServiceMock.Setup(dp => dp.GetLocalSubjects(It.IsAny<SpaceNavigation>()))
                    .Returns(new List<TableSubject>());

                _viewModel = new SpaceViewModel(_localDBServiceMock.Object);
            }

            [Fact]
            private void ShouldMoveUp()
            {
                double OldDec = _viewModel.CurrentLocation.Center.Declination;
                _viewModel.MoveViewNorth.Execute(null);
                Assert.True(OldDec < _viewModel.CurrentLocation.Center.Declination);
                _localDBServiceMock.Verify(vm => vm.FindNextAscendingDec(It.IsAny<double>()), Times.Once);
            }

            [Fact]
            private void ShouldMoveRight()
            {
                double OldRa = _viewModel.CurrentLocation.Center.RightAscension;
                _viewModel.MoveViewEast.Execute(null);
                Assert.True(OldRa > _viewModel.CurrentLocation.Center.RightAscension);
                _localDBServiceMock.Verify(vm => vm.FindNextDescendingRa(It.IsAny<double>()), Times.Once);
            }

            [Fact]
            private void ShouldMoveDown()
            {
                double OldDec = _viewModel.CurrentLocation.Center.Declination;
                _viewModel.MoveViewSouth.Execute(null);
                Assert.True(OldDec > _viewModel.CurrentLocation.Center.Declination);
                _localDBServiceMock.Verify(vm => vm.FindNextDescendingDec(It.IsAny<double>()), Times.Once);
            }

            [Fact]
            private void ShouldMoveLeft()
            {
                double OldRa = _viewModel.CurrentLocation.Center.RightAscension;
                _viewModel.MoveViewWest.Execute(null);
                Assert.True(OldRa < _viewModel.CurrentLocation.Center.RightAscension);
                _localDBServiceMock.Verify(vm => vm.FindNextAscendingRa(It.IsAny<double>()), Times.Once);
            }
        }
    }
}
