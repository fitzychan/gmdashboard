using System.ComponentModel;

namespace DialogService.PowerShellParamDialog
{
    public class FunctionParameters : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string _name;
        public string _description;
        public string _value;


        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if(value != null)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if(value != null)
                {
                    _value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
