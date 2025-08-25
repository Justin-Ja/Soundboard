using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Soundboard.ViewModels;

public interface ICrudToolbarViewModel
{
    string CurrentGridName { get; }
    ICommand NewGridCommand { get; }
    ICommand LoadGridCommand { get; }
    ICommand SaveGridCommand { get; }
    ICommand SaveAsGridCommand { get; }
}
