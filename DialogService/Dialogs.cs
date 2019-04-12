using DialogService.ChartBuilderDialog;
using DialogService.FunctionDialog;

namespace DialogService
{
    public static class  Dialogs
    {
        public static void ActivateChartBuilder()
        {
            ChartBuilderView dlg = new ChartBuilderView();
            dlg.Show();
        }
        public static void ActivateFuctionDialog()
        {
            FunctionView dlg = new FunctionView();
            dlg.Show();
        }
    }
}
