﻿using GalaxyZooTouchTable.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Lib
{
    public class PeripheralItems
    {
        public PeripheralItem Northern { get; set; }
        public PeripheralItem Southern { get; set; }
        public PeripheralItem Eastern { get; set; }
        public PeripheralItem Western { get; set; }
    }

    public class PeripheralItem
    {
        public BitmapImage Cutout { get; set; }
        public SpaceNavigation Location { get; set; }
        public List<TableSubject> Galaxies { get; set; }

        public PeripheralItem(SpaceNavigation location) { Location = location; }
    }
}
