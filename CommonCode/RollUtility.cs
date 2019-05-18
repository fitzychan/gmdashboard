using CommonCode.Charts;
using CommonCode.Interfaces;
using CommonCode.Rolls;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace CommonCode.RollUtility
{
    public interface IRollUtility
    {
        IChart RollOnChart(IChart chart);
    }
    public class RollUtility : IRollUtility
    {

        public IChart RollOnChart(IChart chart)
        {

            if (chart.TypeOfChart == GmDashboardTypes.Chart)
            {
                foreach (var chartRoll in ((Chart)chart).ChartRolls)
                {
                    RollOnRoll(chartRoll);
                }
            }
            else if(chart.TypeOfChart == GmDashboardTypes.PowerShell)
            {
                var powershellFile = (FunctionParamChart)chart;

                powershellFile.PowerShellResult = RunPowershell(powershellFile, powershellFile.Parameters);
            }
            return chart;
        }

        public string RollOnRoll(IRoll roll)
        {
            string rollOnRollResult = string.Empty;
            using (var utility = new RandomUtility())
            {
                if (roll.TypeOfRoll == GmDashboardTypes.StandardRoll || roll.TypeOfRoll == GmDashboardTypes.RangeRoll || roll.TypeOfRoll == GmDashboardTypes.TextRoll)
                {
                    var chartRoll = (StandardRoll)roll;
                    //TODO  THIS SHOULD BE USEING THE DICE NUMBER TO PICK THE ROLL...
                    //THE REASON WE ARE NOT IS BECAUSE WE HAVE NOT FULL GOTTEN THE PRECENT ROLL
                    var diceOutcome = utility.RollDice(((StandardRoll)roll).Outcomes.Count) - 1;

                    var resultOfRoll = chartRoll.Outcomes.ElementAt(diceOutcome);
                    if (resultOfRoll.TypeOfRoll == GmDashboardTypes.StandardRoll)
                    {
                        chartRoll.Outcome += chartRoll.Description + RollOnRoll(resultOfRoll);
                    }
                    else
                    {
                        var outcome = chartRoll.Description + " " + resultOfRoll.GetDescription;
                        chartRoll.Outcome = outcome;
                        return outcome;
                    }
                }
                return "";
            }
        }
         
        public List<string> RunPowershell(FunctionParamChart functionChart, List<Parameter> powershellParams)
        {
            var workingFile = System.IO.Path.GetTempPath() + "//" + Guid.NewGuid() + ".txt";
            var shellResult = new List<string>();

            if (powershellParams.Any(x=>x.Value == null))
            {
                return shellResult;
            }
            using (Runspace runspace = RunspaceFactory.CreateRunspace(RunspaceConfiguration.Create()))
            {
                runspace.Open();
                using (RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace))
                {
                    try
                    {
                        scriptInvoker.Invoke("Set-ExecutionPolicy -Scope CurrentUser Unrestricted");
                        using (PowerShell shell = PowerShell.Create())
                        {
                            shell.AddScript(functionChart.PowershellFileInfo.FullName);
                            string aggParam = string.Empty;
                            foreach (var param in powershellParams)
                            {
                                aggParam += " -" + param.Name + " " + param.Value;
                            }

                            using (var shellProcess = new Process())
                            {
                                shellProcess.StartInfo = new ProcessStartInfo
                                {
                                    FileName = "Powershell.exe",
                                    Arguments = functionChart.PowershellFileInfo.FullName + " " + aggParam,
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                };

                                shellProcess.Start();
                                using ( var stream = File.Create(workingFile))
                                {
                                    shellProcess?.StandardOutput.BaseStream.CopyTo(stream);
                                    shellProcess?.StandardError.BaseStream.CopyTo(stream);
                                    //File.AppendAllText(workingFile, shellProcess.StandardOutput.ReadToEnd());
                                    //File.AppendAllText(workingFile, shellProcess.StandardError.ReadToEnd());
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        shellResult.Add(e.Message);
                    }
                    finally
                    {
                        scriptInvoker.Invoke("Set-ExecutionPolicy -Scope CurrentUser Restricted");
                        runspace.Close();
                        shellResult.AddRange(File.ReadAllLines(workingFile));
                        File.Delete(workingFile);
                    }
                }
               
            }

            return shellResult;
        }
    }
}
