﻿using CommonCode.Interfaces;
using System.Collections.Generic;

namespace PostParsedProject.RollWindowManager
{
    public interface IRollWindowManager
    {
        void AddMainRoll(IChart mainBlocks);
    }

//Test
    public class RollWindowManager : IRollWindowManager
    {
        List<IChart> blocks = new List<IChart>();

        public void AddMainRoll(IChart mainBlock)
        {
            blocks.Add(mainBlock);
        }
    }
}
