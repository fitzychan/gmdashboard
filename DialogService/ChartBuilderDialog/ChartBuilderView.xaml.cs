using CommonCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;
using unvell.Common;
using unvell.ReoGrid;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;

namespace DialogService.ChartBuilderDialog
{
    public class SpecialRollData
    {
        //We really should be making the key CellPosition
        //Where the cell is,  The linked cells id
        public static Dictionary<string, int> SubRollProperty { get; set; }
        //Location of the Main cell in the roll,  A list of the rolls that are under it...
        public static List<CellPosition> TitleCellProperty { get; set; }
        public static int SubCellCount = 1;
    }

    /// <summary>
    /// Description for ChartBuilderView.
    /// </summary>
    public partial class ChartBuilderView : Window
    {
        private readonly IRegexDetectionUtility rollDetection;
        Worksheet Worksheet;
        
        /// <summary>
        /// Initializes a new instance of the ChartBuilderView class.
        /// </summary>
        public ChartBuilderView()
        {
            InitializeComponent();
            rollDetection = new RegexDetectionUtility();
            SpecialRollData.SubRollProperty = new Dictionary<string, int>();
            SpecialRollData.TitleCellProperty = new List<CellPosition>();

            Worksheet = grid.CurrentWorksheet;
            Worksheet.DisableSettings(WorksheetSettings.Edit_AutoFormatCell);
            Worksheet.Resize(1000, 1000);
            grid.ActionPerformed += OnActionPerformed;
            //public event EventHandler<WorkbookActionEventArgs> ActionPerformed;
            //grid.KeyDown += new System.Windows.Input.KeyEventHandler(MainWindow_KeyDown);
        }
        //void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    //if (e.Key == Key.Subtract)
        //    //{
        //    //    // Do something
        //    //}
        //}
        private void OnActionPerformed(object sender, EventArgs e)
        {
            var runningAction = ((WorkbookActionEventArgs)e).Action;
            if(runningAction.GetType().Equals(typeof(RemoveRangeDataAction)))
            {
                RemoveSpecialCell(((RemoveRangeDataAction)runningAction).Range);
                Worksheet.ClearRangeContent(((RemoveRangeDataAction)runningAction).Range, CellElementFlag.All);
            }
        }
        private void ResetSheet_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            grid.CurrentWorksheet.Reset();
            grid.CurrentWorksheet.Resize(1000, 1000);
            SpecialRollData.SubRollProperty = new Dictionary<string, int>();
            SpecialRollData.TitleCellProperty = new List<CellPosition>();
            SpecialRollData.SubCellCount = 1;
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var saveLocation = SaveCurrentChartCommand();

