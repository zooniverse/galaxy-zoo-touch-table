using GalaxyZooTouchTable.Models;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public interface IGraphQLService
    {
        Task<ClassificationCounts> GetReductionAsync(string subjectId);
    }
}
