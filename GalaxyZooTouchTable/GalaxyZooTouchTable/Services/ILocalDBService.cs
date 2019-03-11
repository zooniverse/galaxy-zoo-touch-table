using GalaxyZooTouchTable.Models;
using System.Collections.Generic;

namespace GalaxyZooTouchTable.Services
{
    public interface ILocalDBService
    {
        TableSubject GetLocalSubject(string id);

        List<TableSubject> GetLocalSubjects(double minRa = 0, double maxRa = 360, double minDec = -90, double maxDec = 90);
        List<TableSubject> GetQueuedSubjects();
        List<TableSubject> GetSubjects(string query);

        SpacePoint FindNextAscendingRa(double RaLowerBounds);
        SpacePoint FindNextDescendingRa(double RaUpperBounds);
        SpacePoint FindNextAscendingDec(double DecLowerBounds);
        SpacePoint FindNextDescendingDec(double DecUpperBounds);
        SpacePoint GetPoint(string query);
        SpacePoint GetRandomPoint();
    }
}
