using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;

namespace Soundboard.Common.Components;

public class SoundButtonModel: BaseViewModel
{
    private string _displayText;
    private string? _filePath;
    private bool _isAddButton;
    private ICommand _command;
    private Brush _buttonBrush;
    private Brush _borderBrush;
    private Key? _boundKey;
    private ModifierKeys _modifierKeys;

    public bool HasKeyBinding => BoundKey.HasValue;

    public string DisplayText
    {
        get => _displayText;
        set => SetProperty(ref _displayText, value);
    }

    public string? FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }

    public bool IsAddButton
    {
        get => _isAddButton;
        set => SetProperty(ref _isAddButton, value);
    }

    public ICommand Command
    {
        get => _command;
        set => SetProperty(ref _command, value);
    }

    public Brush ButtonBrush
    {
        get => _buttonBrush;
        set => SetProperty(ref _buttonBrush, value);
    }

    public Brush BorderBrush
    {
        get => _borderBrush;
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

    public ICommand? SetKeyBindingCommand { get; set; }

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
    
    //Constructor for "add sound" button
    public SoundButtonModel(string displayText, bool isAdd, Brush buttonBrush, Brush buttonBorderBrush, ICommand command)
    {
        _displayText = displayText;
        _isAddButton = isAdd;
        _buttonBrush = buttonBrush;
        _borderBrush = buttonBorderBrush;
        _command = command;
    }

    //Normal sound button constructor, has a file path
    public SoundButtonModel(string displayText, string filePath, bool isAdd, Brush buttonBrush, Brush buttonBorderBrush, ICommand command)
    {
        _displayText = displayText;
        _filePath = filePath;
        _isAddButton = isAdd;
        _buttonBrush = buttonBrush;
        _borderBrush = buttonBorderBrush;
        _command = command;
    }
}