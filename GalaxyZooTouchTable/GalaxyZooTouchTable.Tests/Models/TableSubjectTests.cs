using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Tests.Mock;
using PanoptesNetClient.Models;
using Xunit;

namespace GalaxyZooTouchTable.Tests.Models
{
    public class TableSubjectTests
    {
        private TableSubject _tableSubject { get; set; }

        public TableSubjectTests()
        {
            Subject _subject = PanoptesServiceMockData.Subject();
            _tableSubject = new TableSubject(_subject, 0, 0);

            _tableSubject.GalaxyRings.Add(new GalaxyRing(1, GlobalData.GetInstance().StarUser));
            _tableSubject.GalaxyRings.Add(new GalaxyRing(2, GlobalData.GetInstance().EarthUser));
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
            _tableSubject.DimRing(GlobalData.GetInstance().EarthUser);
            GalaxyRing EarthRing;
            foreach (GalaxyRing Ring in _tableSubject.GalaxyRings)
            {
                if (Ring.UserName == GlobalData.GetInstance().EarthUser.Name)
                {
                    EarthRing = Ring;
                    Assert.False(EarthRing.CurrentlyClassifying);
                }
            }
        }

        [Fact]
        private void ShouldAddARing()
        {
            _tableSubject.AddRing(GlobalData.GetInstance().LightUser);
            Assert.Equal(4, _tableSubject.GalaxyRings.Count);
        }
    }
}
