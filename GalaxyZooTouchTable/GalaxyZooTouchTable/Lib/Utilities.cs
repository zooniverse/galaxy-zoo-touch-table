using Newtonsoft.Json.Linq;
using PanoptesNetClient.Models;
using System.Linq;

namespace GalaxyZooTouchTable.Lib
{
    public static class Utilities
    {
        public static string GetSubjectLocation(Subject subject, int frame = 0)
        {
            string[] acceptedImages = { "image/jpeg", "image/png", "image/svg+xml", "image/gif" };

            foreach (JProperty property in subject.Locations[0])
            {
                if (acceptedImages.Contains(property.Name))
                    return (string)property.Value;
            }
            return null;
        }
    }
}
