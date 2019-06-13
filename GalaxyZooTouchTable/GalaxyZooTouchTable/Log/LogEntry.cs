namespace GalaxyZooTouchTable.Log
{
    public class LogEntry
    {
        string Event;
        string User;
        long Time;
        string Context;

        public LogEntry(string entry, string user, string context, long time)
        {
            Event = entry;
            User = user;
            Context = context;
            Time = time;
        }

        public string Print()
        {
            if (User == null)
                return $"{Time},{Event}";
            if (Context == null)
                return $"{Time},{Event},{User}";
            return $"{Time},{Event},{User},{Context}";
        }
    }
}
