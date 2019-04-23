using CommonCode.Charts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DialogService.PowerShellParamDialog
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PowerShellParamsView : Window
    {
        public ObservableCollection<FunctionParameterViewModel> FunctionParams { get; set; }

        public PowerShellParamsView(FunctionParamChart functionChart)
        {
            InitializeComponent();
            FunctionParams = new ObservableCollection<FunctionParameterViewModel>();

            foreach (var param in functionChart.Parameters)
            {
                FunctionParams.Add(new FunctionParameterViewModel { Name = param.First(), Description = param.LastOrDefault()});
            }
        }
    }
}
