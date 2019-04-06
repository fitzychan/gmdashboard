using CommonCode.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.Rolls
{
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
