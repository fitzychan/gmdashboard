using CommonBlocks;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonCode.FileUtility
{
    public class FileUtility
    {
        public static void SaveChartCommand(IMainBlock resultMainBlock)
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
                File.WriteAllText(saveRolldChart.FileName, resultMainBlock.ToString());
            }
        }

        public static void AddToChartCommand(IMainBlock resultMainBlock)
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
                File.AppendAllText(saveRolldChart.FileName, resultMainBlock.ToString());
            }

        }
        public static void SaveSelectedChartCommand(List<RollBlock> blocks)
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
                    textToWrite += block.ToString();
                }
                File.WriteAllText(saveRolldChart.FileName, textToWrite);
            }
        }

        public static void AddSelectedToChartCommand(List<RollBlock> blocks)
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
                    textToWrite += block.ToString();
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
            }
            Process.Start(fileInfo.FullName);
        }
    }
}
