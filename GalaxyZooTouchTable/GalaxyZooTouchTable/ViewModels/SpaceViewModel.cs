using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using PanoptesNetClient;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.ViewModels
{
    public class SpaceViewModel : ViewModelBase
    {
        public double RA { get; set; } = 250.3035;
        public double DEC { get; set; } = 35.09;
        public double SCALE { get; set; } = 1.5;

        public List<TableSubject> _currentGalaxies = new List<TableSubject>();
        public List<TableSubject> CurrentGalaxies
        {
            get => _currentGalaxies;
            set => SetProperty(ref _currentGalaxies, value);
        }

        private string _spaceCutoutUrl;
        public string SpaceCutoutUrl
        {
            get => _spaceCutoutUrl;
            set => SetProperty(ref _spaceCutoutUrl, value);
        }

        public SpaceViewModel()
        {
            PrepareForNewPosition();
            Messenger.Default.Register<ClassificationRingNotifier>(this, OnGalaxyInteraction);
        }

        private void OnGalaxyInteraction(ClassificationRingNotifier RingNotifier)
        {
            foreach (TableSubject SpaceViewGalaxy in CurrentGalaxies)
            {
                if (RingNotifier.Subject == SpaceViewGalaxy.Subject)
                {
                    if (RingNotifier.IsCreating)
                    {
                        SpaceViewGalaxy.AddRing(RingNotifier.User);
                    } else
                    {
                        SpaceViewGalaxy.DimRing(RingNotifier.User);
                    }
                }
            }
        }
        
        private void GetSpaceCutout()
        {
            SpaceCutoutUrl = $"http://skyserver.sdss.org/dr14/SkyServerWS/ImgCutout/getjpeg?ra={RA}&dec={DEC}&width=1248&height=432&scale={SCALE}";
        }

        private async void PrepareForNewPosition()
        {
            GetSpaceCutout();
            await GetSubjectsAsync();
        }

        private async Task GetSubjectsAsync()
        {
            ApiClient client = new ApiClient();
            NameValueCollection query = new NameValueCollection();
            query.Add("workflow_id", Config.WorkflowId);
            query.Add("page_size", "25");

            var GalaxyList = await client.Subjects.GetList("queued", query);
            foreach (Subject subject in GalaxyList)
            {
                TableSubject Galaxy = new TableSubject(subject, RA, DEC);
                CurrentGalaxies.Add(Galaxy);
            }
        }
    }
}
