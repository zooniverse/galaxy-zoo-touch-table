using GraphQL.Common.Response;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public interface IGraphQLService
    {
        Task<GraphQLResponse> GetReductionAsync(string subjectId);
    }
}
