using CommonCode;
using CommonCode.Charts;
using CommonCode.Interfaces;
using DialogService.ChartBuilderDialog;
using DialogService.PowerShellParamDialog;
using System.Collections.Generic;
using System.Linq;

namespace DialogService
{
    public static class  Dialogs
    {
        public static void ActivateChartBuilder()
        {
            ChartBuilderView dlg = new ChartBuilderView();
            dlg.Show();
        }

        public static List<FunctionParameters> ExtractPowerShellParameters(IChart powerShellChart)
        {
            List<FunctionParameters> extractedParams = new List<FunctionParameters>();

            if (powerShellChart.TypeOfChart.Equals(GmDashboardTypes.PowerShell))
            {
                var functionalParams = new FunctionParameterViewModel();
                foreach (var param in ((FunctionParamChart)powerShellChart).Parameters)
                {
                    functionalParams.FunctionParams.Add(new FunctionParameters() { Name = param.Name, Description = param.Description });
                }

                PowerShellParamsView paramView = new PowerShellParamsView(functionalParams);
                paramView.ShowDialog();

                extractedParams = paramView.paramResults;

            }
            else
            {
                throw new System.Exception("WTF dat bad");
            }


            return extractedParams;
        }

    }
}
