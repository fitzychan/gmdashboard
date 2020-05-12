using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace DialogService.ChartBuilderDialog
{
    class StandardRollCell : CellBody
    {
        public override void OnPaint(CellDrawingContext dc)
        {
            base.OnPaint(dc);
            dc.Graphics.DrawRectangle(base.Bounds, new SolidColor("#D8D7DB"));
        }
    }
}
