using System.Windows.Media.Imaging;

namespace GalaxyZooTouchTable.Models
{
    public class CompletedClassification
    {
        public string Answer { get; private set; }
        public BitmapImage Avatar { get; private set; }
        public string SubjectId { get; private set; }

        public CompletedClassification(AnswerButton answer, TableUser user, string subjectId)
        {
            Answer = answer.Label;
            Avatar = user.Avatar;
            SubjectId = subjectId;
        }
    }
}