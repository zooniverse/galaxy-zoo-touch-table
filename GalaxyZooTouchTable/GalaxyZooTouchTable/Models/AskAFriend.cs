using System.Collections.ObjectModel;

namespace GalaxyZooTouchTable.Models
{
    public class AskAFriend
    {
        public TableUser User { get; set; }
        public ObservableCollection<TableUser> AllUsers { get; set; }

        public AskAFriend(TableUser user, ObservableCollection<TableUser> allUsers)
        {
            User = user;
            AllUsers = allUsers;
        }
    }
}
