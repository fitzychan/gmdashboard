using CommonCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonBlocks
{//Needs to be refactored to be able to add stuff to this as well as extract data...
    //Imagine if we are bringin in a totally different MainBlock and need to tell it how to behave so we can make our code work...
    public interface IMainBlock
    {
        string GetPreamble();
    }
    public class MainBlock : IMainBlock
    {
        public string Preamble { get; set; }
        public List<IBlock> Blocks { get; set; }

        public MainBlock()
        {
            Preamble = string.Empty;
            Blocks = new List<IBlock>();
        }

        public string GetPreamble()
        {
            return Preamble;
        }

        public override string ToString()
        {
            if (Blocks.Any())
            {
                string outputString = Preamble;
                foreach(var block in Blocks)
                {
                    outputString += block.BlockDescriptor + Environment.NewLine;
                    
                    if(block.BlockType == typeof(RollBlock))
                    {
                        outputString += ((RollBlock)block).Result + Environment.NewLine;
                    }
                }
                return outputString;
            }
            else
                return "empty";
        }
    }
}
