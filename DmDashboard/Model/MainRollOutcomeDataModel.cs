using CommonBlocks;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DmDashboard.ViewModel
{
    public class MainRollOutcomeDataModel : ViewModelBase
    {
        private bool isSelected;
        private string preamble;
        private string title;
        private string rollResult;
        private IBlock _rollBlock;

        //TODO we need to make it so we  are not storeing extra text...  I Think this shold just have the block and the IsSelected.
        //This will change how to get the selected data items and it will also change how the data is bound...  It may be a bit more complicated...  But MUUUUCH more elegant
        //and easy to use....
        public MainRollOutcomeDataModel(IBlock rollBlock)
        {
            _rollBlock = rollBlock;
            Title = _rollBlock.BlockDescriptor.Replace("\r", "").Replace("\n", "") + Environment.NewLine + "-----------------------------------";
            if (_rollBlock.BlockType == typeof(RollBlock))
            {
                RollResult = ((RollBlock)_rollBlock).Result.Replace("\r", "").Replace("\n", "") + Environment.NewLine;
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
        public RollBlock Block { get { return (RollBlock)_rollBlock; } }
    }
}
