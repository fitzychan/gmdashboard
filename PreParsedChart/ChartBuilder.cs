using CommonCode.Charts;
using CommonCode.Interfaces;
using CommonCode.Rolls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GmDashboard.ChartBuilder
{
    public interface IChartBuilder
    {
        IChart BuildFromTxt(string stringValues);
        IChart BuildFromRgf(XDocument xDoc);
        IChart BuildFromParams(string paramString);
    }

    public class ChartBuilder : IChartBuilder
    {

        readonly Regex chartRegex;
        readonly Regex rangeRegex;
        readonly Regex subrollRegex;

        private const string PARAMETER = ".PARAMETER";

        public ChartBuilder()
        {
            chartRegex = new Regex(@"d-?\d+[\x20]", RegexOptions.IgnoreCase);
            rangeRegex = new Regex(@"-?\d+--?\d+", RegexOptions.IgnoreCase);
            subrollRegex = new Regex(@"[(]d-?\d+[)]", RegexOptions.IgnoreCase);
        }

        public IChart BuildFromTxt(string stringValues)
        {
            //Match the letter 'd' with any number and a space Char
            var mainChart = new Chart();
            if (stringValues == null)
            {
                throw new ArgumentNullException("Add a chart plz");
            }
            var regexSplitCharts = chartRegex.Split(stringValues);
            var dRollList = chartRegex.Matches(stringValues).Cast<Match>().Select(m => m.Value).ToArray();

            var listSplitCharts = new List<string>();

            if (dRollList.Count() != regexSplitCharts.Count())
            {
                mainChart.Preamble = regexSplitCharts.FirstOrDefault();
                listSplitCharts.AddRange(regexSplitCharts.Skip(1));
            }

            foreach (var chart in listSplitCharts.Select((value, i) => new { i, value }))
            {
                var blockToAdd = BuildRolls(chart.value);

                blockToAdd.Description = dRollList.ElementAt(chart.i) + blockToAdd.Description;

                blockToAdd.Dice = int.Parse(dRollList.ElementAt(chart.i).Substring(1, dRollList.ElementAt(chart.i).Length - 1));
                mainChart.ChartRolls.Add(blockToAdd);
            }

            return mainChart;
        }

        private IRoll BuildRollBlocks(string chartString)
        {
            return BuildRolls(chartString);
        }

        private StandardRoll BuildRolls(string chartString)
        {
            var block = new StandardRoll();
            string possibleoutcome = string.Empty;
            var characterArray = chartString.ToCharArray();

            //Here we parse out the description of the Roll
            int j = 0;
            for (; j < characterArray.Count(); j++)
            {
                if (characterArray.ElementAt(j + 1) == '.' && char.IsDigit(characterArray.ElementAt(j)))
                {

                    //We need to run back the array for all found chars that are number
                    for (; char.IsDigit(characterArray.ElementAt(j)) || characterArray.ElementAt(j) == '-'; j--)
                    {
                    }
                    j++;
                    block.Description = block.Description.Substring(0, j);
                    possibleoutcome += characterArray.ElementAt(j);
                    break;
                }
                else
                {
                    block.Description += characterArray.ElementAt(j);
                }
            }


            for (; j < characterArray.Count(); j++)
            {
                string foundInt = string.Empty;
                if (characterArray.ElementAt(j) == '.')
                {
                    //Since we found a '.' we want to find the int that is before it.  No matter what we need to think that this is a possible roll outcome.  If the user fucked it up.  Its going to be fucked up
                    for (int i = j - 1; char.IsDigit(characterArray.ElementAt(i)) || characterArray.ElementAt(i) == '-'; i--)
                    {
                        foundInt += characterArray.ElementAt(i);
                    }

                    //if we find the '.' but we dont find a INT right before it.  We found a regular old peroid.  So we should add it and continue
                    if (foundInt.Equals(string.Empty))
                    {
                        possibleoutcome += characterArray.ElementAt(j);
                        continue;
                    }

                    //We need at least 3 chars to be considerd a valid entry..."1.."
                    //So we have found some stuff!   is the shortest possible valid entry  on the second round.  We need to make sure we have something.  if we have something we are going to need to find out how long the found int is...
                    //We need to know so we can remove it.  At this point it has been already added to the string
                    if (possibleoutcome.Count() > 3)
                    {
                        var splitList = possibleoutcome.Split('-').ToList();
                        //We are going to clip what we have in this string...  this is because its oriented correctly and I dont want to figure out how else to properly re orient the dice value...
                        foundInt = possibleoutcome.Substring(possibleoutcome.Length - foundInt.Length, foundInt.Length);
                        if (!splitList.TrueForAll(x => int.TryParse(x, out int parseResult)))
                        {
                            block.Outcomes.Add(new TextRoll(possibleoutcome.Substring(0, possibleoutcome.Length - foundInt.Length)));
                        }
                    }
                    foundInt += possibleoutcome = characterArray.ElementAt(j).ToString();
                    possibleoutcome = foundInt;
                    foundInt = string.Empty;
                }
                else
                {
                    possibleoutcome += characterArray.ElementAt(j);
                }

                //We have hit the end.. Add what we got and stop...
                if (characterArray.Count() <= j + 1)
                {
                    block.Outcomes.Add(new TextRoll(possibleoutcome));
                    possibleoutcome = string.Empty;
                    break;
                }
            }

            //this is a catchall for if we get pushed out of the list and still have something in the possible outcome.
            if (!string.IsNullOrEmpty(possibleoutcome))
            {
                block.Outcomes.Add(new TextRoll(possibleoutcome));
            }


            //After we have gotten all the rolls into a basic outcome. we want to recomb through and make sure that we account for subrolls
            for (int i = 0; block.Outcomes.Count() > i; i++)
            {
                var outcomeToCheck = ((TextRoll)block.Outcomes.ElementAt(i)).Description;
                if (subrollRegex.IsMatch(outcomeToCheck))
                {
                    var subRoll = new StandardRoll();
                    var foundInt = subrollRegex.Match(outcomeToCheck).Value.Replace("(", "").Replace("d", "").Replace(")", "");
                    subRoll.Dice = int.Parse(foundInt);
                    subRoll.Description = outcomeToCheck;

                    i++;
                    //This is where we iterate over the list and get the next few results that we can
                    for (int x = 0; x < subRoll.Dice; x++)
                    {
                        var outcome = ((TextRoll)block.Outcomes.ElementAt(i)).Description;
                        subRoll.Outcomes.Add(new TextRoll(outcome));
                        block.Outcomes.RemoveAt(i);
                    }
                    block.Outcomes[i - 1] = subRoll;
                    i--;
                }
            }
            return block;
        }


        public IChart BuildFromRgf(XDocument xDoc)
        {
            throw new NotImplementedException();
        }

        public IChart BuildFromParams(string paramString)
        {
            var functionChart = new FunctionParamChart();

            var startIndex = paramString.IndexOf("<#") + 2;
            var endIndex = paramString.IndexOf("#>") - 3;

            var specialParams = Regex.Split(paramString.Substring(startIndex, endIndex), "\r\n|\r|\n");

            var listOfParams = new List<List<string>>();

            for (int i = 0; i < specialParams.Length; i++)
            {
                string param = specialParams[i];
                if (!string.IsNullOrWhiteSpace(param))
                {
                    if(param.Contains(PARAMETER))
                    {
                        List<string> paramAndNotes = new List<string>
                        {
                            param.Replace(PARAMETER, "").TrimStart(' ')
                        };

                        i++;

                        for( ; !string.IsNullOrWhiteSpace(specialParams[i]); i++)
                        {
                            paramAndNotes.Add(specialParams[i]);
                        }
                        listOfParams.Add(paramAndNotes);
                    }
                }
            }
            functionChart.Parameters = listOfParams;
            return functionChart;
        }
    }
}
