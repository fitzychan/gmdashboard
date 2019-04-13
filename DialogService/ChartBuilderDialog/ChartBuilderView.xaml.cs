using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using unvell.ReoGrid;
using unvell.ReoGrid.Graphics;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Forms;

namespace DialogService.ChartBuilderDialog
{
    public class SpecialRollData
    {
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
        
        /// <summary>
        /// Initializes a new instance of the ChartBuilderView class.
        /// </summary>
        public ChartBuilderView()
        {
            InitializeComponent();

            SpecialRollData.SubRollProperty = new Dictionary<string, int>();
            SpecialRollData.TitleCellProperty = new List<CellPosition>();

            var workSheet = grid.CurrentWorksheet;
            workSheet.DisableSettings(WorksheetSettings.Edit_AutoFormatCell);
            workSheet.Resize(1000, 1000);
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
            var sheet = grid.CurrentWorksheet;
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
                            linkedXml.Add(new XElement(item.ToAddress().ToString() + sheet.Cells[item.ToAddress()].Data.ToString()));
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
            var openFileDialog1 = new OpenFileDialog
            {
                Filter = "rgf files (*.rgf) | *.rgf",
                FilterIndex = 2,
                RestoreDirectory = true,
                InitialDirectory = Directory.GetCurrentDirectory() + "\\Tables"
            };

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Reset();
                    var sheet = grid.CurrentWorksheet;
                    sheet.LoadRGF(openFileDialog1.FileName);
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
            //We need to find a way to save the cells that are being highlighted
            //We could also just make a moniker to save to the data.... And save it and rewrite the parserfor that specific chart type.
            var sheet = grid.CurrentWorksheet;
            var selected = sheet.FocusPos;
            var cell = sheet.Cells[selected];
            
            if(cell.Data == null)
            {
                return;
            }
            SpecialRollData.TitleCellProperty.Add(selected);
            sheet[selected] = new DescriptorCell();
            cell.Style.BackColor = SolidColor.LightSteelBlue;
        }

        //Ctrl+W
        private void DesignateRoll_Click(object sender, RoutedEventArgs e)
        {
            var range = grid.CurrentWorksheet.SelectionRange;
            var totalRows = range.Rows - 1;
            int rollCounter = 0;
            var sheet = grid.CurrentWorksheet;

            sheet.IterateCells(range, false, (row, col, cell) =>
            {
                
                if (cell == null || cell.Body != null && cell.Body.GetType().Equals(typeof(DescriptorCell)))
                {
                    if(rollCounter != 0)
                    {
                        cell = sheet.CreateAndGetCell(row, col);
                        cell.Data = rollCounter + ". __________ .";
                    }
                    else
                    {
                        cell = sheet.CreateAndGetCell(row, col);
                        cell.Data = "d" + totalRows + " ___________ .";
                    }

                }
                else
                {
                    if(rollCounter != 0 )
                    {
                        cell.Data = rollCounter + ". " + cell.Data + " .";
                    }
                    else
                    {
                        cell.Data = "d" + totalRows + " " + cell.Data + " ...";
                    }
                }

                rollCounter++;
                return true;

            });
            sheet.AutoFitColumnWidth(range.Col, true);
        }
        
        //Ctrl+E
        private void DesignateSubroll_Click(object sender, RoutedEventArgs e)
        {

            var sheet = grid.CurrentWorksheet;
            var selected = sheet.FocusPos;
            var cell = sheet.Cells[selected];
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
                
                sheet[selected] = new SubRollCell("Link: " + SpecialRollData.SubRollProperty.Last().Value);
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
            var sheet = grid.CurrentWorksheet;
            var range = sheet.SelectionRange;


            var listOfItems = new List<string>();
            
            sheet.IterateCells(range, false, (row, col, cell) =>
            {
                if(cell.DisplayText != null && cell.DisplayText != "")
                {
                    listOfItems.Add(cell.DisplayText);
                }
                
                return true;
            });

            var startCell = range.StartPos;

            for(int i = 0; i < range.Rows; i++)
            {
                if(listOfItems.Count > i)
                {
                    sheet[startCell.Row + i, startCell.Col] = listOfItems[i];
                }
                else
                {
                    sheet[startCell.Row + i, startCell.Col] = string.Empty;
                }
            }

        }
    }
}