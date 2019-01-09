﻿using GraphQL.Common.Response;
using PanoptesNetClient.Models;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public interface IGraphQLRepository
    {
        Task<GraphQLResponse> GetReductionAsync(Workflow workflow, Subject currentSubject);
    }
}
