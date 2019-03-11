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
        private ILocalDBService _localDBService;
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

        public SpaceViewModel(IPanoptesService panoptesService, ILocalDBService localDBService)
        {
            _panoptesService = panoptesService;
            _localDBService = localDBService;

            SpacePoint StartingLocation = _localDBService.GetRandomPoint();
            SpaceNavigation.RA = StartingLocation.RightAscension;
            SpaceNavigation.DEC = StartingLocation.Declination;
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

            DecRange = CutoutHeight * SpaceNavigation.PlateScale / ArcDegreeInSeconds;
            RaRange = Math.Abs(CutoutWidth * SpaceNavigation.PlateScale / ArcDegreeInSeconds / Math.Cos(SpaceNavigation.DEC));
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
            SpaceNavigation.RA += RaRange;
            PrepareForNewPosition();

            if (CurrentGalaxies.Count == 0)
            {
                double NewBounds = SpaceNavigation.RA + (RaRange / 2);
                SpacePoint newCenter = _localDBService.FindNextAscendingRa(NewBounds);
                SpaceNavigation.RA = newCenter.RightAscension;
                SpaceNavigation.DEC = newCenter.Declination;
                PrepareForNewPosition();
            }
        }

        private void OnMoveViewSouth(object obj)
        {
            SpaceNavigation.DEC -= DecRange;
            PrepareForNewPosition();

            if (CurrentGalaxies.Count == 0)
            {
                double NewBounds = SpaceNavigation.DEC - (DecRange / 2);
                SpacePoint newCenter = _localDBService.FindNextDescendingDec(NewBounds);
                SpaceNavigation.RA = newCenter.RightAscension;
                SpaceNavigation.DEC = newCenter.Declination;
                PrepareForNewPosition();
            }
        }

        private void OnMoveViewEast(object obj)
        {
            SpaceNavigation.RA -= RaRange;
            PrepareForNewPosition();

            if (CurrentGalaxies.Count == 0)
            {
                double NewBounds = SpaceNavigation.RA - (RaRange / 2);
                SpacePoint newCenter = _localDBService.FindNextDescendingRa(NewBounds);
                SpaceNavigation.RA = newCenter.RightAscension;
                SpaceNavigation.DEC = newCenter.Declination;
                PrepareForNewPosition();
            }
        }

        private void OnMoveViewNorth(object obj)
        {
            SpaceNavigation.DEC += DecRange;
            PrepareForNewPosition();

            if (CurrentGalaxies.Count == 0)
            {
                double NewBounds = SpaceNavigation.DEC + (DecRange / 2);
                SpacePoint newCenter = _localDBService.FindNextAscendingDec(NewBounds);
                SpaceNavigation.RA = newCenter.RightAscension;
                SpaceNavigation.DEC = newCenter.Declination;
                PrepareForNewPosition();
            }
        }

        private void GetSpaceCutout()
        {
            SpaceCutoutUrl = $"http://skyserver.sdss.org/dr14/SkyServerWS/ImgCutout/getjpeg?ra={SpaceNavigation.RA}&dec={SpaceNavigation.DEC}&width=1248&height=432&scale={SpaceNavigation.PlateScale}";
        }

        private void PrepareForNewPosition()
        {
            GetSpaceCutout();

            double minRa = SpaceNavigation.RA - (RaRange / 2); 
            double maxRa = SpaceNavigation.RA + (RaRange / 2);
            double minDec = SpaceNavigation.DEC - (DecRange /2);
            double maxDec = SpaceNavigation.DEC + (DecRange / 2);
            CurrentGalaxies = _localDBService.GetLocalSubjects(minRa, maxRa, minDec, maxDec);
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
