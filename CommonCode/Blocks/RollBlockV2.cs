using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.Blocks
{
    //This represents all the charts that was passed to us.
    //public class AggregateCharts
    //{
    //    public List<IBlockV2> RollCharts { get; set; }
    //}

    public class ChartAggregate 
    {
        //How many possible outcomes there could be
        public int Dice { get; set; }
        //Title of the chart
        public string Descriptor { get; set; }
        public List<ChartV2> Charts { get; set; }
    }

    public interface IChartV2
    {
        Guid TypeOfChart { get; }
    }

    public class ChartV2 : IChartV2
    {
        public Guid TypeOfChart => ObjectTypes.Chart;
        //this is its place in the list.
        public int Dice { get; set; }
        //This is what we want to display if we land on this
        public string Descriptor { get; set; }

        public List<IRollV2> ChartRolls { get; set; } = new List<IRollV2>();
    }

    public interface IRollV2
    {
        Guid TypeOfRoll { get; }
    }

    public class RollV2 : IRollV2
    {
        public Guid TypeOfRoll => ObjectTypes.SubRoll;
        public string Description { get; set; }
        public int Number { get; set; }
        public List<IRollV2> Outcomes { get; set; } = new List<IRollV2>();
    }

    public class RangeRollV2 : IRollV2
    {
        public Guid TypeOfRoll => ObjectTypes.RangeRoll;
        public string Description { get; set; }
        public int NumberUpperBound { get; set; }
        public int NumberLowerBound { get; set; }
        public List<IRollV2> Outcomes { get; set; } = new List<IRollV2>();
    }

    public class OutcomeRollV2 : IRollV2
    {
        public string Description { get; internal set; }

        public OutcomeRollV2(string description)
        {
            Description = description;
        }
        public Guid TypeOfRoll => ObjectTypes.DescriptorRoll;
    }

}
