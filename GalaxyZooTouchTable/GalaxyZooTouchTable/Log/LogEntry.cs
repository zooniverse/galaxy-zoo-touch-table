using GalaxyZooTouchTable.Lib;

namespace GalaxyZooTouchTable.Log
{
    public class LogEntry
    {
        string Event;
        string User;
        long Time;
        string Context;
        string SubjectId;
        ClassifierViewEnum State;

        public LogEntry(string entry, string user, string subjectId, ClassifierViewEnum state, string context, long time)
        {
            Context = context;
            Event = entry;
            State = state;
            SubjectId = subjectId;
            Time = time;
            User = user;
        }

        public string Print()
        {
            string stateString = null;
            if (State == ClassifierViewEnum.SubjectView)
                stateString = "Classifier";
            else if (State == ClassifierViewEnum.SummaryView)
                stateString = "Summary";

            return $"{Time},{Event},{User ?? ""},{SubjectId ?? ""},{stateString ?? ""},{Context ?? ""}";
        }
    }
}
