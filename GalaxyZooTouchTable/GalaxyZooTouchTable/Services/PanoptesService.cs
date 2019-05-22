using GalaxyZooTouchTable.Models;
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
        ILocalDBService _localDBService { get; set; }

        public PanoptesService(ILocalDBService dbService)
        {
            _localDBService = dbService;
        }

        public async Task<ClassificationCounts> CreateClassificationAsync(Classification classification)
        {
            QueuedClassifications.Add(classification);
            //await SaveAllQueuedClassifications();
            return _localDBService.IncrementClassificationCount(classification);
        }

        private async Task SaveAllQueuedClassifications()
        {
            List<Classification> newQueue = new List<Classification>();
            foreach (Classification classification in QueuedClassifications)
            {
                try
                {
                    //HttpResponseMessage response = await _panoptesClient.Classifications.Create(classification);
                    //if ((int)response.StatusCode != 422) response.EnsureSuccessStatusCode();
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
