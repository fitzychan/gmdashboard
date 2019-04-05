using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.Blocks
{
    public interface IChart
    {
        Guid TypeOfChart { get; }
    }

    public class Chart : IChart
    {
        public Guid TypeOfChart => ObjectTypes.Chart;
        //this is its place in the list.
        //This is what we want to display if we land on this
        public string Preamble { get; set; }

        public List<IRoll> ChartRolls { get; set; } = new List<IRoll>();
    }

    public interface IRoll
    {
        Guid TypeOfRoll { get; }
        string GetDescription { get; }
        int GetDice { get; }
    }

    public class Roll : IRoll
    {
        public Guid TypeOfRoll => ObjectTypes.Roll;
        public string Description { get; set; }
        public int Dice { get; set; }
        public string Outcome { get; set; } = "";
        public List<IRoll> Outcomes { get; set; } = new List<IRoll>();

        public int GetDice => Outcomes.Count;

        public string GetDescription => Description;
    }

    public class RangeRoll : IRoll
    {
        public Guid TypeOfRoll => ObjectTypes.RangeRoll;
        public string Description { get; set; }
        public int NumberUpperBound { get; set; }
        public int NumberLowerBound { get; set; }
        public string Outcome { get; set; } = "";
        public List<IRoll> Outcomes { get; set; } = new List<IRoll>();
        public int GetDice => Outcomes.Count;
        public string GetDescription => Description;
    }

    public class TextRoll : IRoll
    {
        public string Description { get; set; }

        public TextRoll(string description)
        {
            Description = description;
        }
        public Guid TypeOfRoll => ObjectTypes.TextRoll;
        public string GetDescription => Description;
        public int GetDice => 0;
    }
}
