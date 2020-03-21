using CommonCode.Charts;
using CommonCode.FileUtility;
using CommonCode.Interfaces;
using System.Collections.Generic;
using System.Linq;
//todo this just seems incorrect
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
        public void AddSelectedToChartCommand(List<string> blocks)
        {
            if (!blocks.Any())
                return;
            FileUtility.AddSelectedToChartCommand(blocks);
        }

        public void SaveSelectedChartCommand(List<string> blocks)
        {
            if (!blocks.Any())
                return;
            FileUtility.SaveSelectedChartCommand(blocks);
        }
    }
}
