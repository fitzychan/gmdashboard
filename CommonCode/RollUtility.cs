using CommonCode.Charts;
using CommonCode.Interfaces;
using CommonCode.Rolls;
using System.Diagnostics;
using System.Management.Automation;
using System.Linq;

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
                var orginizedParams = string.Empty;

                foreach(var param in powershellFile.Parameters)
                {
                    orginizedParams += " -" + param.Name.ToLower() + " " + param.Value;
                }

                var thing = RunPowershell(powershellFile, orginizedParams);

                Process.Start(new ProcessStartInfo() { FileName = powershellFile.PowershellFileInfo.FullName,  });
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
        public string RunPowershell(FunctionParamChart functionChart, string powershellParams)
        {
            var shell = PowerShell.Create();
            shell.(functionChart.PowershellFileInfo.FullName);
            shell.Commands.AddParameter(powershellParams);

            var results = shell.Invoke();
            //var process = new Process();
            //process.StartInfo.UseShellExecute = false;
            //process.StartInfo.RedirectStandardOutput = true;
            //process.StartInfo.FileName = functionChart.PowershellFileInfo.FullName;
            //process.StartInfo.Arguments =  powershellParams;

            //process.Start();
            //string s = process.StandardOutput.ReadToEnd();
            //process.WaitForExit();

            //using (StreamWriter outfile = new StreamWriter("StandardOutput.txt", true))
            //{
            //    outfile.Write(s);
            //}
            return "";
        }
    }
}
