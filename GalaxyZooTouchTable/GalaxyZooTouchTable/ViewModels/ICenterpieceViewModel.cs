﻿using System.Collections.Specialized;
using System.ComponentModel;

namespace GalaxyZooTouchTable.ViewModels
{
    public interface ICenterpieceViewModel : INotifyPropertyChanged
    {
        void AllUsersCollectionChanged(object sender, NotifyCollectionChangedEventArgs changedEventArgs);
        void CreateTimer();
        void ItemPropertyChanged(object sender, PropertyChangedEventArgs changedEventArgs);
        void OnFlipCenterpiece(object sender, System.EventArgs e);
    }
}
