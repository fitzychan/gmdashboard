using CommonBlocks;
using CommonCode.FileUtility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pipes.PostParsedProject
{
    public interface IPostCommandPipe
    {
        void SaveChartCommand(MainBlock block);
        void AddToChartCommand(MainBlock block);
    }

    public class PostCommandPipe : IPostCommandPipe
    {
        public void AddToChartCommand(MainBlock block)
        {
            if (block == null)
                return;
            FileUtility.AddToChartCommand(block);
        }

        public void SaveChartCommand(MainBlock block)
        {
            if (block == null)
                return;
            FileUtility.SaveChartCommand(block);
        }
        public void AddSelectedToChartCommand(List<RollBlock> blocks)
        {
            if (!blocks.Any())
                return;
            FileUtility.AddSelectedToChartCommand(blocks);
        }

        public void SaveSelectedChartCommand(List<RollBlock> blocks)
        {
            if (!blocks.Any())
                return;
            FileUtility.SaveSelectedChartCommand(blocks);
        }
    }
}
