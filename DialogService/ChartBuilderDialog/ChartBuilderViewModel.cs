using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DialogService.ChartBuilderDialog
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ChartBuilderViewModel : ViewModelBase
    {
        public RelayCommand DesignateRollCommand { get; private set; }
        /// <summary>
        /// Initializes a new instance of the ChartBuilderViewModel class.
        /// </summary>
        public ChartBuilderViewModel()
        {
            
        }
    }
}