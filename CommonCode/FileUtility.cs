using CommonCode.Charts;
using CommonCode.Rolls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonCode.FileUtility
{
    public class FileUtility 
    {
        public static void SaveChartCommand(Chart resultMainBlock)
        {
            var saveRolldChart = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                OverwritePrompt = true
            };

            if (saveRolldChart.ShowDialog() == DialogResult.OK)
            {
                var stringAgg = resultMainBlock.Preamble;
                foreach(var block in resultMainBlock.ChartRolls)
                {
                    stringAgg += ((StandardRoll)block).Outcome;
                }
                File.WriteAllText(saveRolldChart.FileName, stringAgg);
            }
        }

        public static void AddToChartCommand(Chart resultMainBlock)
        {
            var saveRolldChart = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                OverwritePrompt = false
            };

            if (saveRolldChart.ShowDialog() == DialogResult.OK)
            {
                var stringAgg = resultMainBlock.Preamble;
                foreach (var block in resultMainBlock.ChartRolls)
                {
                    stringAgg += ((StandardRoll)block).Outcome;
                }
                File.AppendAllText(saveRolldChart.FileName, stringAgg);
            }

        }

        public static void SaveSelectedChartCommand(List<string> blocks)
        {
            var saveRolldChart = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                OverwritePrompt = true
            };

            if (saveRolldChart.ShowDialog() == DialogResult.OK)
            {
                string textToWrite = string.Empty;
                foreach (var block in blocks)
                {
                    textToWrite += block;
                }
                File.WriteAllText(saveRolldChart.FileName, textToWrite);
            }
        }

        public static void AddSelectedToChartCommand(List<string> blocks)
        {
            var saveRolldChart = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                OverwritePrompt = false
            };

            if (saveRolldChart.ShowDialog() == DialogResult.OK)
            {
                string textToWrite = string.Empty;
                foreach(var block in blocks)
                {
                    textToWrite += block;
                }
                File.AppendAllText(saveRolldChart.FileName, textToWrite);
            }

        }

        public static void AddToFileRepo()
        {
            var saveRolldChart = new OpenFileDialog
            {
                RestoreDirectory = true,
                Multiselect = true
            };

            if (saveRolldChart.ShowDialog() == DialogResult.OK)
            {
                DirectorySecurity dSecurity = new DirectorySecurity();
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Tables", dSecurity);
                File.SetAttributes(Directory.GetCurrentDirectory() + "\\Tables", FileAttributes.Normal);
                foreach (var file in saveRolldChart.FileNames)
                {
                    File.Copy(file, Directory.GetCurrentDirectory() + "\\Tables\\" + file.Split('\\').Last().ToString(), true);
                }
            }
        }

        public static IEnumerable<FileInfo> LocateSpecificCharts(ICollection<string> itemList)
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*" + "*" + "*", SearchOption.AllDirectories);

            foreach (var item in itemList)
            {
                 foreach (var file in files)
                 {
                     if (file.Contains(item))
                     {
                         yield return new FileInfo(file);
                     }
                 }
            }
        }

        public static IEnumerable<FileInfo> LoadChartsFromDefaultLocation(string[] exts)
        {
            var foundFiles = new List<string>();
            //This will be the default path for the files.
            try
            {
                foreach (var ext in exts)
                {
                    var dSecurity = new DirectorySecurity();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Tables", dSecurity);
                    foundFiles.AddRange(Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Tables", "*" + ext + "*", SearchOption.AllDirectories).ToList());
                }
            }
            catch (Exception e)
            {

            }
            foreach (string file in foundFiles)
            {
                yield return new FileInfo(file);
            }
        }

        public static void OpenFileLocation(FileInfo fileInfo)
        {
            Process.Start("explorer.exe", "/select," + fileInfo.FullName);
        }

        public static void OpenFile(FileInfo fileInfo)
        {
            if(fileInfo.Extension == ".rgf")
            {
                MessageBox.Show("This opens the raw data of the rtf file be careful when editing.", "This is not complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }//TODO make this always open ina text window
            Process.Start(fileInfo.FullName);
        }

        public static IEnumerable<FileInfo> LoadFilesFromRemote()
        {

            string _apiKey = "e96cc48193e5e9e5f1bf3b563867beeb2d115cd2";
            string _url = "http://192.168.21.6:32518/api/v1/";
            string _user = "fitzy";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            var auth = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(_user + ":" + _apiKey));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var json = client.GetStringAsync("https://git.dustinti.me/api/v1/users/fitzy/starred").Result;
            return new List<FileInfo>();
        }
    }
}
