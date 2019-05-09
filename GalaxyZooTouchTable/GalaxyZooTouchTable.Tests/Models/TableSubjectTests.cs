using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Tests.Mock;
using Xunit;

namespace GalaxyZooTouchTable.Tests.Models
{
    public class TableSubjectTests
    {
        private TableSubject _tableSubject = PanoptesServiceMockData.TableSubject();

        public TableSubjectTests()
        {
            _tableSubject.GalaxyRings.Add(new GalaxyRing(1, GlobalData.GetInstance().StarUser));
            _tableSubject.GalaxyRings.Add(new GalaxyRing(2, GlobalData.GetInstance().GreenUser));
        }

        [Fact]
        private void ShouldRemoveRing()
        {
            Assert.Equal(3, _tableSubject.GalaxyRings.Count);
            _tableSubject.RemoveRing(GlobalData.GetInstance().StarUser);
            Assert.Equal(2, _tableSubject.GalaxyRings.Count);
        }

        [Fact]
        private void ShouldMarkARingAsClassified()
        {
            _tableSubject.DimRing(GlobalData.GetInstance().GreenUser);
            GalaxyRing GreenRing;
            foreach (GalaxyRing Ring in _tableSubject.GalaxyRings)
            {
                if (Ring.UserName == GlobalData.GetInstance().GreenUser.Name)
                {
                    GreenRing = Ring;
                    Assert.False(GreenRing.CurrentlyClassifying);
                }
            }
        }

        [Fact]
        private void ShouldAddARing()
        {
            _tableSubject.AddRing(GlobalData.GetInstance().AquaUser);
            Assert.Equal(4, _tableSubject.GalaxyRings.Count);
        }
    }
}
