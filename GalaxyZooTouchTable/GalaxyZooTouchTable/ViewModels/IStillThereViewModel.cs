using System;
using System.ComponentModel;

namespace GalaxyZooTouchTable.ViewModels
{
    interface IStillThereViewModel
    {
        void CircleChanged(object sender, PropertyChangedEventArgs e);
        void OnCloseClassifier(object sender);
        void OnCloseModal(object sender);
        void OneSecondElapsed(object sender, EventArgs e);
        void StartTimers();
        void StopTimers();
        void ThirtySecondsElapsed(object sender, EventArgs e);
    }
}
