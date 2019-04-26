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
        public List<FunctionParameters> paramResults = new List<FunctionParameters>();


        public PowerShellParamsView(FunctionParameterViewModel loadedViewModel)
        {
            InitializeComponent();
            DataContext = loadedViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            paramResults = ((FunctionParameterViewModel)DataContext).FunctionParams.ToList();
            Close();
        }
    }
}
