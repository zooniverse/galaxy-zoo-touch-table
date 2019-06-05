using GalaxyZooTouchTable.ViewModels;
using System.Windows;
using System.Windows.Input;
using GalaxyZooTouchTable.Lib;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        private Log Logger;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            _viewModel = viewModel;
            DataContext = viewModel;

            Cursor = Cursors.None;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Load();

            string date = DateTime.Now.ToString("dd-MM-yyyy_HHmmss");
            Logger = new Log($"log_{date}");
        }

        private void Root_Closed(object sender, EventArgs e)
        {
            Logger.FinalizeLog();
        }
    }
}
