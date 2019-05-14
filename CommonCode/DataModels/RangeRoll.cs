using CommonCode.Interfaces;
using System;
using System.Collections.Generic;

namespace CommonCode.Rolls
{
    public class RangeRoll : IRoll
    {
        public Guid TypeOfRoll => GmDashboardTypes.RangeRoll;
        public string Description { get; set; }
        public int NumberUpperBound { get; set; }
        public int NumberLowerBound { get; set; }
        public string Outcome { get; set; } = "";
        public List<IRoll> Outcomes { get; set; } = new List<IRoll>();
        public int GetDice => Outcomes.Count;
        public string GetDescription => Description;
    }
}
