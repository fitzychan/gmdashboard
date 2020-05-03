using CommonCode.DataModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace CommonCode
{
    public class GitUtility : IGitUtility
    {
        public void CloneRepo(string workingDirectory, GitBase gitRepo)
        {
            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.AddScript($"cd {workingDirectory}");
                
                foreach(var dir in Directory.GetDirectories(workingDirectory))
                {
                    //Check to see if the repos already exist.
                    if(dir.Replace('/','\\').Split('\\').Last().Equals(gitRepo.name))
                    {
                        bool isGitRepo = false;
                        foreach (var subDir in Directory.GetDirectories(workingDirectory + "\\" + gitRepo.name))
                        {
                            if(dir.Replace('/', '\\').Split('\\').Last().Equals(".git"))
                            {
                                isGitRepo = true;
                            }
                        }
                        //If its a git repo we want to pull updates.
                        if (isGitRepo)
                        {
                            powershell.AddScript($"cd {gitRepo.name}");
                            powershell.AddScript($"git pull");
                        }
                        else
                        {
                            //Do we throw an error?  Maybe a warning window?
                        }
                    }
                    else //didnt find something. So we just clone it.
                    {
                        //TODO WE NEED TO FIGURE OUT HOW TO GET RID OF THE PORT NUM IN THE URL FROM THE INFRA SIDE
                        var gitUrl = gitRepo.clone_url.Replace(":8280", "");
                        powershell.AddScript($"git clone {gitUrl}");
                    }
                    Collection<PSObject> results = powershell.Invoke();
                }
            }
        }
    }

    public interface IGitUtility
    {
        void CloneRepo(string workingDirectory, GitBase gitRepo);
    }
}
