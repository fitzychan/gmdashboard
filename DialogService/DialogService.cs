using DialogService.ChartBuilderDialog;

namespace DialogService
{
    public static class  DialogService
    {
        public static void ShowChartBuilder()
        {
            ChartBuilderView dlg = new ChartBuilderView();
            dlg.Show();
        }
    }
}
