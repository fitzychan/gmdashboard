using System.ComponentModel;
using System.Security;

namespace DialogService.LoginDialog
{
    public class LoginParamsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string _userName;
        public string _userPass;


        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (value != _userName)
                {
                    _userName = value;
                    NotifyPropertyChanged("UserName");
                }
            }
        }

        public string UserPass
        {
            get
            {
                return _userPass;
            }
            set
            {
                if(value != null)
                {
                    _userPass = value;
                    NotifyPropertyChanged("UserPass");
                }
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
