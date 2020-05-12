/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:GmDashboard"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using DialogService.ChartBuilderDialog;
using GalaSoft.MvvmLight.Ioc;

namespace GmDashboard.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ChartBuilderViewModel>();
            SimpleIoc.Default.Register<CloudRepoViewModel>();
            //SimpleIoc.Default.Register<FunctionParameterViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }


        public ChartBuilderViewModel ChartBuilder
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ChartBuilderViewModel>();
            }
        }

        public CloudRepoViewModel CloudRepo
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CloudRepoViewModel>();
            }
        }

        //public FunctionParameterViewModel FunctionParamDialog
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<FunctionParameterViewModel>();
        //    }
        //}

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}