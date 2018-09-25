﻿using GalaxyZooTouchTable.Utility;
using PanoptesNetClient.Models;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace GalaxyZooTouchTable.Models
{
    public class AnswerButton
    {
        public string Label { get; set; }
        public string Url { get; set; }
        public int Index { get; set; }

        /// <summary>
        /// The Galaxy Zoo workflow contains image links in Markdown. 
        /// The Regex in this constructor extract the necessary URL link and leave the answer text remaining.
        /// </summary>
        public AnswerButton(TaskAnswer answer, int index)
        {
            var ExtractedUrl = Regex.Match(answer.Label, @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");
            string removedBrackets = Regex.Replace(answer.Label, @"!\[(.*?)\]", "");
            string BaseAnswer = Regex.Replace(removedBrackets, @"\(" + ExtractedUrl + @"(.*?)\)", "");

            Url = ExtractedUrl.Value;
            Label = BaseAnswer.Trim();
            Index = index;
        }
    }
}