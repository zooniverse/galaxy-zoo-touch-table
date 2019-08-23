using System;

namespace GalaxyZooTouchTable.Lib
{
    public class Session
    {
        TimeSpan StartTime;

        public Session() { StartTime = DateTime.Now.TimeOfDay; }

        public string Duration()
        {
            TimeSpan duration = DateTime.Now.TimeOfDay - StartTime;
            return duration.ToString(@"hh\:mm\:ss");
        }
    }
}
