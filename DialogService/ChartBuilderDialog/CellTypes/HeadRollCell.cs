using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace DialogService.ChartBuilderDialog
{
    class HeadRollCell : CellBody
    {
        public override void OnPaint(CellDrawingContext dc)
        {
            base.OnPaint(dc);
            dc.Graphics.DrawRectangle(base.Bounds, new SolidColor("#BDBCC3"));
        }
    }
}
