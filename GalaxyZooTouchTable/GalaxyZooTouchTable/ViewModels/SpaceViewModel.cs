using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Utility;
using System.Collections.Generic;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class SpaceViewModel : ViewModelBase
    {
        private ILocalDBService _localDBService;
        private double RaRange { get; set; }
        private double DecRange { get; set; }

        public ICommand MoveViewNorth { get; private set; }
        public ICommand MoveViewEast { get; private set; }
        public ICommand MoveViewSouth { get; private set; }
        public ICommand MoveViewWest { get; private set; }
        public ICommand HideError { get; private set; }

        private bool _showError = false;
        public bool ShowError
        {
            get => _showError;
            set => SetProperty(ref _showError, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private List<TableSubject> _currentGalaxies = new List<TableSubject>();
        public List<TableSubject> CurrentGalaxies
        {
            get => _currentGalaxies;
            set 
            {
                SpaceCutoutUrl = UpdateSpaceCutout();
                SetProperty(ref _currentGalaxies, value);
            }
        }

        private string _spaceCutoutUrl;
        public string SpaceCutoutUrl
        {
            get => _spaceCutoutUrl;
            set => SetProperty(ref _spaceCutoutUrl, value);
        }

        public SpaceViewModel(ILocalDBService localDBService)
        {
            _localDBService = localDBService;

            SpacePoint StartingLocation = _localDBService.GetRandomPoint();
            SpaceNavigation.RA = StartingLocation.RightAscension;
            SpaceNavigation.DEC = StartingLocation.Declination;

            GetRange();
            CurrentGalaxies = FindGalaxiesAtNewBounds();
            LoadCommands();
            Messenger.Default.Register<ClassificationRingNotifier>(this, OnGalaxyInteraction);
            Messenger.Default.Register<string>(this, OnShowError, "DatabaseError");
        }

        private void OnGalaxyInteraction(ClassificationRingNotifier RingNotifier)
        {
            foreach (TableSubject SpaceViewGalaxy in CurrentGalaxies)
            {
                if (RingNotifier.SubjectId == SpaceViewGalaxy.Id)
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
                }
                else if (RingNotifier.Status == RingNotifierStatus.IsLeaving)
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
            RaRange = (CutoutWidth * SpaceNavigation.PlateScale / ArcDegreeInSeconds) / System.Math.Abs(System.Math.Cos((ToRadians(SpaceNavigation.DEC))));
        }

        private double ToRadians(double Degrees)
        {
            return (Degrees * System.Math.PI) / 180.0;
        }

        private void LoadCommands()
        {
            MoveViewNorth = new CustomCommand(OnMoveViewNorth);
            MoveViewEast = new CustomCommand(OnMoveViewEast);
            MoveViewSouth = new CustomCommand(OnMoveViewSouth);
            MoveViewWest = new CustomCommand(OnMoveViewWest);
            HideError = new CustomCommand(OnHideError);
        }

        private void OnHideError(object obj)
        {
            ShowError = false;
        }

        private void OnShowError(string message)
        {
            ShowError = true;
            ErrorMessage = message;
        }

        private void OnMoveViewWest(object obj)
        {
            SpaceNavigation.RA += RaRange;
            CurrentGalaxies = FindGalaxiesAtNewBounds();

            if (CurrentGalaxies.Count == 0)
            {
                double NewBounds = SpaceNavigation.RA + (RaRange / 2);
                SpacePoint newCenter = _localDBService.FindNextAscendingRa(NewBounds);
                SpaceNavigation.RA = newCenter.RightAscension;
                SpaceNavigation.DEC = newCenter.Declination;
                CurrentGalaxies = FindGalaxiesAtNewBounds();
            }
        }

        private void OnMoveViewSouth(object obj)
        {
            SpaceNavigation.DEC -= DecRange;
            CurrentGalaxies = FindGalaxiesAtNewBounds();

            if (CurrentGalaxies.Count == 0)
            {
                double NewBounds = SpaceNavigation.DEC - (DecRange / 2);
                SpacePoint newCenter = _localDBService.FindNextDescendingDec(NewBounds);
                SpaceNavigation.RA = newCenter.RightAscension;
                SpaceNavigation.DEC = newCenter.Declination;
                CurrentGalaxies = FindGalaxiesAtNewBounds();
            }
        }

        private void OnMoveViewEast(object obj)
        {
            SpaceNavigation.RA -= RaRange;
            CurrentGalaxies = FindGalaxiesAtNewBounds();

            if (CurrentGalaxies.Count == 0)
            {
                double NewBounds = SpaceNavigation.RA - (RaRange / 2);
                SpacePoint newCenter = _localDBService.FindNextDescendingRa(NewBounds);
                SpaceNavigation.RA = newCenter.RightAscension;
                SpaceNavigation.DEC = newCenter.Declination;
                CurrentGalaxies = FindGalaxiesAtNewBounds();
            }
        }

        private void OnMoveViewNorth(object obj)
        {
            SpaceNavigation.DEC += DecRange;
            CurrentGalaxies = FindGalaxiesAtNewBounds();

            if (CurrentGalaxies.Count == 0)
            {
                double NewBounds = SpaceNavigation.DEC + (DecRange / 2);
                SpacePoint newCenter = _localDBService.FindNextAscendingDec(NewBounds);
                SpaceNavigation.RA = newCenter.RightAscension;
                SpaceNavigation.DEC = newCenter.Declination;
                CurrentGalaxies = FindGalaxiesAtNewBounds();
            }
        }

        private string UpdateSpaceCutout()
        {
            double WidenedPlateScale = 1.8;
            return $"http://skyserver.sdss.org/dr14/SkyServerWS/ImgCutout/getjpeg?ra={SpaceNavigation.RA}&dec={SpaceNavigation.DEC}&width=1248&height=432&scale={WidenedPlateScale}";
        }

        private List<TableSubject> FindGalaxiesAtNewBounds()
        {
            double minRa = SpaceNavigation.RA - (RaRange / 2); 
            double maxRa = SpaceNavigation.RA + (RaRange / 2);
            double minDec = SpaceNavigation.DEC - (DecRange /2);
            double maxDec = SpaceNavigation.DEC + (DecRange / 2);
            return _localDBService.GetLocalSubjects(minRa, maxRa, minDec, maxDec);
        }
    }
}