            //We will save the file with the default mode that comes with the ReoGrid...  Then we will openit and save a specific chucnk that stores the relationship data...
            if (!string.IsNullOrEmpty(saveLocation))
            {
                using (Stream memStream = new MemoryStream())
                {

                    grid.Save(memStream, unvell.ReoGrid.IO.FileFormat.ReoGridFormat);
                    memStream.Position = 0;

                    var linkedXml = new XElement("LinkedItems");
                    foreach (var item in SpecialRollData.SubRollProperty)
                    {
                        linkedXml.Add(new XElement("LinkedItem",new XAttribute("CellAddress",item.Key.ToString()), new XAttribute("LinkedId", item.Value.ToString())));
                    }
                    foreach (var item in SpecialRollData.TitleCellProperty)
                    {
                        if (string.IsNullOrWhiteSpace(item.ToString()))
                        {
                            linkedXml.Add(new XElement(item.ToAddress().ToString() + Worksheet.Cells[item.ToAddress()].Data.ToString()));
                        }
                    }
                    var xDoc = XDocument.Load(memStream);
                    xDoc.Root.Add(linkedXml);
                    xDoc.Save(saveLocation);
                }
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            var openFileWindow = new OpenFileDialog
            {
                Filter = "rgf files (*.rgf) | *.rgf",
                FilterIndex = 2,
                RestoreDirectory = true,
                InitialDirectory = Directory.GetCurrentDirectory() + "\\Tables"
            };

            if (openFileWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Reset();

                    Worksheet.LoadRGF(openFileWindow.FileName);

                    var xDoc = XDocument.Load(openFileWindow.FileName);
                    var foundElements = xDoc.Descendants("cell").Attributes("body-type").Where(x => x.Value.Equals("DescriptorCell"));

                    foreach(var descriptor in foundElements)
                    {
                        Worksheet.FocusPos = new CellPosition(int.Parse(descriptor.Parent.Attribute("row").Value), int.Parse(descriptor.Parent.Attribute("col").Value));
                        SetDescriptor();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private string SaveCurrentChartCommand()
        {
            var saveRolldChart = new SaveFileDialog
            {
                Filter = "rgf files (*.rgf)|*.rgf",
                FilterIndex = 2,
                RestoreDirectory = true,
                OverwritePrompt = true
            };

            if (saveRolldChart.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return saveRolldChart.FileName;
            }

            //THIs is where we need to reaload the xml sheet and add the other data to it....
            return null;
        }

        //Ctrl+Q
        private void DesignateDescriptor_Click(object sender, RoutedEventArgs e)
        {
            SetDescriptor();
        }

        private void SetDescriptor()
        {
            //We need to find a way to save the cells that are being highlighted
            //We could also just make a moniker to save to the data.... And save it and rewrite the parserfor that specific chart type.
            var selected = Worksheet.FocusPos;
            var cell = Worksheet.Cells[selected];
            
            if (cell.Data == null)
            {
                return;
            }
            SpecialRollData.TitleCellProperty.Add(selected);
            Worksheet[selected] = new DescriptorCell();
            cell.Style.BackColor = SolidColor.LightSteelBlue;
        }

        //Ctrl+W
        private void DesignateRoll_Click(object sender, RoutedEventArgs e)
        {
            var range = Worksheet.SelectionRange;
            var totalRows = range.Rows - 1;
            int rollCounter = 0;

            Worksheet.IterateCells(range, false, (row, col, cell) =>
            {
                
                if (cell == null || cell.Body != null && cell.Body.GetType().Equals(typeof(DescriptorCell)))
                {
                    if(rollCounter != 0)
                    {
                        cell = Worksheet.CreateAndGetCell(row, col);
                        cell.Body = new StandardRollCell();
                        cell.Style.BackColor = new SolidColor("#D8D7DB");
                        cell.Data = rollCounter + ". __________ .";
                    }
                    else
                    {
                        cell = Worksheet.CreateAndGetCell(row, col);
                        cell.Body = new HeadRollCell();
                        cell.Style.BackColor = new SolidColor("#BDBCC3");
                        cell.Data = "d" + totalRows + " ___________ ...";
                    }
                }
                else
                {
                    if(rollCounter != 0 )
                    {
                        cell.Body = new StandardRollCell();
                        cell.Style.BackColor = new SolidColor("#D8D7DB");
                        if (!rollDetection.OutcomeDetector().IsMatch(cell.Data.ToString()))
                        {
                            cell.Data = rollCounter + ". " + cell.Data + " .";
                        }
                    }
                    else
                    {
                        cell.Body = new HeadRollCell();
                        cell.Style.BackColor = new SolidColor("#BDBCC3");
                        //We need to see if the formating is already there.  If it is we can ignore it
                        if (!rollDetection.RollTitleDetector().IsMatch(cell.Data.ToString().TrimStart().TrimEnd()))
                        {
                            cell.Data = "d" + totalRows + " " + cell.Data + " ...";
                        }
                    }
                }

                rollCounter++;
                return true;

            });
            Worksheet.AutoFitColumnWidth(range.Col, true);
        }
        
        //Ctrl+E
        private void DesignateSubroll_Click(object sender, RoutedEventArgs e)
        {
            var selected = Worksheet.FocusPos;
            var cell = Worksheet.Cells[selected];
            if(cell.Data == null)
            {
                return;
            }
            var cellData = cell.Data.ToString();
            {
                if (SpecialRollData.SubRollProperty.ContainsKey(selected.Row + ":" + selected.Col))
                {
                    System.Windows.Forms.MessageBox.Show("Subroll Already added", "Cannot complete action", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //This is useds to push the action to the stack?
                //look at this https://reogrid.net/v0-8-8/workbook-action-dispatching/
                //var action = new SetRangeStyleAction(cell.PositionAsRange, new WorksheetRangeStyle() { BackColor = SolidColor.Black });
                cell.Style.BackColor = SolidColor.LightYellow;


                //This needs to be called before we add it to the list... This is where the counting logic is for adding the linked cells.

                SpecialRollData.SubRollProperty.Add(selected.Row + ":" + selected.Col , AddSubChartMoniker());

                if (cell.Body != null && cell.Body.GetType() == typeof(HeadRollCell))
                {
                    Worksheet[selected] = new SubRollHeadCell("Link: " + SpecialRollData.SubRollProperty.Last().Value);
                }
                else
                {
                    Worksheet[selected] = new SubRollCell("Link: " + SpecialRollData.SubRollProperty.Last().Value);
                }
            }
        }
        
        private int AddSubChartMoniker()
        {
            var shouldIncreaseNumber = SpecialRollData.SubRollProperty.ElementAtOrDefault(SpecialRollData.SubRollProperty.Count() - 2).Value == SpecialRollData.SubCellCount;
            if (SpecialRollData.SubRollProperty.LastOrDefault().Value == SpecialRollData.SubCellCount && shouldIncreaseNumber)
            {
                SpecialRollData.SubCellCount++;
            }
            return SpecialRollData.SubCellCount;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CollapseItems_Click(object sender, RoutedEventArgs e)
        {
            var range = Worksheet.SelectionRange;

            var listOfItems = new List<Cell>();

            Worksheet.IterateCells(range, false, (row, col, cell) =>
            {
                if(cell != null)
                {
                    if(cell.DisplayText != null && cell.DisplayText != "")
                    {
                        listOfItems.Add(cell);
                    }
                }
                
                return true;
            });

            var startCell = range.StartPos;

            for(int i = 0; i < range.Rows; i++)
            {
                if(listOfItems.Count > i)
                {
                    Worksheet[startCell.Row + i, startCell.Col] = listOfItems[i].DisplayText;
                }
                else
                {
                    Worksheet[startCell.Row + i, startCell.Col] = string.Empty;
                }
            }
        }

        private void RemoveSpecialCell(CellPosition cellPosition)
        {
            SpecialRollData.SubRollProperty.Remove(cellPosition.Row + ":" + cellPosition.Col);
            SpecialRollData.TitleCellProperty.Remove(cellPosition);
        }
        private void RemoveSpecialCell(RangePosition cellRange)
        {
            for(var colCounter = cellRange.StartPos.Col; colCounter <= cellRange.EndPos.Col; colCounter++)
            {
                for(var rowCounter = cellRange.StartPos.Row; rowCounter <= cellRange.EndPos.Row; rowCounter++)
                {
                    RemoveSpecialCell(new CellPosition(rowCounter, colCounter));
                }
            }
        }
    }
}