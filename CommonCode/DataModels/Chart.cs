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
        public Guid TypeOfChart => GmDashboardTypes.PowerShellChart;

        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        public FileInfo PowershellFileInfo { get; set; }

        public List<string> PowerShellResult { get; set; }
    }

    public class ChartRgf : IChart
    {
        public string Preamble { get; set; }
        public List<IBlockRgf> Blocks { get; set; }

        public Guid TypeOfChart => GmDashboardTypes.RfgChart;

        public ChartRgf()
        {
            Preamble = string.Empty;
            Blocks = new List<IBlockRgf>();
        }

        public string GetPreamble()
        {
            return Preamble;
        }

        public override string ToString()
        {

            if(Preamble == null)
            {
                return "empty";
            }
            string outputString = Preamble;
            foreach (var block in Blocks)
            {
                outputString += block.BlockDescriptor + Environment.NewLine;

                if (block.BlockType == typeof(RollBlockRgf))
                {
                    outputString += ((RollBlockRgf)block).Result + Environment.NewLine;
                }
            }
            return outputString;
        }
    }
}
