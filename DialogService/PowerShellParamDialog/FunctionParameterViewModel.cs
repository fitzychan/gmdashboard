using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace DialogService.PowerShellParamDialog
{
    public class FunctionParameterViewModel
    {

        public RelayCommand OkCommand { get; private set; }

        public FunctionParameterViewModel()
        {
            FunctionParams = new ObservableCollection<FunctionParameters>();
        }

        public ObservableCollection<FunctionParameters> FunctionParams { get; set; }
    }
}