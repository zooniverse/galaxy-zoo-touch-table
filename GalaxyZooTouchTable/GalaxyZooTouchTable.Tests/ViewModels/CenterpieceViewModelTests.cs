using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.ViewModels;
using System;
using Xunit;

namespace GalaxyZooTouchTable.Tests.ViewModels
{
    public class CenterpieceViewModelTests
    {
        private CenterpieceViewModel ViewModel = new CenterpieceViewModel();

        [Fact]
        private void ShouldInstantiateWithDefaultValues()
        {
            Assert.True(ViewModel.AllUsers.Count > 0);
            Assert.True(ViewModel.ShowJoinMessage);
            Assert.False(ViewModel.CenterpieceIsFlipped);
        }

        [Fact]
        private void ShouldHideJoinMessageWithActiveUser()
        {
            GlobalData.GetInstance().StarUser.Active = true;
            Assert.False(ViewModel.ShowJoinMessage);
        }

        [Fact]
        private void ShouldFlipCenterpiece()
        {
            ViewModel.OnFlipCenterpiece(null, new EventArgs());
            Assert.True(ViewModel.CenterpieceIsFlipped);
        }
    }
}
