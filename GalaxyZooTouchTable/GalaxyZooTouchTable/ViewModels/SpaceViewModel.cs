using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Services;
using GalaxyZooTouchTable.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace GalaxyZooTouchTable.ViewModels
{
    public class SpaceViewModel : ViewModelBase
    {
        private ILocalDBService _localDBService;
        private double RaRange { get; set; }
        private double DecRange { get; set; }
        public SpaceNavigation CurrentLocation { get; set; }
        public event Action<CardinalDirectionEnum> AnimateMovement = delegate { };
        CutoutService CutoutService = new CutoutService();

        public ICommand MoveViewNorth { get; private set; }
        public ICommand MoveViewEast { get; set; }
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
                UpdateSpaceCutout();
                SetProperty(ref _currentGalaxies, value);
            }
        }

        private string _previousSpaceCutoutUrl;
        public string PreviousSpaceCutoutUrl
        {
            get => _previousSpaceCutoutUrl;
            set => SetProperty(ref _previousSpaceCutoutUrl, value);
        }

        private string _spaceCutoutUrl;
        public string SpaceCutoutUrl
        {
            get => _spaceCutoutUrl;
            set
            {
                PreviousSpaceCutoutUrl = _spaceCutoutUrl;
                SetProperty(ref _spaceCutoutUrl, value);
            }
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
                if (RingNotifier.Status == RingNotifierStatus.IsLeaving)
                {
                    SpaceViewGalaxy.RemoveRing(RingNotifier.User);
                } else if (RingNotifier.SubjectId == SpaceViewGalaxy.Id)
                {
                    switch (RingNotifier.Status)
                    {
                        case RingNotifierStatus.IsCreating:
                            RemoveAllActiveRingsByUser(RingNotifier.User);
                            SpaceViewGalaxy.AddRing(RingNotifier.User);
                            break;
                        case RingNotifierStatus.IsSubmitting:
                            SpaceViewGalaxy.DimRing(RingNotifier.User);
                            break;
                        default:
                            SpaceViewGalaxy.RemoveRing(RingNotifier.User);
                            break;
                    }
                }
            }
        }

        void RemoveAllActiveRingsByUser(TableUser user)
        {
            TableSubject Galaxy = CurrentGalaxies.Find(x => x.IsWorkingWithUser(user));
            if (Galaxy != null) Galaxy.RemoveRing(user);
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
            CurrentGalaxies = FindGalaxiesAtNewBounds(CardinalDirectionEnum.West);

            if (CurrentGalaxies.Count == 0)
            {
                CurrentLocation.Center = _localDBService.FindNextAscendingRa(CurrentLocation.MaxRa);
                CurrentGalaxies = FindGalaxiesAtNewBounds(CardinalDirectionEnum.West);
            }
            GlobalData.GetInstance().Logger?.AddEntry(entry: "Move_Map", context: "West");
        }

        private void OnMoveViewSouth(object obj)
        {
            CurrentLocation.MoveSouth();
            CurrentGalaxies = FindGalaxiesAtNewBounds(CardinalDirectionEnum.South);

            if (CurrentGalaxies.Count == 0)
            {
                CurrentLocation.Center = _localDBService.FindNextDescendingDec(CurrentLocation.MinDec);
                CurrentGalaxies = FindGalaxiesAtNewBounds(CardinalDirectionEnum.South);
            }
            GlobalData.GetInstance().Logger?.AddEntry(entry: "Move_Map", context: "South");
        }

        private void OnMoveViewEast(object obj)
        {
            CurrentLocation.MoveEast();
            CurrentGalaxies = FindGalaxiesAtNewBounds(CardinalDirectionEnum.East);

            if (CurrentGalaxies.Count == 0)
            {
                CurrentLocation.Center = _localDBService.FindNextDescendingRa(CurrentLocation.MinRa);
                CurrentGalaxies = FindGalaxiesAtNewBounds(CardinalDirectionEnum.East);
            }
            GlobalData.GetInstance().Logger?.AddEntry(entry: "Move_Map", context: "East");
        }

        private void OnMoveViewNorth(object obj)
        {
            CurrentLocation.MoveNorth();
            CurrentGalaxies = FindGalaxiesAtNewBounds(CardinalDirectionEnum.North);

            if (CurrentGalaxies.Count == 0)
            {
                CurrentLocation.Center = _localDBService.FindNextAscendingDec(CurrentLocation.MaxDec);
                CurrentGalaxies = FindGalaxiesAtNewBounds(CardinalDirectionEnum.North);
            }
            GlobalData.GetInstance().Logger?.AddEntry(entry: "Move_Map", context: "North");
        }

        private async void UpdateSpaceCutout()
        {
            double WidenedPlateScale = 1.75;
            SpaceCutoutUrl = await CutoutService.GetSpaceCutout(CurrentLocation.Center.RightAscension, CurrentLocation.Center.Declination, WidenedPlateScale);
        }

        private List<TableSubject> FindGalaxiesAtNewBounds(CardinalDirectionEnum direction = CardinalDirectionEnum.None)
        {
            List<TableSubject> newSubjects = _localDBService.GetLocalSubjects(CurrentLocation);
            if (newSubjects.Count > 0) AnimateMovement(direction);
            return newSubjects;
        }
    }
}
