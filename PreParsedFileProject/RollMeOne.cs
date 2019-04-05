using CommonCode.Blocks;
using CommonCode.FileUtility;
using CommonCode.RollUtility;
using DmAssistant.BlockBuilder;
using System.Collections.Generic;

namespace DmAssistant.RollMeOne
{
    public interface IRollMeOne
    {
        void SetSelectedItems(ICollection<string> items);
        ICollection<IChart> RollOnTables(ICollection<string> chartPaths);
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

        public void SetSelectedItems(ICollection<string> items)
        {
            selectedItems = items;
        }

        public ICollection<IChart> RollOnTables(ICollection<string> chartPaths)
        {
            List<IChart> rolledMainBlocks = new List<IChart>();
            foreach (var chartPath in FileUtility.LocateSpecificCharts(chartPaths))
            {
                //We dont want to include anything thats not a dummy object in out common code....  This is so we dont get a circular dependency
                rolledMainBlocks.Add(rollUtill.RollOnChart(contentExtractor.ExtractData(chartPath)));
            }
            return rolledMainBlocks;
        }
    }
}
