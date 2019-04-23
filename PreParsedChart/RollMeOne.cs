using CommonCode.FileUtility;
using CommonCode.Interfaces;
using CommonCode.RollUtility;
using GmDashboard.ChartBuilder;
using System.Collections.Generic;

namespace GmDashboard.RollMeOne
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
            foreach (var file in FileUtility.LocateSpecificCharts(chartPaths))
            {
                IChart builtChart;
                if (file.Extension.Contains(".txt"))
                {
                    builtChart = contentExtractor.ExtractFromText(file);
                }
                else if(file.Extension.Contains(".rgf"))
                {
                    builtChart = contentExtractor.ExtractFromRgf(file);
                }
                else if (file.Extension.Contains(".ps1"))
                {
                    builtChart = contentExtractor.ExtractFromPowerShell(file);
                }
                else
                {
                    throw new System.NotImplementedException("File format not supported");
                }
                rolledMainBlocks.Add(rollUtill.RollOnChart(builtChart));
            }
            return rolledMainBlocks;
        }
    }
}
