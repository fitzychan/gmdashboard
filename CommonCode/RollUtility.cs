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
using System.Windows.Forms;

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
            else if (chart.TypeOfChart == GmDashboardTypes.PowerShellChart)
            {
                var powershellFile = (FunctionParamChart)chart;

                powershellFile.PowerShellResult = RunPowershell(powershellFile, powershellFile.Parameters);
            }
            else if (chart.TypeOfChart == GmDashboardTypes.RfgChart)
            {
                RollOnRgf((ChartRgf)chart);
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
            var workingFile = Path.GetTempPath() + "\\" + Guid.NewGuid() + ".txt";
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
                                using (var fileStream = File.Create(workingFile))
                                {
                                    using (var streamWriter = new StreamWriter(fileStream))
                                    {
                                        var errorValues = shellProcess.StandardError.BaseStream.ReadToEnd();
                                        var values = shellProcess.StandardOutput.BaseStream.ReadToEnd();
                                        if (!string.IsNullOrEmpty(errorValues))
                                        {
                                            streamWriter.WriteLine(errorValues);
                                        }
                                        if (!string.IsNullOrEmpty(values))
                                        {
                                            streamWriter.WriteLine(values);
                                        }
                                    }
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

        public IChart RollOnRgf(ChartRgf rgfChart)
        {
            if (rgfChart == null)
            {
                MessageBox.Show("The file is improperly formatted", "Bad file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Chart();
            }
            else
            {
                foreach (var rollBlock in rgfChart.Blocks)
                {
                    if (rollBlock.BlockType == typeof(RollBlockRgf))
                    {
                        var localRollBlock = ((RollBlockRgf)rollBlock);
                        localRollBlock.Result = RollOnRgfBlock(rollBlock);
                    }
                }
            }
            return rgfChart;
        }

        private string RollOnRgfBlock(IBlockRgf roll)
        {
            string returnString = string.Empty;
            try
            {
                using (var randUtill = new RandomUtility())
                {
                    //This is because arrays start at 0 we need to shift the outcome by one.  NOT the dice passed in.
                    int rolledNumber = randUtill.RollDice(((RollBlockRgf)roll).Dice) - 1;

                    if (roll.BlockType == typeof(DescriptorRgf))
                    {
                        return roll.GetOutcome();
                    }

                    var tempRoll = ((RollBlockRgf)roll).Outcomes.ElementAt(rolledNumber);
                    if (tempRoll.GetType() == typeof(RollRgf))
                    {
                        returnString = (((RollRgf)tempRoll).GetOutcome());
                    }
                    else if (tempRoll.GetType() == typeof(SubRollRgf))
                    {
                        //TODO we are here trying to get the spacing correct.... We should be doing this all in one place...
                        returnString = (((SubRollRgf)tempRoll).SubBlockOutcome.BlockDescriptor + Environment.NewLine + "\t" + RollOnRgfBlock(((SubRollRgf)tempRoll).SubBlockOutcome)) + Environment.NewLine;
                    }
                }
                return returnString;
            }
            catch (Exception e)
            {
                // throw;
                return "Error in RollOnBlock:" + e.Message;
            }
        }
    }
}
