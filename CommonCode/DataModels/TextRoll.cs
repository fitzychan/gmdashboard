using CommonCode.Interfaces;
using System;

namespace CommonCode.Rolls
{
    public class TextRoll : IRoll
    {
        public string Description { get; set; }

        public TextRoll(string description)
        {
            Description = description;
        }
        public Guid TypeOfRoll => GmDashboardTypes.TextRoll;
        public string GetDescription => Description;
        public int GetDice => 0;
    }
}
