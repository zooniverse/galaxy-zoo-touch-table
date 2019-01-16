using PanoptesNetClient;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public class PanoptesService : IPanoptesService
    {
        ApiClient _panoptesClient = new ApiClient();

        public async Task CreateClassificationAsync(Classification classification)
        {
            await _panoptesClient.Classifications.Create(classification);
        }

        public async Task<Subject> GetSubjectAsync(string id)
        {
            return await _panoptesClient.Subjects.Get(id);
        }

        public async Task<List<Subject>> GetSubjectsAsync(string route, NameValueCollection query)
        {
            return await _panoptesClient.Subjects.GetList(route, query);
        }

        public async Task<Workflow> GetWorkflowAsync(string id)
        {
            return await _panoptesClient.Workflows.Get(id);
        }
    }
}
