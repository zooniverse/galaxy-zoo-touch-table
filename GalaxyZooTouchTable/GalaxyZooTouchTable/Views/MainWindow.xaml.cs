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
        public MainWindow()
        {
            InitializeComponent();
            this.Cursor = Cursors.None;
        }
    }
}
