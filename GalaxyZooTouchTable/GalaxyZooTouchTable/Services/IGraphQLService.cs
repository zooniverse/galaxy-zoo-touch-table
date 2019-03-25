using GalaxyZooTouchTable.Models;
using GraphQL.Common.Response;
using PanoptesNetClient.Models;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public interface IGraphQLService
    {
        Task<GraphQLResponse> GetReductionAsync(Workflow workflow, TableSubject currentSubject);
    }
}
