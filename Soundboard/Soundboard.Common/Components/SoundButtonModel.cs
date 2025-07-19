using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Soundboard.Common.Components
{
    //TODO: i think this will become a database table at some point... move it to domain
    //TODO: Use setfield
    public class SoundButtonModel: BaseViewModel
    {
        private string _displayText;
        private string _filePath;
        private ICommand _command;
        private Brush _buttonBrush;
        private Brush _borderBrush;

        public bool IsAddButton { get; set; }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                OnPropertyChanged();
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }

        public ICommand Command
        {
            get => _command;
            set
            {
                _command = value;
                OnPropertyChanged();
            }
        }

        public Brush ButtonBrush
        {
            get => _buttonBrush ?? Brushes.LightBlue;
            set
            {
                _buttonBrush = value;
                OnPropertyChanged();
            }
        }

        public Brush BorderBrush
        {
            get => _borderBrush ?? Brushes.DarkBlue;
            set
            {
                _borderBrush = value;
                OnPropertyChanged();
            }
        }
    }
}
