using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soundboard.Domain.DataAccess.Implementations;

namespace Soundboard.Common.Events;

public class GridChangedEventArgs : EventArgs
{
    public SoundButtonGridLayout Grid { get; }
    public string Operation { get; }

    public GridChangedEventArgs(SoundButtonGridLayout grid, string operation)
    {
        Grid = grid;
        Operation = operation;
    }
}


