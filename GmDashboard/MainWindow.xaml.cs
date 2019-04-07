using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GmDashboard.ViewModel;

namespace GmDashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void GetSelectedRollItem_Click(object sender, RoutedEventArgs e)
        {
            var vmLocator = new ViewModelLocator();
            vmLocator.Main.SelectedCharts.Clear();
            foreach (var item in ChartList.SelectedItems)
            {
                vmLocator.Main.SelectedCharts.Add(item.ToString());
            }
        }

        private void CloseProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This was coded with love!  If you have questions hit me up on reddit @GmJam", "Gm-Dashboard", MessageBoxButton.OK);
        }
    }
}
