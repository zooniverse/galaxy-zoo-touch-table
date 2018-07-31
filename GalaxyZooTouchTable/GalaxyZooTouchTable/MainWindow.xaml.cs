using GalaxyZooTouchTable.Models;
using System;
using System.Windows;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetUserContext();
        }

        private void SetUserContext()
        {        
           TableUser personUser = TableUserFactory.Create(UserType.Person);
           TableUser lightUser = TableUserFactory.Create(UserType.Light);
           TableUser starUser = TableUserFactory.Create(UserType.Star);
           TableUser heartUser = TableUserFactory.Create(UserType.Heart);
           TableUser faceUser = TableUserFactory.Create(UserType.Face);
           TableUser earthUser = TableUserFactory.Create(UserType.Earth);

           //PersonUser.DataContext = personUser;
           //LightUser.DataContext = lightUser;
           //StarUser.DataContext = starUser;
           //HeartUser.DataContext = heartUser;
           //FaceUser.DataContext = faceUser;
           //EarthUser.DataContext = earthUser;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
    }
}
