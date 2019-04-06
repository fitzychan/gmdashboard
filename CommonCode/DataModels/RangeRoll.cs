using CommonCode.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.Rolls
{
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
}
