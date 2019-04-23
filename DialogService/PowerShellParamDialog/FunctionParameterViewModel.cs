using System.ComponentModel;

namespace DialogService.PowerShellParamDialog
{
    public class FunctionParameterViewModel : INotifyPropertyChanged
    {
        public string _description;
        public string _name;

        public string Description { get => _description; set { _description = value; NotifyPropertyChanged("Description"); } }

        public string Name { get => _name; set { _name = value; NotifyPropertyChanged("Name"); }  }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
