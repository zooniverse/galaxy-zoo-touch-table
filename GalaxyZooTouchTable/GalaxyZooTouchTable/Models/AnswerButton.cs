using PanoptesNetClient.Models;
using System.Text.RegularExpressions;

namespace GalaxyZooTouchTable.Models
{
    public class AnswerButton
    {
        public string Label { get; set; }
        public string Url { get; set; }
        public int Index { get; set; }
        
        public AnswerButton(TaskAnswer answer, int index)
        {
            var UrlMatch = Regex.Match(answer.Label, @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");
            string removedBrackets = Regex.Replace(answer.Label, @"!\[(.*?)\]", "");
            string removedUrl = Regex.Replace(removedBrackets, @"\(" + UrlMatch + @"(.*?)\)", "");

            Url = UrlMatch.Value;
            Label = removedUrl.Trim();
            Index = index;
        }
    }
}
