using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    public class CompletedClassification
    {
        public string Answer { get; private set; }
        public TableUser User { get; private set; }
        public string SubjectId { get; private set; }

        public CompletedClassification(AnswerButton answer, TableUser user, string subjectId)
        {
            Answer = answer.Label;
            User = user;
            SubjectId = subjectId;
        }
    }
}