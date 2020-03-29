using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace DialogService.ChartBuilderDialog
{
    class SubRollHeadCell : CellBody
    {
        string cornerDisplay = string.Empty;
        public SubRollHeadCell(string subCellDesignation)
        {
            cornerDisplay = subCellDesignation;
        }
        public override void OnPaint(CellDrawingContext dc)
        {
            base.OnPaint(dc);
            dc.Graphics.DrawText(cornerDisplay, "Calibri", 8, SolidColor.Black, Bounds , unvell.ReoGrid.ReoGridHorAlign.Right, unvell.ReoGrid.ReoGridVerAlign.Top);
        }
    }
}
