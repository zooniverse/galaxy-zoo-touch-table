using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Utility;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class SpaceViewModel : ViewModelBase
    {
        private IPanoptesService _panoptesService;
        public double RA { get; set; } = 257.9;
        public double DEC { get; set; } = 23.23;
        public double PLATE_SCALE { get; set; } = 1.5;

        private double RaRange { get; set; }
        private double DecRange { get; set; }

        public ICommand MoveViewNorth { get; private set; }
        public ICommand MoveViewEast { get; private set; }
        public ICommand MoveViewSouth { get; private set; }
        public ICommand MoveViewWest { get; private set; }

        private List<TableSubject> _currentGalaxies = new List<TableSubject>();
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

        public SpaceViewModel(IPanoptesService panoptesService)
        {
            _panoptesService = panoptesService;
            GetRange();
            PrepareForNewPosition();
            LoadCommands();
            Messenger.Default.Register<ClassificationRingNotifier>(this, OnGalaxyInteraction);
        }

        private void OnGalaxyInteraction(ClassificationRingNotifier RingNotifier)
        {
            foreach (TableSubject SpaceViewGalaxy in CurrentGalaxies)
            {
                if (RingNotifier.SubjectId == SpaceViewGalaxy.Subject.Id)
                {
                    switch (RingNotifier.Status)
                    {
                        case RingNotifierStatus.IsCreating:
                            SpaceViewGalaxy.AddRing(RingNotifier.User);
                            break;
                        case RingNotifierStatus.IsSubmitting:
                            SpaceViewGalaxy.DimRing(RingNotifier.User);
                            break;
                        case RingNotifierStatus.IsHelping:
                            SpaceViewGalaxy.RemoveRing(RingNotifier.User);
                            break;
                        default:
                            break;
                    }
                } else if (RingNotifier.Status == RingNotifierStatus.IsLeaving)
                {
                    SpaceViewGalaxy.RemoveRing(RingNotifier.User);
                }
            }
        }

        private void GetRange()
        {
            int CutoutWidth = 1248;
            int CutoutHeight = 432;
            const int ArcDegreeInSeconds = 3600;

            DecRange = CutoutHeight * PLATE_SCALE / ArcDegreeInSeconds;
            RaRange = Math.Abs(CutoutWidth * PLATE_SCALE / ArcDegreeInSeconds / Math.Cos(DEC));
        }

        private void LoadCommands()
        {
            MoveViewNorth = new CustomCommand(OnMoveViewNorth);
            MoveViewEast = new CustomCommand(OnMoveViewEast);
            MoveViewSouth = new CustomCommand(OnMoveViewSouth);
            MoveViewWest = new CustomCommand(OnMoveViewWest);
        }

        private void OnMoveViewWest(object obj)
        {
            RA += RaRange;
            PrepareForNewPosition();
        }

        private void OnMoveViewSouth(object obj)
        {
            DEC -= DecRange;
            PrepareForNewPosition();
        }

        private void OnMoveViewEast(object obj)
        {
            RA -= RaRange;
            PrepareForNewPosition();
        }

        private void OnMoveViewNorth(object obj)
        {
            DEC += DecRange;
            PrepareForNewPosition();
        }

        private void GetSpaceCutout()
        {
            SpaceCutoutUrl = $"http://skyserver.sdss.org/dr14/SkyServerWS/ImgCutout/getjpeg?ra={RA}&dec={DEC}&width=1248&height=432&scale={PLATE_SCALE}";
        }

        private void PrepareForNewPosition()
        {
            GetSpaceCutout();

            double minRa = RA - (RaRange / 2);
            double maxRa = RA + (RaRange / 2);
            double minDec = DEC - (DecRange / 2);
            double maxDec = DEC + (DecRange / 2);
            CurrentGalaxies = LocalDBService.GetLocalSubjects(minRa, maxRa, minDec, maxDec);
        }

        private async Task GetSubjectsAsync()
        {
            NameValueCollection query = new NameValueCollection();
            query.Add("workflow_id", Config.WorkflowId);
            query.Add("page_size", "25");

            var GalaxyList = await _panoptesService.GetSubjectsAsync("queued", query);
            foreach (Subject subject in GalaxyList)
            {
                TableSubject Galaxy = new TableSubject(subject, RA, DEC);
                CurrentGalaxies.Add(Galaxy);
            }
        }
    }
}
