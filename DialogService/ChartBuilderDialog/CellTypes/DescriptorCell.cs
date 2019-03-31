using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace DialogService.ChartBuilderDialog
{
    class DescriptorCell : CellBody
    {
        public override void OnPaint(CellDrawingContext dc)
        {
            base.OnPaint(dc);
            dc.Graphics.DrawRectangle(base.Bounds, SolidColor.LightSteelBlue);
        }
    }
}
