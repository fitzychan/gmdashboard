using CommonCode.FileUtility;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.IO;

namespace GmDashboard.ViewModel
{
    public class CloudRepoViewModel : ViewModelBase
    {
        private FileUtility fileUtility = new FileUtility();
        public RepoState RepoState { get; set; } = RepoState.Unknown;
        public string RepoName { get; set; }
        public List<FileInfo> FilesAtCurrentLevel { get; set; }
        public CloudRepoViewModel()
        {
            //https://stackoverflow.com/questions/23713898/setting-datacontext-in-xaml-in-wpf
            //we were trying to get the tree view working.
            FilesAtCurrentLevel = fileUtility.LocateSpecificCharts(RepoName);
        }
    }
    public enum RepoState
    {
        Unknown = 0,
        NoChanges = 1,//No color on the header name
        LocalChanges = 2,//This will turn the header yellow with a blue arrow pointing up.  To a cloud??
        RemoteChanges = 3//This will turn the header yellow with a blue arrow pointing down.  To a cloud??
    }
}
