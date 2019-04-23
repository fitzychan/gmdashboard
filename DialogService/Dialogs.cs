using CommonCode;
using CommonCode.Charts;
using CommonCode.Interfaces;
using DialogService.ChartBuilderDialog;
using DialogService.PowerShellParamDialog;

namespace DialogService
{
    public static class  Dialogs
    {
        public static void ActivateChartBuilder()
        {
            ChartBuilderView dlg = new ChartBuilderView();
            dlg.Show();
        }

        public static string ExtractPowerShellParameters(IChart powerShellChart)
        {
            string extractedParams = string.Empty;

            if (powerShellChart.TypeOfChart.Equals(GmDashboardTypes.PowerShell))
            {
                PowerShellParamsView paramView = new PowerShellParamsView((FunctionParamChart)powerShellChart);
                paramView.ShowDialog();
            }
            else
            {
                throw new System.Exception("WTF dat bad");
            }


            return extractedParams;
        }
    }
}
