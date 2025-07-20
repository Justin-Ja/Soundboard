using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;

namespace Soundboard.Common.Components;

    //TODO: i think this will become a database table at some point... move it to domain
    //also fix the warnings with constructors
public class SoundButtonModel: BaseViewModel
{
    private string _displayText;
    private string _filePath;
    private ICommand _command;
    private Brush _buttonBrush;
    private Brush _borderBrush;
    private Key? _boundKey;
    private ModifierKeys _modifierKeys;

    public bool IsAddButton { get; set; }
    public bool HasKeyBinding => BoundKey.HasValue;

    public string DisplayText
    {
        get => _displayText;
        set => SetProperty(ref _displayText, value);
    }

    public string FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }

    public ICommand Command
    {
        get => _command;
        set => SetProperty(ref _command, value);
    }

    public Brush ButtonBrush
    {
        get => _buttonBrush ?? Brushes.LightBlue;
        set => SetProperty(ref _buttonBrush, value);
    }

    public Brush BorderBrush
    {
        get => _borderBrush ?? Brushes.DarkBlue;
        set => SetProperty(ref _borderBrush, value);
    }
    public Key? BoundKey
    {
        get => _boundKey;
        set
        {
            SetProperty(ref _boundKey, value);
            OnPropertyChanged(nameof(KeyBindingDisplay));
        }
    }

    public ModifierKeys ModifierKeys
    {
        get => _modifierKeys;
        set
        {
            SetProperty(ref _modifierKeys, value);
            OnPropertyChanged(nameof(KeyBindingDisplay));
        }
    }

    public string KeyBindingDisplay
    {
        get
        {
            if (!BoundKey.HasValue) return "No binding";

            var parts = new List<string>();
            if (ModifierKeys.HasFlag(ModifierKeys.Control)) parts.Add("Ctrl");
            if (ModifierKeys.HasFlag(ModifierKeys.Alt)) parts.Add("Alt");
            if (ModifierKeys.HasFlag(ModifierKeys.Shift)) parts.Add("Shift");
            if (ModifierKeys.HasFlag(ModifierKeys.Windows)) parts.Add("Win");

            parts.Add(BoundKey.Value.ToString());
            return string.Join(" + ", parts);
        }
    }
}