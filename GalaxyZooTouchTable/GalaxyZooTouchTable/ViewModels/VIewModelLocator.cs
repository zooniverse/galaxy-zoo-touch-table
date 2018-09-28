using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ViewModelLocator
    {
        private static LevelerViewModel _levelerViewModel = new LevelerViewModel();
        
        public static LevelerViewModel LevelerViewModel
        {
            get
            {
                return _levelerViewModel;
            }
        }
    }
}
