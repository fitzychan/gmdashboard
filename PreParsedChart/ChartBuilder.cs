using CommonCode;
using CommonCode.Charts;
using CommonCode.Interfaces;
using CommonCode.Rolls;
using DialogService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GmDashboard.ChartBuilder
{
    public interface IChartBuilder
    {
        IChart BuildFromTxt(string stringValues);
        IChart BuildFromRgf(XDocument xDoc);
        IChart BuildFromParams(string paramString, FileInfo file);
    }

    public class ChartBuilder : IChartBuilder
    {

        private readonly IRegexDetectionUtility rollDetection;
        private readonly Regex chartRegex;
        private readonly Regex rangeRegex;
        private readonly Regex subrollRegex;
        private const string PARAMETER = ".PARAMETER";

        public ChartBuilder()
        {
            rollDetection = new RegexDetectionUtility();
            chartRegex = rollDetection.RollTitleDetector();
            rangeRegex = rollDetection.RangeRollDetector();
            subrollRegex = rollDetection.SubRollDetector();
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
                if(characterArray.Count() -1 == j)
                {
                    break;
                }
                if (characterArray.ElementAt(j + 1) == '.' && char.IsDigit(characterArray.ElementAt(j)))
                {

                    //We need to run back the array for all found chars that are a number
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

        /// <summary>
        /// CURRENTLY WE ARE HAVING AN ISSUE WITH KNOWING WHAT A SUBCELL IS...  We are going to need to dinote them as such.
        /// This is so we can type match instead of doing string/char matches
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        public IChart BuildFromRgf(XDocument xDoc)
        {

            var mainBlock = new ChartRgf();
            var rollBlock = new RollBlockRgf();
            var listOfAllRolls = new List<IBlockRgf>();
            var linkedItems = new List<LinkedItem>();

            var cells = xDoc.Descendants().Where(p => p.Name.LocalName == "cell").OrderBy(column => column.Attribute("col").Value);
            if (xDoc.Descendants().Any(p => p.Name.LocalName == "LinkedItems")) //"TestCharts.rgf" has the proper formats  This is used for finding all the sublinks
            {
                foreach (var linkedItem in xDoc.Descendants().Where(p => p.Name.LocalName == "LinkedItem"))
                {
                    var address = linkedItem.Attribute("CellAddress").Value.Split(':');
                    linkedItems.Add(new LinkedItem { Row = int.Parse(address.First()), Column = int.Parse(address.Last()), LinkId = int.Parse(linkedItem.Attribute("LinkedId").Value) });
                }
            }


            foreach (var cell in cells)
            {

                var cellAddress = cell.Attribute("col").Value + ":" + cell.Attribute("row").Value;
                //cell.Value.EndsWith("...") || 
                if (cell.Attribute("body-type") != null && (cell.Attribute("body-type").Value.Equals("SubRollCell") || cell.Attribute("body-type").Value.Equals("HeadRollCell")))
                {
                    string dice = Regex.Match(cell.Value, @"\d+").Value;
                    if (cell.Value.StartsWith("d" + dice))
                    {
                        if (rollBlock.BlockDescriptor.Any())
                        {
                            listOfAllRolls.Add(rollBlock);
                            rollBlock = new RollBlockRgf();
                        }
                        if (cell.Attribute("body-type") != null && cell.Attribute("body-type").Value.Equals("SubRollCell"))
                        {
                            //This denoting that a cell is a headcell and the outcome of another cell...
                            rollBlock.IsSubRollBlock = true;
                        }

                        rollBlock.Dice = int.Parse(dice);
                        rollBlock.BlockDescriptor = cell.Value;
                        rollBlock.CellAddress = cellAddress;
                    }
                    else
                    {
                        if (cell.Attribute("body-type") != null && cell.Attribute("body-type").Value.Equals("SubRollCell"))
                        {
                            var linkItem = linkedItems.Where(x => x.Row == int.Parse(cellAddress.Split(':').Last())).Where(x => x.Column == int.Parse(cellAddress.Split(':').First())).First();
                            // we want everything with the same link id of the titlelink...
                            var itemsToBeLinked = linkedItems.Where(x => x.LinkId.Equals(linkItem.LinkId)).ToList();
                            itemsToBeLinked.Remove(linkItem);
                            rollBlock.Outcomes.Add(new SubRollRgf { RegularOutcome = cell.Value, CellAddress = cellAddress, SubBlockOutcome = new RollBlockRgf { CellAddress = itemsToBeLinked.First().Column + ":" + itemsToBeLinked.First().Row } });
                        }
                        else
                        {
                            rollBlock.Outcomes.Add(new RollRgf { RegularOutcome = cell.Value, CellAddress = cellAddress });
                        }
                    }
                }
                else if (cell.Attribute("body-type") != null && cell.Attribute("body-type").Value.Equals("DescriptorCell"))
                {
                    //This is the descriptor cell color and just looking for the cell type wont work ....  WE need to get the loading to work.
                    // if (cell.Attribute("body-type") != null && cell.Attribute("body-type").Value.Equals("SubRollCell"))
                    //if (cell.Attribute("col").Value == "0" )
                    if(rollBlock.Dice == rollBlock.Outcomes.Count)
                    {
                        if (rollBlock.BlockDescriptor.Any())
                        {
                            listOfAllRolls.Add(rollBlock);
                            rollBlock = new RollBlockRgf();
                        }
                    }
                    listOfAllRolls.Add(new DescriptorRgf(cell.Value));
                }
                else if(cell.Attribute("body-type") != null && cell.Attribute("body-type").Value.Equals("StandardRollCell"))
                {
                    rollBlock.Outcomes.Add(new RollRgf { RegularOutcome = cell.Value, CellAddress = cellAddress });
                }
                if (cell.Equals(cells.LastOrDefault()))
                {
                    //When we hit the end of the list we want to add whatever we have left to the main block.
                    listOfAllRolls.Add(rollBlock);
                }
            }


            //Here when we add the rolls to the list we need to box them as the type they are.
            foreach (var roll in listOfAllRolls)
            {
                if (roll.BlockType.Equals(typeof(DescriptorRgf)))
                {
                    mainBlock.Blocks.Add((DescriptorRgf)roll);
                }
                else if (roll.BlockType.Equals(typeof(RollBlockRgf)))
                {
                    if (((RollBlockRgf)roll).IsSubRollBlock)
                        continue;
                    mainBlock.Blocks.Add((RollBlockRgf)roll);
                }
            }

            ////listOfAllRolls contains all of our charts and all the sub roll outcomes laidout... So we want to skip over anything that is not a subroll
            //fix this... Its fucked.
            foreach (RollBlockRgf roll in listOfAllRolls.Where(x => x.BlockType == typeof(RollBlockRgf)))
            {
                for (int i = 0; i < roll.Outcomes.Count; i++)
                {
                    if (roll.Outcomes[i].GetType().Equals(typeof(SubRollRgf)))
                    {
                        foreach (RollBlockRgf titleRoll in listOfAllRolls.Where(x => x.BlockType != typeof(DescriptorRgf)))
                        {
                            if (((SubRollRgf)roll.Outcomes[i]).SubBlockOutcome.CellAddress == titleRoll.CellAddress)
                            {
                                ((SubRollRgf)roll.Outcomes[i]).SubBlockOutcome = titleRoll;
                                break;
                            }
                        }
                    }
                }
            }
            return mainBlock;
        }

        public IChart BuildFromParams(string paramString, FileInfo file)
        {
            var functionChart = new FunctionParamChart();

            var startIndex = paramString.IndexOf("<#") + 2;
            var endIndex = paramString.IndexOf("#>") - 3;

            var specialParams = Regex.Split(paramString.Substring(startIndex, endIndex), "\r\n|\r|\n");

            var listOfParams = new List<Parameter>();

            for (int i = 0; i < specialParams.Length; i++)
            {
                string param = specialParams[i];
                if (!string.IsNullOrWhiteSpace(param))
                {
                    if (param.Contains(PARAMETER))
                    {
                        var localParam = new Parameter
                        {
                            Name = param.Replace(PARAMETER, "").TrimStart(' ')
                        };

                        i++;

                        for (; !string.IsNullOrWhiteSpace(specialParams[i]); i++)
                        {
                            localParam.Description += specialParams[i];
                        }
                        listOfParams.Add(localParam);
                    }
                }
            }
            functionChart.Parameters = listOfParams;

            var readParams = Dialogs.ExtractPowerShellParameters(functionChart);

            functionChart = new FunctionParamChart();
            foreach (var param in readParams)
            {
                functionChart.Parameters.Add(new Parameter { Name = param.Name, Description = param.Description, Value = param.Value });
            }

            functionChart.PowershellFileInfo = file;

            return functionChart;
        }
    }
}
