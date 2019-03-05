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
        public DispatcherTimer SecondTimer { get; set; } = new DispatcherTimer();
        public DispatcherTimer ThirtySecondTimer { get; set; } = new DispatcherTimer();
        public event Action<object> CloseClassificationPanel = delegate { };
        public event Action ResetFiveMinuteTimer = delegate { };
        private int Percentage { get; set; } = 100;

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

            SetTimers();
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
        }

        private void OnCloseModal(object sender)
        {
            Visible = false;
            ResetFiveMinuteTimer();
        }

        private void SetTimers()
        {
            SecondTimer.Tick += new EventHandler(OneSecondElapsed);
            SecondTimer.Interval = new TimeSpan(0, 0, 1);

            ThirtySecondTimer.Tick += new EventHandler(ThirtySecondsElapsed);
            ThirtySecondTimer.Interval = new TimeSpan(0, 0, 31);
        }

        private void StartTimers()
        {
            CurrentSeconds = 30;
            Percentage = 100;

            SecondTimer.Stop();
            ThirtySecondTimer.Stop();

            SecondTimer.Start();
            ThirtySecondTimer.Start();
            Circle.RenderArc(Percentage);
        }

        private void StopTimers()
        {
            SecondTimer.Stop();
            ThirtySecondTimer.Stop();
        }

        private void ThirtySecondsElapsed(object sender, EventArgs e)
        {
            CloseClassificationPanel(sender);
            Visible = false;
        }

        private void OneSecondElapsed(object sender, EventArgs e)
        {
            CurrentSeconds--;
            decimal StartingSeconds = 30;
            decimal PercentOfSeconds = (CurrentSeconds / StartingSeconds) * 100;
            Percentage = Convert.ToInt16(Math.Floor(PercentOfSeconds));
            Circle.RenderArc(Percentage);
        }
    }
}
