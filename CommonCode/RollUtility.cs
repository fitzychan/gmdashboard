using CommonCode.Charts;
using CommonCode.Interfaces;
using CommonCode.Rolls;
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
            return new Chart();
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
        public string RunPowershell(FunctionParamChart functionChart, List<Parameter> powershellParams)
        {
            var shellResult = string.Empty;
            RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();

            using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration))
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
                            string thing = string.Empty;
                            foreach(var param in powershellParams)
                            {
                                thing += " -" +param.Name + " " + param.Value;
                                //shell.AddParameter(param.Name, param.Value);
                            }

                            //                    shell.AddScript("param($param1) $d = get-date; $s = 'test string value'; " +
                            //"$d; $s; $param1; get-service");

                            //                    // use "AddParameter" to add a single parameter to the last command/script on the pipeline.
                            //                    shell.AddParameter("param1", "parameter 1 value!");

                            var proc = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = "Powershell.exe",
                                    Arguments = functionChart.PowershellFileInfo.FullName + " " + thing,
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true
                                }
                            };

                            proc.Start();
                            //var resper = Process.Start("Powershell.exe", thing);
                            string thinger = string.Empty;
                            while (!proc.StandardOutput.EndOfStream)
                            {
                                shellResult += proc.StandardOutput.ReadLine();
                                // do something with line
                            }

                            //var results = shell.Invoke();
                            //foreach( var info in shell.Streams.Information)
                            //{
                            //    shellResult += info.MessageData + Environment.NewLine;
                            //}
                        }
                    }
                    catch(Exception e)
                    {
                        shellResult = e.Message;
                    }
                    finally
                    {
                        scriptInvoker.Invoke("Set-ExecutionPolicy -Scope CurrentUser Restricted");
                    }
                }
               
            }

            return shellResult;
        }
    }
}
