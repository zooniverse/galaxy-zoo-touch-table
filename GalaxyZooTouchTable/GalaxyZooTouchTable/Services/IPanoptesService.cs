using GalaxyZooTouchTable.Models;
using PanoptesNetClient.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public interface IPanoptesService
    {
        Task<ClassificationCounts> CreateClassificationAsync(Classification classification);
        Task<Subject> GetSubjectAsync(string id);
        Task<List<Subject>> GetSubjectsAsync(string route, NameValueCollection query);
        Task<Workflow> GetWorkflowAsync(string id);
    }
}
