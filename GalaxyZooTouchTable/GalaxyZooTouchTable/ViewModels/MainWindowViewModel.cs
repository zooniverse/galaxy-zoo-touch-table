using GalaxyZooTouchTable.Models;
using PanoptesNetClient.Models;
using System.Collections.ObjectModel;

namespace GalaxyZooTouchTable.ViewModels
{
    public class MainWindowViewModel
    {
        public ObservableCollection<TableUser> ActiveUsers { get; set; }
        public Workflow Workflow { get; set; }
        public Project Project { get; set; }
    }
}
