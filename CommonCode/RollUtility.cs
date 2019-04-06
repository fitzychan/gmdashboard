using CommonCode.Blocks;
using CommonCode.Rolls;
using System.Collections.Generic;
using System.Linq;

namespace CommonCode.RollUtility
{
    public interface IRollUtility
    {
        IChart RollOnChart(IChart chart);
    }
    public class RollUtility : IRollUtility
    {

        public IChart RollOnChart(IChart chart)
        {

            if (chart.TypeOfChart == ObjectTypes.Chart)
            {
                foreach (var chartRoll in ((Chart)chart).ChartRolls)
                {
                    RollOnRoll(chartRoll);
                }
            }
            return chart;
        }

        public string RollOnRoll(IRoll roll)
        {
            string rollOnRollResult = string.Empty;
            using (var utility = new RandomUtility())
            {
                if (roll.TypeOfRoll == ObjectTypes.StandardRoll || roll.TypeOfRoll == ObjectTypes.RangeRoll || roll.TypeOfRoll == ObjectTypes.TextRoll)
                {
                    var chartRoll = (StandardRoll)roll;
                    //TODO  THIS SHOULD BE USEING THE DICE NUMBER TO PICK THE ROLL...
                    //THE REASON WE ARE NOT IS BECAUSE WE HAVE NOT FULL GOTTEN THE PRECENT ROLL
                    var diceOutcome = utility.RollDice(((StandardRoll)roll).Outcomes.Count) - 1;

                    var resultOfRoll = chartRoll.Outcomes.ElementAt(diceOutcome);
                    if (resultOfRoll.TypeOfRoll == ObjectTypes.StandardRoll)
                    {
                        chartRoll.Outcome += chartRoll.Description + RollOnRoll(resultOfRoll);
                    }
                    else
                    {
                        var outcome = chartRoll.Description + " " + resultOfRoll.GetDescription;
                        chartRoll.Outcome = outcome;
                        return outcome;
                    }
                }
                return "";
            }
        }
    }
}
