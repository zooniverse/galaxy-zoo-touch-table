using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public class GraphQLService : IGraphQLService
    {
        private GraphQLHttpClient GraphQLClient = new GraphQLHttpClient(Config.CaesarHost);

        public async Task<GraphQLResponse> GetReductionAsync(string subjectId)
        {
            GraphQLResponse response = new GraphQLResponse();
            var answersRequest = new GraphQLRequest
            {
                Query = @"
                    query AnswerCount($workflowId: ID!, $subjectId: ID!) {
                      workflow(id: $workflowId) {
                        subject_reductions(subjectId: $subjectId, reducerKey: T0_Stats) {
                            data
                        }
                      }
                    }",
                OperationName = "AnswerCount",
                Variables = new
                {
                    workflowId = Config.WorkflowId,
                    subjectId
                }
            };

            try
            {
                response = await GraphQLClient.SendQueryAsync(answersRequest);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Graph QL Error: {0}", e.Message);
            }
            return response;
        }
    }
}
