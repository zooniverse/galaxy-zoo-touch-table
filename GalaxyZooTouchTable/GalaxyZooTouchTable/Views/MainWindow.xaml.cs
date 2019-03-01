﻿using System;
using System.Windows;
using System.Windows.Input;
using GalaxyZooTouchTable.ViewModels;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            _viewModel = viewModel;
            DataContext = viewModel;

            this.Cursor = Cursors.None;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Load();
        }
    }
}
