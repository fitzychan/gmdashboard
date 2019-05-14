using CommonCode.Interfaces;
using System;
using System.Collections.Generic;

namespace CommonCode.Rolls
{
    public class StandardRoll : IRoll
    {
        public Guid TypeOfRoll => GmDashboardTypes.StandardRoll;
        public string Description { get; set; }
        public int Dice { get; set; }
        public string Outcome { get; set; } = "";
        public List<IRoll> Outcomes { get; set; } = new List<IRoll>();

        public int GetDice => Outcomes.Count;

        public string GetDescription => Description;
    }
}
