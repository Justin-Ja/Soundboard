using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Soundboard.Commands;
using Soundboard.Common;

namespace Soundboard.ViewModels;

public class KeyBindingDialogViewModel : BaseViewModel
{
    private string _soundName;
    private Key? _selectedKey;
    private ModifierKeys _selectedModifiers;
    private string _keyDisplayText;
    private bool _isOkEnabled;

    public string SoundName
    {
        get => _soundName;
        set => SetProperty(ref _soundName, value);
    }

    public Key? SelectedKey
    {
        get => _selectedKey;
        set
        {
            _selectedKey = value;
            OnPropertyChanged();
            UpdateKeyDisplay();
            UpdateOkEnabled();
        }
    }

    public ModifierKeys SelectedModifiers
    {
        get => _selectedModifiers;
        set
        {
            _selectedModifiers = value;
            OnPropertyChanged();
            UpdateKeyDisplay();
        }
    }

    public string KeyDisplayText
    {
        get => _keyDisplayText;
        set => SetProperty(ref _keyDisplayText, value);
    }

    public bool IsOkEnabled
    {
        get => _isOkEnabled;
        set => SetProperty(ref _isOkEnabled, value);
    }

    public bool IsCleared { get; private set; }
    public ICommand ClearBindingCommand { get; }
    public ICommand OkCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action<bool>? RequestClose;

    public KeyBindingDialogViewModel(string soundName, Key? currentKey = null, ModifierKeys currentModifiers = ModifierKeys.None)
    {
        _soundName = soundName;
        _selectedKey = currentKey;
        _selectedModifiers = currentModifiers;
        _keyDisplayText = "";

        ClearBindingCommand = new RelayCommand(() => { IsCleared = true; RequestClose?.Invoke(true); });
        OkCommand = new RelayCommand(() => RequestClose?.Invoke(true), () => IsOkEnabled);
        CancelCommand = new RelayCommand(() => RequestClose?.Invoke(false));

        UpdateKeyDisplay();
        UpdateOkEnabled();
    }

    public void OnKeyPressed(Key key, ModifierKeys modifiers)
    {
        //Ignore modifier-only presses
        if (key == Key.LeftCtrl || key == Key.RightCtrl ||
            key == Key.LeftAlt || key == Key.RightAlt ||
            key == Key.LeftShift || key == Key.RightShift ||
            key == Key.LWin || key == Key.RWin)
        {
            return;
        }

        SelectedKey = key;
        SelectedModifiers = modifiers;
    }

    private void UpdateKeyDisplay()
    {
        if (!SelectedKey.HasValue)
        {
            KeyDisplayText = "Press a key...";
            return;
        }

        var parts = new List<string>();
        if (SelectedModifiers.HasFlag(ModifierKeys.Control)) parts.Add("Ctrl");
        if (SelectedModifiers.HasFlag(ModifierKeys.Alt)) parts.Add("Alt");
        if (SelectedModifiers.HasFlag(ModifierKeys.Shift)) parts.Add("Shift");
        if (SelectedModifiers.HasFlag(ModifierKeys.Windows)) parts.Add("Win");
        parts.Add(SelectedKey.ToString());

        KeyDisplayText = string.Join(" + ", parts);
    }

    private void UpdateOkEnabled()
    {
        IsOkEnabled = SelectedKey.HasValue;
    }
}

