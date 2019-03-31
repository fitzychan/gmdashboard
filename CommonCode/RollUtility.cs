using CommonBlocks;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CommonCode.RollUtility
{
    public interface IRollUtility
    {
        IMainBlock RollOnMainBlock(IMainBlock mainBlock);
    }
    public class RollUtility : IRollUtility
    {
        MainBlock localMainBlock;
        int rolledNumber;

        public IMainBlock RollOnMainBlock(IMainBlock mainBlock)
        {
            localMainBlock = (MainBlock)mainBlock;
            if (localMainBlock == null)
            {
                MessageBox.Show("The file is improperly formatted", "Bad file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new MainBlock();
            }
            else
            {
                foreach (var rollBlock in localMainBlock.Blocks)
                {
                    if (rollBlock.BlockType == typeof(RollBlock))
                    {
                        var localRollBlock = ((RollBlock)rollBlock);
                        localRollBlock.Result = RollOnBlock(localRollBlock);
                    }

                }
            }
            return localMainBlock;
        }

        private string RollOnBlock(IRoll roll)
        {
            string returnString = string.Empty;
            try
            {                                   //This is because arrays start at 0 we need to shift the outcome by one.  NOT the dice passed in.
                rolledNumber = RandomUtility.RollDice(((RollBlock)roll).Dice) - 1;

                if(roll.RollType == typeof(DescriptorBlock))
                {
                    return roll.GetOutcome();
                }

                var tempRoll = ((RollBlock)roll).Outcomes.ElementAt(rolledNumber);
                if (tempRoll.GetType() == typeof(RegularRoll))
                {
                    returnString = (((RegularRoll)tempRoll).GetOutcome());
                }
                else if (tempRoll.GetType() == typeof(SubRoll))
                {
                    //TODO we are here trying to get the spaceing correct.... We should be doing this all in one place...
                    returnString = (((SubRoll)tempRoll).SubBlockOutcome.BlockDescriptor + Environment.NewLine + "\t" + RollOnBlock(((SubRoll)tempRoll).SubBlockOutcome)) + Environment.NewLine;
                }
                return returnString;
            }
            catch (Exception e)
            {
                // throw;
                return "Error in RollOnBlock:" + e.Message;
            }
        }
    }
}
