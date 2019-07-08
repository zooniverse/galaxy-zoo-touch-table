using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

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
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;

            _viewModel = viewModel;
            DataContext = viewModel;

            Cursor = Cursors.None;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalData.GetInstance().EstablishLog();
            _viewModel.Load();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            GlobalData.GetInstance().Logger?.FinalizeLog();
        }
    }
}
