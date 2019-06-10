using System;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using GalaxyZooTouchTable.Lib;

namespace GalaxyZooTouchTable.ViewModels
{
    public class StillThereViewModel : ViewModelBase
    {
        public DispatcherTimer SecondTimer { get; set; } = new DispatcherTimer();
        public event Action<object> CloseClassificationPanel = delegate { };
        public event Action ResetFiveMinuteTimer = delegate { };
        private int Percentage { get; set; } = 100;
        public event Action CheckOverlay = delegate { };
        TableUser User { get; set; }

        public ICommand CloseClassifier { get; set; }
        public ICommand CloseModal { get; set; }

        private CircularProgress _circle;
        public CircularProgress Circle
        {
            get => _circle;
            set => SetProperty(ref _circle, value);
        }

        private decimal _currentSeconds = 30;
        public decimal CurrentSeconds
        {
            get => _currentSeconds;
            set => SetProperty(ref _currentSeconds, value);
        }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == true)
                {
                    StartTimer();
                } else
                {
                    SecondTimer.Stop();
                }
                SetProperty(ref _isVisible, value);
                CheckOverlay();
            }
        }

        public StillThereViewModel(TableUser user)
        {
            User = user;
            Circle = new CircularProgress(41);
            LoadCommands();
            Circle.RenderArc(Percentage);
            Circle.PropertyChanged += CircleChanged;

            SetTimer();
        }

        private void CircleChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Circle");
        }

        private void LoadCommands()
        {
            CloseClassifier = new CustomCommand(OnCloseClassifier);
            CloseModal = new CustomCommand(OnCloseModal);
        }

        private void OnCloseClassifier(object sender)
        {
            CloseClassificationPanel(sender);
            Visible = false;
            GlobalData.GetInstance().Logger.AddEntry("Close_From_Still_There", User.Name);
        }

        private void OnCloseModal(object sender)
        {
            IsVisible = false;
            ResetFiveMinuteTimer();
            GlobalData.GetInstance().Logger.AddEntry("Dismiss_Still_There", User.Name);
        }

        private void SetTimer()
        {
            SecondTimer.Tick += new EventHandler(OneSecondElapsed);
            SecondTimer.Interval = new TimeSpan(0, 0, 1);
        }

        private void StartTimer()
        {
            CurrentSeconds = 30;
            Percentage = 100;

            SecondTimer.Stop();
            SecondTimer.Start();
            Circle.RenderArc(Percentage);
        }

        private void OneSecondElapsed(object sender, EventArgs e)
        {
            CurrentSeconds--;
            decimal StartingSeconds = 30;
            decimal PercentOfSeconds = (CurrentSeconds / StartingSeconds) * 100;
            Percentage = Convert.ToInt16(Math.Floor(PercentOfSeconds));
            Circle.RenderArc(Percentage);

            if (CurrentSeconds == 0)
            {
                Console.WriteLine(User);
                CloseClassificationPanel(null);
                IsVisible = false;
                GlobalData.GetInstance().Logger.AddEntry("Close_Timeout");
            }
        }
    }
}
