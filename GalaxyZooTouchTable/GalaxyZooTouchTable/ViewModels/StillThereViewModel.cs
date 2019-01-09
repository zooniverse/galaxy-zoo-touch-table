using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.Utility;
using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace GalaxyZooTouchTable.ViewModels
{
    public class StillThereViewModel : ViewModelBase
    {
        public ClassificationPanelViewModel Classifier { get; set; }
        public DispatcherTimer SecondTimer { get; set; }
        public DispatcherTimer ThirtySecondTimer { get; set; }
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

        public StillThereViewModel(ClassificationPanelViewModel classifier)
        {
            Classifier = classifier;
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

        private void OnCloseClassifier(object sender)
        {
            Classifier.OnCloseClassifier();
        }

        private void OnCloseModal(object sender)
        {
            Visible = false;
            Classifier.ResetTimer();
        }

        private void StartTimers()
        {
            CurrentSeconds = 30;
            Percentage = 100;
            SecondTimer = new DispatcherTimer();
            SecondTimer.Tick += new System.EventHandler(OneSecondElapsed);
            SecondTimer.Interval = new System.TimeSpan(0, 0, 1);
            SecondTimer.Start();

            ThirtySecondTimer = new DispatcherTimer();
            ThirtySecondTimer.Tick += new System.EventHandler(ThirtySecondsElapsed);
            ThirtySecondTimer.Interval = new System.TimeSpan(0, 0, 31);
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
            Classifier.OnCloseClassifier();
            Visible = false;
        }

        private void OneSecondElapsed(object sender, System.EventArgs e)
        {
            CurrentSeconds--;
            decimal StartingSeconds = 30;
            decimal PercentOfSeconds = (CurrentSeconds / StartingSeconds) * 100;
            Percentage = Convert.ToInt16(Math.Floor(PercentOfSeconds));
            Circle.RenderArc(Percentage);
        }
    }
}
