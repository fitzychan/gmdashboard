using CommonCode.Charts;
using CommonCode.FileUtility;
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
        FileUtility fileUtility;
        public PostCommandPipe()
        {
            fileUtility = new FileUtility();
        }

        public void AddToChartCommand(Chart block)
        {
            if (block == null)
                return;
            fileUtility.AddToChartCommand(block);
        }

        public void SaveChartCommand(Chart block)
        {
            if (block == null)
                return;
            fileUtility.SaveChartCommand(block);
        }
        public void AddSelectedToChartCommand(List<string> blocks)
        {
            if (!blocks.Any())
                return;
            fileUtility.AddSelectedToChartCommand(blocks);
        }

        public void SaveSelectedChartCommand(List<string> blocks)
        {
            if (!blocks.Any())
                return;
            fileUtility.SaveSelectedChartCommand(blocks);
        }
    }
}
