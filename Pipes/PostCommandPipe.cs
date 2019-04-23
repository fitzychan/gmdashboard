using CommonCode.Charts;
using CommonCode.FileUtility;
using CommonCode.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Pipes.PostParsedProject
{
    public interface IPostCommandPipe
    {
        void SaveChartCommand(Chart block);
        void AddToChartCommand(Chart block);
    }

    public class PostCommandPipe : IPostCommandPipe
    {
        public void AddToChartCommand(Chart block)
        {
            if (block == null)
                return;
            FileUtility.AddToChartCommand(block);
        }

        public void SaveChartCommand(Chart block)
        {
            if (block == null)
                return;
            FileUtility.SaveChartCommand(block);
        }
        public void AddSelectedToChartCommand(List<IRoll> blocks)
        {
            if (!blocks.Any())
                return;
            FileUtility.AddSelectedToChartCommand(blocks);
        }

        public void SaveSelectedChartCommand(List<IRoll> blocks)
        {
            if (!blocks.Any())
                return;
            FileUtility.SaveSelectedChartCommand(blocks);
        }
    }
}
