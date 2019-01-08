using GalaxyZooTouchTable.Models;
using PanoptesNetClient;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.ViewModels
{
    public class SpaceViewModel : INotifyPropertyChanged
    {
        public double RA { get; set; } = 250.3035;
        public double DEC { get; set; } = 35.09;
        public double SCALE { get; set; } = 1.5;

        public List<TableSubject> _currentGalaxies = new List<TableSubject>();
        public List<TableSubject> CurrentGalaxies
        {
            get { return _currentGalaxies; }
            set
            {
                _currentGalaxies = value;
                OnPropertyRaised("CurrentGalaxies");
            }
        }

        private string _spaceCutoutUrl;
        public string SpaceCutoutUrl
        {
            get { return _spaceCutoutUrl; }
            set
            {
                _spaceCutoutUrl = value;
                OnPropertyRaised("SpaceCutoutUrl");
            }
        }

        public SpaceViewModel()
        {
            PrepareForNewPosition();
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
