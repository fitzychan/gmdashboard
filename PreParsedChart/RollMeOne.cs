using CommonCode.Blocks;
using CommonCode.FileUtility;
using CommonCode.RollUtility;
using GmDashboard.BlockBuilder;
using System.Collections.Generic;

namespace GmDashboard.RollMeOne
{
    public interface IRollMeOne
    {
        ICollection<IChart> RollOnSelected(ICollection<string> chartPaths);
        //Need methods that collect data that have been selected and uses that to make a contextual roll....
    }

    public class RollMeOne : IRollMeOne
    {
        private readonly IContentExtractor contentExtractor;
        private readonly IRollUtility rollUtill;
        ICollection<string> selectedItems = new List<string>();

        public RollMeOne()
        {
            contentExtractor = new ContentExtractor();
            rollUtill = new RollUtility();
        }

        public ICollection<IChart> RollOnSelected(ICollection<string> chartPaths)
        {
            List<IChart> rolledMainBlocks = new List<IChart>();
            foreach (var chartPath in FileUtility.LocateSpecificFiles(chartPaths))
            {
                rolledMainBlocks.Add(rollUtill.RollOnChart(contentExtractor.ExtractData(chartPath)));
            }
            return rolledMainBlocks;
        }
    }
}
