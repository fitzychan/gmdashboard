using System;

namespace CommonCode.Interfaces
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
