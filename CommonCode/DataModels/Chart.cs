using CommonCode.Interfaces;
using System;
using System.Collections.Generic;

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

        public List<List<string>> Parameters { get; set; } = new List<List<string>>();
    }
}
