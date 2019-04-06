using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.Blocks
{
    public interface IChart
    {
        Guid TypeOfChart { get; }
    }

    public interface IRoll
    {
        Guid TypeOfRoll { get; }
        string GetDescription { get; }
        int GetDice { get; }
    }
}
