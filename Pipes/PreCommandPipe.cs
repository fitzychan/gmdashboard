using CommonCode.FileUtility;
using CommonCode.Interfaces;
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
        FileUtility fileUtility;

        public PreCommandPipe()
        {
            fileUtility = new FileUtility();
            RollMeHandle = new RollMeOne();
        }

        public ObservableCollection<string> LoadCommand()
        {
            var mainFileList = new ObservableCollection<string>();
            //here is async quick attempt to call an API
            var files = fileUtility.LoadFilesFromRemote();

            foreach (var chart in fileUtility.LoadChartsFromDefaultLocation(new string[] { ".txt", ".rgf", ".ps1"}))
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
            fileUtility.AddToFileRepo();
        }

        public void DeleteTableCommand(ObservableCollection<string> selectedCharts)
        {
            var foundFiles = fileUtility.LocateSpecificCharts(selectedCharts).ToList();
            foundFiles.ForEach(x => File.Delete(x.FullName));
        }
        public void OpenFileLocation(ICollection<string> selectedChart)
        {
            var foundFile = fileUtility.LocateSpecificCharts(selectedChart).FirstOrDefault();
            fileUtility.OpenFileLocation(foundFile);
        }
        public void OpenFile(ICollection<string> selectedChart)
        {
            var foundFile = fileUtility.LocateSpecificCharts(selectedChart).FirstOrDefault();
            fileUtility.OpenFile(foundFile);
        }
    }
}
