using CommonCode.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.Rolls
{
    public class Chart : IChart
    {
        public Guid TypeOfChart => ObjectTypes.Chart;
        //this is its place in the list.
        //This is what we want to display if we land on this
        public string Preamble { get; set; }

        public List<IRoll> ChartRolls { get; set; } = new List<IRoll>();
    }
}
