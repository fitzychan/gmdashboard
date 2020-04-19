using System.Collections.ObjectModel;

namespace DialogService.PowerShellParamDialog
{
    public class FunctionParameterViewModel
    {
        public FunctionParameterViewModel()
        {
            FunctionParams = new ObservableCollection<FunctionParameters>();
        }

        public ObservableCollection<FunctionParameters> FunctionParams { get; set; }
    }
}