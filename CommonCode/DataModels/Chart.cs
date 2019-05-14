using CommonCode.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace CommonCode.Charts
{
    public class Chart : IChart
    {
        public Guid TypeOfChart => GmDashboardTypes.Chart;
        //this is its place in the list.
        //This is what we want to display if we land on this
        public string Preamble { get; set; }

        public List<IRoll> ChartRolls { get; set; } = new List<IRoll>();
    }

    public class FunctionParamChart : IChart
    {
        public Guid TypeOfChart => GmDashboardTypes.PowerShell;

        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        public FileInfo PowershellFileInfo { get; set; }

        public List<string> PowerShellResult { get; set; }
    }
}
