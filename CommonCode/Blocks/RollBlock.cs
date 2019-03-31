using System;
using System.Collections.Generic;

namespace CommonBlocks
{
    public interface IRoll
    {
        string GetOutcome();
        string CellAddress { get; set; }
        Type RollType { get; }
    }

    public interface IBlock
    {
        string BlockDescriptor { get; set; }
        Type BlockType { get; }
    }

    public class RegularRoll : IRoll
    {
        public string RegularOutcome = string.Empty;
        public string CellAddress { get; set; }

        public RegularRoll()
        {
        }
        public RegularRoll(string outcome)
        {
            RegularOutcome = outcome;
        }

        public string GetOutcome()
        {
            return RegularOutcome;
        }

        public Type RollType
        {
            get
            {
                return typeof(RegularRoll);
            }
        }

    }

    public class SubRoll : RegularRoll
    {
        public RollBlock SubBlockOutcome { get; set; }
    }

    public class RollBlock : IRoll, IBlock
    {
        public string CellAddress { get; set; }
        public string BlockDescriptor { get; set; }
        public int Dice { get; set; }
        public List<IRoll> Outcomes { get; set; }
        public RollBlock HorizontalTable { get; set; }
        public bool IsSubRollBlock = false;
        public string Result { get; set; }
        public Type BlockType 
        {
            get
            {
                return typeof(RollBlock);
            }
        }

        public Type RollType
        {
            get
            {
                return typeof(RollBlock);
            }
        }

        public RollBlock()
        {
            BlockDescriptor = string.Empty;
            Result = string.Empty;
            Dice = 0;
            Outcomes = new List<IRoll>();
        }

        public string GetOutcome()
        {
            return BlockDescriptor + Result + Environment.NewLine;
        }
    }

    public class DescriptorBlock : IBlock
    {
        public string BlockDescriptor { get; set; }
        public Type BlockType 
        {
            get
            {
                return typeof(DescriptorBlock);
            }
        }
        public DescriptorBlock(string descriptor)
        {
            BlockDescriptor = descriptor;
        }
    }
}