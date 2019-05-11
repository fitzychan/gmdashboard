using CommonCode.Charts;
using CommonCode.Interfaces;
using CommonCode.Rolls;
using System.Diagnostics;
using System.Management.Automation;
using System.Linq;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

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
                            //var scriptCommand = new Command(functionChart.PowershellFileInfo.FullName);
                            //pipeline.Commands.Add(scriptCommand);

                            //var psObjects = pipeline.Invoke();
                            shell.AddScript(functionChart.PowershellFileInfo.FullName);
                            powershellParams.ForEach(x => shell.AddParameter(x.Name, x.Value));


                            var results = shell.Invoke();
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

            return shellResult ;
        }
    }
}
