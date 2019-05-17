using GalaxyZooTouchTable.Models;
using System.Collections.Generic;

namespace GalaxyZooTouchTable.Services
{
    public interface ILocalDBService
    {
        TableSubject GetLocalSubject(string id);

        List<TableSubject> GetLocalSubjects(SpaceNavigation currentLocation);
        List<TableSubject> GetQueuedSubjects();
        List<TableSubject> GetSubjects(string query, SpaceNavigation currentLocation = null);

        SpacePoint FindNextAscendingRa(double raLowerBounds);
        SpacePoint FindNextDescendingRa(double raUpperBounds);
        SpacePoint FindNextAscendingDec(double decLowerBounds);
        SpacePoint FindNextDescendingDec(double decUpperBounds);
        SpacePoint GetPoint(string query);
        SpacePoint GetRandomPoint();

        int IncrementClassificationCount(string subjectId);
        int GetClassificationCount(string SubjectId);
    }
}
