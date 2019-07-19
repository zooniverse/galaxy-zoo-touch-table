using GalaxyZooTouchTable.Models;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        ClassificationCounts IncrementClassificationCount(Classification classification);
        ClassificationCounts IncrementAndUpdateCounts(Classification classification, ClassificationCounts counts);
        ClassificationCounts GetCountsBySubjectId(string id);

        Task UpdateDBFromGraphQL(string id);

        void UpdateSubject(string id, ClassificationCounts counts);
    }
}
