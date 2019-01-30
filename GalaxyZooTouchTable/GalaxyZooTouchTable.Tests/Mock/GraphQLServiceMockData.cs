using GalaxyZooTouchTable.Services;
using GraphQL.Common.Response;
using PanoptesNetClient.Models;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Tests.Mock
{
    public class GraphQLServiceMockData : IGraphQLService
    {
        public Task<GraphQLResponse> GetReductionAsync(Workflow workflow, Subject currentSubject)
        {
            return Task.FromResult<GraphQLResponse>(new GraphQLResponse());
        }
    }
}
