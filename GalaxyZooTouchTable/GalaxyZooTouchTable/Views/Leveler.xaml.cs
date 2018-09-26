using GalaxyZooTouchTable.ViewModels;
using System.Windows.Controls;

namespace GalaxyZooTouchTable.Views
{
    /// <summary>
    /// Interaction logic for Leveler.xaml
    /// </summary>
    public partial class Leveler : UserControl
    {
        public Leveler()
        {
            InitializeComponent();
            DataContext = new LevelerViewModel();
        }
    }
}
