using CommonCode;
using CommonCode.DataModels;
using CommonCode.FileUtility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.Generic;
using System.IO;
/// <summary>
/// NOTES 
///https://social.msdn.microsoft.com/Forums/vstudio/en-US/53b8ca17-dbc1-4082-8474-7e8ce3d454a9/dynamically-add-treeview-items-using-mvvm
///https://stackoverflow.com/questions/721714/notification-when-a-file-changes
///https://stackoverflow.com/questions/13587270/keypress-event-equivalent-in-wpf
///https://stackoverflow.com/questions/23713898/setting-datacontext-in-xaml-in-wpf
/// </summary>


namespace GmDashboard.ViewModel
{
    //Im not sure what to do wit this.  I think Im trying to use this as a main class that will have a list of all the repos and files associated to a user.
    public class CloudRepoViewModel : ViewModelBase
    {
        private FileUtility fileUtility = new FileUtility();
        private IGitUtility _gitUtility = new GitUtility();
        public RepoState RepoState { get; set; } = RepoState.Unknown;
        public List<GitBase> Repos { get; set; }
        //Do we even need this?  This class is used for getting repos, updateing repos and pushing repos
        public IEnumerable<FileInfo> FilesAtCurrentLevel { get; set; }
        private string UserName { get; set; }
        public CloudRepoViewModel(Creds user)
        {
            UserName = user.UserName;
            //RepoName = repoName;
            ////https://stackoverflow.com/questions/23713898/setting-datacontext-in-xaml-in-wpf
            ////we were trying to get the tree view working.
            //FilesAtCurrentLevel = fileUtility.LoadChartsFromLocation(fileUtility.ChartsLocation + repoName);
            Repos = fileUtility.LoadReposFromRemote(UserName);
            foreach(var repo in Repos)
            {
                _gitUtility.CloneRepo(fileUtility.ChartsLocation, repo);
            }
        }

        [PreferredConstructorAttribute]
        public CloudRepoViewModel()
        {
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
