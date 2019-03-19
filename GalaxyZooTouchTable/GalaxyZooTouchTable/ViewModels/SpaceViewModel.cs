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
        private SpaceNavigation CurrentLocation { get; set; }

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
            CurrentLocation = new SpaceNavigation(StartingLocation);

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
            CurrentLocation.MoveWest();
            CurrentGalaxies = FindGalaxiesAtNewBounds();

            if (CurrentGalaxies.Count == 0)
            {
                CurrentLocation.Center = _localDBService.FindNextAscendingRa(CurrentLocation.MaxRa);
                CurrentGalaxies = FindGalaxiesAtNewBounds();
            }
        }

        private void OnMoveViewSouth(object obj)
        {
            CurrentLocation.MoveSouth();
            CurrentGalaxies = FindGalaxiesAtNewBounds();

            if (CurrentGalaxies.Count == 0)
            {
                CurrentLocation.Center = _localDBService.FindNextDescendingDec(CurrentLocation.MinDec);
                CurrentGalaxies = FindGalaxiesAtNewBounds();
            }
        }

        private void OnMoveViewEast(object obj)
        {
            CurrentLocation.MoveEast();
            CurrentGalaxies = FindGalaxiesAtNewBounds();

            if (CurrentGalaxies.Count == 0)
            {
                CurrentLocation.Center = _localDBService.FindNextDescendingRa(CurrentLocation.MinRa);
                CurrentGalaxies = FindGalaxiesAtNewBounds();
            }
        }

        private void OnMoveViewNorth(object obj)
        {
            CurrentLocation.MoveNorth();
            CurrentGalaxies = FindGalaxiesAtNewBounds();

            if (CurrentGalaxies.Count == 0)
            {
                CurrentLocation.Center = _localDBService.FindNextAscendingDec(CurrentLocation.MaxDec);
                CurrentGalaxies = FindGalaxiesAtNewBounds();
            }
        }

        private string UpdateSpaceCutout()
        {
            double WidenedPlateScale = 1.75;
            return $"http://skyserver.sdss.org/dr14/SkyServerWS/ImgCutout/getjpeg?ra={CurrentLocation.Center.RightAscension}&dec={CurrentLocation.Center.Declination}&width=1248&height=432&scale={WidenedPlateScale}";
        }

        private List<TableSubject> FindGalaxiesAtNewBounds()
        {
            return _localDBService.GetLocalSubjects(CurrentLocation);
        }
    }
}
