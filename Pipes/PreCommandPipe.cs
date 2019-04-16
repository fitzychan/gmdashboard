using CommonCode.Blocks;
using CommonCode.FileUtility;
using GmDashboard.RollMeOne;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Pipes.PreParsedFileProject
{
    public interface IPreCommandPipe
    {
        ObservableCollection<string> LoadCommand();
        ICollection<IChart> RollOneCommand(ICollection<string> selectedItems);
        void OpenFileLocation(ICollection<string> selectedChart);
        void OpenFile(ICollection<string> selectedChart);
    }

    public class PreCommandPipe : IPreCommandPipe
    {
        IRollMeOne RollMeHandle;

        public PreCommandPipe()
        {
            RollMeHandle = new RollMeOne();
        }

        public ObservableCollection<string> LoadCommand()
        {
            var mainFileList = new ObservableCollection<string>();

            foreach (var chart in FileUtility.LoadChartsFromDefaultLocation(new string[] { ".txt", ".rgf", ".ps1"}))
            {
                mainFileList.Add(chart.Name);
            }
            return mainFileList;
        }

        public ICollection<IChart> RollOneCommand(ICollection<string> selectedItems)
        {
            if (selectedItems != null && selectedItems.Count > 0)
            {
                 return RollMeHandle.RollOnTables(selectedItems);
            }
            return new List<IChart>();
        }
        public void AddTablesToRepo()
        {
            FileUtility.AddToFileRepo();
        }

        public void DeleteTableCommand(ObservableCollection<string> selectedCharts)
        {
            var foundFiles = FileUtility.LocateSpecificCharts(selectedCharts).ToList();
            foundFiles.ForEach(x => File.Delete(x.FullName));
        }
        public void OpenFileLocation(ICollection<string> selectedChart)
        {
            var foundFile = FileUtility.LocateSpecificCharts(selectedChart).FirstOrDefault();
            FileUtility.OpenFileLocation(foundFile);
        }
        public void OpenFile(ICollection<string> selectedChart)
        {
            var foundFile = FileUtility.LocateSpecificCharts(selectedChart).FirstOrDefault();
            FileUtility.OpenFile(foundFile);
        }
    }
}
