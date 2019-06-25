using GalaxyZooTouchTable.Models;
using GraphQL.Common.Response;

namespace GalaxyZooTouchTable.Tests.Mock
{
    public static class GraphQLServiceMockData
    {
        public static GraphQLResponse GraphQLResponse()
        {
            return new GraphQLResponse();
        }

        public static ClassificationCounts ClassificationCounts()
        {
            return new ClassificationCounts(6, 1, 2, 3);
        }
    }
}
