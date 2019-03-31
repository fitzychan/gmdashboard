using CommonBlocks;
using System.Collections.Generic;

namespace PostParsedProject.RollWindowManager
{
    public interface IRollWindowManager
    {
        void AddMainRoll(IMainBlock mainBlocks);
    }


    public class RollWindowManager : IRollWindowManager
    {
        List<IMainBlock> blocks = new List<IMainBlock>();


        public void AddMainRoll(IMainBlock mainBlock)
        {
            blocks.Add(mainBlock);
        }
    }
}
