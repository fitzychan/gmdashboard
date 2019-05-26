using CommonCode;
using CommonCode.Charts;
using CommonCode.Interfaces;
using CommonCode.Rolls;
using DialogService;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Pipes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GmDashboard.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        //IPreCommandPipe preCommandPipe;
        private ObservableCollection<string> foundCharts = new ObservableCollection<string>();
        private ObservableCollection<string> selectedCharts = new ObservableCollection<string>();
        private ObservableCollection<MainRollOutcomeDataModel> rollBlockOutcome = new ObservableCollection<MainRollOutcomeDataModel>();
        private ObservableCollection<IChart> mainFinishedBlock;

        //This needs to be bound to the datamodel
        public string preamble;

        //Main menu bar Commands
        public RelayCommand StartChartBuilderCommand { get; private set; }

        //IPreCommandPipe
        public RelayCommand LoadCommand { get; private set; }
        public RelayCommand RollCommand { get; private set; }
        public RelayCommand LocateCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand OpenContainingFoldersCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; }

        //IPostCommandPipe postCommandPipe;
        public RelayCommand SaveToFileCommand { get; private set; }
        public RelayCommand AddToFileCommand  { get; private set; }
        public RelayCommand SaveSelectedToFileCommand { get; private set; }
        public RelayCommand AddSelectedToFileCommand { get; private set; }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            MainFinishedBlock = new ObservableCollection<IChart>();
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            //These commands are used in the main menu stuff.  Its where we are going to stick most of our secondary functions
            StartChartBuilderCommand = new RelayCommand(() => Dialogs.ActivateChartBuilder());

            //These commands and this command pipe is for code relating to loading and parseing charts
            LoadCommand = new RelayCommand(() => FoundCharts = PipeAssessor.PrePipe.LoadCommand());
            RollCommand = new RelayCommand(() => MainFinishedBlock = new ObservableCollection<IChart>(PipeAssessor.PrePipe.RollOneCommand(SelectedCharts)));
            LocateCommand = new RelayCommand(() => { PipeAssessor.PrePipe.AddTablesToRepo(); LoadCommand.Execute(null); });
            DeleteCommand = new RelayCommand(() => { PipeAssessor.PrePipe.DeleteTableCommand(SelectedCharts); LoadCommand.Execute(null); });
            OpenContainingFoldersCommand = new RelayCommand(() => PipeAssessor.PrePipe.OpenFileLocation(SelectedCharts));
            OpenFileCommand = new RelayCommand(() => PipeAssessor.PrePipe.OpenFile(SelectedCharts));
            //end of pre commands


            //The Commmands are releated to AFTER we have data and its parsed
            SaveToFileCommand = new RelayCommand(() => PipeAssessor.PostPipe.SaveChartCommand((Chart)MainFinishedBlock.FirstOrDefault()));
            AddToFileCommand = new RelayCommand(() => PipeAssessor.PostPipe.AddToChartCommand((Chart)MainFinishedBlock.FirstOrDefault()));
            SaveSelectedToFileCommand = new RelayCommand(() => PipeAssessor.PostPipe.SaveSelectedChartCommand(GetSelected()));
            AddSelectedToFileCommand = new RelayCommand(() => PipeAssessor.PostPipe.AddSelectedToChartCommand(GetSelected()));
            //end of postcommand pipe

            //When main is populated we want to load up the tables
            LoadCommand.Execute(null);
        }

        public ObservableCollection<MainRollOutcomeDataModel> OutcomeDataModel
        {
            get
            {
                return rollBlockOutcome;
            }
            set
            {
                rollBlockOutcome = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// We need to figure out the data model better.  WE are close!  Its looking good!
        /// </summary>
        public string Preamble
        {
            get
            {
                return preamble;
            }
            set
            {
                preamble = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> FoundCharts
        {
            get { return foundCharts; }
            set
            {
                foundCharts = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> SelectedCharts
        {
            get { return selectedCharts; }
            set
            {
                selectedCharts = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<IChart> MainFinishedBlock
        {
            get { return mainFinishedBlock; }
            set
            {
                mainFinishedBlock = value;
                OutcomeDataModel = new ObservableCollection<MainRollOutcomeDataModel>();
                foreach (var mainBlock in mainFinishedBlock)
                {
                    if(mainBlock.TypeOfChart == GmDashboardTypes.PowerShellChart)
                    {
                        foreach (var powerShellResult in ((FunctionParamChart)mainBlock).PowerShellResult)
                        {
                            OutcomeDataModel.Add(new MainRollOutcomeDataModel( powerShellResult));
                        }
                    }
                    else if (mainBlock.TypeOfChart == GmDashboardTypes.Chart)
                    {
                        foreach (var subBlockItem in ((Chart)mainBlock).ChartRolls)
                        {
                            OutcomeDataModel.Add(new MainRollOutcomeDataModel(((StandardRoll)subBlockItem).Outcome.Replace("\r", "").Replace("\n", "") + Environment.NewLine));
                        }
                    }
                    else if (mainBlock.TypeOfChart == GmDashboardTypes.RfgChart)
                    {
                        foreach (var subBlockItem in ((ChartRgf)mainBlock).Blocks)
                        {
                            if(subBlockItem.BlockType == typeof(DescriptorRgf))
                            {
                                OutcomeDataModel.Add(new MainRollOutcomeDataModel(subBlockItem.BlockDescriptor.Replace("\r", "").Replace("\n", "") + Environment.NewLine));
                            }
                            else
                            {
                                OutcomeDataModel.Add(new MainRollOutcomeDataModel(((RollBlockRgf)subBlockItem).GetOutcome().Replace("\r", "").Replace("\n", "") + Environment.NewLine));
                            }
                        }
                    }
                }
            }
        }
        private List<IRoll> GetSelected()
        {
            var selectedBlocks = new List<IRoll>();
            foreach (var dataModel in OutcomeDataModel.Where(x => x.IsSelected))
            {
                selectedBlocks.Add(dataModel.Block);
            }
            return selectedBlocks;
        }
    }

}