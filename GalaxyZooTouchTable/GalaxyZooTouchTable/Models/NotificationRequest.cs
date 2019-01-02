using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Models
{
    public class NotificationRequest
    {
        public TableUser User { get; set; }
        public string SubjectID { get; set; }

        public NotificationRequest(TableUser user, string id)
        {
            User = user;
            SubjectID = id;
        }
    }
}
