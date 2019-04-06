using CommonCode.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.Rolls
{
    public class StandardRoll : IRoll
    {
        public Guid TypeOfRoll => ObjectTypes.StandardRoll;
        public string Description { get; set; }
        public int Dice { get; set; }
        public string Outcome { get; set; } = "";
        public List<IRoll> Outcomes { get; set; } = new List<IRoll>();

        public int GetDice => Outcomes.Count;

        public string GetDescription => Description;
    }
}
