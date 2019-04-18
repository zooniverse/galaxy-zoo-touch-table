using PanoptesNetClient;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public class PanoptesService : IPanoptesService
    {
        ApiClient _panoptesClient = new ApiClient();
        List<Classification> QueuedClassifications { get; set; } = new List<Classification>();
        LocalDBService _localDBService { get; set; }

        public PanoptesService(LocalDBService dbService)
        {
            _localDBService = dbService;
        }

        public async Task<int> CreateClassificationAsync(Classification classification)
        {
            QueuedClassifications.Add(classification);
            await SaveAllQueuedClassifications();
            return _localDBService.GetClassificationCount(classification.Links.Subjects[0]);
        }

        private async Task SaveAllQueuedClassifications()
        {
            int newCount = 0;
            List<Classification> newQueue = new List<Classification>();
            foreach (Classification classification in QueuedClassifications)
            {
                try
                {
                    HttpResponseMessage response = await _panoptesClient.Classifications.Create(classification);
                    if ((int)response.StatusCode != 422) response.EnsureSuccessStatusCode();
                    newCount = _localDBService.IncrementClassificationCount(classification.Links.Subjects[0]);
                }
                catch (HttpRequestException error)
                {
                    newQueue.Add(classification);
                    System.Console.WriteLine(error.Message);
                }
            }
            QueuedClassifications = newQueue;
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
