﻿using GalaxyZooTouchTable.Models;
using GalaxyZooTouchTable.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GalaxyZooTouchTable.Views
{
    /// <summary>
    /// Interaction logic for DropZone.xaml
    /// </summary>
    public partial class DropZone : UserControl, IDroppableArea
    {
        public DropZone()
        {
            InitializeComponent();
        }

        public bool IsUnder(Point p)
        {
            throw new NotImplementedException();
        }

        void IDroppableArea.Drop(FrameworkElement element)
        {
            TableSubject passedSubject = element.DataContext as TableSubject;
            ClassificationPanelViewModel viewModel = DataContext as ClassificationPanelViewModel;

            viewModel.DropSubject(passedSubject);
        }

        protected override System.Windows.Media.HitTestResult HitTestCore(System.Windows.Media.PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}