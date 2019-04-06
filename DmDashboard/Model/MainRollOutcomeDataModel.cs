using CommonCode.Blocks;
using CommonCode.Rolls;
using GalaSoft.MvvmLight;
using System;

namespace GmDashboard.ViewModel
{
    public class MainRollOutcomeDataModel : ViewModelBase
    {
        private bool isSelected;
        private string preamble;
        private string title;
        private string rollResult;
        private IRoll _rollBlock;

        //TODO we need to make it so we  are not storeing extra text...  I Think this shold just have the block and the IsSelected.
        //This will change how to get the selected data items and it will also change how the data is bound...  It may be a bit more complicated...  But MUUUUCH more elegant
        //and easy to use....
        public MainRollOutcomeDataModel(IRoll rollBlock)
        {
            _rollBlock = rollBlock;
            Title = _rollBlock.GetDescription.Replace("\r", "").Replace("\n", "") + Environment.NewLine + "-----------------------------------";
            if (_rollBlock.TypeOfRoll == ObjectTypes.StandardRoll)
            {
                RollResult = ((StandardRoll)_rollBlock).Outcome.Replace("\r", "").Replace("\n", "") + Environment.NewLine;
            }
        }

        public string Preamble
        {
            get { return preamble; }
            set
            {
                preamble = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged();
            }
        }
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged();
            }
        }
        public string RollResult
        {
            get { return rollResult; }
            set
            {
                rollResult = value;
                RaisePropertyChanged();
            }
        }
        //This is something that will keep us form getting the wierd data model shit done...
        public StandardRoll Block { get { return (StandardRoll)_rollBlock; } }
    }
}
