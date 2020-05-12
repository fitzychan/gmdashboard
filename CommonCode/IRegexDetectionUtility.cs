using System.Text.RegularExpressions;

namespace CommonCode
{
    public interface IRegexDetectionUtility
    {
        Regex SubRollDetector();
        Regex RangeRollDetector();
        Regex RollTitleDetector();
        //not sure if this is good or not...
        Regex CustomDetector(string pattern);
        Regex OutcomeDetector();
    }
}
