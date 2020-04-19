using CommonCode.DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows;

namespace DialogService.LoginDialog
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LoginParamsView : Window
    {
        public Creds Credentials { get; set; }
        


        public LoginParamsView()
        {
            InitializeComponent();
            DataContext = new LoginParamsViewModel();
        }

        private void Button_Ok(object sender, RoutedEventArgs e)
        {
            Credentials = new Creds()
            {
                UserName = ((LoginParamsViewModel)DataContext).UserName,
                Password = ((LoginParamsViewModel)DataContext).UserPass
            };
            Close();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
