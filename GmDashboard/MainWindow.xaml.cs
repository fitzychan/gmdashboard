using GmDashboard.ViewModel;
using System.Windows;
using System.Windows.Controls;

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
            MessageBox.Show("This was coded with love by GmJam!  If you have questions you can reach me on reddit at @GmJam", "Gm-Dashboard", MessageBoxButton.OK);
        }

        private void ChartList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
