using System;
using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class StillThereViewModel : ViewModelBase
    {
        public DispatcherTimer SecondTimer { get; set; }
        public DispatcherTimer ThirtySecondTimer { get; set; }
        public event Action<object> CloseClassificationPanel = delegate { };
        public event Action ResetFiveMinuteTimer = delegate { };
        public int Percentage { get; set; } = 100;

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

        private bool _visible = false;
        public bool Visible
        {
            get => _visible;
            set
            {
                if (value == true)
                {
                    StartTimers();
                } else
                {
                    StopTimers();
                }
                SetProperty(ref _visible, value);
            }
        }

        public StillThereViewModel()
        {
            Circle = new CircularProgress(41);
            LoadCommands();
            Circle.RenderArc(Percentage);
            Circle.PropertyChanged += CircleChanged;
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

        public void OnCloseClassifier(object sender)
        {
            CloseClassificationPanel(sender);
        }

        public void OnCloseModal(object sender)
        {
            Visible = false;
            ResetFiveMinuteTimer();
        }

        private void StartTimers()
        {
            CurrentSeconds = 30;
            Percentage = 100;
            SecondTimer = new DispatcherTimer();
            SecondTimer.Tick += new EventHandler(OneSecondElapsed);
            SecondTimer.Interval = new TimeSpan(0, 0, 1);
            SecondTimer.Start();

            ThirtySecondTimer = new DispatcherTimer();
            ThirtySecondTimer.Tick += new EventHandler(ThirtySecondsElapsed);
            ThirtySecondTimer.Interval = new TimeSpan(0, 0, 31);
            ThirtySecondTimer.Start();
            Circle.RenderArc(Percentage);
        }

        private void StopTimers()
        {
            if (SecondTimer != null && ThirtySecondTimer != null)
            {
                SecondTimer.Stop();
                ThirtySecondTimer.Stop();
            }
        }

        public void ThirtySecondsElapsed(object sender, EventArgs e)
        {
            CloseClassificationPanel(sender);
            Visible = false;
        }

        public void OneSecondElapsed(object sender, EventArgs e)
        {
            CurrentSeconds--;
            decimal StartingSeconds = 30;
            decimal PercentOfSeconds = (CurrentSeconds / StartingSeconds) * 100;
            Percentage = Convert.ToInt16(Math.Floor(PercentOfSeconds));
            Circle.RenderArc(Percentage);
        }
    }
}
