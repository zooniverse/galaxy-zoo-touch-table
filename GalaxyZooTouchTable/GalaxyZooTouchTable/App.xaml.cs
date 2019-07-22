using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.ViewModels;
using System;
using System.Windows;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string DatabasePath = System.IO.Path.Combine(folderPath, Config.DatabaseName);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Window mainWindow = new MainWindow(new MainWindowViewModel());
            mainWindow.Show();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            GlobalData.GetInstance().EstablishLog();
        }
    }
}
