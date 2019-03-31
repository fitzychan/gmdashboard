using CommonBlocks;
using CommonCode;
using CommonCode.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DmAssistant.BlockBuilder
{
    public interface IBlockBuilder
    {
        IMainBlock BuildFromTxt(string stringValues);
        IMainBlock BuildFromRgf(XDocument xDoc);
    }
    class BlockBuilder : IBlockBuilder
    {
        public IMainBlock BuildFromRgf(XDocument xDoc)
        {
            var mainBlock = new MainBlock();
            var rollBlock = new RollBlock();
            var listOfAllRolls = new List<IBlock>();
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
                                                                                //This is the descriptor cell color and just looking for the cell type wont work ....  WE need to get the loading to work.
               // if (cell.Attribute("body-type") != null && cell.Attribute("body-type").Value.Equals("SubRollCell"))
                //if (cell.Attribute("col").Value == "0" )
                if (cell.Attribute("body-type") != null && cell.Attribute("body-type").Value.Equals("DescriptorCell"))
                {
                    listOfAllRolls.Add(new DescriptorBlock(cell.Value));
                    continue;
                }
                var cellAddress = cell.Attribute("col").Value + ":" + cell.Attribute("row").Value;
                if (cell.Value.EndsWith("..."))
                {
                    string dice = Regex.Match(cell.Value, @"\d+").Value;
                    if (cell.Value.StartsWith("d" + dice))
                    {
                        if (rollBlock.BlockDescriptor.Any())
                        {
                            listOfAllRolls.Add(rollBlock);
                            rollBlock = new RollBlock();
                        }
                        if (cell.Attribute("body-type") != null && cell.Attribute("body-type").Value.Equals("SubRollCell"))
                        {
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
                            rollBlock.Outcomes.Add(new SubRoll { RegularOutcome = cell.Value, CellAddress = cellAddress, SubBlockOutcome = new RollBlock { CellAddress = itemsToBeLinked.First().Column + ":" + itemsToBeLinked.First().Row } });
                        }
                        else
                        {
                            rollBlock.Outcomes.Add(new RegularRoll { RegularOutcome = cell.Value, CellAddress = cellAddress });
                        }
                    }
                }
                else
                {
                    rollBlock.Outcomes.Add(new RegularRoll { RegularOutcome = cell.Value, CellAddress = cellAddress });
                }
                if(cell.Equals(cells.LastOrDefault()))
                {
                    //When we hit the end of the list we want to add whatever we have left to the main block.
                    listOfAllRolls.Add(rollBlock);
                }
            }

            
            //Here when we add the rolls to the list we need to box them as the type they are.
            foreach(var roll in listOfAllRolls)
            {
                if(roll.BlockType.Equals(typeof(DescriptorBlock)))
                {
                    mainBlock.Blocks.Add((DescriptorBlock)roll);
                }
                else if(roll.BlockType.Equals(typeof(RollBlock)))
                {
                    if (((RollBlock)roll).IsSubRollBlock)
                        continue;
                    mainBlock.Blocks.Add((RollBlock)roll);
                }
            }

            ////listOfAllRolls contains all of our charts and all the sub roll outcomes laidout... So we want to skip over anything that is not a subroll
            foreach (RollBlock roll in listOfAllRolls.Where(x => x.BlockType == typeof(RollBlock)))
            {
                for (int i = 0; i < roll.Outcomes.Count; i++)
                {
                    if (roll.Outcomes[i].GetType().Equals(typeof(SubRoll)))
                    {
                        foreach (RollBlock titleRoll in listOfAllRolls.Where(x => x.BlockType != typeof(DescriptorBlock)))
                        {
                            if (((SubRoll)roll.Outcomes[i]).SubBlockOutcome.CellAddress == titleRoll.CellAddress)
                            {
                                ((SubRoll)roll.Outcomes[i]).SubBlockOutcome = titleRoll;
                                break;
                            }
                        }
                    }
                }
            }
            return mainBlock;
        }

        public IMainBlock BuildFromTxt(string stringValues)
        {
            var mainBlock = new MainBlock();
            bool unfinishedParseing = false;
            bool workingOnSubBlock = false;

            //THIS PARSES OUT THE PREAMBLE TILL THE FIRST d[num]
            var chartChars = stringValues.ToCharArray();
            int Chart_Count = chartChars.Count();
            int j = 0;
            for (; j < chartChars.Count(); j++)
            {
                if (chartChars.ElementAt(j) == 'd' && char.IsDigit(chartChars.ElementAt(j + 1)))
                {
                    break;
                }
                else
                {
                    mainBlock.Preamble += chartChars.ElementAt(j);
                }
            }


            var block = new RollBlock();
            var partialBlockRoll = new SubRoll();

            string subChart = "";

            try
            {
                for (int x = j; x <= Chart_Count; x++)
                {
                    string diceNumber = "";
                    if ((x + 1) < Chart_Count)
                    {
                        if (chartChars.ElementAt(x) == 'd' && char.IsDigit(chartChars.ElementAt(x + 1)))
                        {
                            if (block.Dice.Equals(block.Outcomes.Count) && block.Outcomes.Count != 0 && !workingOnSubBlock)
                            {
                                mainBlock.Blocks.Add(block);
                                block = new RollBlock();
                                unfinishedParseing = false;
                            }
                            else if(unfinishedParseing)
                            {
                                // we need to build up the sub block here and asign it to an IRoll and stick it in the main 
                                partialBlockRoll.RegularOutcome = block.Outcomes.ElementAt(block.Outcomes.Count() - 1).GetOutcome();
                                partialBlockRoll.SubBlockOutcome = new RollBlock
                                {
                                    BlockDescriptor = block.Outcomes.ElementAt(block.Outcomes.Count() - 1).GetOutcome()
                                };

                            }
                            while (char.IsDigit(chartChars.ElementAt(x + 1)))
                            {
                                diceNumber += chartChars.ElementAt(x + 1);
                                x++;
                            }
                            if (!unfinishedParseing)
                            {
                                subChart += "d" + diceNumber;
                                block.Dice = short.Parse(diceNumber);
                            }
                            else
                            {
                                partialBlockRoll.RegularOutcome += "d" + diceNumber;
                                partialBlockRoll.SubBlockOutcome.Dice = short.Parse(diceNumber);
                            }

                            while (((!char.IsDigit(chartChars.ElementAt(x + 1))) || !chartChars.ElementAt(x + 2).Equals('.')) && ((!char.IsDigit(chartChars.ElementAt(x + 1))) || !chartChars.ElementAt(x + 2).Equals('-')))
                            {
                                subChart += chartChars.ElementAt(x + 1);
                                x++;
                            }


                            if (!unfinishedParseing)
                            {
                                block.BlockDescriptor = subChart;
                                subChart = "";
                            }
                            else
                            {
                                partialBlockRoll.RegularOutcome += subChart;
                            }
                        }
                        else
                        {
                            //This is where we should look for a number followed by a '.'  this will dinote a sub answer and parse and add it to the list
                            //This is also where we will look for sub sub selections. Look for a Numer and ')'
                            string tempIntStr = string.Empty;
                            while (char.IsDigit(chartChars.ElementAt(x)) || chartChars.ElementAt(x).Equals('-'))
                            {
                                if (string.IsNullOrEmpty(subChart) || !string.IsNullOrEmpty(tempIntStr))
                                {
                                    subChart += chartChars.ElementAt(x);
                                    //This means we are not done with our block yet. 
                                    unfinishedParseing = true;
                                }
                                if(subChart.Contains("):"))
                                {
                                    subChart += chartChars.ElementAt(x);
                                }
                                tempIntStr += chartChars.ElementAt(x);
                                //newSubListCount = short.Parse(tempIntStr);
                                x++;
                            }


                            bool finishLoop = false;
                          
                            while (!finishLoop )
                            {

                                while (Chart_Count > x && !char.IsDigit(chartChars.ElementAt(x)))
                                {

                                    if((chartChars.ElementAt(x).Equals('d') && char.IsDigit(chartChars.ElementAt(x + 1))))
                                    {
                                        workingOnSubBlock = false;
                                        finishLoop = true;
                                        break;
                                    }
                                    //THis means we have found a sub roll
                                    else if(chartChars.ElementAt(x).Equals('(') && chartChars.ElementAt(x + 1).Equals('d') && char.IsDigit(chartChars.ElementAt(x + 2)))
                                    {
                                        workingOnSubBlock = true;
                                        finishLoop = true;
                                        x++;

                                        break;
                                    }
                                    subChart += chartChars.ElementAt(x);
                                    x++;
                                }
                                //This is where we finish the Whole deal.  This is because we will be searching for the final part of a roll
                                if (Chart_Count <= x)
                                {
                                    finishLoop = true;
                                    break;
                                }

                                while (char.IsDigit(chartChars.ElementAt(x)))
                                {
                                    //THIS WORKS FOR NOW...  HOWEVER THIS WILL OMLY ALLOW FOR TRIPPLE DIGITS FOR subrlls.....
                                    if (!finishLoop && char.IsDigit(chartChars.ElementAt(x)) && (!chartChars.ElementAt(x + 1).Equals('.') && (!chartChars.ElementAt(x + 2).Equals('.'))))
                                    {
                                        subChart += chartChars.ElementAt(x);
                                        x++;
                                    }
                                    //This is where we left off.  We can now do the regular rolls ok with this code.  We just now need to figure out how to get it orginized proper.
                                    else if(!finishLoop && char.IsDigit(chartChars.ElementAt(x)) && (chartChars.ElementAt(x + 1).Equals('-') ))
                                    {
                                        subChart += chartChars.ElementAt(x);
                                        x++;
                                    }
                                    else
                                    {
                                        finishLoop = true;
                                        break;
                                    }
                                }
                                    //This outer loop is too keep us from not properly adding integers to the outcome blocks.
                                    //The finishLoop in this statement needs to come first...  We will set it in a previous statement
                                    //This is because we will get a out of bounds if we dont...


                            }
                            //We do a decrement here so when the main loop increments we are looking at the proper space
                            x--;
                            if (string.IsNullOrEmpty(partialBlockRoll.RegularOutcome))
                            {
                                block.Outcomes.Add(new RegularRoll(subChart));
                            }
                            else
                            {
                                partialBlockRoll.SubBlockOutcome.Outcomes.Add(new RegularRoll(subChart));
                                if (partialBlockRoll.SubBlockOutcome.Dice.Equals(partialBlockRoll.SubBlockOutcome.Outcomes.Count))
                                {
                                    block.Outcomes.RemoveAt(block.Outcomes.Count - 1);
                                    block.Outcomes.Add(partialBlockRoll);
                                    partialBlockRoll = new SubRoll();
                                }
                            }
                            subChart = "";
                        }
                    }
                    else
                    {
                        mainBlock.Blocks.Add(block);
                        return mainBlock;
                    }
                }
            }
            catch (Exception e)
            {
                //TODO
            }
            return null;
        }
    }
    class BlockBuilderV2 : IBlockBuilder
    {
        readonly string subRollPattern = @"[(]d-?\d+[)]";
        readonly string chartPattern = @"d-?\d+[\x20]";
        readonly string rangeRollPattern = @"-?\d+--?\d+";

        public IMainBlock BuildFromRgf(XDocument xDoc)
        {
            throw new NotImplementedException();
        }

        public IMainBlock BuildFromTxt(string stringValues)
        {
            var Preamble = string.Empty;


            //Match the letter 'd' with any number and a space Char
            var regex = new Regex(chartPattern, RegexOptions.IgnoreCase);
            var rawChartList = new List<string>();
            var regexSplitCharts = regex.Split(stringValues);
            var dRollList = regex.Matches(stringValues).Cast<Match>().Select(m => m.Value).ToArray();

            var aggregatedRolls = new List<string>();
            var listSplitCharts = new List<string>();

            if (dRollList.Count() != regexSplitCharts.Count())
            {
                Preamble = regexSplitCharts.FirstOrDefault();
                listSplitCharts.AddRange(regexSplitCharts.Skip(1));
            }
            //Now we need to look for all ints that COULD have a '-'between them OR could have a sub roll...  Should we do some recursion to iterate a bunch?  Should we keep regexing?
            //listSplitCharts
            
            foreach(var chart in listSplitCharts)
            {
                var ting = BuildRollBlocks(chart);
            }




            //aggregatedRolls.AddRange(dRollList.Zip(listSplitCharts, (dNum, chart) => dNum + " " + chart));

            //throw new NotImplementedException();
            return null;
        }

        private IRollV2 BuildRollBlocks(string chartString)
        {
            //var charArr = block.ToCharArray();
            if (Regex.IsMatch(chartString, rangeRollPattern, RegexOptions.IgnoreCase))
            {
                return BuildRangeBlockV2(chartString);
            }
            else if(Regex.IsMatch(chartString, subRollPattern, RegexOptions.IgnoreCase))
            {
                //Do something to expect subrolls?
            }
            return BuildBlockV2(chartString);
            //if (Regex.IsMatch(block, subRollPattern, RegexOptions.IgnoreCase))
            //{

            //}
            //else
            //{
            //    //Regular parsing
            //}
        }

        private RangeRollV2 BuildRangeBlockV2(string chartString)
        {
            return null;
        }

        private RollV2 BuildBlockV2(string chartString)
        {
            var roll = new RollV2();
            string possibleoutcome = string.Empty;
            var characterArray = chartString.ToCharArray();


            var splitResult = Regex.Split(chartString, "(?<=[.])");






            //Here we parse out the description of the Roll
            int j = 0;
            for (; j < characterArray.Count(); j++)
            {
                if (characterArray.ElementAt(j +1) == '.' && char.IsDigit(characterArray.ElementAt(j)))
                {
                    possibleoutcome += characterArray.ElementAt(j);
                    j++;
                    break;
                }
                else
                {
                    roll.Description += characterArray.ElementAt(j);
                }
            }
            var countThing = roll.Description.Count();
            var thingerThing = Regex.Split(chartString.Substring(countThing), "(?<=[.])");



            for (; j < characterArray.Count(); j++)
            {
                //if (characterArray.ElementAt(j + 1) == '.' && char.IsDigit(characterArray.ElementAt(j)))
                string foundInt = string.Empty;
                if (characterArray.ElementAt(j) == '.')
                {
                    //Since we found a '.' we want to find the int that is before it.  No matter what we need to think that this is a possible roll outcome.  If the user fucked it up.  Its going to be fucked up
                    for(int i = j - 1; char.IsDigit(characterArray.ElementAt(i)) ;i--)
                    {
                        foundInt += characterArray.ElementAt(i);
                    }

                    //if we find the '.' but we dont find a INT right before it.  We found a regular old peroid.  So we should add it and continue
                    if(foundInt.Equals(string.Empty))
                    {
                        possibleoutcome += characterArray.ElementAt(j);
                        continue;
                    }

                    //We need at least 3 chars to be considerd a valid entry..."1.."
                    //So we have found some stuff!   is the shortest possible valid entry  on the second round.  We need to make sure we have something.  if we have something we are going to need to find out how long the found int is...
                    //We need to know so we can remove it.  At this point it has been already added to the string
                    if (possibleoutcome.Count() > 2)
                    {
                        //We are going to clip what we have in this string...  this is because its oriented correctly and I dont want to figure out how else to properly re orient the dice value...
                        foundInt = possibleoutcome.Substring(possibleoutcome.Length - foundInt.Length, foundInt.Length);
                        var clippedString = possibleoutcome.Substring(0, possibleoutcome.Length - foundInt.Length);
                        roll.Outcomes.Add(new OutcomeRollV2(clippedString));
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
                    roll.Outcomes.Add(new OutcomeRollV2(possibleoutcome));
                    break;
                }
            }

            return null;
        }

    }
}
