using GalaxyZooTouchTable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.ViewModels
{
    public class ExamplesPanelViewModel
    {
        public List<GalaxyExample> ExampleGalaxies { get; set; } = new List<GalaxyExample>();

        public ExamplesPanelViewModel()
        {
            ExampleGalaxies.Add(GalaxyExampleFactory.Create(GalaxyType.Smooth));
            ExampleGalaxies.Add(GalaxyExampleFactory.Create(GalaxyType.Features));
            ExampleGalaxies.Add(GalaxyExampleFactory.Create(GalaxyType.Star));
        }
    }
}
