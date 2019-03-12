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
        static string databaseName = "GZ_Staging_Subjects.db";
        static string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string DatabasePath = System.IO.Path.Combine(folderPath, databaseName);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow(new MainWindowViewModel());
            mainWindow.Show();
        }
    }
}
