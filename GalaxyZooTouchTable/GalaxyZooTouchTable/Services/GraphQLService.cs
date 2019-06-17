using GalaxyZooTouchTable.Models;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public class GraphQLService : IGraphQLService
    {
        private GraphQLHttpClient GraphQLClient = new GraphQLHttpClient(Config.CaesarHost);

        public async Task<ClassificationCounts> GetReductionAsync(string subjectId)
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
            return ParseGraphQLResponse(response);
        }

        public ClassificationCounts ParseGraphQLResponse(GraphQLResponse response)
        {
            int total, smooth, features, star;
            total = smooth = features = star = 0;

            if (response != null && response.Data != null)
            {
                var reductions = response.Data.workflow.subject_reductions;

                if (reductions.Count > 0)
                {
                    JObject data = reductions.First.data;
                    foreach (var count in data)
                    {
                        var index = count.Key;
                        switch (index)
                        {
                            case "0":
                                smooth = (int)count.Value;
                                break;
                            case "1":
                                features = (int)count.Value;
                                break;
                            case "2":
                                star = (int)count.Value;
                                break;
                        }
                    }
                }
            }
            total = smooth + features + star;
            return new ClassificationCounts(total, smooth, features, star);
        }
    }
}
