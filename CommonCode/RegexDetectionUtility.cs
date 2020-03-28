using System.Text.RegularExpressions;

namespace CommonCode
{
    public class RegexDetectionUtility : IRegexDetectionUtility
    {
        readonly Regex chartRegex = new Regex(@"d-?\d+[\x20]", RegexOptions.IgnoreCase);
        readonly Regex rangeRegex = new Regex(@"-?\d+--?\d+", RegexOptions.IgnoreCase);
        readonly Regex subrollRegex = new Regex(@"[(]d-?\d+[)]", RegexOptions.IgnoreCase);
        readonly Regex outcomeRegex = new Regex(@"(\d+.\s?[A-Za-z\W.*]+\s?.)$", RegexOptions.IgnoreCase);// If this is not good enough we can use this. "(\d+.\s?[A-Za-z\W.*]+\s?.)$" .... ^\d+\..*\. 


        public Regex RollTitleDetector()
        {
            return chartRegex;
        }
        public Regex RangeRollDetector()
        {
            return rangeRegex;
        }
        public Regex SubRollDetector()
        {
            return subrollRegex;
        }

        public Regex OutcomeDetector()
        {
            return outcomeRegex;
        }

        public Regex CustomDetector(string pattern)
        {
            return new Regex(pattern, RegexOptions.IgnoreCase);
        }
    }
}
