using System.Collections.ObjectModel;

namespace GalaxyZooTouchTable.Models
{
    public class AskAFriend
    {
        public TableUser User { get; set; }
        public ObservableCollection<TableUser> AvailableUsers { get; set; }

        public AskAFriend(TableUser user, ObservableCollection<TableUser> allUsers)
        {
            User = user;
            RemoveCurrentUser(allUsers);
        }

        private void RemoveCurrentUser(ObservableCollection<TableUser> allUsers)
        {
            ObservableCollection<TableUser> allUsersCopy = new ObservableCollection<TableUser>(allUsers);
            foreach (TableUser tableUser in allUsersCopy)
            {
                if (User == tableUser)
                {
                    allUsersCopy.Remove(User);
                    break;
                }
            }
            AvailableUsers = allUsersCopy;
        }
    }
}
