using System;
using System.Collections.Generic;

namespace CommonCode
{
    public interface IRollRgf
    {
        string GetOutcome();
        string CellAddress { get; set; }
        Type RollType { get; }
    }

    public interface IBlockRgf
    {
        string GetOutcome();
        string BlockDescriptor { get; set; }
        Type BlockType { get; }
    }

    public class RollRgf : IRollRgf
    {
        public string RegularOutcome = string.Empty;
        public string CellAddress { get; set; }

        public RollRgf()
        {
        }
        public RollRgf(string outcome)
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
                return typeof(RollRgf);
            }
        }

    }

    public class SubRollRgf : RollRgf
    {
        public RollBlockRgf SubBlockOutcome { get; set; }
    }

    public class RollBlockRgf : IRollRgf, IBlockRgf
    {
        public string CellAddress { get; set; }
        public string BlockDescriptor { get; set; }
        public int Dice { get; set; }
        public List<IRollRgf> Outcomes { get; set; }
        public RollBlockRgf HorizontalTable { get; set; }
        public bool IsSubRollBlock = false;
        public string Result { get; set; }
        public Type BlockType 
        {
            get
            {
                return typeof(RollBlockRgf);
            }
        }

        public Type RollType
        {
            get
            {
                return typeof(RollBlockRgf);
            }
        }

        public RollBlockRgf()
        {
            BlockDescriptor = string.Empty;
            Result = string.Empty;
            Dice = 0;
            Outcomes = new List<IRollRgf>();
        }

        public string GetOutcome()
        {
            return BlockDescriptor + Result + Environment.NewLine;
        }
    }

    public class DescriptorRgf : IBlockRgf
    {
        public string BlockDescriptor { get; set; }
        public Type BlockType 
        {
            get
            {
                return typeof(DescriptorRgf);
            }
        }
        public DescriptorRgf(string descriptor)
        {
            BlockDescriptor = descriptor;
        }

        public string GetOutcome()
        {
            return BlockDescriptor;
        }
    }
}